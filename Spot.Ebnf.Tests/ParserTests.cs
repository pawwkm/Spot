using NSubstitute;
using NUnit.Framework;
using Pote.Text;
using System.Collections.Generic;

namespace Spot.Ebnf
{
    /// <summary>
    /// Provides tests for the <see cref="Parser"/> class.
    /// </summary>
    [TestFixture]
    public class ParserTests
    {
        /// <summary>
        /// Test that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse a rule that contains terminal string.
        /// </summary>
        [Test]
        public void Parse_TerminalString_Success()
        {
            TokenBuilder builder = new TokenBuilder();
            var tokens = builder.MetaIdentifier("Rule")
                                .Symbol("=")
                                .TerminalString("Abc")
                                .Symbol(";")
                                .Build();

            Parser parser = new Parser();
            Syntax syntax = parser.Parse(LexicalAnalyzer(tokens));

            RuleReferenceResolver resolver = new RuleReferenceResolver();
            resolver.Resolve(syntax);

            Assert.AreEqual("Rule", syntax.Start.MetaIdentifier.Text);

            Assert.AreEqual(1, syntax.Start.Branches.Count);
            Assert.AreEqual(1, syntax.Start.Branches[0].Count);
            
            SingleDefinition single = (SingleDefinition)syntax.Start.Branches[0][0];
            Assert.AreEqual(1, single.SyntacticTerms.Count);
            Assert.Null(single.SyntacticTerms[0].Exception);

            Assert.AreEqual(1, single.SyntacticTerms[0].Factor.NumberOfRepetitions);

            Assert.AreEqual("Abc", ((TerminalString)single.SyntacticTerms[0].Factor.SyntacticPrimary).Value.Text);
        }

        /// <summary>
        /// Test that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse a rule that contains 2 terminal string branches.
        /// </summary>
        [Test]
        public void Parse_2TerminalStringBranches_Success()
        {
            TokenBuilder builder = new TokenBuilder();
            var tokens = builder.MetaIdentifier("Rule")
                                .Symbol("=")
                                .TerminalString("Abc")
                                .Symbol("|")
                                .TerminalString("Def")
                                .Symbol(";")
                                .Build();

            Parser parser = new Parser();
            Syntax syntax = parser.Parse(LexicalAnalyzer(tokens));

            RuleReferenceResolver resolver = new RuleReferenceResolver();
            resolver.Resolve(syntax);

            Assert.AreEqual("Rule", syntax.Start.MetaIdentifier.Text);

            Assert.AreEqual(2, syntax.Start.Branches.Count);
            Assert.AreEqual(1, syntax.Start.Branches[0].Count);
            Assert.AreEqual(1, syntax.Start.Branches[1].Count);

            SingleDefinition single1 = (SingleDefinition)syntax.Start.Branches[0][0];
            Assert.AreEqual(1, single1.SyntacticTerms.Count);
            Assert.Null(single1.SyntacticTerms[0].Exception);

            Assert.AreEqual(1, single1.SyntacticTerms[0].Factor.NumberOfRepetitions);
            
            Assert.AreEqual("Abc", ((TerminalString)single1.SyntacticTerms[0].Factor.SyntacticPrimary).Value.Text);

            SingleDefinition single2 = (SingleDefinition)syntax.Start.Branches[1][0];
            Assert.AreEqual(1, single2.SyntacticTerms.Count);
            
            Assert.AreEqual(1, single2.SyntacticTerms[0].Factor.NumberOfRepetitions);
            
            Assert.AreEqual("Def", ((TerminalString)single2.SyntacticTerms[0].Factor.SyntacticPrimary).Value.Text);
        }

        /// <summary>
        /// Test that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse a rule that contains terminal string times x.
        /// </summary>
        [Test]
        public void Parse_TerminalStringTimesX_Success()
        {
            TokenBuilder builder = new TokenBuilder();
            var tokens = builder.MetaIdentifier("Rule")
                                .Symbol("=")
                                .Integer("3")
                                .Symbol("*")
                                .TerminalString("Abc")
                                .Symbol(";")
                                .Build();

            Parser parser = new Parser();
            Syntax syntax = parser.Parse(LexicalAnalyzer(tokens));

            RuleReferenceResolver resolver = new RuleReferenceResolver();
            resolver.Resolve(syntax);

            Assert.AreEqual("Rule", syntax.Start.MetaIdentifier.Text);

            Assert.AreEqual(1, syntax.Start.Branches.Count);
            Assert.AreEqual(1, syntax.Start.Branches[0].Count);

            SingleDefinition single = (SingleDefinition)syntax.Start.Branches[0][0];
            Assert.AreEqual(1, single.SyntacticTerms.Count);
            Assert.Null(single.SyntacticTerms[0].Exception);

            Assert.AreEqual(3, single.SyntacticTerms[0].Factor.NumberOfRepetitions);

            Assert.AreEqual("Abc", ((TerminalString)single.SyntacticTerms[0].Factor.SyntacticPrimary).Value.Text);
        }

