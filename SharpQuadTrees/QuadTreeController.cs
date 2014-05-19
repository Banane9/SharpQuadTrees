using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpQuadTrees
{
    public class QuadTreeController<TContent, TAverage>
    {
        /// <summary>
        /// Backing field for the AggregateAverages property.
        /// </summary>
        private Func<TAverage, TAverage, TAverage> aggregateAverages;

        /// <summary>
        /// Backing field for the GetAverage property.
        /// </summary>
        private Func<TContent, TAverage> getAverage;

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
                    onAverageCalculationChanged();
                }
            }
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
                    onAverageCalculationChanged();
                }
            }
        }

        /// <summary>
        /// Gets a content item's x coordinate.
        /// </summary>
        public Func<TContent, double> GetContentX { get; protected set; }

        /// <summary>
        /// Gets a content item's y coordinate.
        /// </summary>
        public Func<TContent, double> GetContentY { get; protected set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="getContentX">Function that retrieves a double from a content item that represents the x coordinate of it.</param>
        /// <param name="getContentY">Function that retrieves a double from a content item that represents the y coordinate of it.</param>
        /// <param name="getAverage">Function that takes a content item and returns its average-value, that is later aggregated with the others.</param>
        /// <param name="aggregateAverages">Function that takes an average-value and the average-value aggregator, and returns the resulting average-value.</param>
        public QuadTreeController(Func<TContent, double> getContentX,
                    Func<TContent, double> getContentY,
                    Func<TContent, TAverage> getAverage,
                    Func<TAverage, TAverage, TAverage> aggregateAverages)
        {
            GetContentX = getContentX;
            GetContentY = getContentY;
            GetAverage = getAverage;
            AggregateAverages = aggregateAverages;
        }

        /// <summary>
        /// Fires the AverageCalculationChanged event.
        /// </summary>
        protected void onAverageCalculationChanged()
        {
            if (AverageCalculationChanged != null)
                AverageCalculationChanged(this);
        }

        /// <summary>
        /// EventHandler for the AverageCalculationChanged event.
        /// </summary>
        /// <param name="sender">The QuadTreeController for which the way it calculates averages changed.</param>
        public delegate void AverageCalculationChangedEventHandler(QuadTreeController<TContent, TAverage> sender);

        /// <summary>
        /// Fires after the way the QuadTreeController calculates averages changed.
        /// </summary>
        public event AverageCalculationChangedEventHandler AverageCalculationChanged;
    }
}