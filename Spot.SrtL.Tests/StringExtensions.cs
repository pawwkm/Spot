using System.ComponentModel;
using System.IO;
using System.Text;

namespace Spot.SrtL
{
    /// <summary>
    /// Provides extension methods for the <see cref="string"/> class.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class StringExtensions
    {
        /// <summary>
        /// Converts the <paramref name="input"/> to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="input">The input to convert.</param>
        /// <returns>The converted input.</returns>
        public static Stream ToStream(this string input)
        {
            return input.ToStream(Encoding.Unicode);
        }

        /// <summary>
        /// Converts the <paramref name="input"/> to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="input">The input to convert.</param>
        /// <param name="encoding">The encoding the use for the <paramref name="input"/>.</param>
        /// <returns>The converted input.</returns>
        public static Stream ToStream(this string input, Encoding encoding)
        {
            MemoryStream stream = new MemoryStream();

            StreamWriter writer = new StreamWriter(stream, encoding);
            writer.Write(input);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }

        /// <summary>
        /// Converts the <paramref name="input"/> to a <see cref="StreamReader"/>.
        /// </summary>
        /// <param name="input">The input to convert.</param>
        /// <returns>The converted input.</returns>
        public static StreamReader ToStreamReader(this string input)
        {
            return new StreamReader(input.ToStream());
        }

        /// <summary>
        /// Converts the <paramref name="input"/> to a <see cref="StreamReader"/>.
        /// </summary>
        /// <param name="input">The input to convert.</param>
        /// <param name="encoding">The encoding of the reader.</param>
        /// <returns>The converted input.</returns>
        public static StreamReader ToStreamReader(this string input, Encoding encoding)
        {
            return new StreamReader(input.ToStream(encoding));
        }
    }
}
