using System;

namespace Spot.Ebnf
{
    /// <summary>
    /// Provides extensions for the <see cref="Random"/> class.
    /// </summary>
    internal static class RandomExtensions
    {
        /// <summary>
        /// Generates a random bool value.
        /// </summary>
        /// <param name="random">The random instance to operate on.</param>
        /// <returns>A random bool value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="random"/> is null.
        /// </exception>
        public static bool Bool(this Random random)
        {
            if (random == null)
                throw new ArgumentNullException(nameof(random));

            return random.Next(2) == 0;
        }
    }
}