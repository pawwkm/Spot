using Pote.IO;
using Pote.Text;
using System;
using System.IO;

namespace Spot.Ebnf
{
    /// <summary>
    /// Lexical analyzer for ISO/IEC 14977 : 1996(E) Ebnf
    /// </summary>
    internal sealed class LexicalAnalyzer : LexicalAnalyzer<TokenType>
    {
        /// <summary>
        /// This must be arranged from longest to shortest symbol.
        /// </summary>
        private static readonly string[] Symbols =
        {
            "(:", "(/", "/)", ":)",
            ",", "=", "|", "/", "!", 
            ")", "]", "}", "-", "(", 
            "[", "{", ";", ".", "*"
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="LexicalAnalyzer"/> class.
        /// </summary>
        /// <param name="reader">The source to analyze.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is null.
        /// </exception>
        public LexicalAnalyzer(StreamReader reader) : base(reader)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LexicalAnalyzer"/> class.
        /// </summary>
        /// <param name="reader">The source to analyze.</param>
        /// <param name="origin">
        /// The origin of the <paramref name="reader"/>.
        /// This is used to give more detailed information if errors occur.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> or <paramref name="origin"/> is null.
        /// </exception>
        public LexicalAnalyzer(StreamReader reader, string origin) : base(reader, origin)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LexicalAnalyzer"/> class.
        /// </summary>
        /// <param name="source">The source to analyze.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.
        /// </exception>
        public LexicalAnalyzer(string source) : base(source)
        {
        }

        /// <summary>
        /// Gets the next token from the input.
        /// </summary>
        /// <returns>
        /// The next token from the input. If there is 
        /// no more tokens in the input then a token 
        /// with the type <see cref="TokenType.EndOfInput"/>.
        /// is returned.
        /// </returns>
        /// <remarks>
        /// This method is called by <see cref="LexicalAnalyzer{TokenType}.Next()"/> if there
        /// are no more buffered tokens from looking ahead.
        /// </remarks>
        /// <exception cref="SyntaxException">
        /// A lexical exception has occurred while analyzing.
        /// </exception>
        protected override Token<TokenType> NextTokenFromSource()
        {
            SkipWhitespaces();
            if (Source.EndOfStream)
                return new Token<TokenType>("", TokenType.EndOfInput, Position.DeepCopy());

            char c = (char)Source.Peek();
            if (c == '"' || c == '\'')
                return TerminalString();
            if (char.IsLetter(c))
                return MetaIdentifier();
            if (c == '?')
                return SpecialSequence();
            if (char.IsDigit(c))
                return Integer();
            if (Source.Match("(*"))
            {
                Comment();
                return Next();
            }

            if (Source.MatchesAnyOf(Symbols))
                return Symbol();

            return Unknown();
        }

        /// <summary>
        /// Checks of the given character is a letter.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>True if the given character is a letter.</returns>
        private static bool IsLetter(char c)
        {
            return c >= 'A' && c <= 'Z' ||
                   c >= 'a' && c <= 'z';
        }

        /// <summary>
        /// Checks of the given character is a letter or digit.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>True if the given character is a letter or digit.</returns>
        private static bool IsLetterOrDigit(char c)
        {
            return IsLetter(c) || char.IsDigit(c);
        }

        /// <summary>
        /// Consumes the next terminal from the input.
        /// </summary>
        /// <returns>The consumed terminal from the input.</returns>
        /// <remarks>
        /// It is assumed that the next character in the input 
        /// a double quote.
        /// </remarks>
        private Token<TokenType> TerminalString()
        {
            InputPosition start = Position.DeepCopy();
            string text = "";

            Advance();
            while (!Source.EndOfStream)
            {
                char c = (char)Source.Peek();
                if (c == '"' || c == '\'')
                {
                    Advance();
                    break;
                }

                text += Advance();
            }

            return new Token<TokenType>(text, TokenType.TerminalString, start);
        }

        /// <summary>
        /// Consumes the next meta identifier from the input.
        /// </summary>
        /// <returns>The consumed meta identifier from the input.</returns>
        private Token<TokenType> MetaIdentifier()
        {
            InputPosition position = Position.DeepCopy();
            string metaIdentifier = "";

            char c;
            while (!Source.EndOfStream)
            {
                c = (char)Source.Peek();
                if (!char.IsLetter(c))
                    break;

                string identifier = "" + Advance();
                while (!Source.EndOfStream && IsLetterOrDigit((char)Source.Peek()))
                    identifier += Advance();

                if (metaIdentifier.Length == 0)
                    metaIdentifier = identifier;
                else
                    metaIdentifier += " " + identifier;

                SkipWhitespaces();
            }

            return new Token<TokenType>(metaIdentifier, TokenType.MetaIdentifier, position);
        }

        /// <summary>
        /// Consumes the next symbol from the input.
        /// </summary>
        /// <returns>The consumed symbol from the input.</returns>
        private Token<TokenType> Symbol()
        {
            InputPosition position = Position.DeepCopy();
            foreach (string symbol in Symbols)
            {
                if (Source.Match(symbol))
                {
                    Consume(symbol);
                    return new Token<TokenType>(symbol, TokenType.Symbol, position);
                }
            }

            return Unknown();
        }

        /// <summary>
        /// Consumes the next integer from the input.
        /// </summary>
        /// <returns>The consumed integer from the input.</returns>
        private Token<TokenType> Integer()
        {
            string text = "";
            InputPosition start = Position.DeepCopy();

            while (!Source.EndOfStream)
            {
                char c = (char)Source.Peek();
                if (!char.IsDigit(c))
                    break;

                text += Advance();
            }

            return new Token<TokenType>(text, TokenType.Integer, start);
        }

        /// <summary>
        /// Consumes the next special sequence from the input.
        /// </summary>
        /// <returns>The consumed special sequence from the input.</returns>
        /// <exception cref="SyntaxException">
        /// Missing question mark.
        /// </exception>
        private Token<TokenType> SpecialSequence()
        {
            InputPosition start = Position.DeepCopy();
            string text = "";

            if (!Consume("?"))
                throw new SyntaxException(start.ToString("Expected ?."));

            while (!Source.EndOfStream && !Source.Match("?"))
                text += Advance();

            if (!Consume("?"))
                throw new SyntaxException(Position.ToString("Expected ?."));

            return new Token<TokenType>(text, TokenType.SpecialSequence, start);
        }

        /// <summary>
        /// Consumes the next comment from the input.
        /// </summary>
        /// <returns>The consumed comment from the input.</returns>
        /// <exception cref="SyntaxException">
        /// Missing start or end comment mark.
        /// </exception>
        private Token<TokenType> Comment()
        {
            InputPosition start = Position.DeepCopy();
            if (!Consume("(*"))
                throw new SyntaxException(start.ToString("Expected (*"));

            string text = "";
            while (!Source.EndOfStream)
            {
                if (Source.Match("*)"))
                    break;

                text += Advance();
            }

            if (!Consume("*)")) 
                throw new SyntaxException(Position.ToString("Expected *)"));

            return new Token<TokenType>(text, TokenType.Comment, start);
        }

        /// <summary>
        /// Consumes the next string of text that doesn't contain 
        /// white spaces and labels it as "Unknown".
        /// </summary>
        /// <returns>The consumed unknown from the input.</returns>
        private Token<TokenType> Unknown()
        {
            InputPosition position = Position.DeepCopy();
            string text = "";

            while (!Source.EndOfStream)
            {
                char c = (char)Source.Peek();
                if (char.IsWhiteSpace(c))
                    break;

                text += Advance();
            }

            return new Token<TokenType>(text, TokenType.Unknown, position);
        }

        /// <summary>
        /// Skips the white spaces in the input.
        /// </summary>
        private void SkipWhitespaces()
        {
            while (!Source.EndOfStream)
            {
                char c = (char)Source.Peek();
                if (!char.IsWhiteSpace(c))
                    break;

                Advance();
            }
        }
    }
}