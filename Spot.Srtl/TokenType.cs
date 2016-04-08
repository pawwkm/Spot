namespace Spot.SrtL
{
    /// <summary>
    /// Defines the types of tokens in SrtL.
    /// </summary>
    internal enum TokenType
    {
        /// <summary>
        /// There is no more input,
        /// </summary>
        EndOfInput,

        /// <summary>
        /// The contents of the token could not be identified.
        /// </summary>
        Unknown,

        /// <summary>
        /// The token is a keyword.
        /// </summary>
        Keyword,

        /// <summary>
        /// The token is a string of characters.
        /// </summary>
        String,

        /// <summary>
        /// The token is a symbol.
        /// </summary>
        Symbol
    }
}