        /// <summary>
        /// Test that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse a rule that contains meta identifier.
        /// </summary>
        [Test]
        public void Parse_MetaIdentifier_Success()
        {
            TokenBuilder builder = new TokenBuilder();
            var tokens = builder.MetaIdentifier("Rule")
                                .Symbol("=")
                                .MetaIdentifier("Rule")
                                .Symbol(";")
                                .Build();

            Parser parser = new Parser();
            Syntax syntax = parser.Parse(LexicalAnalyzer(tokens));

            RuleReferenceResolver resolver = new RuleReferenceResolver();
            resolver.Resolve(syntax);

            Assert.AreEqual("Rule", syntax.Start.MetaIdentifier.Text);

            Assert.AreEqual(1, syntax.Start.Branches.Count);
            Assert.AreEqual(1, syntax.Start.Branches[0].Count);

            SingleDefinition single = (SingleDefinition)syntax.Start.Branches[0][0];
            Assert.AreEqual(1, single.SyntacticTerms.Count);
            Assert.Null(single.SyntacticTerms[0].Exception);

            Assert.AreEqual(1, single.SyntacticTerms[0].Factor.NumberOfRepetitions);

            Assert.AreEqual("Rule", ((MetaIdentifier)single.SyntacticTerms[0].Factor.SyntacticPrimary).Value.Text);
        }

        /// <summary>
        /// Test that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse a rule that special sequence identifier.
        /// </summary>
        [Test]
        public void Parse_SpecialSequence_Success()
        {
            TokenBuilder builder = new TokenBuilder();
            var tokens = builder.MetaIdentifier("Rule")
                                .Symbol("=")
                                .SpecialSequence("Something")
                                .Symbol(";")
                                .Build();

            Parser parser = new Parser();
            Syntax syntax = parser.Parse(LexicalAnalyzer(tokens));

            RuleReferenceResolver resolver = new RuleReferenceResolver();
            resolver.Resolve(syntax);

            Assert.AreEqual("Rule", syntax.Start.MetaIdentifier.Text);

            Assert.AreEqual(1, syntax.Start.Branches.Count);
            Assert.AreEqual(1, syntax.Start.Branches[0].Count);

            SingleDefinition single = (SingleDefinition)syntax.Start.Branches[0][0];
            Assert.AreEqual(1, single.SyntacticTerms.Count);
            Assert.Null(single.SyntacticTerms[0].Exception);

            Assert.AreEqual(1, single.SyntacticTerms[0].Factor.NumberOfRepetitions);

            Assert.AreEqual("Something", ((SpecialSequence)single.SyntacticTerms[0].Factor.SyntacticPrimary).Value.Text);
        }

        /// <summary>
        /// Test that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse a rule that empty sequence identifier.
        /// </summary>
        [Test]
        public void Parse_EmptySequence_Success()
        {
            TokenBuilder builder = new TokenBuilder();
            var tokens = builder.MetaIdentifier("Rule")
                                .Symbol("=")
                                .Symbol(";")
                                .Build();

            Parser parser = new Parser();
            Syntax syntax = parser.Parse(LexicalAnalyzer(tokens));

            RuleReferenceResolver resolver = new RuleReferenceResolver();
            resolver.Resolve(syntax);

            Assert.AreEqual("Rule", syntax.Start.MetaIdentifier.Text);

            Assert.AreEqual(0, syntax.Start.Branches.Count);
        }

