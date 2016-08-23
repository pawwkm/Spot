using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Spot.Ebnf
{
    /// <summary>
    /// Wrties fuzzy tests to a stream.
    /// </summary>
    public class FuzzyTestWriter
    {
        /// <summary>
        /// Writes a set of tests to a stream.
        /// </summary>
        /// <param name="tests">The tests to write to the <paramref name="destination"/>.</param>
        /// <param name="destination">The stream where the tests are written to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="tests"/> or <paramref name="destination"/> is null.
        /// </exception>
        public void Write(IEnumerable<string> tests, Stream destination)
        {
            if (tests == null)
                throw new ArgumentNullException(nameof(tests));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            foreach (var test in tests)
            {
                ushort length = (ushort)test.Length;

                var bytes = BitConverter.GetBytes(length);
                destination.Write(bytes, 0, bytes.Length);

                bytes = Encoding.UTF8.GetBytes(test);
                destination.Write(bytes, 0, bytes.Length);
            }
        }
    }
}