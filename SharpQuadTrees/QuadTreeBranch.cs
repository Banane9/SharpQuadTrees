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
        /// Not supported.
        /// </summary>
        /// <returns></returns>
        public override QuadTreeNode<TContent, TAverage> Split()
        {
            throw new NotSupportedException("This node is already split.");
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override QuadTreeNode<TContent, TAverage> Split(double x, double y)
        {
            throw new NotSupportedException("This node is already split.");
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