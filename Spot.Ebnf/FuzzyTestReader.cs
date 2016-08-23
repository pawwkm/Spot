using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Spot.Ebnf
{
    /// <summary>
    /// Reads a suite of fuzzy tests from a stream.
    /// </summary>
    public class FuzzyTestReader
    {
        /// <summary>
        /// Reads a suite of fuzzy tests from a stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>A list of the tests read from the <paramref name="stream"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is null.
        /// </exception>
        public IEnumerable<string> Read(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            byte[] buffer;
            while (stream.Position != stream.Length)
            {
                buffer = new byte[2];
                stream.Read(buffer, 0, buffer.Length);

                var amount = BitConverter.ToUInt16(buffer, 0);

                buffer = new byte[amount];
                stream.Read(buffer, 0, buffer.Length);

                yield return Encoding.UTF8.GetString(buffer);
            }
        }
    }
}