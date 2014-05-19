using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SharpQuadTrees
{
    /// <summary>
    /// Represents a node of a quad tree that doesn't have any children.
    /// </summary>
    /// <typeparam name="TContent">The type of the items that are stored in the quad tree.</typeparam>
    /// <typeparam name="TAverage">The type used for averaging the values of the content items.</typeparam>
    public class QuadTreeLeaf<TContent, TAverage> : QuadTreeNode<TContent, TAverage>
    {
        /// <summary>
        /// Backing field for the AggregateAverages property.
        /// </summary>
        private Func<TAverage, TAverage, TAverage> aggregateAverages;

        /// <summary>
        /// Stores the content of this QuadTreeLeaf.
        /// </summary>
        private List<TContent> content;

        /// <summary>
        /// Backing field for the GetAverage property.
        /// </summary>
        private Func<TContent, TAverage> getAverage;

        /// <summary>
        /// Gets a content item's x coordinate.
        /// </summary>
        private Func<TContent, double> getContentX;

        /// <summary>
        /// Gets a content item's y coordinate.
        /// </summary>
        private Func<TContent, double> getContentY;

        /// <summary>
        /// Gets a read only collection of the content.
        /// </summary>
        public ReadOnlyCollection<TContent> Content
        {
            get { return new ReadOnlyCollection<TContent>(content); }
        }

        /// <summary>
        /// Takes a content item and returns its average-value, that is later aggregated with the others.
        /// </summary>
        public Func<TContent, TAverage> GetAverage
        {
            get { return getAverage; }
            set
            {
                if (!getAverage.Equals(value))
                {
                    getAverage = value;
                    calculateAverage();
                }
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SharpQuadTrees.QuadTreeLeaf"/> class with the given size ranges, content, and its accessors.
        /// The content array is filtered based on the specified ranges for x and y.
        /// <para>
        /// The minimum value is inclusive, while the maximum value is exclusive.
        /// Meaning that an item is inside if it's coordinate on each axis is greater or equal to the minimum and less than the maximum for the axis.
        /// </para>
        /// </summary>
        /// <param name="xStart">Start of range for the x coordinate.</param>
        /// <param name="xEnd">End of range for the x coordinate.</param>
        /// <param name="yStart">Start of range for the y coordinate.</param>
        /// <param name="yEnd">End of range for the y coordinate.</param>
        /// <param name="getContentX">Function that retrieves a double from a content item that represents the x coordinate of it.</param>
        /// <param name="getContentY">Function that retrieves a double from a content item that represents the y coordinate of it.</param>
        /// <param name="getAverage">Function that takes a content item and returns its average-value, that is later aggregated with the others.</param>
        /// <param name="aggregateAverages">Function that takes an average-value and the average-value aggregator, and returns the resulting average-value.</param>
        /// <param name="content">An array of content items.</param>
        public QuadTreeLeaf(double xStart, double xEnd, double yStart, double yEnd,
            Func<TContent, double> getContentX,
            Func<TContent, double> getContentY,
            Func<TContent, TAverage> getAverage,
            Func<TAverage, TAverage, TAverage> aggregateAverages,
            params TContent[] content)
        {
            XMin = Math.Min(xStart, xEnd);
            XMax = Math.Max(xStart, xEnd);
            YMin = Math.Min(yStart, yEnd);
            YMax = Math.Max(yStart, yEnd);

            this.getContentX = getContentX;
            this.getContentY = getContentY;
            this.getAverage = getAverage;
            this.aggregateAverages = aggregateAverages;

            this.content = content.Where(item =>
                {
                    double itemX = this.getContentX(item);
                    double itemY = this.getContentY(item);
                    return itemX >= XMin || itemX < XMax || itemY >= YMin || itemY < YMax;
                }).ToList();

            calculateAverage();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SharpQuadTrees.QuadTreeLeaf"/> class with the given content, and its accessors.
        /// The size ranges are found dynamically.
        /// <para>
        /// The minimum value is inclusive, while the maximum value is exclusive.
        /// Meaning that an item is inside if it's coordinate on each axis is greater or equal to the minimum and less than the maximum for the axis.
        /// </para>
        /// </summary>
        /// <param name="getContentX">Function that retrieves a double from a content item that represents the x coordinate of it.</param>
        /// <param name="getContentY">Function that retrieves a double from a content item that represents the y coordinate of it.</param>
        /// <param name="getAverage">Function that takes a content item and returns its average-value, that is later aggregated with the others.</param>
        /// <param name="aggregateAverages">Function that takes an average-value and the average-value aggregator, and returns the resulting average-value.</param>
        /// <param name="content">An array of content items.</param>
        public QuadTreeLeaf(Func<TContent, double> getContentX,
            Func<TContent, double> getContentY,
            Func<TContent, TAverage> getAverage,
            Func<TAverage, TAverage, TAverage> aggregateAverages,
            params TContent[] content)
        {
            if (content.Length < 1)
                throw new ArgumentOutOfRangeException("content", "To use this constructor, there has to be at least one content item.");

            this.getContentX = getContentX;
            this.getContentY = getContentY;
            this.getAverage = getAverage;
            this.aggregateAverages = aggregateAverages;

            XMin = getContentX(content[0]);
            XMax = XMin;
            YMin = getContentY(content[0]);
            YMax = YMin;

            //Find the lowest/highest x and y. Because Max is exclusive, the value is increased by the smallest amount,
            //which means that the next one only has to equal to it, to be higher than the last.
            foreach (TContent item in content.Skip(1))
            {
                double x = getContentX(item);
                double y = getContentY(item);

                if (x < XMin)
                    XMin = x;
                else if (x >= XMax)
                    XMax = x + double.Epsilon;

                if (y < YMin)
                    YMin = y;
                else if (y >= YMax)
                    YMax = y + double.Epsilon;
            }

            calculateAverage();
        }

        /// <summary>
        /// Splits the QuadTreeLeaf in the center and returns the resulting QuadTree.
        /// </summary>
        /// <returns>The QuadTree resulting from the split.</returns>
        public override QuadTreeNode<TContent, TAverage> Split()
        {
            return Split(XCenter, YCenter);
        }

        /// <summary>
        /// Splits the QuadTreeLeaf at the given point and returns the resulting QuadTree.
        /// </summary>
        /// <param name="x">The x coordinate of the point.</param>
        /// <param name="y">The y coordinate of the point.</param>
        /// <returns>The QuadTree resulting from the split.</returns>
        public override QuadTreeNode<TContent, TAverage> Split(double x, double y)
        {
            //a coordinate has to be less than or equal to its max - double.Epsilon, because a content item's maximum coordinate can be max - double.Epsilon
            //so to ensure that there can be a content item in the greater-than-part the division must be double.Epsilon before the max of the axis.
            //For the less-than-part it must be at least min + double.Epsilon, so <= is enough.
            if ((x <= XMin || x >= (XMax - double.Epsilon)) && (y <= YMin || y >= (YMax - double.Epsilon)))
                throw new ArgumentOutOfRangeException("x & y", "Both coordinates of the point weren't inside the range of this QuadTreeLeaf.");
            else if (x <= XMin || x >= (XMax - double.Epsilon))
                throw new ArgumentOutOfRangeException("x", "The x coordinate of the point wasn't inside the range of this QuadTreeLeaf.");
            else if (y <= YMin || y >= (YMax - double.Epsilon))
                throw new ArgumentOutOfRangeException("y", "The y coordinate of the point wasn't inside the range of this QuadTreeLeaf.");

            var topRight = new QuadTreeLeaf<TContent, TAverage>(x, XMax, y, YMax,
                getContentX, getContentY,
                GetAverage, AggregateAverages,
                content.ToArray());

            var bottomRight = new QuadTreeLeaf<TContent, TAverage>(x, XMax, YMin, y,
                getContentX, getContentY,
                GetAverage, AggregateAverages,
                content.ToArray());

            var bottomLeft = new QuadTreeLeaf<TContent, TAverage>(XMin, x, YMin, y,
                getContentX, getContentY,
                GetAverage, AggregateAverages,
                content.ToArray());

            var topLeft = new QuadTreeLeaf<TContent, TAverage>(XMin, x, y, YMax,
                getContentX, getContentY,
                GetAverage, AggregateAverages,
                content.ToArray());

            return new QuadTreeBranch<TContent, TAverage>(AggregateAverages, topRight, bottomRight, bottomLeft, topLeft);
        }

        /// <summary>
        /// Calculates the average-value of the content items and sets the Average property to it.
        /// </summary>
        protected override void calculateAverage()
        {
            if (content.Count == 0)
                Average = default(TAverage);

            TAverage aggregator = GetAverage(content[0]);

            foreach (TContent item in content.Skip(1))
            {
                aggregator = AggregateAverages(GetAverage(item), aggregator);
            }

            Average = aggregator;
        }
    }
}