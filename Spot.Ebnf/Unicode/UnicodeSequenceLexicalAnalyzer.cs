using Pote;
using Pote.Text;
using System;
using System.Globalization;
using System.Linq;

namespace Spot.Ebnf.Unicode
{
    /// <summary>
    /// Lexical analyzer for the unicode special sequence dsl.
    /// </summary>
    internal sealed class UnicodeSequenceLexicalAnalyzer : LexicalAnalyzer<TokenType>
    {
        private static readonly string[] Classes =
        {
            "cc", "cf", "cn", "co", "cs", "ll",
            "lm", "lo", "lt", "lu", "mc", "me",
            "mn", "nd", "nl", "no", "pc", "pd",
            "pe", "pf", "pi", "po", "ps", "sc",
            "sk", "sm", "so", "zi", "zp", "zs"
        };

        private static readonly string[] Keywords =
        {
            "Unicode",
            "class",
            "classes",
            "character",
            "characters",
            "and",
            "All",
            "except"
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="UnicodeSequenceLexicalAnalyzer"/> class.
        /// </summary>
        /// <param name="source">The source to analyze.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.
        /// </exception>
        public UnicodeSequenceLexicalAnalyzer(string source) : base(source)
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
        protected override Token<TokenType> NextTokenFromSource()
        {
            SkipWhiteSpaces();
            if (Source.EndOfStream)
                return new Token<TokenType>("", TokenType.EndOfInput, Position.DeepCopy());

            char c = (char)Source.Peek();
            if (c == '\\')
                return Character();
            if (c.IsOneOf(','))
                return Symbol();
            if (Source.MatchesAnyOf(Keywords))
                return Keyword();
            if (Source.MatchesAnyOf(Classes))
                return ClassLiteral();

            InputPosition start = Position.DeepCopy();
            return new Token<TokenType>(Advance(), TokenType.Invalid, start);
        }

        /// <summary>
        /// Skips the white spaces in the input.
        /// Any character in that returns true in the <see cref="char.IsWhiteSpace(char)"/>
        /// is considered a white space by this method.
        /// </summary>
        private void SkipWhiteSpaces()
        {
            char[] characters =
            {
                '\u0009', '\u000B', '\u000C', '\u000D',
                '\u000A', '\u0085', '\u2028', '\u2029'
            };

            while (!Source.EndOfStream)
            {
                char c = (char)Source.Peek();
                if (!characters.Contains(c) && char.GetUnicodeCategory(c) != UnicodeCategory.SpaceSeparator)
                    break;

                Advance();
            }
        }

        /// <summary>
        /// Consumes the next character from the input.
        /// </summary>
        /// <returns>The consumed character from the input.</returns>
        private Token<TokenType> Character()
        {
            InputPosition start = Position.DeepCopy();
            string text = "" + Advance();

            if (Source.EndOfStream)
                return new Token<TokenType>(text, TokenType.EndOfInput, start);

            char c = (char)Source.Peek();
            if (c != 'u')
            {
                text += Advance();
                return new Token<TokenType>(text, TokenType.Invalid, start);
            }

            text += Advance();
            for (int i = 0; i < 4; i++)
            {
                if (Source.EndOfStream)
                    return new Token<TokenType>(text, TokenType.EndOfInput, start);

                c = Advance();
                text += c;

                if (!char.IsDigit(c))
                    return new Token<TokenType>(text, TokenType.Invalid, start);
            }

            return new Token<TokenType>(text, TokenType.Character, start);
        }

        /// <summary>
        /// Consumes the next symbol from the input.
        /// </summary>
        /// <returns>The consumed symbol from the input.</returns>
        private Token<TokenType> Symbol()
        {
            InputPosition start = Position.DeepCopy();
            return new Token<TokenType>(Advance(), TokenType.Symbol, start);
        }

        /// <summary>
        /// Consumes the next keyword from the input.
        /// </summary>
        /// <returns>The consumed keyword from the input.</returns>
        private Token<TokenType> Keyword()
        {
            InputPosition start = Position.DeepCopy();
            string text = "";

            char c;
            while (!Source.EndOfStream)
            {
                c = (char)Source.Peek();
                if (!char.IsLetter(c))
                    break;

                text += Advance();
            }

            if (!text.IsOneOf(Keywords))
                return new Token<TokenType>(text, TokenType.Invalid, start);

            return new Token<TokenType>(text, TokenType.Keyword, start);
        }

        /// <summary>
        /// Consumes the next class literal from the input.
        /// </summary>
        /// <returns>The consumed class literal from the input.</returns>
        private Token<TokenType> ClassLiteral()
        {
            InputPosition start = Position.DeepCopy();
            string text = "";

            text += Advance();
            text += Advance();

            return new Token<TokenType>(text, TokenType.ClassLiteral, start);
        }
    }
}