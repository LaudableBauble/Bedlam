using System;
using System.Xml;
using System.Collections.Generic;
using System.Windows.Forms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using Library;
using Library.Animate;
using Library.Core;
using Library.Enums;
using Library.Factories;
using Library.Imagery;

namespace Library
{
    /// <summary>
    /// A class that serves as a helper in varying forms to other instances in the game.
    /// </summary>
    public static class Helper
    {
        #region Fields
        private static Random _Random = new Random();
        #endregion

        #region Methods
        /// <summary>
        /// Calculates the angle that an object should face, given its position, its
        /// target's position, its current angle, and its maximum turning speed.
        /// </summary>
        /// <param name="position">The position of the object.</param>
        /// <param name="faceThis">The position to face.</param>
        /// <param name="currentAngle">The current angle of the object.</param>
        /// <param name="turnSpeed">The maximum turning speed.</param>
        /// <param name="facingDirection">The direction this object faces.</param>
        /// <returns>The angle the object should face, considering all the prerequests.</returns>
        public static float TurnToFace(Vector2 position, Vector2 faceThis, float currentAngle, float turnSpeed, FacingDirection facingDirection)
        {
            //Calculate the desired angle.
            float desiredAngle = (float)Math.Atan2((faceThis.Y - position.Y), (faceThis.X - position.X));
            //Depending on the facing direction, add half a turn to the desired angle.
            if (facingDirection == FacingDirection.Left) { desiredAngle += (float)Math.PI; }

            //Wrap the complete angle and return it.
            return WrapAngle(currentAngle + MathHelper.Clamp(WrapAngle(desiredAngle - currentAngle), -turnSpeed, turnSpeed));
        }
        /// <summary>
        /// Returns the angle expressed in radians between 0 and two Pi.
        /// </summary>
        /// <param name="radians">The angle in radians.</param>
        /// <returns>The wrapped angle.</returns>
        public static float WrapAngle(float radians)
        {
            //Check whether to add or subtract two PI.
            while (radians < 0) { radians += MathHelper.TwoPi; }
            while (radians > MathHelper.TwoPi) { radians -= MathHelper.TwoPi; }

            //Return the shifted angle.
            return radians;
        }
        /// <summary>
        /// Calculate the angle to face the chosen position.
        /// </summary>
        /// <param name="pos1">The first position.</param>
        /// <param name="pos2">The position to face.</param>
        /// <returns>The angle.</returns>
        public static float FaceAngle(Vector2 pos1, Vector2 pos2)
        {
            //The angle to face.
            float feta = (float)Math.Atan((pos2.X - pos1.X) / (pos2.Y - pos1.Y));
            //If the first position is lower than the second, add half a lap to it.
            if (pos2.Y < pos1.Y) { feta += MathHelper.Pi; }
            //Return the angle to face.
            return feta;
        }
        /// <summary>
        /// Limit an angle between a min and a max value.
        /// </summary>
        /// <param name="angle">The angle to limit.</param>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        /// <returns>The limited angle.</returns>
        public static float LimitAngle(float angle, float min, float max)
        {
            //Create a temp rotation variable.
            float _angle = angle;

            //Check if it isn't angled within set limits.
            if ((_angle > max) || (_angle < min))
            {
                //Check if it is angled downwards.
                if (_angle < ((max + min) / 2)) { _angle = min; }
                //If it's angled upwards.
                else { _angle = max; }
            }

            //Return the clamped rotation value.
            return (-_angle);
        }
        /// <summary>
        /// Move a source vector to a destination vector, within given values.
        /// </summary>
        /// <param name="source">The source vector.</param>
        /// <param name="destination">The destination vector.</param>
        /// <param name="min">The minimum value to move.</param>
        /// <param name="max">The maximum value to move.</param>
        /// <returns>The constrained vector.</returns>
        public static Vector2 ConstrainMovement(Vector2 source, Vector2 destination, Vector2 min, Vector2 max)
        {
            //Return the constrained vector.
            return (source + Vector2.Clamp(destination, min, max));
        }
        /// <summary>
        /// Move a source vector a number of steps, within given values.
        /// </summary>
        /// <param name="source">The source vector.</param>
        /// <param name="amount">The amount to move.</param>
        /// <param name="boundary">The maximum movement possible.</param>
        /// <returns>The constrained vector.</returns>
        public static Vector2 ConstrainMovement(Vector2 source, Vector2 amount, Vector2 boundary)
        {
            //Return the constrained vector.
            return (source + Vector2.Clamp(amount, -boundary, boundary));
        }
        /// <summary>
        /// Enable, for example, the camera to focus squarely on a vector in the top left position of the viewport. That is to have it in the middle of the camera.
        /// </summary>
        /// <param name="topLeft">The original top left position vector.</param>
        /// <param name="viewport">The bounds of the viewport.</param>
        /// <returns>The transformed camera vector.</returns>
        public static Vector2 TransformCameraPosition(Vector2 topLeft, Rectangle viewport)
        {
            //Return the transformed vector.
            return (new Vector2(topLeft.X - (viewport.Width / 2), topLeft.Y - (viewport.Height / 2)));
        }
        /// <summary>
        /// Return the camera's matrix transformation.
        /// </summary>
        /// <param name="position">The position of the camera.</param>
        /// <param name="rotation">The rotation of the camera.</param>
        /// <param name="zoom">The zoom of the camera.</param>
        /// <param name="origin">The origin of the camera.</param>
        /// <returns>The transformation matrix.</returns>
        public static Matrix TransformCameraMatrix(Vector2 position, float rotation, float zoom, Vector2 origin)
        {
            //Return the transformation matrix.
            return (Matrix.CreateTranslation(new Vector3(-position, 0)) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateScale(new Vector3(zoom, zoom, 1)) *
                Matrix.CreateTranslation(new Vector3(origin, 0)));
        }
        /// <summary>
        /// Get the bounding box of an object.
        /// </summary>
        /// <param name="item">An item carrying data about position, rotation, scale, width and height.</param>
        /// <returns>The bounding box.</returns>
        public static Rectangle GetBoundingBox(Item item)
        {
            //Return as a rectangle.
            return GetBoundingBox(item.Position, item.Rotation, item.Scale, item.Width, item.Height, item.Origin);
        }
        /// <summary>
        /// Get the bounding box of an object.
        /// </summary>
        /// <param name="item">An item carrying data about position, rotation, scale, width and height.</param>
        /// <param name="origin">The origin of the object.</param>
        /// <returns>The bounding box.</returns>
        public static Rectangle GetBoundingBox(Item item, Vector2 origin)
        {
            //Return as a rectangle.
            return GetBoundingBox(item.Position, item.Rotation, item.Scale, item.Width, item.Height, origin);
        }
        /// <summary>
        /// Get the bounding box of an object.
        /// </summary>
        /// <param name="item">An item carrying data about position, rotation and scale.</param>
        /// <param name="width">The width of the object.</param>
        /// <param name="height">The height of the object.</param>
        /// <param name="origin">The origin of the object.</param>
        /// <returns>The bounding box.</returns>
        public static Rectangle GetBoundingBox(Item item, float width, float height, Vector2 origin)
        {
            //Return as a rectangle.
            return GetBoundingBox(item.Position, item.Rotation, item.Scale, width, height, origin);
        }
        /// <summary>
        /// Get the bounding box of an object.
        /// </summary>
        /// <param name="position">The position of the object.</param>
        /// <param name="rotation">The rotation of the object.</param>
        /// <param name="scale">The scale of the object.</param>
        /// <param name="width">The width of the object.</param>
        /// <param name="height">The height of the object.</param>
        /// <param name="origin">The origin of the object.</param>
        /// <returns>The bounding box.</returns>
        public static Rectangle GetBoundingBox(Vector2 position, float rotation, Vector2 scale, float width, float height, Vector2 origin)
        {
            //Create a matrix for the given data.
            Matrix transform =
                Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
                Matrix.CreateScale(scale.X, scale.Y, 1) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateTranslation(new Vector3(position, 0.0f));

            //Transform all four corners into work space.
            Vector2 leftTop = Vector2.Transform(new Vector2(0, 0), transform);
            Vector2 rightTop = Vector2.Transform(new Vector2(width, 0), transform);
            Vector2 leftBottom = Vector2.Transform(new Vector2(0, height), transform);
            Vector2 rightBottom = Vector2.Transform(new Vector2(width, height), transform);

            //Find the minimum and maximum extents of the rectangle in world space.
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop), Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop), Vector2.Max(leftBottom, rightBottom));

            //Return as a rectangle.
            return new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }
        /// <summary>
        /// Convert radians into a direction vector.
        /// </summary>
        /// <param name="radians">The radians to use.</param>
        /// <returns>The unit vector, i.e the direction.</returns>
        public static Vector2 RadiansToVectorOld(float radians)
        {
            return new Vector2((float)Math.Cos(radians), (float)Math.Sin(radians));
        }
        /// <summary>
        /// Get the difference in direction between two positions in radians.
        /// </summary>
        /// <param name="parentPosition">The parent position.</param>
        /// <param name="parentRotation">The parent rotation.</param>
        /// <param name="childPosition">The child position.</param>
        /// <returns>Radians depicting the difference in direction.</returns>
        public static float DifferenceInDirectionOld(Vector2 parentPosition, float parentRotation, Vector2 childPosition)
        {
            //Calculate the difference between the parent position and the child position.
            Vector2 direction = Vector2.Subtract(childPosition, parentPosition);

            //If the difference between the two position doesn't equal zero, normalize it. Otherwise just leave it alone.
            if (!direction.Equals(Vector2.Zero)) { direction.Normalize(); }

            //Return the direction spelled out in radians. The added PI/2 is for the Atan2 rotational change.
            return (parentRotation - ((float)Math.PI / 2 + (float)Math.Atan2(direction.Y, direction.X)));
        }
        /// <summary>
        /// Convert radians into a direction vector, using a right handed coordination system.
        /// </summary>
        /// <param name="radians">The radians to use.</param>
        /// <returns>The unit vector, i.e the direction.</returns>
        public static Vector2 RadiansToVector(float radians)
        {
            return new Vector2((float)Math.Sin(radians), -(float)Math.Cos(radians));
        }
        /// <summary>
        /// Get a random number between specified bounds.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The randomized value.</returns>
        public static float RandomNumber(float min, float max)
        {
            return (float)((max - min) * _Random.NextDouble() + min);
        }
        /// <summary>
        /// Get a random number between specified bounds.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The inclusive maximum value.</param>
        /// <returns>The randomized value.</returns>
        public static int RandomNumber(int min, int max)
        {
            return _Random.Next(min, max + 1);
        }
        /// <summary>
        /// Add two angles.
        /// </summary>
        /// <param name="radian1">The first angle to add.</param>
        /// <param name="radian2">The second angle to add.</param>
        /// <returns>The angle sum.</returns>
        public static float AddAngles(float radian1, float radian2)
        {
            //Add the angles together.
            float addResult = radian1 + radian2;
            //Check if the sum of the angles has overreached a full lap, aka two PI, and if so fix it.
            if (addResult > (Math.PI * 2)) { return (addResult - ((float)Math.PI * 2)); }
            else { return addResult; }
        }
        /// <summary>
        /// Subtracts an angle from an angle.
        /// </summary>
        /// <param name="radian1">The angle to subtract from.</param>
        /// <param name="radian2">The angle to subtract.</param>
        /// <returns>The subtracted angle.</returns>
        public static float SubtractAngles(float radian1, float radian2)
        {
            //Subtract the angles from eachother.
            float subtractResult = radian1 - radian2;
            //If the difference has exceeded a full lap, aka 0, fix that.
            if (subtractResult < 0) { return (subtractResult + ((float)Math.PI * 2)); }
            else { return subtractResult; }
        }
        /// <summary>
        /// Get the difference in direction between two positions in radians, using a right handed coordination system.
        /// </summary>
        /// <param name="parentPosition">The parent position.</param>
        /// <param name="parentRotation">The parent rotation.</param>
        /// <param name="childPosition">The child position.</param>
        /// <returns>Radians depicting the difference in direction.</returns>
        public static float DifferenceInDirection(Vector2 parentPosition, float parentRotation, Vector2 childPosition)
        {
            //Calculate the difference between the parent position and the child position.
            Vector2 direction = Vector2.Subtract(childPosition, parentPosition);

            //If the difference between the two position doesn't equal zero, normalize it. Otherwise just leave it alone.
            if (!direction.Equals(Vector2.Zero)) { direction.Normalize(); }

            //Return the direction spelled out in radians.
            return ((float)Math.Atan2(direction.X, -direction.Y) - parentRotation);
        }
        /// <summary>
        /// Converts radians from a left handed coordinate system to a right handed, or the opposite.
        /// Question: Perhaps the presence of the WrapAngle method complicates things?
        /// </summary>
        /// <param name="radians">The angle in radians.</param>
        /// <returns>The converted angle.</returns>
        public static float ConvertAngle(float radians)
        {
            //Wrap the angle.
            radians = WrapAngle(radians);

            //Convert the angle correctly.
            if (radians > (float)Math.PI / 4) { return (radians - ((radians - (float)Math.PI / 4) * 2)); }
            else { return (radians + (((float)Math.PI / 4 - radians) * 2)); }
        }
        /// <summary>
        /// Wrap a value between minimum and maximum boundaries.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        /// <returns>The wrapped value.</returns>
        public static float WrapValue(float value, float min, float max)
        {
            //Wrap the value.
            if (value > max) { return (value - max) + min; }
            if (value < min) { return max - (min - value); }

            //Return the original if the value was within limits.
            return value;
        }
        /// <summary>
        /// Get the current position of the mouse.
        /// </summary>
        /// <returns>The position of the mouse.</returns>
        public static Vector2 GetMousePosition()
        {
            //Return the mouse's position.
            return new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
        }
        /// <summary>
        /// Round a vector.
        /// </summary>
        /// <param name="vector">The vector to round.</param>
        /// <param name="decimals">The number of decimals to round to.</param>
        /// <returns>The rounded vector.</returns>
        public static Vector2 Round(Vector2 vector, int decimals)
        {
            return new Vector2((float)Math.Round(vector.X, decimals), (float)Math.Round(vector.Y, decimals));
        }
        /// <summary>
        /// Get the closest vector to a point out of two options.
        /// </summary>
        /// <param name="focus">The first vector.</param>
        /// <param name="other">The second vector.</param>
        /// <param name="point">The point to measure from.</param>
        /// <returns>The closest vector.</returns>
        public static Vector2 ClosestTo(Vector2 v1, Vector2 v2, Vector2 point)
        {
            //Return the closest vector.
            if (Vector2.Distance(v1, point) < Vector2.Distance(v2, point)) { return v1; }
            else { return v2; }
        }
        /// <summary>
        /// Whether a vector is closer to a point than another.
        /// </summary>
        /// <param name="v1">The focus vector.</param>
        /// <param name="v2">The other vector.</param>
        /// <param name="point">The point to measure from.</param>
        /// <returns>Whether the given vector is closer than the other.</returns>
        public static bool CloserThan(Vector2 focus, Vector2 other, Vector2 point)
        {
            //Return whether the vector is closer than the other or not.
            if (focus.Equals(ClosestTo(focus, other, point))) { return true; }
            else { return false; }
        }
        /// <summary>
        /// If the point is within the given rectangle box, return true.
        /// </summary>
        /// <param name="point">The point's position.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>Whether the point truly is within the box.</returns>
        public static bool IsPointWithinBox(Vector2 point, Rectangle rectangle)
        {
            //If the point is within the rectangle, return true.
            return ((point.X > rectangle.X) && (point.X < (rectangle.X + rectangle.Width)) && (point.Y > rectangle.Y) && (point.Y < (rectangle.Y + rectangle.Height)));
        }
        /// <summary>
        /// If the point is within the given rectangle box, return true.
        /// </summary>
        /// <param name="point">The point's position.</param>
        /// <param name="boxPosition">The position of the box.</param>
        /// <param name="boxWidth">The width of the box.</param>
        /// <param name="boxHeight">The box's height.</param>
        /// <returns>Whether the point truly is within the box.</returns>
        public static bool IsPointWithinBox(Vector2 point, Vector2 boxPosition, float boxWidth, float boxHeight)
        {
            //If the point is within the rectangle, return true.
            return ((point.X > boxPosition.X) && (point.X < (boxPosition.X + boxWidth)) && (point.Y > boxPosition.Y) && (point.Y < (boxPosition.Y + boxHeight)));
        }
        /// <summary>
        /// If the point is within the bounds of an image, return true. An alpha value of zero does not count as being part of the image.
        /// This method assumes that the image's position is the location of its top-left corner.
        /// </summary>
        /// <param name="point">The point's position.</param>
        /// <param name="sprite">The sprite.</param>
        /// <returns>Whether the point truly is within the bounds of the sprite.</returns>
        public static bool IsPointWithinImage(Vector2 point, Sprite sprite)
        {
            return IsPointWithinImage(point, sprite.Position, sprite.Rotation, sprite.Scale, sprite[0].Origin, sprite.Texture);
        }
        /// <summary>
        /// If the point is within the bounds of an image, return true. An alpha value of zero does not count as being part of the image.
        /// This method assumes that the image's position is the location of its top-left corner.
        /// </summary>
        /// <param name="point">The point's position.</param>
        /// <param name="position">The position of the image.</param>
        /// <param name="rotation">The rotation of the image.</param>
        /// <param name="scale">The scale of the image.</param>
        /// <param name="origin">The origin of the image.</param>
        /// <param name="image">The image.</param>
        /// <returns>Whether the point truly is within the bounds of the image.</returns>
        public static bool IsPointWithinImage(Vector2 point, Vector2 position, float rotation, Vector2 scale, Vector2 origin, Texture2D image)
        {
            //If the image actually exists.
            if (image == null) { return false; }

            //Create a matrix for the given data.
            Matrix transform =
                Matrix.CreateTranslation(new Vector3(-origin.X, -origin.Y, 0.0f)) *
                Matrix.CreateScale(scale.X, scale.Y, 1) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateTranslation(new Vector3(position, 0.0f));

            //Transform the world point into a local vector for the image.
            Vector2 localPosition = Vector2.Transform(point, Matrix.Invert(transform));

            //Get the color data of the image.
            Color[] colorData = new Color[image.Width * image.Height];
            image.GetData(colorData);

            //Get the local position and round the float point coordinates into integers.
            int x = (int)Math.Round(localPosition.X - position.X);
            int y = (int)Math.Round(localPosition.Y - position.Y);

            //If the point is within the image's bounds.
            if (IsPointWithinBox(new Vector2(x, y), Vector2.Zero, image.Bounds.Width, image.Bounds.Height))
            {
                //If the pixel at the point has an alpha value over zero, return true.
                if (colorData[x + y * image.Width].A != 0) { return true; }
            }

            //Return false.
            return false;
        }
        /// <summary>
        /// Calculate an orbit position.
        /// </summary>
        /// <param name="position">The center position of the orbit.</param>
        /// <param name="rotation">The current rotation around the center of the orbit.</param>
        /// <param name="offset">The offset from the orbit center.</param>
        /// <returns>The orbit position.</returns>
        public static Vector2 CalculateOrbitPosition(Vector2 position, float rotation, float offset)
        {
            //Return the orbit position.
            return (Vector2.Add(Vector2.Multiply(Helper.RadiansToVector(rotation), offset), position));
        }
        /// <summary>
        /// Calculate the angle between the center of the orbit and a position on the orbit.
        /// </summary>
        /// <param name="center">The center of the orbit.</param>
        /// <param name="orbit">The orbiting position.</param>
        /// <returns>The angle between the two positions.</returns>
        public static float CalculateAngleFromOrbitPosition(Vector2 center, Vector2 orbit)
        {
            //The direction.
            Vector2 direction = Vector2.Normalize(orbit - center);
            //Return the angle.
            return ((float)Math.Atan2(direction.X, -direction.Y));
        }
        /// <summary>
        /// Calculate the angle between the center of the orbit and a position on the orbit, currently only used for sprite to bone.
        /// </summary>
        /// <param name="center">The center of the orbit.</param>
        /// <param name="orbit">The orbiting position.</param>
        /// <returns>The angle between the two positions.</returns>
        public static float CalculateAngleFromOrbitPositionBone(Vector2 center, Vector2 orbit)
        {
            //The direction.
            Vector2 direction = Vector2.Normalize(orbit - center);
            //Return the angle.
            return ((float)Math.Atan2(direction.X, direction.Y));
        }
        /// <summary>
        /// Calculate the rotation offset between the angle of a bone and its sprite.
        /// </summary>
        /// <param name="origin">The end position of the bone.</param>
        /// <param name="end">The origin of the bone.</param>
        /// <returns>The angle between the two positions expressed as a viable rotation offset.</returns>
        public static float CalculateRotationOffset(Vector2 origin, Vector2 end)
        {
            //The direction.
            Vector2 direction = Vector2.Normalize(origin - end);
            //Return the angle.
            return ((float)Math.Atan2(direction.X, direction.Y));
        }
        /// <summary>
        /// Save an animation as a xml file.
        /// </summary>
        /// <param name="animation">The animation to save.</param>
        public static void SaveAnimation(Animation animation)
        {
            //The path.
            //string tempPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "temp"));
            //Directory.CreateDirectory(tempPath);
            //string fileName = Path.Combine(tempPath, "temp.anim");

            //Save the animation.
            //SaveAnimation(animation, 
        }
        /// <summary>
        /// Save an animation as a xml file.
        /// </summary>
        /// <param name="animation">The animation to save.</param>
        /// <param name="path">Where to save the file.</param>
        public static void SaveAnimation(Animation animation, string path)
        {
            //Create the xml writer.
            XmlTextWriter textWriter = new XmlTextWriter(path + animation.Name + ".anim", null);
            //Set the formatting to use indent.
            textWriter.Formatting = Formatting.Indented;

            //Begin with the animation.
            textWriter.WriteStartDocument();
            textWriter.WriteStartElement("Animation");
            textWriter.WriteAttributeString("Name", animation.Name);

            //The frametime.
            textWriter.WriteStartElement("FrameTime");
            textWriter.WriteString(animation.FrameTime.ToString());
            textWriter.WriteEndElement();
            //The number of frames.
            textWriter.WriteStartElement("NumberOfFrames");
            textWriter.WriteString(animation.NumberOfFrames.ToString());
            textWriter.WriteEndElement();
            //The keyframes.
            foreach (Keyframe keyframe in animation.Keyframes)
            {
                //Begin with the keyframe.
                textWriter.WriteStartElement("Keyframe");

                //The frame number.
                textWriter.WriteAttributeString("FrameNumber", keyframe.FrameNumber.ToString());
                //The bones.
                foreach (Bone bone in keyframe.BonesToBe)
                {
                    //Begin with the bone.
                    textWriter.WriteStartElement("Bone");
                    textWriter.WriteAttributeString("Name", bone.Name);

                    //The index.
                    textWriter.WriteStartElement("Index");
                    textWriter.WriteString(bone.Index.ToString());
                    textWriter.WriteEndElement();
                    //The parent index.
                    textWriter.WriteStartElement("ParentIndex");
                    textWriter.WriteString(bone.ParentIndex.ToString());
                    textWriter.WriteEndElement();
                    //The absolute position.
                    textWriter.WriteStartElement("Position");
                    textWriter.WriteStartElement("X");
                    textWriter.WriteString(bone.AbsolutePosition.X.ToString());
                    textWriter.WriteEndElement();
                    textWriter.WriteStartElement("Y");
                    textWriter.WriteString(bone.AbsolutePosition.Y.ToString());
                    textWriter.WriteEndElement();
                    textWriter.WriteEndElement();
                    //The absolute rotation.
                    textWriter.WriteStartElement("Rotation");
                    textWriter.WriteString(bone.AbsoluteRotation.ToString());
                    textWriter.WriteEndElement();
                    //The scale.
                    textWriter.WriteStartElement("Scale");
                    textWriter.WriteStartElement("X");
                    textWriter.WriteString(bone.Scale.X.ToString());
                    textWriter.WriteEndElement();
                    textWriter.WriteStartElement("Y");
                    textWriter.WriteString(bone.Scale.Y.ToString());
                    textWriter.WriteEndElement();
                    textWriter.WriteEndElement();
                    //The length.
                    textWriter.WriteStartElement("Length");
                    textWriter.WriteString(bone.Length.ToString());
                    textWriter.WriteEndElement();

                    //End with the bone.
                    textWriter.WriteEndElement();
                }

                //End with the keyframe.
                textWriter.WriteEndElement();
            }

            //End with the animation.
            textWriter.WriteEndElement();
            //End with the document.
            textWriter.WriteEndDocument();
            //Close the writer.
            textWriter.Close();
        }
        /// <summary>
        /// Save a skeleton to an xml file.
        /// </summary>
        /// <param name="skeleton">The skeleton to save.</param>
        /// <param name="path">Where to save the file.</param>
        public static void SaveSkeleton(Skeleton skeleton, string path)
        {
            //Create the xml writer.
            XmlTextWriter textWriter = new XmlTextWriter(path + skeleton.Name + ".skel", null);
            //Set the formatting to use indent.
            textWriter.Formatting = Formatting.Indented;

            //Begin with the skeleton.
            textWriter.WriteStartDocument();
            textWriter.WriteStartElement("Skeleton");

            //Begin with the bones.
            textWriter.WriteStartElement("Bones");

            //The bones.
            foreach (Bone bone in skeleton.Bones)
            {
                //Begin with the bone.
                textWriter.WriteStartElement("Bone");
                textWriter.WriteAttributeString("Name", bone.Name);

                //The index.
                textWriter.WriteStartElement("Index");
                textWriter.WriteString(bone.Index.ToString());
                textWriter.WriteEndElement();
                //The parent index.
                textWriter.WriteStartElement("ParentIndex");
                textWriter.WriteString(bone.ParentIndex.ToString());
                textWriter.WriteEndElement();
                //The absolute position.
                textWriter.WriteStartElement("Position");
                textWriter.WriteStartElement("X");
                textWriter.WriteString(bone.AbsolutePosition.X.ToString());
                textWriter.WriteEndElement();
                textWriter.WriteStartElement("Y");
                textWriter.WriteString(bone.AbsolutePosition.Y.ToString());
                textWriter.WriteEndElement();
                textWriter.WriteEndElement();
                //The absolute rotation.
                textWriter.WriteStartElement("Rotation");
                textWriter.WriteString(bone.AbsoluteRotation.ToString());
                textWriter.WriteEndElement();
                //The scale.
                textWriter.WriteStartElement("Scale");
                textWriter.WriteStartElement("X");
                textWriter.WriteString(bone.Scale.X.ToString());
                textWriter.WriteEndElement();
                textWriter.WriteStartElement("Y");
                textWriter.WriteString(bone.Scale.Y.ToString());
                textWriter.WriteEndElement();
                textWriter.WriteEndElement();
                //The length.
                textWriter.WriteStartElement("Length");
                textWriter.WriteString(bone.Length.ToString());
                textWriter.WriteEndElement();

                //End with the bone.
                textWriter.WriteEndElement();
            }

            //End with all bones.
            textWriter.WriteEndElement();

            //Begin with the animations.
            textWriter.WriteStartElement("Animations");

            //The animations.
            foreach (Animation animation in skeleton.Animations)
            {
                //Begin with the animation.
                textWriter.WriteStartElement("Animation");
                textWriter.WriteAttributeString("Name", animation.Name);
                textWriter.WriteEndElement();

                //Save the animation as well.
                SaveAnimation(animation, path);
            }

            //End with all animations.
            textWriter.WriteEndElement();

            //Begin with the sprites.
            textWriter.WriteStartElement("Sprites");

            //The sprites.
            foreach (Sprite sprite in skeleton.Sprites.Sprites)
            {
                //Begin with the animation.
                textWriter.WriteStartElement("Sprite");
                textWriter.WriteAttributeString("Name", sprite.Name);

                //The tag.
                textWriter.WriteStartElement("Tag");
                textWriter.WriteString(sprite.Tag);
                textWriter.WriteEndElement();
                //The rotation offset.
                textWriter.WriteStartElement("RotationOffset");
                textWriter.WriteString(sprite.RotationOffset.ToString());
                textWriter.WriteEndElement();

                //Begin with the frames.
                textWriter.WriteStartElement("Frames");

                //The frames.
                foreach (Frame frame in sprite.Frames)
                {
                    //Begin with the frame.
                    textWriter.WriteStartElement("Frame");
                    textWriter.WriteAttributeString("Name", frame.Path);

                    //The origin.
                    textWriter.WriteStartElement("Origin");
                    textWriter.WriteStartElement("X");
                    textWriter.WriteString(frame.Origin.X.ToString());
                    textWriter.WriteEndElement();
                    textWriter.WriteStartElement("Y");
                    textWriter.WriteString(frame.Origin.Y.ToString());
                    textWriter.WriteEndElement();
                    textWriter.WriteEndElement();

                    //End the frame.
                    textWriter.WriteEndElement();
                }

                //End with all frames.
                textWriter.WriteEndElement();
                textWriter.WriteEndElement();
            }

            //End with all the sprites.
            textWriter.WriteEndElement();

            //End with the animation.
            textWriter.WriteEndElement();
            //End with the document.
            textWriter.WriteEndDocument();
            //Close the writer.
            textWriter.Close();
        }
        /// <summary>
        /// Load an animation and add it to a skeleleton.
        /// </summary>
        /// <param name="skeleton">The skeleton that will house the animation.</param>
        /// <param name="path">From where to load the file.</param>
        /// <returns>The loaded animation.</returns>
        public static Animation LoadAnimation(Skeleton skeleton, string path)
        {
            try
            {
                //Create the animation and clear the list of keyframes.
                Animation animation = new Animation(skeleton);
                animation.Keyframes.Clear();
                //Set up and load the xml file.
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(path + ".anim");

                //Parse the xml data.
                animation.Name = xmlDocument.SelectSingleNode("/Animation").Attributes["Name"].Value;
                animation.FrameTime = float.Parse(xmlDocument.SelectSingleNode("/Animation/FrameTime").InnerText);
                animation.NumberOfFrames = int.Parse(xmlDocument.SelectSingleNode("/Animation/NumberOfFrames").InnerText);

                //The keyframes.
                foreach (XmlNode keyframeNode in xmlDocument.SelectNodes("/Animation/Keyframe"))
                {
                    //Add a keyframe to the animation.
                    Keyframe keyframe = new Keyframe(Int32.Parse(keyframeNode.Attributes["FrameNumber"].Value));

                    //The bones.
                    foreach (XmlNode boneNode in keyframeNode.SelectNodes("Bone"))
                    {
                        //Parse the xml data.
                        string name = boneNode.Attributes["Name"].Value;
                        int index = int.Parse(boneNode.SelectSingleNode("Index").InnerText);
                        int parentIndex = int.Parse(boneNode.SelectSingleNode("ParentIndex").InnerText);
                        Vector2 position = new Vector2(float.Parse(boneNode.SelectSingleNode("Position/X").InnerText),
                            float.Parse(boneNode.SelectSingleNode("Position/Y").InnerText));
                        float rotation = float.Parse(boneNode.SelectSingleNode("Rotation").InnerText);
                        Vector2 scale = new Vector2(float.Parse(boneNode.SelectSingleNode("Scale/X").InnerText),
                            float.Parse(boneNode.SelectSingleNode("Scale/Y").InnerText));
                        float length = float.Parse(boneNode.SelectSingleNode("Length").InnerText);

                        //Add the loaded bone to the keyframe.
                        keyframe.AddBone(new Bone(skeleton, name, index, parentIndex, position, scale, rotation, length));

                        //The code below is only a temporary fix and corrects a problem with relative bone rotations in a keyframe.
                        try
                        {
                            Bone bone = keyframe.BonesToBe[keyframe.BonesToBe.Count - 1];
                            Keyframe prev = keyframe.BonesToBe.Exists(b => (parentIndex == b.Index)) ? keyframe : animation.GetPreviousKeyframe(parentIndex, keyframe.FrameNumber + 1);
                            Bone parent = prev.GetBone(parentIndex);

                            bone.UpdateRelativeRotation(parent.AbsoluteRotation);
                        }
                        catch { }
                    }

                    //Add the keyframe to the animation.
                    animation.AddKeyframe(keyframe);
                }

                //Let the skeleton know of its new animation.
                skeleton.AddAnimation(animation);

                //Return the loaded animation.
                return animation;
            }
            catch { return null; }
        }
        /// <summary>
        /// Load a skeleton.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="path">From where to load the file.</param>
        /// <returns>The loaded skeleton.</returns>
        public static Skeleton LoadSkeleton(GraphicsDevice graphicsDevice, string path)
        {
            //Create the skeleton.
            Skeleton skeleton = new Skeleton(graphicsDevice);
            //Set up and load the xml file.
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(path + ".skel");

            //The bones.
            foreach (XmlNode boneNode in xmlDocument.SelectNodes("/Skeleton/Bones/Bone"))
            {
                //Parse the xml data.
                string name = boneNode.Attributes["Name"].Value;
                int index = int.Parse(boneNode.SelectSingleNode("Index").InnerText);
                int parentIndex = int.Parse(boneNode.SelectSingleNode("ParentIndex").InnerText);
                Vector2 position = new Vector2(float.Parse(boneNode.SelectSingleNode("Position/X").InnerText),
                    float.Parse(boneNode.SelectSingleNode("Position/Y").InnerText));
                float rotation = float.Parse(boneNode.SelectSingleNode("Rotation").InnerText);
                Vector2 scale = new Vector2(float.Parse(boneNode.SelectSingleNode("Scale/X").InnerText),
                    float.Parse(boneNode.SelectSingleNode("Scale/Y").InnerText));
                float length = float.Parse(boneNode.SelectSingleNode("Length").InnerText);

                //Add the loaded bone to the skeleton.
                Factory.Instance.AddBone(skeleton, name, index, parentIndex, position, scale, rotation, length);
            }

            //The animations.
            foreach (XmlNode node in xmlDocument.SelectNodes("/Skeleton/Animations/Animation"))
            {
                //Parse the xml data.
                string name = node.Attributes["Name"].Value;

                //Load the animation.
                Animation animation = LoadAnimation(skeleton, (path.Replace(skeleton.Name + ".skel", "") + name));
                //Add the loaded bone to the skeleton.
                skeleton.AddAnimation(animation != null ? animation : new Animation(skeleton));
            }

            //The sprites.
            foreach (XmlNode spriteNode in xmlDocument.SelectNodes("/Skeleton/Sprites/Sprite"))
            {
                //Parse the xml data.
                string spriteName = spriteNode.Attributes["Name"].Value;
                string tag = spriteNode.SelectSingleNode("Tag").InnerText;
                float rotationOffset = float.Parse(spriteNode.SelectSingleNode("RotationOffset").InnerText);

                //Add a sprite to the skeleton.
                Sprite sprite = skeleton.Sprites.Add(new Sprite(skeleton.Sprites, spriteName));
                sprite.RotationOffset = rotationOffset;
                sprite.Tag = tag;

                //The frames.
                foreach (XmlNode frameNode in spriteNode.SelectNodes("Frames/Frame"))
                {
                    //Parse the xml data.
                    string name = frameNode.Attributes["Name"].Value;
                    Vector2 origin = new Vector2(float.Parse(frameNode.SelectSingleNode("Origin/X").InnerText),
                        float.Parse(frameNode.SelectSingleNode("Origin/Y").InnerText));

                    //Add the frame to the sprite.
                    sprite.AddFrame(name, origin);
                }
            }

            //Return the loaded skeleton.
            return skeleton;
        }
        #endregion
    }
}
