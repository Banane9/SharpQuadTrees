using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpQuadTrees
{
    public class QuadTreeBranch<TContent, TAverage>
    {
        /// <summary>
        /// Backing field for the AggregateAverages property.
        /// </summary>
        private Func<TAverage, TAverage, TAverage> aggregateAverages;

        /// <summary>
        /// Takes an average-value and the average-value aggregator, and returns the resulting average-value.
        /// </summary>
        public Func<TAverage, TAverage, TAverage> AggregateAverages
        {
            get { return aggregateAverages; }
            set
            {
                throw new NotImplementedException("Invalidate the current average-value calculation.");
            }
        }

        /// <summary>
        /// Gets the average-value of the content.
        /// </summary>
        public TAverage Average { get; private set; }

        /// <summary>
        /// The QuadTreeLeaf in the bottom left part of the QuadTreeBranch.
        /// </summary>
        public QuadTreeLeaf<TContent, TAverage> BottomLeft { get; private set; }

        /// <summary>
        /// The QuadTreeLeaf in the bottom right part of the QuadTreeBranch.
        /// </summary>
        public QuadTreeLeaf<TContent, TAverage> BottomRight { get; private set; }

        /// <summary>
        /// The QuadTreeLeaf in the top left part of the QuadTreeBranch.
        /// </summary>
        public QuadTreeLeaf<TContent, TAverage> TopLeft { get; private set; }

        /// <summary>
        /// The QuadTreeLeaf in the top right part of the QuadTreeBranch.
        /// </summary>
        public QuadTreeLeaf<TContent, TAverage> TopRight { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="SharpQuadTrees.QuadTreeBranch"/> class with the given <see cref="SharpQuadTrees.QuadTreeLeaf"/>s and average-aggrigation function
        /// </summary>
        /// <param name="aggregateAverages">Function that takes an average-value and the average-value aggregator, and returns the resulting average-value.</param>
        /// <param name="topRight">The QuadTreeLeaf in the top right part of the QuadTreeBranch.</param>
        /// <param name="bottomRight">The QuadTreeLeaf in the bottom right part of the QuadTreeBranch.</param>
        /// <param name="bottomLeft">The QuadTreeLeaf in the bottom left part of the QuadTreeBranch.</param>
        /// <param name="topLeft">The QuadTreeLeaf in the top left part of the QuadTreeBranch.</param>
        public QuadTreeBranch(Func<TAverage, TAverage, TAverage> aggregateAverages, QuadTreeLeaf<TContent, TAverage> topRight, QuadTreeLeaf<TContent, TAverage> bottomRight, QuadTreeLeaf<TContent, TAverage> bottomLeft, QuadTreeLeaf<TContent, TAverage> topLeft)
        {
            this.aggregateAverages = aggregateAverages;
            TopRight = topRight;
            BottomRight = bottomRight;
            BottomLeft = bottomLeft;
            TopLeft = topLeft;
        }
    }
}