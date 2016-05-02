using Pote.Text;
using System;

namespace Spot.SrtL
{
    /// <summary>
    /// Token builder for ebnf tokens.
    /// </summary>
    internal sealed class TokenBuilder : TokenBuilder<TokenBuilder, TokenType>
    {
        private InputPosition position = new InputPosition();

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenBuilder"/> class.
        /// </summary>
        public TokenBuilder()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenBuilder"/> class
        /// with position data.
        /// </summary>
        /// <param name="line">Specifies the line number of the first token.</param>
        /// <param name="column">Specifies the column number of the first token.</param>
        /// <param name="index">Specifies the index of the first token.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="line"/> or <paramref name="column"/>
        /// is less than one. <paramref name="index"/> is less 
        /// than zero.
        /// </exception>
        public TokenBuilder(int line, int column, int index)
        {
            position = new InputPosition(line, column, index);
        }

        /// <summary>
        /// Creates a token of the 'test' keyword.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Test()
        {
            var token = Token("test", TokenType.Keyword, position.DeepCopy());
            position.Advance("test\n");

            return token;
        }

        /// <summary>
        /// Creates a token of the 'description' keyword.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Description()
        {
            var token = Token("description", TokenType.Keyword, position.DeepCopy());
            position.Advance("\tdescription ");

            return token;
        }

        /// <summary>
        /// Creates a token of the 'input' keyword.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Input()
        {
            var token = Token("input", TokenType.Keyword, position.DeepCopy());
            position.Advance("\tinput ");

            return token;
        }

        /// <summary>
        /// Creates a token of the 'is' keyword.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Is()
        {
            var token = Token("is", TokenType.Keyword, position.DeepCopy());
            position.Advance("\tis ");

            return token;
        }

        /// <summary>
        /// Creates a token of the 'not' keyword.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Not()
        {
            var token = Token("not", TokenType.Keyword, position.DeepCopy());
            position.Advance("not ");

            return token;
        }

        /// <summary>
        /// Creates a token of the 'valid' keyword.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Valid()
        {
            var token = Token("valid", TokenType.Keyword, position.DeepCopy());
            position.Advance("valid\n\n");

            return token;
        }

        /// <summary>
        /// Creates a token of the 'start' keyword.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Start()
        {
            var token = Token("start", TokenType.Keyword, position.DeepCopy());
            position.Advance("\tstart");

            return token;
        }

        /// <summary>
        /// Creates a token of the 'from' keyword.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder From()
        {
            var token = Token("from", TokenType.Keyword, position.DeepCopy());
            position.Advance("\tfrom");

            return token;
        }

        /// <summary>
        /// Creates a token of the 'exclude' keyword.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Exclude()
        {
            var token = Token("exclude", TokenType.Keyword, position.DeepCopy());
            position.Advance("\texclude");

            return token;
        }

        /// <summary>
        /// Creates a token of the 'include' keyword.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Include()
        {
            var token = Token("include", TokenType.Keyword, position.DeepCopy());
            position.Advance("\tinclude ");

            return token;
        }

        /// <summary>
        /// Creates a token of the 'all' keyword.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder All()
        {
            var token = Token("all", TokenType.Keyword, position.DeepCopy());
            position.Advance("\tall");

            return token;
        }

        /// <summary>
        /// Creates a token of the 'rules' keyword.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Rules()
        {
            var token = Token("rules", TokenType.Keyword, position.DeepCopy());
            position.Advance("\trules");

            return token;
        }

        /// <summary>
        /// Creates a string token.
        /// </summary>
        /// <param name="text">The content of the string.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder String(string text)
        {
            var token = Token(text, TokenType.String, position.DeepCopy());

            position.Advance('"');
            position.Advance(text);
            position.Advance('"');

            return token;
        }

        /// <summary>
        /// Creates a token of the ',' symbol.
        /// </summary>
        /// <returns>This builder.</returns>
        public TokenBuilder Comma()
        {
            var token = Token(",", TokenType.Symbol, position.DeepCopy());
            position.Advance(",");

            return token;
        }

        /// <summary>
        /// Creates an unknown token.
        /// </summary>
        /// <param name="text">The content of the token.</param>
        /// <returns>This builder.</returns>
        public TokenBuilder Unknown(string text)
        {
            var token = Token(text, TokenType.Unknown, position.DeepCopy());

            position.Advance(text);
            position.Advance(' ');

            return token;
        }
    }
}