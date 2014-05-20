using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpQuadTrees
{
    public abstract class QuadTreeController<TContent, TAverage>
    {
        /// <summary>
        /// Takes an average-value and the average-value aggregator, and returns the resulting average-value.
        /// </summary>
        /// <param name="itemAverage">Average-value of a content item.</param>
        /// <param name="aggregator">Average aggregator.</param>
        public abstract TAverage AggregateAverages(TAverage itemAverage, TAverage aggregator);

        /// <summary>
        /// Takes a content item and returns its average-value, that is later aggregated with the others.
        /// </summary>
        /// <param name="item">The content item.</param>
        public abstract TAverage GetAverage(TContent item);

        /// <summary>
        /// Gets a content item's x coordinate.
        /// </summary>
        /// <param name="item">The content item.</param>
        public abstract double GetContentX(TContent item);

        /// <summary>
        /// Gets a content item's y coordinate.
        /// </summary>
        public abstract double GetContentY(TContent item);

        /// <summary>
        /// Decides which leafs will be split.
        /// </summary>
        /// <param name="leafs">All leafs of the quad tree.</param>
        /// <returns>The leafs to split.</returns>
        public abstract IEnumerable<QuadTreeNode<TContent, TAverage>> GetNodesToSplit(IEnumerable<QuadTreeNode<TContent, TAverage>> leafs);

        /// <summary>
        /// Decides the x coordinate at which the given leaf is going to be split.
        /// </summary>
        /// <param name="leaf">The leaf that is going to be split.</param>
        /// <returns>The x coordinate at whioch it's going to be split.</returns>
        public abstract double GetSplitX(QuadTreeNode<TContent, TAverage> leaf);

        /// <summary>
        /// Decides the y coordinate at which the given leaf is going to be split.
        /// </summary>
        /// <param name="leaf">The leaf that is going to be split.</param>
        /// <returns>The y coordinate at whioch it's going to be split.</returns>
        public abstract double GetSplitY(QuadTreeNode<TContent, TAverage> leaf);
    }
}