        /// <summary>
        /// Test that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse a rule that grouped sequence.
        /// </summary>
        [Test]
        public void Parse_GroupedSequence_Success()
        {
            TokenBuilder builder = new TokenBuilder();
            var tokens = builder.MetaIdentifier("Rule")
                                .Symbol("=")
                                .Symbol("(")
                                .TerminalString("Abc")
                                .Symbol(")")
                                .Symbol(";")
                                .Build();

            Parser parser = new Parser();
            Syntax syntax = parser.Parse(LexicalAnalyzer(tokens));

            RuleReferenceResolver resolver = new RuleReferenceResolver();
            resolver.Resolve(syntax);

            Assert.AreEqual("Rule", syntax.Start.MetaIdentifier.Text);

            Assert.AreEqual(1, syntax.Start.Branches.Count);
            Assert.AreEqual(1, syntax.Start.Branches[0].Count);

            SingleDefinition single1 = (SingleDefinition)syntax.Start.Branches[0][0];
            Assert.AreEqual(1, single1.SyntacticTerms.Count);
            Assert.Null(single1.SyntacticTerms[0].Exception);

            Assert.AreEqual(1, single1.SyntacticTerms[0].Factor.NumberOfRepetitions);

            GroupedSequence group = (GroupedSequence)single1.SyntacticTerms[0].Factor.SyntacticPrimary;
            SingleDefinition single2 = (SingleDefinition)group.Branches[0][0];
            Assert.AreEqual(1, single2.SyntacticTerms.Count);
            Assert.Null(single2.SyntacticTerms[0].Exception);

            Assert.AreEqual(1, single2.SyntacticTerms[0].Factor.NumberOfRepetitions);

            Assert.AreEqual("Abc", ((TerminalString)single2.SyntacticTerms[0].Factor.SyntacticPrimary).Value.Text);
        }

        /// <summary>
        /// Test that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse a rule that grouped sequence with two branches.
        /// </summary>
        [Test]
        public void Parse_GroupedSequenceWith2Branches_Success()
        {
            TokenBuilder builder = new TokenBuilder();
            var tokens = builder.MetaIdentifier("Rule")
                                .Symbol("=")
                                .Symbol("(")
                                .TerminalString("Abc")
                                .Symbol("|")
                                .TerminalString("Def")
                                .Symbol(")")
                                .Symbol(";")
                                .Build();

            Parser parser = new Parser();
            Syntax syntax = parser.Parse(LexicalAnalyzer(tokens));

            RuleReferenceResolver resolver = new RuleReferenceResolver();
            resolver.Resolve(syntax);

            Assert.AreEqual("Rule", syntax.Start.MetaIdentifier.Text);

            Assert.AreEqual(1, syntax.Start.Branches.Count);
            Assert.AreEqual(1, syntax.Start.Branches[0].Count);

            SingleDefinition single1 = (SingleDefinition)syntax.Start.Branches[0][0];
            Assert.AreEqual(1, single1.SyntacticTerms.Count);
            Assert.Null(single1.SyntacticTerms[0].Exception);

            Assert.AreEqual(1, single1.SyntacticTerms[0].Factor.NumberOfRepetitions);

            GroupedSequence group = (GroupedSequence)single1.SyntacticTerms[0].Factor.SyntacticPrimary;
            SingleDefinition single2 = (SingleDefinition)group.Branches[0][0];
            Assert.AreEqual(1, single2.SyntacticTerms.Count);
            Assert.Null(single2.SyntacticTerms[0].Exception);

            Assert.AreEqual(1, single2.SyntacticTerms[0].Factor.NumberOfRepetitions);

            Assert.AreEqual("Abc", ((TerminalString)single2.SyntacticTerms[0].Factor.SyntacticPrimary).Value.Text);

            SingleDefinition single3 = (SingleDefinition)group.Branches[1][0];
            Assert.AreEqual(1, single3.SyntacticTerms.Count);
            Assert.Null(single3.SyntacticTerms[0].Exception);

            Assert.AreEqual(1, single3.SyntacticTerms[0].Factor.NumberOfRepetitions);

            Assert.AreEqual("Def", ((TerminalString)single3.SyntacticTerms[0].Factor.SyntacticPrimary).Value.Text);
        }

        /// <summary>
        /// Test that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse a rule that optional sequence.
        /// </summary>
        [Test]
        public void Parse_OptionalSequence_Success()
        {
            TokenBuilder builder = new TokenBuilder();
            var tokens = builder.MetaIdentifier("Rule")
                                .Symbol("=")
                                .Symbol("[")
                                .TerminalString("Abc")
                                .Symbol("]")
                                .Symbol(";")
                                .Build();

            Parser parser = new Parser();
            Syntax syntax = parser.Parse(LexicalAnalyzer(tokens));

            RuleReferenceResolver resolver = new RuleReferenceResolver();
            resolver.Resolve(syntax);

            Assert.AreEqual("Rule", syntax.Start.MetaIdentifier.Text);

            Assert.AreEqual(1, syntax.Start.Branches.Count);
            Assert.AreEqual(1, syntax.Start.Branches[0].Count);

            SingleDefinition single1 = (SingleDefinition)syntax.Start.Branches[0][0];
            Assert.AreEqual(1, single1.SyntacticTerms.Count);
            Assert.Null(single1.SyntacticTerms[0].Exception);

            Assert.AreEqual(1, single1.SyntacticTerms[0].Factor.NumberOfRepetitions);

            OptionalSequence sequence = (OptionalSequence)single1.SyntacticTerms[0].Factor.SyntacticPrimary;
            SingleDefinition single2 = (SingleDefinition)sequence.Branches[0][0];
            Assert.AreEqual(1, single2.SyntacticTerms.Count);
            Assert.Null(single2.SyntacticTerms[0].Exception);

            Assert.AreEqual(1, single2.SyntacticTerms[0].Factor.NumberOfRepetitions);

            Assert.AreEqual("Abc", ((TerminalString)single2.SyntacticTerms[0].Factor.SyntacticPrimary).Value.Text);
        }

