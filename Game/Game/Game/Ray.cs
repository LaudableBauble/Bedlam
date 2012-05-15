using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Controllers;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Dynamics.Joints;
using FarseerGames.FarseerPhysics.Dynamics.Springs;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics.Interfaces;
using FarseerGames.FarseerPhysics.Mathematics;

namespace Halo_Game
{
    class Ray
    {
        public struct IntersecInfo
        {
            public Vector2 point;
            public Vector2 p1;
            public Vector2 p2;

            public Geom geom;
        }

        /// <summary>
        /// Checks if a floating point value is equal to another,
        /// within a certain tolerance.
        /// </summary>
        /// <param name="a">The first floating point value.</param>
        /// <param name="b">The second floating point value.</param>
        /// <param name="delta">The floating point tolerance.</param>
        /// <returns>True if the values are "equal", false otherwise.</returns>
        private static bool FloatEquals(float a, float b, float delta)
        {
            return FloatInRange(a, b - delta, b + delta);
        }

        /// <summary>
        /// Checks if a floating point value is within a specified
        /// range of values (inclusive).
        /// </summary>
        /// <param name="a">The value to check.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>True if the value is within the range specified,
        /// false otherwise.</returns>
        private static bool FloatInRange(float a, float min, float max)
        {
            return (a >= min && a <= max);
        }

        /// <summary>
        /// This method detects if two line segments (or lines) intersect,
        /// and, if so, the point of intersection. Use the onFirst and
        /// onSecond parameters to set whether the intersection point
        /// must be on the first and second line segments. Setting these
        /// both to true means you are doing a line-segment to line-segment
        /// intersection. Setting one of them to true means you are doing a
        /// line to line-segment intersection test, and so on.
        /// Note: If two line segments are coincident, then 
        /// no intersection is detected (there are actually
        /// infinite intersection points).
        /// Author: Jeremy Bell
        /// </summary>
        /// <param name="p1">The first point of the first line segment.</param>
        /// <param name="p2">The second point of the first line segment.</param>
        /// <param name="p3">The first point of the second line segment.</param>
        /// <param name="p4">The second point of the second line segment.</param>
        /// <param name="intersectPoint">This is set to the intersection
        /// point if an intersection is detected.</param>
        /// <param name="onFirst">Set this to true to require that the 
        /// intersection point be on the first line segment.</param>
        /// <param name="onSecond">Set this to true to require that the
        /// intersection point be on the second line segment.</param>
        /// <param name="floatTolerance">Some of the calculations require
        /// checking if a floating point value equals another. This is
        /// the tolerance that is used to determine this (ie value +
        /// or - floatTolerance)</param>
        /// <returns>True if an intersection is detected, false otherwise.</returns>
        public static bool LineIntersection(Vector2 p1, Vector2 p2,
        Vector2 p3, Vector2 p4, out Vector2 intersectPoint,
        bool onFirst, bool onSecond, float floatTolerance)
        {
            bool ret = false;
            intersectPoint = new Vector2();

            // these are reused later.
            // each lettered sub-calculation is used twice, except
            // for b and d, which are used 3 times
            float a = p4.Y - p3.Y;
            float b = p2.X - p1.X;
            float c = p4.X - p3.X;
            float d = p2.Y - p1.Y;

            // denominator to solution of linear system
            //float denom = ((p4.Y - p3.Y) * (p2.X - p1.X)) -
            // ((p4.X - p3.X) * (p2.Y - p1.Y));
            float denom = (a * b) - (c * d);

            // if denominator is 0, then lines are parallel
            if (!FloatEquals(denom, 0f, floatTolerance))
            {
                float e = p1.Y - p3.Y;
                float f = p1.X - p3.X;
                float oneOverDenom = 1.0f / denom;

                // numerator of first equation
                //float ua = ((p4.X - p3.X) * (p1.Y - p3.Y)) - 
                // ((p4.Y - p3.Y) * (p1.X - p3.X));
                float ua = (c * e) - (a * f);
                ua *= oneOverDenom;

                // check if intersection point of the two lines is on line segment 1
                if (!onFirst || FloatInRange(ua, 0f, 1f))
                {
                    // numerator of second equation
                    //float ub = ((p2.X - p1.X) * (p1.Y - p3.Y)) - 
                    // ((p2.Y - p1.Y) * (p1.X - p3.X));
                    float ub = (b * e) - (d * f);
                    ub *= oneOverDenom;

                    // check if intersection point of the two lines is on line segment 2
                    // means the line segments intersect, since we know it is on
                    // segment 1 as well.
                    if (!onSecond || FloatInRange(ub, 0f, 1f))
                    {
                        // check if they are coincident (no collision in this case)
                        if (!(FloatEquals(ua, 0f, floatTolerance) &&
                        FloatEquals(ub, 0f, floatTolerance)))
                        {
                            ret = true;
                            //intersectPoint.X = p1.X + ua * (p2.X - p1.X);
                            //intersectPoint.Y = p1.Y + ua * (p2.Y - p1.Y);

                            intersectPoint.X = p1.X + ua * b;
                            intersectPoint.Y = p1.Y + ua * d;
                        } // end if
                    } // end if
                } // end if
            }

            return ret;
        }

