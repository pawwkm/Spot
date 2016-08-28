using NUnit.Framework;
using Spot.Ebnf;
using Spot.Ebnf.Unicode;
using System.IO;
using System.Linq;
using System.Text;

namespace Spot.SrtL
{
    /// <summary>
    /// Tests implementations using grammar based fuzzing.
    /// </summary>
    public class Fuzzer
    {
        private const string Ebnf = "..\\..\\SrtL\\SrtL.ebnf";

        /// <summary>
        /// Tests the <see cref="Parser"/> 
        /// class using fuzzing.
        /// </summary>
        [Test]
        [Explicit]
        public void SrtL()
        {
            UnicodeSequence.ClearCache();

            var stream = new MemoryStream();
            var reader = new SyntaxReader();
            var syntax = reader.Read(Ebnf);
            var fuzzer = new FuzzyTestGenerator();
            fuzzer.SpecialSequenceGenerators.Add(new UnicodeSequenceGenerator());

            var parser = new Parser();
            foreach (string program in fuzzer.Generate(syntax).Take(1000))
            {
                stream.SetLength(0);

                var writer = new StreamWriter(stream, Encoding.UTF32);
                writer.Write(program);
                stream.Position = 0;

                using (var sr = new StreamReader(stream, Encoding.UTF32, false, 4096, true))
                {
                    var lexer = new LexicalAnalyzer(sr);
                    var tests = parser.Parse(lexer);

                    if (tests.Errors.Count == 0)
                        continue;

                    File.WriteAllText("D:\\Fuck.srtl", program, Encoding.UTF32);

                    var builder = new StringBuilder();
                    builder.AppendLine($"Could not parse the program '{program}'.");

                    foreach (var error in tests.Errors)
                    {
                        builder.Append("    ")
                               .AppendLine(error);
                    }

                    Assert.Fail(builder.ToString());
                }
            }
        }

        /// <summary>
        /// Tests that the <see cref="LexicalAnalyzer"/> identifies all
        /// the possible strings in SrtL.
        /// /// </summary>
        [Test]
        [Explicit]
        public void Strings()
        {
            var stream = new MemoryStream();
            var reader = new SyntaxReader();
            var syntax = reader.Read(Ebnf);
            var fuzzer = new FuzzyTestGenerator();
            fuzzer.SpecialSequenceGenerators.Add(new UnicodeSequenceGenerator());

            foreach (string program in fuzzer.Generate(syntax, "string"))
            {
                stream.SetLength(0);

                var writer = new StreamWriter(stream, Encoding.UTF32);
                writer.Write(program);
                stream.Position = 0;

                using (var sr = new StreamReader(stream, Encoding.UTF32, false, 4096, true))
                {
                    var lexer = new LexicalAnalyzer(sr);
                    while (!lexer.EndOfInput)
                    {
                        var token = lexer.Next();
                        Assert.AreEqual(TokenType.String, token.Type, $"Could not analyze the string '{program}'");
                    }
                }
            }
        }
    }
}