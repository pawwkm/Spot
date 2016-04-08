using Pote;
using Pote.Text;
using System;
using System.IO;
using System.Text;

namespace Spot.Ebnf.Unicode
{
    /// <summary>
    /// Validates a special sequence consisting of unicode 
    /// character categories and individual characters.
    /// This is the syntax of the validator specified in Ebnf.
    /// </summary>
    public sealed class UnicodeSpecialSequenceValidator : ISpecialSequenceValidator
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
        /// Consumes the given <paramref name="source"/> as dictated by the special 
        /// <paramref name="sequence"/>.
        /// </summary>
        /// <param name="source">The source to consume from.</param>
        /// <param name="sequence">The special sequence to interpret.</param>
        /// <param name="position">
        /// The current position in the source measured in characters.
        /// The position is increased if any character is consumed.
        /// </param>
        /// <returns>
        /// True if the special sequence consumed at least one character in 
        /// the <paramref name="source"/>; otherwise false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/>, <paramref name="sequence"/> or <paramref name="position"/> is null.
        /// </exception>
        public bool Consume(string source, string sequence, InputPosition position)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));
            if (position == null)
                throw new ArgumentNullException(nameof(position));

            return Consume(source.ToStream(), sequence, position);
        }

        /// <summary>
        /// Consumes the given <paramref name="source"/> as dictated by the special 
        /// <paramref name="sequence"/>.
        /// </summary>
        /// <param name="source">The source to consume from.</param>
        /// <param name="sequence">The special sequence to interpret.</param>
        /// <param name="position">
        /// The current position in the source measured in characters.
        /// The position is increased if any character is consumed.
        /// </param>
        /// <returns>
        /// True if the special sequence consumed at least one character in 
        /// the <paramref name="source"/>; otherwise false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/>, <paramref name="sequence"/> or <paramref name="position"/> is null.
        /// </exception>
        public bool Consume(Stream source, string sequence, InputPosition position)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));
            if (position == null)
                throw new ArgumentNullException(nameof(position));

            if (source.Position == source.Length)
                return false;

            UnicodeSequence us = Get(sequence);
            if (!us.IsValidSequence)
                return false;

            long start = source.Position;
            using (StreamReader reader = new StreamReader(source, Encoding.UTF8, false, 32, true))
            {
                char c = (char)reader.Peek();
                if (us.Categories.Contains(char.GetUnicodeCategory(c)) || us.Characters.Contains(c))
                {
                    position.Advance(c);
                    source.Position = Encoding.UTF8.GetByteCount(new char[] { c });
                    return true;
                }
                else
                    source.Position = start;
            }

            return false;
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