        /// <summary>
        /// Get all intersections between a line segment and a list of
        /// vertices representing a polygon. The vertices reuse adjacent
        /// points, so for example edges one and two are between the first
        /// and second vertices and between the second and third vertices.
        /// The last edge is between vertex vertsverts.Count - 1 and verts0.
        /// (ie, vertices from a Geometry or AABB)
        /// </summary>
        /// <param name="p1">The first point of the line segment to test</param>
        /// <param name="p2">The second point of the line segment to test.</param>
        /// <param name="verts">The vertices, as described above</param>
        /// <param name="onFirst">Whether the intersection need be on the line segment.</param>
        /// <param name="infosOut">An existing list of intersections to add to</param>
        /// <param name="g">A geometry to assign to the info.geom parameter.</param>
        public static void LineSegmentVerticiesIntersect(Vector2 p1, Vector2 p2,
        Vertices verts, bool onFirst, ref List<IntersecInfo> infosOut, Geom g)
        {
            for (int i = 0; i < verts.Count; i++)
            {
                Vector2 intersect;
                int nextIndex = (i == verts.Count - 1 ? 0 : i + 1);
                if (LineIntersection(verts[i], verts[nextIndex],
                p1, p2, out intersect, onFirst, true, 0.00001f))
                {
                    IntersecInfo info = new IntersecInfo();
                    info.point = intersect;
                    info.p1 = verts[i];
                    info.p2 = verts[nextIndex];
                    info.geom = g;

                    infosOut.Add(info);
                }
            }
        }

        /// <summary>
        /// Add intersections between a line segment and a geometry's 
        /// Axis Aligned Bounding box to an existing list of 
        /// intersection infos.
        /// </summary>
        /// <param name="p1">First point of the line segment to test</param>
        /// <param name="p2">Second point of the line segment to test</param>
        /// <param name="geom">The geometry who's AABB you wish to test.</param>
        /// <param name="onFirst">Whether the collision needs to be on the segment, or just the line</param>
        /// <param name="infosOut">An existing list of intersection infos</param>
        public static void LineSegmentAABBIntersect(Vector2 p1, Vector2 p2,
        Geom geom, bool onFirst, ref List<IntersecInfo> infosOut)
        {
            LineSegmentVerticiesIntersect(p1, p2, geom.AABB.GetVertices(), onFirst, ref infosOut, geom);
        }

        /// <summary>
        /// Add all intersection infos from all intersections between a line segment
        /// and all of the edges of a geometry. Slower than AABB but more exact.
        /// </summary>
        /// <param name="p1">The first point of the line segment to test</param>
        /// <param name="p2">The second point of the line segment to test</param>
        /// <param name="geom">The geometry to test.</param>
        /// <param name="onFirst">Whether intersections need to be on the line segment or not.</param>
        /// <param name="infosOut">An existing intersect info list to add to</param>
        public static void LineSegmentGeomIntersect(Vector2 p1, Vector2 p2,
        Geom geom, bool onFirst, ref List<IntersecInfo> infosOut)
        {
            LineSegmentVerticiesIntersect(p1, p2, geom.WorldVertices, onFirst, ref infosOut, geom);
        }

    }
}
