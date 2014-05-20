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
    }
}