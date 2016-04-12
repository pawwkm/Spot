using System;
using System.Collections.Generic;
using System.IO;

namespace Spot.SrtL
{
    /// <summary>
    /// Reads tests defined in SrtL.
    /// </summary>
    public class SrtLReader
    {
        /// <summary>
        /// Reads the tests in the <paramref name="files"/>.
        /// </summary>
        /// <param name="files">The files to read tests from.</param>
        /// <returns>The tests read from the <paramref name="files"/>.</returns>
        public TestCollection Read(IEnumerable<string> files)
        {
            if (files == null)
                throw new ArgumentNullException(nameof(files));

            var tests = new TestCollection();
            foreach (var file in files)
            {
                if (!File.Exists(file))
                    throw new FileNotFoundException("The file could not be found", file);

                using (Stream stream = File.OpenRead(file))
                    tests.Add(Read(stream, file));
            }

            return tests;
        }

        /// <summary>
        /// Reads the tests in the <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The stream to read tests from.</param>
        /// <returns>The tests read from the <paramref name="source"/>.</returns>
        public TestCollection Read(Stream source)
        {
            return Read(source, "");
        }

        /// <summary>
        /// Reads the tests in the <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The stream to read tests from.</param>
        /// <param name="origin">
        /// The origin of the <paramref name="source"/>.
        /// This is used to give more detailed information if errors occur.
        /// </param>
        /// <returns>The tests read from the <paramref name="source"/>.</returns>
        public TestCollection Read(Stream source, string origin)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (origin == null)
                throw new ArgumentNullException(nameof(origin));

            using (StreamReader reader = new StreamReader(source))
            {
                var analyzer = new LexicalAnalyzer(reader, origin);
                var parser = new Parser();

                return parser.Parse(analyzer);
            }
        }
    }
}