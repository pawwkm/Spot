using Pote;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Spot.Ebnf
{
    /// <summary>
    /// Represents a rule trace, which is an ordered collection of one of more rule frames.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = Justifications.MissingCollectionSuffix)]
    public sealed class RuleTrace : IEnumerable<RuleFrame>, IDeepCopy<RuleTrace>
    {
        private List<RuleFrame> frames = new List<RuleFrame>();

        /// <summary>
        /// The number of frames contained in the rule trace.
        /// </summary>
        public int Length
        {
            get
            {
                return frames.Count;
            }
        }

        /// <summary>
        /// Returns the frame at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The zero based index to the frame to get.</param>
        /// <returns>The frame at the specified <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0.-or-<paramref name="index"/> is equal to or greater than <see cref="Length"/>.
        /// </exception>
        public RuleFrame this[int index]
        {
            get
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return frames[index];
            }
        }

        /// <summary>
        /// Adds a frame to the rule trace.
        /// </summary>
        /// <param name="frame">The frame to add to the trace.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="frame"/> is null.
        /// </exception>
        public void Add(RuleFrame frame)
        {
            if (frame == null)
                throw new ArgumentNullException(nameof(frame));

            frames.Add(frame);
        }

        /// <summary>
        /// Searches for the specified <paramref name="frame"/> and returns the zero 
        /// based index of the first occurrence within the entire rule trace.
        /// </summary>
        /// <param name="frame">
        /// The frame to locate in the rule trace. The value can be null for reference types.
        /// </param>
        /// <returns>
        /// The zero based index of the first occurrence of item within
        /// the entire rule trace, if found; otherwise, –1.
        /// </returns>
        public int IndexOf(RuleFrame frame)
        {
            if (frame == null)
                throw new ArgumentNullException(nameof(frame));

            return frames.IndexOf(frame);
        }

        /// <summary>
        /// Deep copies all the data of the rule trace.
        /// </summary>
        /// <returns>A deep copy of the rule trace.</returns>
        public RuleTrace DeepCopy()
        {
            RuleTrace trace = new RuleTrace();
            foreach (var frame in frames)
                trace.frames.Add(frame.DeepCopy());

            return trace;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate 
        /// through the collection.
        /// </returns>
        IEnumerator<RuleFrame> IEnumerable<RuleFrame>.GetEnumerator()
        {
            return frames.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate 
        /// through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return frames.GetEnumerator();
        }
    }
}