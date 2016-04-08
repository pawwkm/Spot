using Pote;
using Pote.Text;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Spot.SrtL
{
    /// <summary>
    /// The lexical analyzer for SrtL tests.
    /// </summary>
    internal sealed class LexicalAnalyzer : LexicalAnalyzer<TokenType>
    {
        private static readonly string[] allowedEncodings = 
        {
            "utf-8",
            "utf-16",   // Little endian.
            "utf-16BE",
            "utf-32",   // Little endian.
            "utf-32BE"
        };

        private static readonly string[] Keywords =
        {
            "test",
            "input",
            "is",
            "not",
            "valid",
            "description",
            "start",
            "from",
            "include",
            "exclude",
            "rules",
            "all"
        };

        private static readonly string[] EscapeSequences =
        {
            @"\""",
            @"\\",
            @"\0",
            @"\a",
            @"\b",
            @"\f",
            @"\n",
            @"\r",
            @"\t",
            @"\v"
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="LexicalAnalyzer"/> class.
        /// </summary>
        /// <param name="reader">The source to analyze.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="reader"/> has invalid encoding. Only
        /// Utf8, Utf16be, Utf16le, Utf32be and Utf32le encodings 
        /// are allowed.
        /// </exception>
        public LexicalAnalyzer(StreamReader reader) : base(reader)
        {
            // The value is not needed. This call forces the reader
            // to determine the encoding of the underlying stream.
            reader.Peek();

            if (!reader.CurrentEncoding.BodyName.IsOneOf(allowedEncodings))
                throw new ArgumentException("Invalid encoding.", nameof(reader));
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
        /// <exception cref="ArgumentException">
        /// <paramref name="reader"/> has invalid encoding. Only
        /// Utf8, Utf16be, Utf16le, Utf32be and Utf32le encodings 
        /// are allowed.
        /// </exception>
        public LexicalAnalyzer(StreamReader reader, string origin) : base(reader, origin)
        {
            // The value is not needed. This call forces the reader
            // to determine the encoding of the underlying stream.
            reader.Peek();

            if (!reader.CurrentEncoding.BodyName.IsOneOf(allowedEncodings))
                throw new ArgumentException("Invalid encoding.", nameof(reader));
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
        protected override Token<TokenType> NextTokenFromSource()
        {
            SkipWhitespaces();
            if (Source.EndOfStream)
                return new Token<TokenType>("", TokenType.EndOfInput, Position.DeepCopy());

            char c = (char)Source.Peek();
            if (c.IsOneOf(','))
                return Symbol();
            if (c == '"')
                return String();
            if (Source.MatchesAnyOf(Keywords))
                return Keyword();

            InputPosition start = Position.DeepCopy();
            return new Token<TokenType>(Advance(), TokenType.Unknown, start);
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
                return new Token<TokenType>(text, TokenType.Unknown, start);

            return new Token<TokenType>(text, TokenType.Keyword, start);
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
        /// Consumes the next string from the input.
        /// </summary>
        /// <returns>The consumed string from the input.</returns>
        private Token<TokenType> String()
        {
            InputPosition start = Position.DeepCopy();
            string text = "";

            char c = Advance();
            if (c != '"')
                return new Token<TokenType>(text, TokenType.Unknown, start);

            while (!Source.EndOfStream)
            {
                c = (char)Source.Peek();
                if (!IsStringCharacter(c))
                    break;

                text += Advance();
            }

            c = Advance();
            if (c != '"')
                return new Token<TokenType>(text, TokenType.Unknown, start);

            return new Token<TokenType>(text, TokenType.String, start);
        }

        /// <summary>
        /// Skips the white spaces in the input.
        /// </summary>
        private void SkipWhitespaces()
        {
            char[] characters = 
            {
                '\u0009', '\u000B', '\u000C', '\u000D',
                '\u000A', '\u0085', '\u2028', '\u2029'
            };

            while (!Source.EndOfStream)
            {
                char c = (char)Source.Peek();
                UnicodeCategory category = char.GetUnicodeCategory(c);

                if (category != UnicodeCategory.SpaceSeparator || !characters.Contains(c))
                    break;

                Advance();
            }
        }

        private static bool IsStringCharacter(char c)
        {
            char[] characters =
            {
                '\u0022', '\u007F', '\u0081',
                '\u008D', '\u008F', '\u0090',
                '\u009D', '\u1680', '\u180E',
                '\u205F', '\u2060', '\u00A0',
                '\u3000', '\uFEFF'
            };

            UnicodeCategory[] categories = 
            {
                UnicodeCategory.LineSeparator,
                UnicodeCategory.ParagraphSeparator,
            };

            UnicodeCategory category = char.GetUnicodeCategory(c);
            if (categories.Contains(category))
                return false;

            if (characters.Contains(c))
                return false;

            if (c >= '\u0000' && c <= '\u001F')
                return false;

            if (c >= '\u2000' && c <= '\u200F')
                return false;

            if (c >= '\u202A' && c <= '\u202F')
                return false;

            if (c >= '\u2066' && c <= '\u206F')
                return false;

            if (c >= '\uFFF9' && c <= '\uFFFB')
                return false;

            return true;
        }
    }
}