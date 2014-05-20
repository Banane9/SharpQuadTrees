using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpQuadTrees
{
    /// <summary>
    /// Contains properties and functions that make it suitable as part of a quad tree.
    /// </summary>
    /// <typeparam name="TContent">The type of the items that are stored in the quad tree.</typeparam>
    /// <typeparam name="TAverage">The type used for averaging the values of the content items.</typeparam>
    public abstract class QuadTreeNode<TContent, TAverage>
    {
        /// <summary>
        /// Backing field for the Average property.
        /// </summary>
        private TAverage average;

        /// <summary>
        /// Whether the Average property was calculated with current information.
        /// </summary>
        private bool isAverageCurrent = false;

        /// <summary>
        /// Gets the average-value of the content.
        /// </summary>
        public TAverage Average
        {
            get
            {
                if (!isAverageCurrent)
                    calculateAverage();

                return average;
            }
            set
            {
                average = value;
                validateAverage();
            }
        }

        /// <summary>
        /// Gets the x coordinate of the quad's center.
        /// </summary>
        public double XCenter
        {
            get { return (XMin + XMax) / 2; }
        }

        /// <summary>
        /// Gets the exclusive maximum value for the x coordinate.
        /// </summary>
        public double XMax { get; protected set; }

        /// <summary>
        /// Gets the inclusive minimum value for the x coordinate.
        /// </summary>
        public double XMin { get; protected set; }

        /// <summary>
        /// Gets the y coordinate of the quad's center.
        /// </summary>
        public double YCenter
        {
            get { return (YMin + YMax) / 2; }
        }

        /// <summary>
        /// Gets the exclusive maximum value for the y coordinate.
        /// </summary>
        public double YMax { get; protected set; }

        /// <summary>
        /// Gets the inclusive minimum value for the y coordinate.
        /// </summary>
        public double YMin { get; protected set; }

        /// <summary>
        /// Gets the <see cref="SharpQuadTrees.QuadTreeController"/> used for handling the content.
        /// </summary>
        protected QuadTreeController<TContent, TAverage> controller { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="SharpQuadTrees.QuadTreeNode"/> class with the given controller.
        /// Only available in derived classes.
        /// </summary>
        /// <param name="controller">Controller used for handling the content.</param>
        protected QuadTreeNode(QuadTreeController<TContent, TAverage> controller)
        {
            this.controller = controller;
        }

        /// <summary>
        /// Splits the node in the center and returns the resulting node.
        /// </summary>
        /// <returns>The node resulting from the split.</returns>
        public abstract QuadTreeNode<TContent, TAverage> Split();

        /// <summary>
        /// Splits the node at the given point and returns the resulting node.
        /// </summary>
        /// <param name="x">The x coordinate of the point.</param>
        /// <param name="y">The y coordinate of the point.</param>
        /// <returns>The node resulting from the split.</returns>
        public abstract QuadTreeNode<TContent, TAverage> Split(double x, double y);

        /// <summary>
        /// Calculates the average-value of the content and sets the Average property to it.
        /// </summary>
        protected abstract void calculateAverage();

        /// <summary>
        /// Marks the Average property as not being calculated with current information.
        /// </summary>
        protected void invalidateAverage()
        {
            isAverageCurrent = false;
        }

        /// <summary>
        /// Marks the Average property as having been calculated with current information.
        /// </summary>
        private void validateAverage()
        {
            isAverageCurrent = true;
        }
    }
}