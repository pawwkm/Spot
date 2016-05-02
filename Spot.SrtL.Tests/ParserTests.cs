using NSubstitute;
using NUnit.Framework;
using Pote.Text;
using System.Collections.Generic;
using System.Linq;

namespace Spot.SrtL
{
    /// <summary>
    /// Provides tests for the <see cref="Parser"/> class
    /// </summary>
    public class ParserTests
    {
        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/> 
        /// can parse a test consisting of some input and the validity 
        /// set to true.
        /// </summary>
        [Test]
        public void Parse_MinimalValidTest_TestParses()
        {
            var builder = new TokenBuilder();
            var tokens = builder.Test()
                                .Input().String("Abc")
                                .Is().Valid()
                                .Build();

            var parser = new Parser();
            var result = parser.Parse(LexicalAnalyzer(tokens));

            Assert.AreEqual(0, result.Errors.Count);

            var tests = result.ToArray();
            Assert.AreEqual(1, tests.Length);

            var test = tests[0];
            Assert.AreEqual(1, test.DefinedAt.Line);
            Assert.AreEqual(1, test.DefinedAt.Column);

            Assert.IsNull(test.Description);
            Assert.IsNull(test.StartFrom);
            Assert.IsNull(test.ExcludingAllRules);
            Assert.IsNull(test.IncludedRules);

            Assert.AreEqual(2, test.Input.DefinedAt.Line);
            Assert.AreEqual(1, test.Input.DefinedAt.Column);

            Assert.AreEqual(1, test.Input.Contents.Strings.Count);
            Assert.AreEqual(2, test.Input.Contents.Strings[0].DefinedAt.Line);
            Assert.AreEqual(8, test.Input.Contents.Strings[0].DefinedAt.Column);
            Assert.AreEqual("Abc", test.Input.Contents.Strings[0].Content);

            Assert.AreEqual(2, test.Validity.DefinedAt.Line);
            Assert.AreEqual(13, test.Validity.DefinedAt.Column);
            Assert.True(test.Validity.IsValid);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/> 
        /// can parse a test consisting of some input and the validity 
        /// set to false.
        /// </summary>
        [Test]
        public void Parse_MinimalInvalidTest_TestParses()
        {
            var builder = new TokenBuilder();
            var tokens = builder.Test()
                                .Input().String("Abc")
                                .Is().Not().Valid()
                                .Build();

            var parser = new Parser();
            var result = parser.Parse(LexicalAnalyzer(tokens));

            Assert.AreEqual(0, result.Errors.Count);

            var tests = result.ToArray();
            Assert.AreEqual(1, tests.Length);

            var test = tests[0];
            Assert.AreEqual(1, test.DefinedAt.Line);
            Assert.AreEqual(1, test.DefinedAt.Column);

            Assert.IsNull(test.Description);
            Assert.IsNull(test.StartFrom);
            Assert.IsNull(test.ExcludingAllRules);
            Assert.IsNull(test.IncludedRules);

            Assert.AreEqual(2, test.Input.DefinedAt.Line);
            Assert.AreEqual(1, test.Input.DefinedAt.Column);

            Assert.AreEqual(1, test.Input.Contents.Strings.Count);
            Assert.AreEqual(2, test.Input.Contents.Strings[0].DefinedAt.Line);
            Assert.AreEqual(8, test.Input.Contents.Strings[0].DefinedAt.Column);
            Assert.AreEqual("Abc", test.Input.Contents.Strings[0].Content);

            Assert.AreEqual(2, test.Validity.DefinedAt.Line);
            Assert.AreEqual(13, test.Validity.DefinedAt.Column);
            Assert.False(test.Validity.IsValid);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/> 
        /// can parse a test when there is an unknown token before the 
        /// 'test' keyword.
        /// </summary>
        [Test]
        public void Parse_UnknownTokenBeforeTestKeyword_ErrorIssued()
        {
            var builder = new TokenBuilder(2, 4, 1);
            var tokens = builder.Unknown("nonsense")
                                .Test()
                                .Input().String("Abc")
                                .Is().Valid()
                                .Build();

            var parser = new Parser();
            var result = parser.Parse(LexicalAnalyzer(tokens));

            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("2:4: Expected 'test' keyword.", result.Errors[0]);

            var tests = result.ToArray();
            Assert.AreEqual(1, tests.Length);

            var test = tests[0];
            Assert.IsNull(test.Description);
            Assert.IsNull(test.StartFrom);
            Assert.IsNull(test.ExcludingAllRules);
            Assert.IsNull(test.IncludedRules);
            Assert.AreEqual(1, test.Input.Contents.Strings.Count);
            Assert.AreEqual("Abc", test.Input.Contents.Strings[0].Content);
            Assert.True(test.Validity.IsValid);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/> 
        /// can parse a test when there is an unknown token before the 
        /// 'input' keyword.
        /// </summary>
        [Test]
        public void Parse_UnknownTokenBeforeInputKeyword_ErrorIssued()
        {
            var builder = new TokenBuilder();
            var tokens = builder.Test()
                                .Unknown("nonsense")
                                .Input().String("Abc")
                                .Is().Valid()
                                .Build();

            var parser = new Parser();
            var result = parser.Parse(LexicalAnalyzer(tokens));

            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("2:1: Expected 'input' keyword.", result.Errors[0]);

            var tests = result.ToArray();
            Assert.AreEqual(1, tests.Length);

            var test = tests[0];
            Assert.IsNull(test.Description);
            Assert.IsNull(test.StartFrom);
            Assert.IsNull(test.ExcludingAllRules);
            Assert.IsNull(test.IncludedRules);
            Assert.AreEqual(1, test.Input.Contents.Strings.Count);
            Assert.AreEqual("Abc", test.Input.Contents.Strings[0].Content);
            Assert.True(test.Validity.IsValid);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/> 
        /// can parse a test when there is an unknown token before the 
        /// string after the 'input' keyword.
        /// </summary>
        [Test]
        public void Parse_UnknownTokenBeforeInputString_ErrorIssued()
        {
            var builder = new TokenBuilder();
            var tokens = builder.Test()
                                .Input().Unknown("nonsense").String("Abc")
                                .Is().Valid()
                                .Build();

            var parser = new Parser();
            var result = parser.Parse(LexicalAnalyzer(tokens));

            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("2:8: Expected a concatenated string.", result.Errors[0]);

            var tests = result.ToArray();
            Assert.AreEqual(1, tests.Length);

            var test = tests[0];
            Assert.IsNull(test.Description);
            Assert.IsNull(test.StartFrom);
            Assert.IsNull(test.ExcludingAllRules);
            Assert.IsNull(test.IncludedRules);
            Assert.AreEqual(1, test.Input.Contents.Strings.Count);
            Assert.AreEqual("Abc", test.Input.Contents.Strings[0].Content);
            Assert.True(test.Validity.IsValid);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/> 
        /// can parse a test when there is an unknown token before the 
        /// string after the 'input' keyword.
        /// </summary>
        [Test]
        public void Parse_UnknownTokenBeforeIsKeyword_ErrorIssued()
        {
            var builder = new TokenBuilder();
            var tokens = builder.Test()
                                .Input().String("Abc")
                                .Unknown("nonsense").Is().Valid()
                                .Build();

            var parser = new Parser();
            var result = parser.Parse(LexicalAnalyzer(tokens));

            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("2:13: Expected 'is' keyword.", result.Errors[0]);

            var tests = result.ToArray();
            Assert.AreEqual(1, tests.Length);

            var test = tests[0];
            Assert.IsNull(test.Description);
            Assert.IsNull(test.StartFrom);
            Assert.IsNull(test.ExcludingAllRules);
            Assert.IsNull(test.IncludedRules);
            Assert.AreEqual(1, test.Input.Contents.Strings.Count);
            Assert.AreEqual("Abc", test.Input.Contents.Strings[0].Content);
            Assert.True(test.Validity.IsValid);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/> 
        /// can parse a test when there is an unknown token before the 
        /// string after the 'input' keyword.
        /// </summary>
        [Test]
        public void Parse_UnknownTokenBeforeValidKeyword_ErrorIssued()
        {
            var builder = new TokenBuilder();
            var tokens = builder.Test()
                                .Input().String("Abc")
                                .Is().Unknown("nonsense").Valid()
                                .Build();

            var parser = new Parser();
            var result = parser.Parse(LexicalAnalyzer(tokens));

            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("2:17: Expected 'valid' keyword.", result.Errors[0]);

            var tests = result.ToArray();
            Assert.AreEqual(1, tests.Length);

            var test = tests[0];
            Assert.IsNull(test.Description);
            Assert.IsNull(test.StartFrom);
            Assert.IsNull(test.ExcludingAllRules);
            Assert.IsNull(test.IncludedRules);
            Assert.AreEqual(1, test.Input.Contents.Strings.Count);
            Assert.AreEqual("Abc", test.Input.Contents.Strings[0].Content);
            Assert.True(test.Validity.IsValid);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/> 
        /// can parse a test consisting of a description, some input and the validity 
        /// set to true.
        /// </summary>
        [Test]
        public void Parse_TestWithDescription_TestParses()
        {
            var builder = new TokenBuilder();
            var tokens = builder.Test()
                                .Description().String("This tests something.")
                                .Input().String("Abc")
                                .Is().Valid()
                                .Build();

            var parser = new Parser();
            var result = parser.Parse(LexicalAnalyzer(tokens));

            Assert.AreEqual(0, result.Errors.Count);

            var tests = result.ToArray();
            Assert.AreEqual(1, tests.Length);

            var test = tests[0];
            Assert.AreEqual(1, test.DefinedAt.Line);
            Assert.AreEqual(1, test.DefinedAt.Column);

            Assert.AreEqual(2, test.Description.DefinedAt.Line);
            Assert.AreEqual(1, test.Description.DefinedAt.Column);
            Assert.AreEqual("This tests something.", test.Description.Text.Concatenate());

            Assert.AreEqual(2, test.Input.DefinedAt.Line);
            Assert.AreEqual(37, test.Input.DefinedAt.Column);

            Assert.AreEqual(1, test.Input.Contents.Strings.Count);
            Assert.AreEqual(2, test.Input.Contents.Strings[0].DefinedAt.Line);
            Assert.AreEqual(44, test.Input.Contents.Strings[0].DefinedAt.Column);
            Assert.AreEqual("Abc", test.Input.Contents.Strings[0].Content);

            Assert.IsNull(test.StartFrom);
            Assert.IsNull(test.ExcludingAllRules);
            Assert.IsNull(test.IncludedRules);

            Assert.AreEqual(2, test.Validity.DefinedAt.Line);
            Assert.AreEqual(49, test.Validity.DefinedAt.Column);
            Assert.True(test.Validity.IsValid);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/> 
        /// can parse a test consisting of some input, starting point and the validity 
        /// set to true.
        /// </summary>
        [Test]
        public void Parse_TestWithStartingPoint_TestParses()
        {
            var builder = new TokenBuilder();
            var tokens = builder.Test()
                                .Input().String("Abc")
                                .Start().From().String("my rule")
                                .Is().Valid()
                                .Build();

            var parser = new Parser();
            var result = parser.Parse(LexicalAnalyzer(tokens));

            Assert.AreEqual(0, result.Errors.Count);

            var tests = result.ToArray();
            Assert.AreEqual(1, tests.Length);

            var test = tests[0];
            Assert.AreEqual(1, test.DefinedAt.Line);
            Assert.AreEqual(1, test.DefinedAt.Column);

            Assert.IsNull(test.Description);
            Assert.IsNull(test.ExcludingAllRules);
            Assert.IsNull(test.IncludedRules);

            Assert.AreEqual(2, test.Input.DefinedAt.Line);
            Assert.AreEqual(1, test.Input.DefinedAt.Column);

            Assert.AreEqual(1, test.Input.Contents.Strings.Count);
            Assert.AreEqual(2, test.Input.Contents.Strings[0].DefinedAt.Line);
            Assert.AreEqual(8, test.Input.Contents.Strings[0].DefinedAt.Column);
            Assert.AreEqual("Abc", test.Input.Contents.Strings[0].Content);

            Assert.AreEqual(2, test.StartFrom.DefinedAt.Line);
            Assert.AreEqual(13, test.StartFrom.DefinedAt.Column);
            Assert.AreEqual("my rule", test.StartFrom.Rule.Content);

            Assert.AreEqual(2, test.Validity.DefinedAt.Line);
            Assert.AreEqual(33, test.Validity.DefinedAt.Column);
            Assert.True(test.Validity.IsValid);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/> 
        /// can parse a test consisting of some input, 'exclude all rules' keyowrds
        /// and the validity set to true.
        /// </summary>
        [Test]
        public void Parse_TestWithExcludeAllRules_TestParses()
        {
            var builder = new TokenBuilder();
            var tokens = builder.Test()
                                .Input().String("Abc")
                                .Exclude().All().Rules()
                                .Is().Valid()
                                .Build();

            var parser = new Parser();
            var result = parser.Parse(LexicalAnalyzer(tokens));

            Assert.AreEqual(0, result.Errors.Count);

            var tests = result.ToArray();
            Assert.AreEqual(1, tests.Length);

            var test = tests[0];
            Assert.AreEqual(1, test.DefinedAt.Line);
            Assert.AreEqual(1, test.DefinedAt.Column);

            Assert.IsNull(test.Description);
            Assert.IsNull(test.StartFrom);
            Assert.IsNull(test.IncludedRules);

            Assert.AreEqual(2, test.Input.DefinedAt.Line);
            Assert.AreEqual(1, test.Input.DefinedAt.Column);

            Assert.AreEqual(1, test.Input.Contents.Strings.Count);
            Assert.AreEqual(2, test.Input.Contents.Strings[0].DefinedAt.Line);
            Assert.AreEqual(8, test.Input.Contents.Strings[0].DefinedAt.Column);
            Assert.AreEqual("Abc", test.Input.Contents.Strings[0].Content);

            Assert.AreEqual(2, test.ExcludingAllRules.ExcludeDefinedAt.Line);
            Assert.AreEqual(13, test.ExcludingAllRules.ExcludeDefinedAt.Column);

            Assert.AreEqual(2, test.ExcludingAllRules.AllDefinedAt.Line);
            Assert.AreEqual(21, test.ExcludingAllRules.AllDefinedAt.Column);

            Assert.AreEqual(2, test.ExcludingAllRules.RulesDefinedAt.Line);
            Assert.AreEqual(25, test.ExcludingAllRules.RulesDefinedAt.Column);

            Assert.AreEqual(2, test.Validity.DefinedAt.Line);
            Assert.AreEqual(31, test.Validity.DefinedAt.Column);
            Assert.True(test.Validity.IsValid);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/> 
        /// can parse a test consisting of some input, include rules part and the validity 
        /// set to true.
        /// </summary>
        [Test]
        public void Parse_TestWithIncludedRules_TestParses()
        {
            var builder = new TokenBuilder();
            var tokens = builder.Test()
                                .Input().String("Abc")
                                .Include().String("rule1").Comma().String("rule2")
                                .Is().Valid()
                                .Build();

            var parser = new Parser();
            var result = parser.Parse(LexicalAnalyzer(tokens));

            Assert.AreEqual(0, result.Errors.Count);

            var tests = result.ToArray();
            Assert.AreEqual(1, tests.Length);

            var test = tests[0];
            Assert.AreEqual(1, test.DefinedAt.Line);
            Assert.AreEqual(1, test.DefinedAt.Column);

            Assert.IsNull(test.Description);
            Assert.IsNull(test.StartFrom);
            Assert.IsNull(test.ExcludingAllRules);

            Assert.AreEqual(2, test.Input.DefinedAt.Line);
            Assert.AreEqual(1, test.Input.DefinedAt.Column);

            Assert.AreEqual(1, test.Input.Contents.Strings.Count);
            Assert.AreEqual(2, test.Input.Contents.Strings[0].DefinedAt.Line);
            Assert.AreEqual(8, test.Input.Contents.Strings[0].DefinedAt.Column);
            Assert.AreEqual("Abc", test.Input.Contents.Strings[0].Content);

            Assert.AreEqual(2, test.IncludedRules.Rules.Count);

            Assert.AreEqual("rule1", test.IncludedRules.Rules[0].Content);
            Assert.AreEqual(2, test.IncludedRules.Rules[0].DefinedAt.Line);
            Assert.AreEqual(22, test.IncludedRules.Rules[0].DefinedAt.Column);

            Assert.AreEqual("rule2", test.IncludedRules.Rules[1].Content);
            Assert.AreEqual(2, test.IncludedRules.Rules[1].DefinedAt.Line);
            Assert.AreEqual(30, test.IncludedRules.Rules[1].DefinedAt.Column);

            Assert.AreEqual(2, test.Validity.DefinedAt.Line);
            Assert.AreEqual(37, test.Validity.DefinedAt.Column);
            Assert.True(test.Validity.IsValid);
        }

        /// <summary>
        /// Creates a substitute for an <see cref="LexicalAnalyzer{TokenType}"/>.
        /// </summary>
        /// <param name="tokens">The tokens the analyzer will consume.</param>
        /// <returns>The substitute analyzer.</returns>
        private static LexicalAnalyzer<TokenType> LexicalAnalyzer(IList<Token<TokenType>> tokens)
        {
            LexicalAnalyzer<TokenType> analyzer = Substitute.For<LexicalAnalyzer<TokenType>>();

            int current = 0;
            analyzer.Next().Returns(x =>
            {
                if (analyzer.EndOfInput)
                    return new Token<TokenType>("", TokenType.EndOfInput, new InputPosition());

                return tokens[current++];
            });

            analyzer.LookAhead(Arg.Any<int>()).Returns(x =>
            {
                if (current + x.Arg<int>() - 1 == tokens.Count)
                    return new Token<TokenType>("", TokenType.EndOfInput, new InputPosition());

                return tokens[current + x.Arg<int>() - 1];
            });

            analyzer.EndOfInput.Returns(x => current == tokens.Count);

            return analyzer;
        }
    }
}