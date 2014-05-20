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
        /// The IQuadTreeNode in the bottom left part of the QuadTreeBranch.
        /// </summary>
        public QuadTreeNode<TContent, TAverage> BottomLeft { get; private set; }

        /// <summary>
        /// The IQuadTreeNode in the bottom right part of the QuadTreeBranch.
        /// </summary>
        public QuadTreeNode<TContent, TAverage> BottomRight { get; private set; }

        /// <summary>
        /// The IQuadTreeNode in the top left part of the QuadTreeBranch.
        /// </summary>
        public QuadTreeNode<TContent, TAverage> TopLeft { get; private set; }

        /// <summary>
        /// The IQuadTreeNode in the top right part of the QuadTreeBranch.
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
        public QuadTreeBranch(QuadTreeController<TContent, TAverage> controller,
            QuadTreeNode<TContent, TAverage> topRight, QuadTreeNode<TContent, TAverage> bottomRight,
            QuadTreeNode<TContent, TAverage> bottomLeft, QuadTreeNode<TContent, TAverage> topLeft)
            : base(controller)
        {
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
            //Change pattern with example from http://blogs.msdn.com/b/wesdyer/archive/2007/03/23/all-about-iterators.aspx ?

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
            //Change pattern with example from http://blogs.msdn.com/b/wesdyer/archive/2007/03/23/all-about-iterators.aspx ?

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
        /// Splits the given leaf at the given point and returns itself (with changes, if they happened).
        /// </summary>
        /// <param name="leaf">The leaf supposed to be split.</param>
        /// <param name="x">The x coordinate of the point.</param>
        /// <param name="y">The y coordinate of the point.</param>
        /// <returns>Itself (with changes, if they happened).</returns>
        public override QuadTreeNode<TContent, TAverage> Split(QuadTreeNode<TContent, TAverage> leaf, double x, double y)
        {
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
        /// Calculates the average-value with the average-values of the children and sets the Average with it.
        /// </summary>
        protected override void calculateAverage()
        {
            //Nested call that uses the last one's return value as aggregator; because I can.
            Average = controller.AggregateAverages(TopLeft.Average,
                controller.AggregateAverages(BottomLeft.Average,
                    controller.AggregateAverages(BottomRight.Average,
                        TopRight.Average)));
        }

        /// <summary>
        /// Sets the size properties from the 4 children.
        /// </summary>
        private void setSize()
        {
            XMin = Math.Min(TopLeft.XMin,
                Math.Min(BottomLeft.XMin,
                    Math.Min(BottomRight.XMin,
                        TopRight.XMin)));

            XMax = Math.Max(TopLeft.XMax,
                Math.Max(BottomLeft.XMax,
                    Math.Max(BottomRight.XMax,
                        TopRight.XMax)));

            YMin = Math.Min(TopLeft.YMin,
                Math.Min(BottomLeft.YMin,
                    Math.Min(BottomRight.YMin,
                        TopRight.YMin)));

            YMax = Math.Max(TopLeft.YMax,
                Math.Max(BottomLeft.YMax,
                    Math.Max(BottomRight.YMax,
                        TopRight.YMax)));
        }
    }
}