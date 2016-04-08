using NSubstitute;
using NUnit.Framework;
using Pote;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Spot.Ebnf
{
    /// <summary>
    /// Provides tests for the <see cref="FuzzyTestGenerator"/> class.
    /// </summary>
    [TestFixture]
    public class FuzzyTestGeneratorTests
    {
        /// <summary>
        /// Tests that <see cref="FuzzyTestGenerator.Generate(Syntax)"/>
        /// can handle an empty syntax
        /// </summary>
        [Test]
        public void Generate_EmptySyntax_Success()
        {
            string text = "syntax = ;";

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            FuzzyTestGenerator generator = new FuzzyTestGenerator();

            ICollection<string> tests = generator.Generate(syntax);

            Assert.AreEqual(0, tests.Count);
        }

        /// <summary>
        /// Tests that <see cref="FuzzyTestGenerator.Generate(Syntax)"/>
        /// can handle a rule consisting of a single terminal.
        /// </summary>
        [Test]
        public void Generate_SingleTerminal_Success()
        {
            string text = "syntax = 'abc' ;";

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            FuzzyTestGenerator generator = new FuzzyTestGenerator();

            ICollection<string> tests = generator.Generate(syntax);

            Assert.AreEqual(1, tests.Count);
            Assert.True(tests.Contains("abc"));
        }

        /// <summary>
        /// Tests that <see cref="FuzzyTestGenerator.Generate(Syntax)"/>
        /// can handle a rule consisting of a single terminal x times.
        /// </summary>
        [Test]
        public void Generate_RepeatedSingleTerminal_Success()
        {
            string text = "syntax = 3 * 'abc' ;";

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            FuzzyTestGenerator generator = new FuzzyTestGenerator();

            ICollection<string> tests = generator.Generate(syntax);

            Assert.AreEqual(1, tests.Count);
            Assert.True(tests.Contains("abcabcabc"));
        }

        /// <summary>
        /// Tests that <see cref="FuzzyTestGenerator.Generate(Syntax)"/>
        /// can handle nested rules consisting of a single terminal.
        /// </summary>
        [Test]
        public void Generate_NestedRuleWithSingleTerminal_Success()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("syntax = rule ;");
            text.AppendLine("rule = 'abc' ;");

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            FuzzyTestGenerator generator = new FuzzyTestGenerator();

            ICollection<string> tests = generator.Generate(syntax);

            Assert.AreEqual(1, tests.Count);
            Assert.True(tests.Contains("abc"));
        }

        /// <summary>
        /// Tests that <see cref="FuzzyTestGenerator.Generate(Syntax)"/>
        /// can handle nested rules consisting of a single terminal x times.
        /// </summary>
        [Test]
        public void Generate_NestedRuleWithRepeatedSingleTerminal_Success()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("syntax = 3 * rule ;");
            text.AppendLine("rule = 'abc' ;");

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            FuzzyTestGenerator generator = new FuzzyTestGenerator();

            ICollection<string> tests = generator.Generate(syntax);

            Assert.AreEqual(1, tests.Count);
            Assert.True(tests.Contains("abcabcabc"));
        }

        /// <summary>
        /// Tests that <see cref="FuzzyTestGenerator.Generate(Syntax)"/>
        /// can handle branches.
        /// </summary>
        [Test]
        public void Generate_RuleWithBranches_Success()
        {
            string text = "syntax = 'abc' | 'def' ;";
            string[] expected = { "abc", "def" };

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            FuzzyTestGenerator generator = new FuzzyTestGenerator();

            ICollection<string> tests = generator.Generate(syntax);

            CollectionAssert.AreEquivalent(expected, tests);
        }

        /// <summary>
        /// Tests that <see cref="FuzzyTestGenerator.Generate(Syntax)"/>
        /// can handle rules consisting of groups with terminals.
        /// </summary>
        [Test]
        public void Generate_RuleConsistingOfGroupOfTerminals_Success()
        {
            string text = "syntax = 'a', ( 'b' | 'c' ), 'd' ;";
            string[] expected = { "abd", "acd" };

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            FuzzyTestGenerator generator = new FuzzyTestGenerator();

            ICollection<string> tests = generator.Generate(syntax);

            CollectionAssert.AreEquivalent(expected, tests);
        }

        /// <summary>
        /// Tests that <see cref="FuzzyTestGenerator.Generate(Syntax)"/>
        /// can handle nested rules consisting of groups with terminals.
        /// </summary>
        [Test]
        public void Generate_NestedRuleConsistingOfGroupOfTerminals_Success()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("syntax = 'a', rule, 'd' ;");
            text.AppendLine("rule = ( 'b' | 'c' ) ;");

            string[] expected = { "abd", "acd" };

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            FuzzyTestGenerator generator = new FuzzyTestGenerator();

            ICollection<string> tests = generator.Generate(syntax);

            CollectionAssert.AreEquivalent(expected, tests);
        }

        /// <summary>
        /// Tests that <see cref="FuzzyTestGenerator.Generate(Syntax)"/>
        /// can handle rules consisting of optional terminals.
        /// </summary>
        [Test]
        public void Generate_RuleConsistingOfOptionalOfTerminals_Success()
        {
            string text = "syntax = 'a', [ 'b' | 'c' ], 'd' ;";
            string[] expected = { "abd", "acd", "ad" };

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            FuzzyTestGenerator generator = new FuzzyTestGenerator();

            ICollection<string> tests = generator.Generate(syntax);

            CollectionAssert.AreEquivalent(expected, tests);
        }

        /// <summary>
        /// Tests that <see cref="FuzzyTestGenerator.Generate(Syntax)"/>
        /// can handle nested rules consisting of optional terminals.
        /// </summary>
        [Test]
        public void Generate_NestedRuleConsistingOfOptionalOfTerminals_Success()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("syntax = 'a', rule, 'd' ;");
            text.AppendLine("rule = [ 'b' | 'c' ] ;");

            string[] expected = { "abd", "acd", "ad" };

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            FuzzyTestGenerator generator = new FuzzyTestGenerator();

            ICollection<string> tests = generator.Generate(syntax);

            CollectionAssert.AreEquivalent(expected, tests);
        }

        /// <summary>
        /// Tests that <see cref="FuzzyTestGenerator.Generate(Syntax)"/>
        /// can handle rules consisting of a repeated sequence terminals.
        /// </summary>
        [Test]
        public void Generate_RuleConsistingOfARepeatedSequenceOfTerminals_Success()
        {
            string text = "syntax = 'a', { 'b' | 'c' }, 'd' ;";
            string[] expected = 
            { 
                "ad",
                "abd",
                "acd",
                "abbd",
                "abcd",
                "acbd",
                "accd"
            };

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            FuzzyTestGenerator generator = new FuzzyTestGenerator();

            ICollection<string> tests = generator.Generate(syntax);

            CollectionAssert.AreEquivalent(expected, tests);
        }

        /// <summary>
        /// Tests that <see cref="FuzzyTestGenerator.Generate(Syntax)"/>
        /// can handle nested rules consisting of a repeated sequence terminals.
        /// </summary>
        [Test]
        public void Generate_NestedRuleConsistingOfRepeatedSequenceOfTerminals_Success()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("syntax = 'a', rule, 'd' ;");
            text.AppendLine("rule = { 'b' | 'c' } ;");

            string[] expected = 
            { 
                "ad",
                "abd",
                "acd",
                "abbd",
                "abcd",
                "acbd",
                "accd"
            };

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            FuzzyTestGenerator generator = new FuzzyTestGenerator();

            ICollection<string> tests = generator.Generate(syntax);

            CollectionAssert.AreEquivalent(expected, tests);
        }

        /// <summary>
        /// Tests that <see cref="FuzzyTestGenerator.Generate(Syntax)"/>
        /// can handle nested rules with exceptions.
        /// </summary>
        [Test]
        public void Generate_NestedRuleWithExceptions_Success()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("syntax = rule - 'abc' ;");
            text.AppendLine("rule = ( 'abc' | 'def' | 'ghi' ) - 'ghi', 'jkl' ;");

            string[] expected = { "abcjkl", "defjkl" };

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            FuzzyTestGenerator generator = new FuzzyTestGenerator();

            var tests = generator.Generate(syntax);

            CollectionAssert.AreEquivalent(expected, tests);
        }

        /// <summary>
        /// Tests that <see cref="FuzzyTestGenerator.Generate(Syntax)"/>
        /// can handle special sequences.
        /// </summary>
        [Test]
        public void Generate_SepcialSequence_Success()
        {
            string text = "syntax = ? digit ? ;";
            string[] expected = 
            {
                "0", "1", "2", "3", "4", 
                "5", "6", "7", "8", "9"
            };

            ISpecialSequenceGenerator sequence = Substitute.For<ISpecialSequenceGenerator>();
            sequence.IsValid(" digit ").Returns(true);
            sequence.Generate(" digit ").Returns(new Collection<string>(expected));

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            FuzzyTestGenerator generator = new FuzzyTestGenerator();
            generator.SpecialSequenceGenerator.Add(sequence);

            ICollection<string> tests = generator.Generate(syntax);

            CollectionAssert.AreEquivalent(expected, tests);
        }
    }
}