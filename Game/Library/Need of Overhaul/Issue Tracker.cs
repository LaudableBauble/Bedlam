/// SYSTEM
/// GetID method: The GetId method adds the object which it shouldn't.
///
/// SPRITECOLLECTION
/// Main Methods: Discard the "Sprite" suffix to avoid confusion. - DONE!
/// Why's there math functions in the SpriteCollection class?
/// The SpriteCount variable may be obsolete. Use the List's Count function instead. - DONE!
/// Instead of using a fixed depth value when drawing use a dynamic one instead.
/// Ponder either introducing a body system or a go-by-name system to find sprites. - DONE!
/// Perhaps introduce an Indexer to the class in order to make it easier to find sprites. - DONE!
/// Initialize the sprites and Load all their content. - DONE!
///
/// SPRITE
/// Why's there math functions in the Sprite class?
/// Add a rotation offset variable that offsets the rotation by a certain amount every update. - DONE!
/// Distincuate the difference towards the other rotaion offset variable. - DONE!
/// See if it's not more beneficial to use lists instead of arrays. - DONE!
/// Perhaps rework the way the Sprite class works by using more classes. - DONE!
/// The FrameCount variable may be obsolete. Use the List's Count function instead. - DONE!
/// Relay the SpriteBatch and ContentManager to each Sprite instead of storing them at site. - DONE!
/// Perhaps introduce an Indexer to the class in order to make it easier to find frames. - DONE!
///
/// OBJECT
/// Put the bodies and geoms in lists instead of an array. - DONE!
/// AddBody: There's a runtime error in the method. - DONE!
/// The BodyCount variable may be obsolete. Use the List's Count function instead. - DONE!
/// The GeomCount variable may be obsolete. Use the List's Count function instead. - DONE!
/// 
/// TREE VIEW
/// All node's checkboxes and buttons are not positioned correctly at the initialization.
///
/// SPARTAN
/// Remove the weapon index variables as it's easier to search by tag.
///
/// ITEM
/// Remove the ModifiedFlag for a more robust event system.
/// 
/// BONE
/// Refactor the updating of rotation, direction and position between the relative and absolute formats.
/// Take a look at how the index of a bone is given to it. It seems awfully complicated as it is.
///
/// ENTITY
/// Move the skeleton from the character class into the entity class instead, which makes it easier to load animated objects from file.
/// 
/// FORM
/// Look over at how the form class is being used. The code seems to clash with my idea.
///
/// GENERAL
/// The different rotational formats are driving me crazy! Go through them and design a system that works.
/// The process of equipping a weapon is unnecessary complex. Simplify it, will ya'?
/// Use the reference mode instead of the hardware mode when dealing with graphics. (Temporary) - DONE!
/// Change the way method overloading is set up at the moment. They shouldn't be so seperate.
/// When drawing a sprite there's no benefit of checking their visibility twice.
/// Classes lack properties and documentation.
/// Fix the custom content pipeline error regarding loading animations from file.