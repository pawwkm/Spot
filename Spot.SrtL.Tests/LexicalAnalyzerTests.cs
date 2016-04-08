using NUnit.Framework;
using Pote.Text;
using System.IO;
using System.Text;

namespace Spot.SrtL
{
    /// <summary>
    /// Provides tests for the <see cref="LexicalAnalyzer"/> class
    /// </summary>
    public class LexicalAnalyzerTests
    {
        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer(StreamReader)"/> accepts 
        /// Utf8 encoding input.
        /// </summary>
        [Test]
        public void LexicalAnalyzer_Utf8EncodedInput_Accepted()
        {
            using (MemoryStream input = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(input, Encoding.UTF8, 128, true))
                {
                    writer.Write("test");
                    writer.Flush();
                }

                input.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(input))
                    Assert.DoesNotThrow(() => new LexicalAnalyzer(reader));
            }
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer(StreamReader)"/> accepts 
        /// Utf16 big endian encoding input.
        /// </summary>
        [Test]
        public void LexicalAnalyzer_Utf16BeEncodedInput_Accepted()
        {
            using (MemoryStream input = new MemoryStream())
            {
                Encoding encoding = new UnicodeEncoding(true, true);
                using (StreamWriter writer = new StreamWriter(input, encoding, 128, true))
                {
                    writer.Write("test");
                    writer.Flush();
                }

                input.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(input))
                    Assert.DoesNotThrow(() => new LexicalAnalyzer(reader));
            }
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer(StreamReader)"/> accepts 
        /// Utf16 little endian encoding input.
        /// </summary>
        [Test]
        public void LexicalAnalyzer_Utf16LeEncodedInput_Accepted()
        {
            using (MemoryStream input = new MemoryStream())
            {
                Encoding encoding = new UnicodeEncoding(false, true);
                using (StreamWriter writer = new StreamWriter(input, encoding, 128, true))
                {
                    writer.Write("test");
                    writer.Flush();
                }

                input.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(input))
                    Assert.DoesNotThrow(() => new LexicalAnalyzer(reader));
            }
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer(StreamReader)"/> accepts 
        /// Utf32 big endian encoding input.
        /// </summary>
        [Test]
        public void LexicalAnalyzer_Utf32BeEncodedInput_Accepted()
        {
            using (MemoryStream input = new MemoryStream())
            {
                Encoding encoding = new UTF32Encoding(true, true);
                using (StreamWriter writer = new StreamWriter(input, encoding, 128, true))
                {
                    writer.Write("test");
                    writer.Flush();
                }

                input.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(input))
                    Assert.DoesNotThrow(() => new LexicalAnalyzer(reader));
            }
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer(StreamReader)"/> accepts 
        /// Utf32 little endian encoding input.
        /// </summary>
        [Test]
        public void LexicalAnalyzer_Utf32LeEncodedInput_Accepted()
        {
            using (MemoryStream input = new MemoryStream())
            {
                Encoding encoding = new UTF32Encoding(false, true);
                using (StreamWriter writer = new StreamWriter(input, encoding, 128, true))
                {
                    writer.Write("test");
                    writer.Flush();
                }

                input.Seek(0, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(input))
                    Assert.DoesNotThrow(() => new LexicalAnalyzer(reader));
            }
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer{TokenType}.Next()"/> can read 
        /// all the keywords.
        /// </summary>
        [Test]
        public void Next_KeywordsAsInput_KeywordsRecognized()
        {
            string[] keywords = 
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

            foreach (string keyword in keywords)
            {
                LexicalAnalyzer analyzer = new LexicalAnalyzer(keyword);
                Token<TokenType> token = analyzer.Next();

                Assert.AreEqual(keyword, token.Text);
                Assert.AreEqual(TokenType.Keyword, token.Type);
            }
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer{TokenType}.Next()"/> can read 
        /// comma.
        /// </summary>
        [Test]
        public void Next_CommaAsInput_RecognizedAsSymbol()
        {
            LexicalAnalyzer analyzer = new LexicalAnalyzer(",");
            Token<TokenType> token = analyzer.Next();

            Assert.AreEqual(",", token.Text);
            Assert.AreEqual(TokenType.Symbol, token.Type);
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer{TokenType}.Next()"/> signals 
        /// the caller correctly when there is no more input.
        /// </summary>
        [Test]
        public void Next_NoMoreInput_ReturnsEndOfInputToken()
        {
            LexicalAnalyzer analyzer = new LexicalAnalyzer("");
            Token<TokenType> token = analyzer.Next();

            Assert.AreEqual("", token.Text);
            Assert.AreEqual(TokenType.EndOfInput, token.Type);
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer{TokenType}.Next()"/> can read 
        /// empty strings.
        /// </summary>
        [Test]
        public void Next_EmptyStringAsInput_StringRecognized()
        {
            LexicalAnalyzer analyzer = new LexicalAnalyzer("\"\"");
            Token<TokenType> token = analyzer.Next();

            Assert.AreEqual("", token.Text);
            Assert.AreEqual(TokenType.String, token.Type);
        }

        /// <summary>
        /// Tests that <see cref="LexicalAnalyzer{TokenType}.Next()"/> can read 
        /// strings with content.
        /// </summary>
        [Test]
        public void Next_StringWithContentAsInput_StringRecognized()
        {
            LexicalAnalyzer analyzer = new LexicalAnalyzer("\"Abc def\"");
            Token<TokenType> token = analyzer.Next();

            Assert.AreEqual("Abc def", token.Text);
            Assert.AreEqual(TokenType.String, token.Type);
        }
    }
}