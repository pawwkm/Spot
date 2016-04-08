using Pote.Text;

namespace Spot.Ebnf
{
    /// <summary>
    /// Token builder for ebnf tokens.
    /// </summary>
    public sealed class TokenBuilder : TokenBuilder<TokenBuilder, TokenType>
    {
        /// <summary>
        /// Creates a token type of <see cref="TokenType.Unknown"/>
        /// with text.
        /// </summary>
        /// <param name="text">The content of the unknown token.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder Unknown(string text)
        {
            return Token(text, TokenType.Unknown);
        }

        /// <summary>
        /// Creates a token type of <see cref="TokenType.Integer"/>
        /// with text.
        /// </summary>
        /// <param name="text">The content of the integer.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder Integer(string text)
        {
            return Token(text, TokenType.Integer);
        }

        /// <summary>
        /// Creates a token of type <see cref="TokenType.Symbol"/>
        /// with a symbol.
        /// </summary>
        /// <param name="symbol">The symbol of the token.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder Symbol(string symbol)
        {
            return Token(symbol, TokenType.Symbol);
        }

        /// <summary>
        /// Creates a token of type <see cref="TokenType.SpecialSequence"/>
        /// with a sequence.
        /// </summary>
        /// <param name="rule">The contents of the sequence, without the question marks.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder SpecialSequence(string rule)
        {
            return Token(rule, TokenType.SpecialSequence);
        }

        /// <summary>
        /// Creates a token of type <see cref="TokenType.Comment"/>
        /// with some text.
        /// </summary>
        /// <param name="rule">The contents of the comment, without the start and end symbols.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder Comment(string rule)
        {
            return Token(rule, TokenType.MetaIdentifier);
        }

        /// <summary>
        /// Creates a token of type <see cref="TokenType.MetaIdentifier"/>
        /// with a rule name.
        /// </summary>
        /// <param name="rule">The rule name of the meta identifier.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder MetaIdentifier(string rule)
        {
            return Token(rule, TokenType.MetaIdentifier);
        }

        /// <summary>
        /// Creates a token of type <see cref="TokenType.TerminalString"/>
        /// with text.
        /// </summary>
        /// <param name="terminal">The text of the terminal string.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder TerminalString(string terminal)
        {
            return Token(terminal, TokenType.TerminalString);
        }
    }
}