        /// <summary>
        /// Test that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse a rule that optional sequence with two branches.
        /// </summary>
        [Test]
        public void Parse_OptionalSequenceWith2Branches_Success()
        {
            TokenBuilder builder = new TokenBuilder();
            var tokens = builder.MetaIdentifier("Rule")
                                .Symbol("=")
                                .Symbol("[")
                                .TerminalString("Abc")
                                .Symbol("|")
                                .TerminalString("Def")
                                .Symbol("]")
                                .Symbol(";")
                                .Build();

            Parser parser = new Parser();
            Syntax syntax = parser.Parse(LexicalAnalyzer(tokens));

            RuleReferenceResolver resolver = new RuleReferenceResolver();
            resolver.Resolve(syntax);

            Assert.AreEqual("Rule", syntax.Start.MetaIdentifier.Text);

            Assert.AreEqual(1, syntax.Start.Branches.Count);
            Assert.AreEqual(1, syntax.Start.Branches[0].Count);

            SingleDefinition single1 = (SingleDefinition)syntax.Start.Branches[0][0];
            Assert.AreEqual(1, single1.SyntacticTerms.Count);
            Assert.Null(single1.SyntacticTerms[0].Exception);

            Assert.AreEqual(1, single1.SyntacticTerms[0].Factor.NumberOfRepetitions);

            OptionalSequence sequence = (OptionalSequence)single1.SyntacticTerms[0].Factor.SyntacticPrimary;
            SingleDefinition single2 = (SingleDefinition)sequence.Branches[0][0];
            Assert.AreEqual(1, single2.SyntacticTerms.Count);
            Assert.Null(single2.SyntacticTerms[0].Exception);

            Assert.AreEqual(1, single2.SyntacticTerms[0].Factor.NumberOfRepetitions);

            Assert.AreEqual("Abc", ((TerminalString)single2.SyntacticTerms[0].Factor.SyntacticPrimary).Value.Text);

            SingleDefinition single3 = (SingleDefinition)sequence.Branches[1][0];
            Assert.AreEqual(1, single3.SyntacticTerms.Count);
            Assert.Null(single3.SyntacticTerms[0].Exception);

            Assert.AreEqual(1, single3.SyntacticTerms[0].Factor.NumberOfRepetitions);

            Assert.AreEqual("Def", ((TerminalString)single3.SyntacticTerms[0].Factor.SyntacticPrimary).Value.Text);
        }

        /// <summary>
        /// Test that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse a rule that repeated sequence.
        /// </summary>
        [Test]
        public void Parse_RepeatedSequence_Success()
        {
            TokenBuilder builder = new TokenBuilder();
            var tokens = builder.MetaIdentifier("Rule")
                                .Symbol("=")
                                .Symbol("{")
                                .TerminalString("Abc")
                                .Symbol("}")
                                .Symbol(";")
                                .Build();

            Parser parser = new Parser();
            Syntax syntax = parser.Parse(LexicalAnalyzer(tokens));

            RuleReferenceResolver resolver = new RuleReferenceResolver();
            resolver.Resolve(syntax);

            Assert.AreEqual("Rule", syntax.Start.MetaIdentifier.Text);

            Assert.AreEqual(1, syntax.Start.Branches.Count);
            Assert.AreEqual(1, syntax.Start.Branches[0].Count);

            SingleDefinition single1 = (SingleDefinition)syntax.Start.Branches[0][0];
            Assert.AreEqual(1, single1.SyntacticTerms.Count);
            Assert.Null(single1.SyntacticTerms[0].Exception);

            Assert.AreEqual(1, single1.SyntacticTerms[0].Factor.NumberOfRepetitions);

            RepeatedSequence sequence = (RepeatedSequence)single1.SyntacticTerms[0].Factor.SyntacticPrimary;
            SingleDefinition single2 = (SingleDefinition)sequence.Branches[0][0];
            Assert.AreEqual(1, single2.SyntacticTerms.Count);
            Assert.Null(single2.SyntacticTerms[0].Exception);

            Assert.AreEqual(1, single2.SyntacticTerms[0].Factor.NumberOfRepetitions);

            Assert.AreEqual("Abc", ((TerminalString)single2.SyntacticTerms[0].Factor.SyntacticPrimary).Value.Text);
        }

