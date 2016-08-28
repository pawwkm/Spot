using System;
using System.Collections.Generic;
using System.Globalization;

namespace Spot.Ebnf.Unicode
{
    /// <summary>
    /// Represents a parsed unicode special sequence.
    /// </summary>
    internal sealed class UnicodeSequence
    {
        private static Dictionary<string, UnicodeSequence> cache = new Dictionary<string, UnicodeSequence>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UnicodeSequence"/> class.
        /// </summary>
        public UnicodeSequence()
        {
            Categories = new List<UnicodeCategory>();
            Characters = new List<char>();
            IsValidSequence = true;
        }

        /// <summary>
        /// The unicode categories that are available in this sequence.
        /// </summary>
        public IList<UnicodeCategory> Categories
        {
            get;
            private set;
        }

        /// <summary>
        /// The unicode characters that are available in this sequence.
        /// </summary>
        public IList<char> Characters
        {
            get;
            private set;
        }

        /// <summary>
        /// If true the sequence is parsed without error. 
        /// If false, the sequence is invalid and the data is incomplete.
        /// </summary>
        public bool IsValidSequence
        {
            get;
            set;
        }

        /// <summary>
        /// Looks for the <paramref name="sequence"/> in the cache and returns it,
        /// if it exists.
        /// </summary>
        /// <param name="sequence">The sequence to use for the lookup.</param>
        /// <returns>The parsed results of the sequence.</returns>
        public static UnicodeSequence Find(string sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            if (cache.ContainsKey(sequence))
                return cache[sequence];

            UnicodeSequenceParser parser = new UnicodeSequenceParser();
            UnicodeSequence result = parser.Parse(sequence);

            cache.Add(sequence, result);

            return result;
        }

        /// <summary>
        /// Caches the given <paramref name="sequence"/> and associates
        /// it with the <paramref name="raw"/> form of the sequence.
        /// </summary>
        /// <param name="raw">The raw text representation of the sequence.</param>
        /// <param name="sequence">The parsed sequence.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="raw"/> or <paramref name="sequence"/> is null.
        /// </exception>
        public static void Cache(string raw, UnicodeSequence sequence)
        {
            if (raw == null)
                throw new ArgumentNullException(nameof(raw));
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            if (!cache.ContainsKey(raw))
                cache.Add(raw, sequence);
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        internal static void ClearCache()
        {
            cache = new Dictionary<string, UnicodeSequence>();
        }
    }
}