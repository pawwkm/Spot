using System.Collections.Generic;
using System;
using System.Collections;

namespace Spot.SrtL
{
    /// <summary>
    /// A list of <see cref="String"/>s.
    /// </summary>
    public class StringList : List<String>
    {
        /// <summary>
        /// Returns a list of the raw string values of this list.
        /// </summary>
        /// <returns>A list of the raw string values of this list.</returns>
        public IEnumerable<string> ToRawStrings()
        {
            foreach (var s in this)
                yield return s.Content;
        }
    }
}