        /// <summary>
        /// Test that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can parse a rule that repeated sequence with two branches.
        /// </summary>
        [Test]
        public void Parse_RepeatedSequenceWith2Branches_Success()
        {
            TokenBuilder builder = new TokenBuilder();
            var tokens = builder.MetaIdentifier("Rule")
                                .Symbol("=")
                                .Symbol("{")
                                .TerminalString("Abc")
                                .Symbol("|")
                                .TerminalString("Def")
                                .Symbol("}")
                                .Symbol(";")
                                .Build();

            Parser parser = new Parser();
            Syntax syntax = parser.Parse(LexicalAnalyzer(tokens));

            RuleReferenceResolver resolver = new RuleReferenceResolver();
            resolver.Resolve(syntax);

            Assert.AreEqual("Rule", syntax.Start.MetaIdentifier.Text);

            Assert.AreEqual(1, syntax.Start.Branches.Count);
            Assert.AreEqual(1, syntax.Start.Branches[0].Count);

            SingleDefinition single1 = (SingleDefinition)syntax.Start.Branches[0][0];
            Assert.AreEqual(1, single1.SyntacticTerms.Count);
            Assert.Null(single1.SyntacticTerms[0].Exception);

            Assert.AreEqual(1, single1.SyntacticTerms[0].Factor.NumberOfRepetitions);

            RepeatedSequence sequence = (RepeatedSequence)single1.SyntacticTerms[0].Factor.SyntacticPrimary;
            SingleDefinition single2 = (SingleDefinition)sequence.Branches[0][0];
            Assert.AreEqual(1, single2.SyntacticTerms.Count);
            Assert.Null(single2.SyntacticTerms[0].Exception);

            Assert.AreEqual(1, single2.SyntacticTerms[0].Factor.NumberOfRepetitions);

            Assert.AreEqual("Abc", ((TerminalString)single2.SyntacticTerms[0].Factor.SyntacticPrimary).Value.Text);

            SingleDefinition single3 = (SingleDefinition)sequence.Branches[1][0];
            Assert.AreEqual(1, single3.SyntacticTerms.Count);
            Assert.Null(single3.SyntacticTerms[0].Exception);

            Assert.AreEqual(1, single3.SyntacticTerms[0].Factor.NumberOfRepetitions);

            Assert.AreEqual("Def", ((TerminalString)single3.SyntacticTerms[0].Factor.SyntacticPrimary).Value.Text);
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can detect direct rule references in an exception.
        /// </summary>
        [Test]
        public void Parse_RuleWithExceptionContainingRuleReference_ThrowsException()
        {
            TokenBuilder builder = new TokenBuilder();
            var tokens = builder.MetaIdentifier("syntax")
                                .Symbol("=")
                                .MetaIdentifier("rule1")
                                .Symbol("-")
                                .MetaIdentifier("rule2")
                                .Symbol(";")
                                .Build();

            Parser parser = new Parser();
            Assert.That(
                () => parser.Parse(LexicalAnalyzer(tokens)),
                Throws.Exception.TypeOf<ParsingException>());
        }

        /// <summary>
        /// Tests that <see cref="Parser.Parse(LexicalAnalyzer{TokenType})"/>
        /// can detect indirect rule references in an exception.
        /// </summary>
        [Test]
        public void Parse_RuleWithExceptionContainingNestedRuleReference_ThrowsException()
        {
            TokenBuilder builder = new TokenBuilder();
            var tokens = builder.MetaIdentifier("syntax")
                                .Symbol("=")
                                .MetaIdentifier("rule1")
                                .Symbol("-")
                                .Symbol("(")
                                .TerminalString("abc")
                                .Symbol("|")
                                .Symbol("(")
                                .MetaIdentifier("rule2")
                                .Symbol(")")
                                .Symbol(")")
                                .Symbol(";")
                                .Build();

            string message = "1:1: Rule 'rule2' referenced in exception.";

            Parser parser = new Parser();
            Assert.That(
                () => parser.Parse(LexicalAnalyzer(tokens)),
                Throws.Exception.TypeOf<ParsingException>().With.Message.EqualTo(message));
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