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
        /// Backing field for the AggregateAverages property.
        /// </summary>
        private Func<TAverage, TAverage, TAverage> aggregateAverages;

        /// <summary>
        /// Backing field for the Average property.
        /// </summary>
        private TAverage average;

        /// <summary>
        /// Takes an average-value and the average-value aggregator, and returns the resulting average-value.
        /// </summary>
        public Func<TAverage, TAverage, TAverage> AggregateAverages
        {
            get { return aggregateAverages; }
            set
            {
                if (!aggregateAverages.Equals(value))
                {
                    aggregateAverages = value;
                    calculateAverage();
                }
            }
        }

        /// <summary>
        /// Gets the average-value of the content.
        /// </summary>
        public TAverage Average
        {
            get { return average; }
            set
            {
                if (!average.Equals(value))
                {
                    average = value;
                    onAverageChanged();
                }
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
        /// Fires the AverageChanged event.
        /// </summary>
        protected void onAverageChanged()
        {
            if (AverageChanged != null)
                AverageChanged(this);
        }

        /// <summary>
        /// EventHandler for the AverageChanged event.
        /// </summary>
        /// <param name="sender"></param>
        public delegate void AverageChangedEventHandler(QuadTreeNode<TContent, TAverage> sender);

        /// <summary>
        /// Fires after the value of the Average property changed.
        /// </summary>
        public event AverageChangedEventHandler AverageChanged;
    }
}