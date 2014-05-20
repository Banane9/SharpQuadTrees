using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpQuadTrees
{
    /// <summary>
    /// Represents a node in a quad tree that has 4 (hence quad) children.
    /// </summary>
    /// <typeparam name="TContent">The type of the items that are stored in the quad tree.</typeparam>
    /// <typeparam name="TAverage">The type used for averaging the values of the content items.</typeparam>
    public class QuadTreeBranch<TContent, TAverage> : QuadTreeNode<TContent, TAverage>
    {
        /// <summary>
        /// The QuadTreeNode in the bottom left part of the QuadTreeBranch.
        /// </summary>
        public QuadTreeNode<TContent, TAverage> BottomLeft { get; private set; }

        /// <summary>
        /// The QuadTreeNode in the bottom right part of the QuadTreeBranch.
        /// </summary>
        public QuadTreeNode<TContent, TAverage> BottomRight { get; private set; }

        /// <summary>
        /// The QuadTreeNode in the top left part of the QuadTreeBranch.
        /// </summary>
        public QuadTreeNode<TContent, TAverage> TopLeft { get; private set; }

        /// <summary>
        /// The QuadTreeNode in the top right part of the QuadTreeBranch.
        /// </summary>
        public QuadTreeNode<TContent, TAverage> TopRight { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="SharpQuadTrees.QuadTreeBranch"/> class with the given <see cref="SharpQuadTrees.QuadTreeLeaf"/>s and average-aggrigation function
        /// </summary>
        /// <param name="controller">Controller used for handling the content.</param>
        /// <param name="topRight">The QuadTreeLeaf in the top right part of the QuadTreeBranch.</param>
        /// <param name="bottomRight">The QuadTreeLeaf in the bottom right part of the QuadTreeBranch.</param>
        /// <param name="bottomLeft">The QuadTreeLeaf in the bottom left part of the QuadTreeBranch.</param>
        /// <param name="topLeft">The QuadTreeLeaf in the top left part of the QuadTreeBranch.</param>
        public QuadTreeBranch(IQuadTreeController<TContent, TAverage> controller,
            QuadTreeNode<TContent, TAverage> topRight, QuadTreeNode<TContent, TAverage> bottomRight,
            QuadTreeNode<TContent, TAverage> bottomLeft, QuadTreeNode<TContent, TAverage> topLeft)
            : base(controller)
        {
            if (topRight == null)
                throw new ArgumentNullException("topRight", "Child node can't be null.");

            if (bottomRight == null)
                throw new ArgumentNullException("bottomRight", "Child node can't be null.");

            if (bottomLeft == null)
                throw new ArgumentNullException("bottomLeft", "Child node can't be null.");

            if (topLeft == null)
                throw new ArgumentNullException("topLeft", "Child node can't be null.");

            TopRight = topRight;
            BottomRight = bottomRight;
            BottomLeft = bottomLeft;
            TopLeft = topLeft;

            setSize();
        }

        /// <summary>
        /// Gets an IEnumerable of the branches' content items.
        /// </summary>
        /// <returns>IEnumerable of the branches' content items.</returns>
        public override IEnumerable<TContent> GetContent()
        {
            //TODO Change pattern with example from http://blogs.msdn.com/b/wesdyer/archive/2007/03/23/all-about-iterators.aspx ?

            foreach (var item in TopRight.GetContent())
                yield return item;

            foreach (var item in BottomRight.GetContent())
                yield return item;

            foreach (var item in BottomLeft.GetContent())
                yield return item;

            foreach (var item in TopLeft.GetContent())
                yield return item;
        }

        /// <summary>
        /// Gets an IEnumerable of this branch's leafs.
        /// </summary>
        /// <returns>IEnumerable of this branch's leafs.</returns>
        public override IEnumerable<QuadTreeNode<TContent, TAverage>> GetLeafs()
        {
            //TODO Change pattern with example from http://blogs.msdn.com/b/wesdyer/archive/2007/03/23/all-about-iterators.aspx ?

            foreach (var node in TopRight.GetLeafs())
                yield return node;

            foreach (var node in BottomRight.GetLeafs())
                yield return node;

            foreach (var node in BottomLeft.GetLeafs())
                yield return node;

            foreach (var node in TopLeft.GetLeafs())
                yield return node;
        }

        /// <summary>
        /// Splits leaf(s) based on the controller's GetNodesToSplit method and returns itself (with changes, if they happened).
        /// </summary>
        /// <returns>Itself (with changes, if they happened).</returns>
        public override QuadTreeNode<TContent, TAverage> Split()
        {
            foreach (var leaf in controller.GetNodesToSplit(GetLeafs()))
                Split(leaf, controller.GetSplitX(leaf), controller.GetSplitY(leaf));

            return this;
        }

        /// <summary>
        /// Splits the leaf node that contains the point at the given point and returns itself (with changes, if they happened).
        /// </summary>
        /// <param name="x">The x coordinate of the point.</param>
        /// <param name="y">The y coordinate of the point.</param>
        /// <returns>Itself (with changes, if they happened).</returns>
        public override QuadTreeNode<TContent, TAverage> Split(double x, double y)
        {
            throwWhenOutsideNode(x, y);

            if (TopRight.IsInNode(x, y))
                TopRight = TopRight.Split(x, y);

            if (BottomRight.IsInNode(x, y))
                BottomRight = BottomRight.Split(x, y);

            if (BottomLeft.IsInNode(x, y))
                BottomLeft = BottomLeft.Split(x, y);

            if (TopLeft.IsInNode(x, y))
                TopLeft = TopLeft.Split(x, y);

            return this;
        }

        /// <summary>
        /// Splits the given leaf at the given point and returns itself (with changes, if the leaf was found).
        /// </summary>
        /// <param name="leaf">The leaf supposed to be split.</param>
        /// <param name="x">The x coordinate of the point.</param>
        /// <param name="y">The y coordinate of the point.</param>
        /// <returns>Itself (with changes, if they happened).</returns>
        public override QuadTreeNode<TContent, TAverage> Split(QuadTreeNode<TContent, TAverage> leaf, double x, double y)
        {
            // Can't find a null leaf.
            if (leaf == null)
                return this;

            throwWhenOutsideNode(x, y);

            if (TopRight.IsInNode(x, y))
                TopRight = TopRight.Split(leaf, x, y);

            if (BottomRight.IsInNode(x, y))
                BottomRight = BottomRight.Split(leaf, x, y);

            if (BottomLeft.IsInNode(x, y))
                BottomLeft = BottomLeft.Split(leaf, x, y);

            if (TopLeft.IsInNode(x, y))
                TopLeft = TopLeft.Split(leaf, x, y);

            return this;
        }

        /// <summary>
        /// Calculates the average-value with the average-values of the children and sets the Average with it. Skips those with NoContentAverage as average-value.
        /// </summary>
        protected override void calculateAverage()
        {
            TAverage aggregator = controller.NoContentAverage;

            foreach (var child in enumerateChildren())
            {
                if (!child.Average.Equals(controller.NoContentAverage))
                {
                    if (!aggregator.Equals(controller.NoContentAverage))
                        aggregator = controller.AggregateAverages(child.Average, aggregator);
                    else
                        aggregator = child.Average;
                }
            }

            Average = aggregator;
        }

        /// <summary>
        /// Gets an IEnumerable of the 4 child nodes.
        /// </summary>
        /// <returns>IEnumerable of the 4 child nodes.</returns>
        protected IEnumerable<QuadTreeNode<TContent, TAverage>> enumerateChildren()
        {
            yield return TopRight;
            yield return BottomRight;
            yield return BottomLeft;
            yield return TopLeft;
        }

        /// <summary>
        /// Sets the size properties based on the 4 children.
        /// </summary>
        private void setSize()
        {
            XMin = enumerateChildren().Min(child => child.XMin);

            XMax = enumerateChildren().Max(child => child.XMax);

            YMin = enumerateChildren().Min(child => child.YMin);

            YMax = enumerateChildren().Max(child => child.YMax);
        }
    }
}