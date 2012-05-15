using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Library.GUI.Basic;

namespace Library.GUI
{
    /// <summary>
    /// Compares two GUI components by draw order.
    /// </summary>
    public class ComponentComparer : IComparer<Component>
    {
        #region Constructors
        /// <summary>
        /// Create a Component comparer.
        /// </summary>
        public ComponentComparer() { }
        #endregion

        #region Methods
        /// <summary>
        /// Compares two GUI components by descending draw order, ie. the item with the smallest draw order is last.
        /// </summary>
        /// <param name="x">The first item.</param>
        /// <param name="y">The second item.</param>
        /// <returns>An integer explaining the result.</returns>
        int IComparer<Component>.Compare(Component x, Component y)
        {
            //The lists of hierarchical draw orders.
            List<int> drawOrder1 = GetDrawOrderHierarchy(x);
            List<int> drawOrder2 = GetDrawOrderHierarchy(y);

            //For each entry in the lists.
            for (int i = 0; i < (drawOrder1.Count < drawOrder2.Count ? drawOrder1.Count : drawOrder2.Count); i++)
            {
                //See which one of the items come out on top. Return the one who did.
                if (drawOrder1[i] > drawOrder2[i]) { return -1; }
                else if (drawOrder1[i] < drawOrder2[i]) { return 1; }
            }

            //If the items had exactly equal draw orders, choose the 'youngest' child.
            if (drawOrder1.Count == drawOrder2.Count) { return 0; }
            else { return (drawOrder1.Count < drawOrder2.Count) ? -1 : 1; }
        }
        /// <summary>
        /// Get a list of an item's draw orders, beginning with the topmost parent.
        /// </summary>
        /// <param name="item">The item in question.</param>
        /// <returns>A list of drawing orders for a family of items.</returns>
        private List<int> GetDrawOrderHierarchy(Component item)
        {
            //The list of draw orders to return.
            List<int> drawOrder = new List<int>();
            //The parent item.
            Component parent = item;

            //While the parent item isn't null, continue.
            while (parent != null)
            {
                //Add the draw order and goto the next parent.
                drawOrder.Add(parent.DrawOrder);
                parent = parent.Parent;
            }

            //Reverse the list so that the topmost parent is first.
            drawOrder.Reverse();

            //return the list.
            return drawOrder;
        }
        #endregion
    }
}
