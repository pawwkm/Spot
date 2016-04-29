using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Spot.Ebnf
{
    /// <summary>
    /// A set of Assert methods that operates on generated fuzz tests.
    /// </summary>
    internal static class FuzzAssert
    {
        /// <summary>
        /// Verifies that a stream of generated tests contains the expected
        /// set of tests in a particular order.
        /// </summary>
        /// <param name="actual">The actual stream of fuzz tests.</param>
        /// <param name="expected">
        /// The set of tests that are expected to be present in the <paramref name="actual"/> stream.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="actual"/> or <paramref name="expected"/> is null.
        /// </exception>
        public static void AreEqual(MemoryStream actual, params string[] expected)
        {
            if (actual == null)
                throw new ArgumentNullException(nameof(actual));
            if (expected == null)
                throw new ArgumentNullException(nameof(expected));

            actual.Seek(0, SeekOrigin.Begin);
            CollectionAssert.AreEqual(expected, GetTests(actual));
        }

        private static List<string> GetTests(MemoryStream actual)
        {
            var list = new List<string>();
            while (actual.Position != actual.Length)
            {
                if (actual.Position + 2 >= actual.Length)
                    Assert.Fail("Unexpected end of stream.");

                byte[] bytes = { (byte)actual.ReadByte(), (byte)actual.ReadByte() };
                ushort length = BitConverter.ToUInt16(bytes, 0);

                bytes = new byte[length];
                var bytesRead = actual.Read(bytes, 0, bytes.Length);

                string value = Encoding.UTF8.GetString(bytes, 0, bytesRead);

                list.Add(value);
            }

            return list;
        }
    }
}