namespace Spot.Ebnf
{
    /// <summary>
    /// Defines the current state of a <see cref="SyntaxPath"/>.
    /// </summary>
    internal enum PathState
    {
        /// <summary>
        /// The path in still being parsed.
        /// </summary>
        Parsing = 0,

        /// <summary>
        /// An error has occurred while parsing the path.
        /// </summary>
        Error = 1,

        /// <summary>
        /// The path was parsed successfully.
        /// </summary>
        Success = 2
    }
}