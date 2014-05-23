using System;
using System.Collections.Generic;
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
        /// Stores the content items of this QuadTreeLeaf.
        /// </summary>
        private List<TContent> content;

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
            IQuadTreeController<TContent, TAverage> controller,
            params TContent[] content)
            : base(controller)
        {
            if (content == null)
                throw new ArgumentNullException("content", "Content can't be null.");

            XMin = Math.Min(xStart, xEnd);
            XMax = Math.Max(xStart, xEnd);
            YMin = Math.Min(yStart, yEnd);
            YMax = Math.Max(yStart, yEnd);

            this.content = content.Where(item =>
                IsInNode(controller.GetContentX(item), controller.GetContentY(item))
                ).ToList();
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
        public QuadTreeLeaf(IQuadTreeController<TContent, TAverage> controller, params TContent[] content)
            : base(controller)
        {
            if (content == null)
                throw new ArgumentNullException("content", "Content can't be null.");

            if (content.Length < 1)
                throw new ArgumentOutOfRangeException("content", "To use this constructor, there has to be at least one content item.");

            this.content = content.ToList();

            //Intermediate IEnumerable of an anonymous type with the coordinates for each item to reduce calls to the controller's method.
            var coordinates = content.Select(item => new { X = controller.GetContentX(item), Y = controller.GetContentY(item) });

            //Add double.Epsilon to max values, because they're exclusive.
            XMin = coordinates.Min(coordinate => coordinate.X);
            XMax = coordinates.Max(coordinate => coordinate.X) + double.Epsilon;

            YMin = coordinates.Min(coordinate => coordinate.Y);
            YMax = coordinates.Max(coordinate => coordinate.Y) + double.Epsilon;
        }

        /// <summary>
        /// Gets an IEnumerable of content items.
        /// </summary>
        /// <returns>IEnumerable of the content items.</returns>
        public override IEnumerable<TContent> GetContent()
        {
            foreach (TContent item in content)
                yield return item;
        }

        /// <summary>
        /// Gets an IEnumerable of the nodes that don't have children (leafs).
        /// </summary>
        /// <returns>This leaf.</returns>
        public override IEnumerable<QuadTreeNode<TContent, TAverage>> GetLeafs()
        {
            yield return this;
        }

        /// <summary>
        /// Splits the leaf at the point given by the controller's GetSplitX and GetSplitY methods.
        /// </summary>
        /// <returns>The node resulting from the split.</returns>
        public override QuadTreeNode<TContent, TAverage> Split()
        {
            return Split(controller.GetSplitX(this), controller.GetSplitY(this));
        }

        /// <summary>
        /// Splits this QuadTreeLeaf at the given point and returns the resulting QuadTreeBranch.
        /// </summary>
        /// <param name="x">The x coordinate of the point.</param>
        /// <param name="y">The y coordinate of the point.</param>
        /// <returns>The QuadTreeBranch resulting from the split.</returns>
        public override QuadTreeNode<TContent, TAverage> Split(double x, double y)
        {
            throwWhenOutsideNode(x, y);

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
        /// Splits the leaf at the given point if it matches the parameter and returns the resulting node (itself if it wasn't split, or the branch resulting from the split).
        /// </summary>
        /// <param name="leaf">The leaf supposed to be split.</param>
        /// <param name="x">The x coordinate of the point.</param>
        /// <param name="y">The y coordinate of the point.</param>
        /// <returns>The resulting node (itself if it wasn't split, or the branch resulting from the split).</returns>
        public override QuadTreeNode<TContent, TAverage> Split(QuadTreeNode<TContent, TAverage> leaf, double x, double y)
        {
            // Can't find a null leaf.
            if (leaf == null)
                return this;

            throwWhenOutsideNode(x, y);

            if (!this.Equals(leaf))
                return this;

            return Split(x, y);
        }

        /// <summary>
        /// Calculates the average-value of the content items and sets the Average property to it. Uses the controller's NoContentAverage if there's no content.
        /// </summary>
        protected override void calculateAverage()
        {
            if (content.Count == 0)
            {
                Average = controller.NoContentAverage;
                return;
            }

            Average = controller.GetAverage(GetContent());
        }
    }
}