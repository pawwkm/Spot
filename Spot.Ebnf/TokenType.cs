namespace Spot.Ebnf
{
    /// <summary>
    /// Defines the types of tokens in ebnf.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// The token is not a known type to ebnf.
        /// </summary>
        Unknown,

        /// <summary>
        /// The token is an integer. 
        /// </summary>
        Integer,

        /// <summary>
        /// The token is a symbol.
        /// </summary>
        Symbol,

        /// <summary>
        /// The token contains a special sequence.
        /// </summary>
        SpecialSequence,

        /// <summary>
        /// The token is a comment.
        /// </summary>
        Comment,

        /// <summary>
        /// The token is a meta identifier.
        /// </summary>
        MetaIdentifier,

        /// <summary>
        /// The token is a terminal string.
        /// </summary>
        TerminalString,

        /// <summary>
        /// The token represents no more input.
        /// </summary>
        EndOfInput
    }
}