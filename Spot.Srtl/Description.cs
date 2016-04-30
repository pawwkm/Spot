using Pote.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spot.SrtL
{
    /// <summary>
    /// Describes the purpose of a test.
    /// </summary>
    public sealed class Description
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Description"/> class.
        /// </summary>
        /// <param name="position">The location where the description was defined.</param>
        /// <param name="text">The actual description.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> or <paramref name="text"/> is null.
        /// </exception>
        public Description(InputPosition position, ConcatenatedString text)
        {
            if (position == null)
                throw new ArgumentNullException(nameof(position));
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            DefinedAt = position;
            Text = text;
        }

        /// <summary>
        /// The location where the description was defined.
        /// </summary>
        public InputPosition DefinedAt
        {
            get;
            private set;
        }

        /// <summary>
        /// The actual description.
        /// </summary>
        public ConcatenatedString Text
        {
            get;
            private set;
        }
    }
}