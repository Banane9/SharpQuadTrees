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
        /// Stores the content of this QuadTreeLeaf.
        /// </summary>
        private List<TContent> content;

        /// <summary>
        /// Gets a read only collection of the content.
        /// </summary>
        public ReadOnlyCollection<TContent> Content
        {
            get { return new ReadOnlyCollection<TContent>(content); }
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
        /// <param name="controller">Controller used for handling the content.</param>
        /// <param name="content">An array of content items.</param>
        public QuadTreeLeaf(double xStart, double xEnd, double yStart, double yEnd,
            QuadTreeController<TContent, TAverage> controller,
            params TContent[] content)
            : base(controller)
        {
            XMin = Math.Min(xStart, xEnd);
            XMax = Math.Max(xStart, xEnd);
            YMin = Math.Min(yStart, yEnd);
            YMax = Math.Max(yStart, yEnd);

            this.content = content.Where(item =>
                {
                    double itemX = controller.GetContentX(item);
                    double itemY = controller.GetContentY(item);
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
        /// <param name="controller">Controller used for handling the content.</param>
        /// <param name="content">An array of content items.</param>
        public QuadTreeLeaf(QuadTreeController<TContent, TAverage> controller, params TContent[] content)
            : base(controller)
        {
            if (content.Length < 1)
                throw new ArgumentOutOfRangeException("content", "To use this constructor, there has to be at least one content item.");

            XMin = controller.GetContentX(content[0]);
            XMax = XMin;
            YMin = controller.GetContentY(content[0]);
            YMax = YMin;

            //Find the lowest/highest x and y. Because Max is exclusive, the value is increased by the smallest amount,
            //which means that the next one only has to equal to it, to be higher than the last.
            foreach (TContent item in content.Skip(1))
            {
                double x = controller.GetContentX(item);
                double y = controller.GetContentY(item);

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
                controller, content.ToArray());

            var bottomRight = new QuadTreeLeaf<TContent, TAverage>(x, XMax, YMin, y,
                controller, content.ToArray());

            var bottomLeft = new QuadTreeLeaf<TContent, TAverage>(XMin, x, YMin, y,
                controller, content.ToArray());

            var topLeft = new QuadTreeLeaf<TContent, TAverage>(XMin, x, y, YMax,
                controller, content.ToArray());

            return new QuadTreeBranch<TContent, TAverage>(controller, topRight, bottomRight, bottomLeft, topLeft);
        }

        /// <summary>
        /// Calculates the average-value of the content items and sets the Average property to it.
        /// </summary>
        protected override void calculateAverage()
        {
            if (content.Count == 0)
                Average = default(TAverage);

            TAverage aggregator = controller.GetAverage(content[0]);

            foreach (TContent item in content.Skip(1))
            {
                aggregator = controller.AggregateAverages(controller.GetAverage(item), aggregator);
            }

            Average = aggregator;
        }
    }
}