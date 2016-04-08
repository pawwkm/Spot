using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Spot.Ebnf.Unicode
{
    /// <summary>
    /// Generates strings based on an Unicode sequence.
    /// </summary>
    public sealed class UnicodeSequenceGenerator : ISpecialSequenceGenerator
    {
        /// <summary>
        /// Checks that the given sequence is valid.
        /// </summary>
        /// <param name="sequence">The sequence to validate.</param>
        /// <returns>True if the sequence is valid; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sequence"/> is null.
        /// </exception>
        public bool IsValid(string sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            return Get(sequence).IsValidSequence;
        }

        /// <summary>
        /// Generates all possible strings for the given <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence">The sequence to generate strings from.</param>
        /// <returns>All generated strings.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sequence"/> is null.
        /// </exception>
        public Collection<string> Generate(string sequence)
        {
            UnicodeSequence us = Get(sequence);
            if (!us.IsValidSequence)
                return new Collection<string>();

            List<string> strings = new List<string>();
            foreach (char c in us.Characters)
                strings.Add(c.ToString());

            for (char c = '\0'; c < char.MaxValue; c++)
            {
                UnicodeCategory category = char.GetUnicodeCategory(c);
                if (us.Categories.Contains(category))
                    strings.Add(c.ToString());
            }

            return new Collection<string>(strings);
        }

        /// <summary>
        /// Gets the parsed results of the given <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence">The sequence to use for the lookup.</param>
        /// <returns>The parsed results of the sequence.</returns>
        private static UnicodeSequence Get(string sequence)
        {
            UnicodeSequence result = UnicodeSequence.Find(sequence);
            if (result != null)
                return result;

            UnicodeSequenceParser parser = new UnicodeSequenceParser();
            result = parser.Parse(sequence);

            UnicodeSequence.Cache(sequence, result);

            return result;
        }
    }
}