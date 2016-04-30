using System.Collections.Generic;
using System.Text;

namespace Spot.SrtL
{
    /// <summary>
    /// Defines a set of concatenated strings.
    /// </summary>
    public sealed class ConcatenatedString
    {
        private List<String> strings = new List<String>();

        /// <summary>
        /// The strings that make up this concatenated string.
        /// </summary>
        public IList<String> Strings
        {
            get
            {
                return strings;
            }
        }

        /// <summary>
        /// Returns the concatenated value of <see cref="Strings"/>.
        /// </summary>
        /// <returns>The concatenated value of <see cref="Strings"/>.</returns>
        public string Concatenate()
        {
            var builder = new StringBuilder();
            foreach (var s in Strings)
                builder.Append(s.Content);

            return builder.ToString();
        }
    }
}