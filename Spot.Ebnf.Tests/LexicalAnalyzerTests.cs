using NUnit.Framework;
using Pote.Text;

namespace Spot.Ebnf
{
    /// <summary>
    /// Provides tests for the <see cref="LexicalAnalyzer"/> class
    /// </summary>
    [TestFixture]
    public class LexicalAnalyzerTests
    {
        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer{TokenType}.Next()"/> can read 
        /// symbols.
        /// </summary>
        [Test]
        public void Next_SymbolsAsInput_Success()
        {
            string[] symbols =
            {
                "(:", "(/", "/)", ":)",
                ",", "=", "|", "/", "!", 
                ")", "]", "}", "-", "(", 
                "[", "{", ";", ".", "*"
            };

            foreach (string symbol in symbols)
            {
                LexicalAnalyzer analyzer = new LexicalAnalyzer(symbol);
                Token<TokenType> token = analyzer.Next();

                Assert.AreEqual(symbol, token.Text);
                Assert.AreEqual(TokenType.Symbol, token.Type);
            }
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer{TokenType}.Next()"/> can read 
        /// meta identifiers.
        /// </summary>
        [Test]
        public void Next_MetaIdentifiersAsInput_Success()
        {
            string[] identifiers = 
            {
                "a", 
                "ab", 
                "a8", 
                "ab ba"
            };

            foreach (string identifier in identifiers)
            {
                LexicalAnalyzer analyzer = new LexicalAnalyzer(identifier);
                Token<TokenType> token = analyzer.Next();

                Assert.AreEqual(identifier, token.Text);
                Assert.AreEqual(TokenType.MetaIdentifier, token.Type);
            }
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer{TokenType}.Next()"/> can read 
        /// special sequences.
        /// </summary>
        [Test]
        public void Next_SpecialSequencesAsInput_Success()
        {
            string[] sequences = 
            { 
                "??",
                "? abc ?"
            };

            foreach (string sequence in sequences)
            {
                LexicalAnalyzer analyzer = new LexicalAnalyzer(sequence);
                Token<TokenType> token = analyzer.Next();

                Assert.AreEqual(sequence.Replace("?", ""), token.Text);
                Assert.AreEqual(TokenType.SpecialSequence, token.Type);
            }
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer{TokenType}.Next()"/> throws an
        /// exception if a special sequence is missing its end mark.
        /// </summary>
        [Test]
        public void Next_MissingEndMarkInSpecialSequenceAsInput_ThrowsException()
        {
            LexicalAnalyzer analyzer = new LexicalAnalyzer("? my special sequence");
            Assert.That(
                () => analyzer.Next(),
                Throws.Exception.TypeOf<SyntaxException>().With.Message.EqualTo("1:22: Expected ?."));
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer{TokenType}.Next()"/> can read 
        /// integers.
        /// </summary>
        [Test]
        public void Next_IntegersAsInput_Success()
        {
            string[] integers = 
            { 
                "1",
                "123",
                "00456"
            };

            foreach (string integer in integers)
            {
                LexicalAnalyzer analyzer = new LexicalAnalyzer(integer);
                Token<TokenType> token = analyzer.Next();

                Assert.AreEqual(integer, token.Text);
                Assert.AreEqual(TokenType.Integer, token.Type);
            }
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer{TokenType}.Next()"/> can read 
        /// terminal strings.
        /// </summary>
        [Test]
        public void Next_TerminalStringsAsInput_Success()
        {
            string[] terminals = 
            { 
                "\"\"",
                "\"abc\"",
                "\"abc def\""
            };

            foreach (string terminal in terminals)
            {
                LexicalAnalyzer analyzer = new LexicalAnalyzer(terminal);
                Token<TokenType> token = analyzer.Next();

                Assert.AreEqual(terminal.Replace("\"", ""), token.Text);
                Assert.AreEqual(TokenType.TerminalString, token.Type);
            }
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer{TokenType}.Next()"/> can 
        /// identify unknown tokens.
        /// </summary>
        [Test]
        public void Next_UnknownAsInput_Success()
        {
            LexicalAnalyzer analyzer = new LexicalAnalyzer(" @");
            Token<TokenType> token = analyzer.Next();

            Assert.AreEqual("@", token.Text);
            Assert.AreEqual(TokenType.Unknown, token.Type);
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer{TokenType}.Next()"/> can 
        /// identify white spaces.
        /// </summary>
        [Test]
        public void Next_WhiteSpaceAsInput_Success()
        {
            var whitespaces = new char[]
            {
                // Unicode class Zs.
                '\u0020', '\u00A0', '\u1680', '\u2000', '\u2001',
                '\u2002', '\u2003', '\u2004', '\u2005', '\u2006',
                '\u2007', '\u2008', '\u2009', '\u200A', '\u202F',
                '\u205F', '\u3000',

                '\u0009', '\u000A', '\u000B', '\u000C', '\u000D',
                '\u0085', '\u2028', '\u2029'
            };

            foreach (var whitespace in whitespaces)
            {
                LexicalAnalyzer analyzer = new LexicalAnalyzer(whitespace.ToString());

                analyzer.Next();

                Assert.AreEqual(1, analyzer.Position.Index);
            }
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer{TokenType}.Next()"/> can 
        /// matches double quotes enclosing a terminal string.
        /// </summary>
        [Test]
        public void Next_TerminalEnclosedInDoubleQuotesContaningSingleQuote_Success()
        {
            var analyzer = new LexicalAnalyzer("\"'\"");
            var token = analyzer.Next();

            Assert.AreEqual("'", token.Text);
            Assert.AreEqual(TokenType.TerminalString, token.Type);
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer{TokenType}.Next()"/> can 
        /// matches single quotes enclosing a terminal string.
        /// </summary>
        [Test]
        public void Next_TerminalEnclosedInSingleQuotesContaningDoubleQuote_Success()
        {
            var analyzer = new LexicalAnalyzer("'\"'");
            var token = analyzer.Next();

            Assert.AreEqual("\"", token.Text);
            Assert.AreEqual(TokenType.TerminalString, token.Type);
        }
    }
}