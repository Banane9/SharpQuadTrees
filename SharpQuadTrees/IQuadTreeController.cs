﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpQuadTrees
{
    public interface IQuadTreeController<TContent, TAverage>
    {
        /// <summary>
        /// Gets the average-value that represents an average for no content.
        /// </summary>
        TAverage NoContentAverage { get; }

        /// <summary>
        /// Takes an array of average-values and returns the resulting average-value.
        /// </summary>
        /// <param name="averages"></param>
        TAverage AggregateAverages(IEnumerable<TAverage> averages);

        /// <summary>
        /// Takes an array of content items and returns the average-value.
        /// </summary>
        /// <param name="item">The content item.</param>
        TAverage GetAverage(IEnumerable<TContent> content);

        /// <summary>
        /// Gets a content item's x coordinate.
        /// </summary>
        /// <param name="item">The content item.</param>
        double GetContentX(TContent item);

        /// <summary>
        /// Gets a content item's y coordinate.
        /// </summary>
        double GetContentY(TContent item);

        /// <summary>
        /// Decides which leafs will be split.
        /// </summary>
        /// <param name="leafs">All leafs of the quad tree.</param>
        /// <returns>The leafs to split.</returns>
        IEnumerable<QuadTreeNode<TContent, TAverage>> GetNodesToSplit(IEnumerable<QuadTreeNode<TContent, TAverage>> leafs);

        /// <summary>
        /// Decides the x coordinate at which the given leaf is going to be split.
        /// </summary>
        /// <param name="leaf">The leaf that is going to be split.</param>
        /// <returns>The x coordinate at whioch it's going to be split.</returns>
        double GetSplitX(QuadTreeNode<TContent, TAverage> leaf);

        /// <summary>
        /// Decides the y coordinate at which the given leaf is going to be split.
        /// </summary>
        /// <param name="leaf">The leaf that is going to be split.</param>
        /// <returns>The y coordinate at whioch it's going to be split.</returns>
        double GetSplitY(QuadTreeNode<TContent, TAverage> leaf);
    }
}