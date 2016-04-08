namespace Spot.Ebnf.Unicode
{
    /// <summary>
    /// Defines the types of tokens in a Unicode sequence.
    /// </summary>
    internal enum TokenType
    {
        /// <summary>
        /// The token is not a known type to unicode sequences.
        /// </summary>
        Unknown,

        /// <summary>
        /// The token is recognizes, but is not valid.
        /// </summary>
        Invalid,

        /// <summary>
        /// The token is a single character.
        /// </summary>
        Character,

        /// <summary>
        /// The token is a symbol.
        /// </summary>
        Symbol,

        /// <summary>
        /// The token is a keyword.
        /// </summary>
        Keyword,

        /// <summary>
        /// The is a single class.
        /// </summary>
        ClassLiteral,

        /// <summary>
        /// The token represents no more input.
        /// </summary>
        EndOfInput
    }
}