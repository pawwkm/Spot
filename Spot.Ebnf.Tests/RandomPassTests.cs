using NSubstitute;
using NUnit.Framework;
using Pote;

namespace Spot.Ebnf
{
    /// <summary>
    /// Provides tests for the <see cref="RandomPass"/> class.
    /// </summary>
    [TestFixture]
    public class RandomPassTests
    {
        /// <summary>
        /// Tests that <see cref="RandomPass.Visit(Syntax)"/> generates 
        /// random sentence from a valid syntax.
        /// </summary>
        /// <param name="text">The syntax to randomize a sentence from.</param>
        /// <param name="expected">The possible end results.</param>
        [Test]
        [TestCase("syntax = ;", "")]
        [TestCase("syntax = 'abc' ;", "abc")]
        [TestCase("syntax = 'abc' | 'def' ;", "abc", "def")]
        [TestCase("syntax = 'a', ( '0' | '1' ) ;", "a0", "a1")]
        [TestCase("syntax = 'a', ( '0' | '1' ), 'b' ;", "a0b", "a1b")]
        [TestCase("syntax = 'a', [ '0' ] ;", "a", "a0")]
        [TestCase("syntax = ( 'a' | 'b' ) - 'a' ;", "b")]
        public void Visit_ValidSyntax_SentrenceGenerated(string text, params string[] expected)
        {
            var reader = new SyntaxReader();
            var syntax = reader.Read(text.ToStream());
            var random = new RandomPass();

            random.Visit(syntax);
            if (!random.Sentence.IsOneOf(expected))
                Assert.Fail("None of the expected sentences were generated.");
        }

        /// <summary>
        /// Tests that <see cref="RandomPass.Visit(Syntax)"/> 
        /// can handle special sequences.
        /// </summary>
        [Test]
        public void Visit_SepcialSequence_SentrenceGenerated()
        {
            var text = "syntax = ? digit ? ;";
            var expected = new string[]
            {
                "0", "1", "2", "3", "4",
                "5", "6", "7", "8", "9"
            };

            var sequence = Substitute.For<ISpecialSequenceGenerator>();
            sequence.IsValid(" digit ").Returns(true);
            sequence.Generate(" digit ").Returns(expected);

            var reader = new SyntaxReader();
            var syntax = reader.Read(text.ToStream());
            var random = new RandomPass();

            random.SpecialSequenceGenerators.Add(sequence);
            random.Visit(syntax);

            if (!random.Sentence.IsOneOf(expected))
                Assert.Fail("None of the expected sentences were generated.");
        }
    }
}