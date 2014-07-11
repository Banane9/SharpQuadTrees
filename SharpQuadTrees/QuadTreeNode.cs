using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpQuadTrees
{
    /// <summary>
    /// Contains properties and functions that make it suitable as part of a quad tree.
    /// </summary>
    /// <typeparam name="TContent">The type of the items that are stored in the quad tree.</typeparam>
    /// <typeparam name="TAverage">The type used for averaging the values of the content items.</typeparam>
    public abstract class QuadTreeNode<TContent, TAverage>
    {
        /// <summary>
        /// Backing field for the Average property.
        /// </summary>
        private TAverage average;

        /// <summary>
        /// Whether the Average property was calculated with current information.
        /// </summary>
        private bool isAverageCurrent = false;

        /// <summary>
        /// Gets the average-value of the content. Or the controller's NoContentAverage if there's no content.
        /// </summary>
        public TAverage Average
        {
            get
            {
                if (!isAverageCurrent)
                    calculateAverage();

                return average;
            }
            set
            {
                average = value;
                validateAverage();
            }
        }

        /// <summary>
        /// Gets the x coordinate of the quad's center.
        /// </summary>
        public double XCenter
        {
            get { return (XMin + XMax) / 2; }
        }

        /// <summary>
        /// Gets the exclusive maximum value for the x coordinate.
        /// </summary>
        public double XMax { get; protected set; }

        /// <summary>
        /// Gets the inclusive minimum value for the x coordinate.
        /// </summary>
        public double XMin { get; protected set; }

        /// <summary>
        /// Gets the y coordinate of the quad's center.
        /// </summary>
        public double YCenter
        {
            get { return (YMin + YMax) / 2; }
        }

        /// <summary>
        /// Gets the exclusive maximum value for the y coordinate.
        /// </summary>
        public double YMax { get; protected set; }

        /// <summary>
        /// Gets the inclusive minimum value for the y coordinate.
        /// </summary>
        public double YMin { get; protected set; }

        /// <summary>
        /// Gets the <see cref="SharpQuadTrees.QuadTreeController"/> used for handling the content.
        /// </summary>
        protected IQuadTreeController<TContent, TAverage> controller { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="SharpQuadTrees.QuadTreeNode"/> class with the given controller.
        /// Only available in derived classes.
        /// </summary>
        /// <param name="controller">Controller used for handling the content.</param>
        protected QuadTreeNode(IQuadTreeController<TContent, TAverage> controller)
        {
            if (controller == null)
                throw new ArgumentNullException("controller", "Controller can't be null.");

            this.controller = controller;
        }

        /// <summary>
        /// Returns whether all of the given items would be inside the node.
        /// </summary>
        /// <param name="items">The items to check.</param>
        /// <returns>Whether all of the items would be inside the node.</returns>
        public bool AreInsideNode(IEnumerable<TContent> items)
        {
            return items.Any(item => !IsInsideNode(item));
        }

        /// <summary>
        /// Gets an IEnumerable of content items.
        /// </summary>
        /// <returns>IEnumerable of the content items.</returns>
        public abstract IEnumerable<TContent> GetContent();

        /// <summary>
        /// Gets an IEnumerable of the nodes that don't have children (leafs).
        /// </summary>
        /// <returns>IEnumerable of quad tree leafs.</returns>
        public abstract IEnumerable<QuadTreeNode<TContent, TAverage>> GetLeafs();

        /// <summary>
        /// Returns whether the given point is inside the node.
        /// </summary>
        /// <param name="x">The x coordinate of the point.</param>
        /// <param name="y">The y coordinate of the point.</param>
        /// <returns>Whether the point is inside the node.</returns>
        public bool IsInsideNode(double x, double y)
        {
            //min is inclusive, max exclusive
            return XMin <= x && XMax > x && YMin <= y && YMax > y;
        }

        /// <summary>
        /// Return whether the given item would be inside the node.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Whether the item would be inside the node.</returns>
        public bool IsInsideNode(TContent item)
        {
            return IsInsideNode(controller.GetContentX(item), controller.GetContentY(item));
        }

        /// <summary>
        /// Splits the given leaf at the given point and returns the resulting node (itself if it wasn't split, or the branch resulting from the split).
        /// </summary>
        /// <param name="leaf">The leaf supposed to be split.</param>
        /// <param name="x">The x coordinate of the point.</param>
        /// <param name="y">The y coordinate of the point.</param>
        /// <returns>The resulting node (itself if it wasn't split, or the branch resulting from the split).</returns>
        public abstract QuadTreeNode<TContent, TAverage> Split(QuadTreeNode<TContent, TAverage> leaf, double x, double y);

        /// <summary>
        /// Splits leaf(s) based on the controller.
        /// </summary>
        /// <returns>The node resulting from the split (itself if it wasn't split, or the branch resulting from the split).</returns>
        public abstract QuadTreeNode<TContent, TAverage> Split();

        /// <summary>
        /// Splits the leaf node that contains the point at the given point and returns the resulting node (itself if it wasn't split, or the branch resulting from the split).
        /// </summary>
        /// <param name="x">The x coordinate of the point.</param>
        /// <param name="y">The y coordinate of the point.</param>
        /// <returns>The node resulting from the split (itself if it wasn't split, or the branch resulting from the split).</returns>
        public abstract QuadTreeNode<TContent, TAverage> Split(double x, double y);

        /// <summary>
        /// Calculates the average-value of the content and sets the Average property to it.
        /// </summary>
        protected abstract void calculateAverage();

        /// <summary>
        /// Marks the Average property as not being calculated with current information.
        /// </summary>
        protected void invalidateAverage()
        {
            isAverageCurrent = false;
        }

        /// <summary>
        /// Throws a descriptive ArgumentOutOfRangeException when the given point is outside of this node.
        /// </summary>
        /// <param name="x">The x coordinate of the point.</param>
        /// <param name="y">The y coordinate of the point.</param>
        protected void throwWhenOutsideNode(double x, double y)
        {
            //a coordinate has to be less than or equal to its max - double.Epsilon, because a content item's maximum coordinate can be max - double.Epsilon
            //so to ensure that there can be a content item in the greater-than-part the division must be double.Epsilon before the max of the axis.
            //For the less-than-part it must be at least min + double.Epsilon, so <= is enough.
            if ((x <= XMin || x >= (XMax - double.Epsilon)) && (y <= YMin || y >= (YMax - double.Epsilon)))
                throw new ArgumentOutOfRangeException("x & y", "Both coordinates of the point weren't inside the range of this QuadTreeLeaf.");
            else if (x <= XMin || x >= XMax)
                throw new ArgumentOutOfRangeException("x", "The x coordinate of the point wasn't inside the range of this QuadTreeLeaf.");
            else if (y <= YMin || y >= YMax)
                throw new ArgumentOutOfRangeException("y", "The y coordinate of the point wasn't inside the range of this QuadTreeLeaf.");
        }

        /// <summary>
        /// Marks the Average property as having been calculated with current information.
        /// </summary>
        private void validateAverage()
        {
            isAverageCurrent = true;
        }
    }
}