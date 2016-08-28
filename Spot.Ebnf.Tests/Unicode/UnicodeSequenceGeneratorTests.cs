using NUnit.Framework;

namespace Spot.Ebnf.Unicode
{
    /// <summary>
    /// Provides tests for the <see cref="UnicodeSequenceGenerator"/> class.
    /// </summary>
    [TestFixture]
    public class UnicodeSequenceGeneratorTests
    {
        /// <summary>
        /// Tests that <see cref="UnicodeSequenceGenerator.Generate(string)"/>
        /// generates all the characters in a unicode special sequence.
        /// </summary>
        /// <param name="sequence">The sequence to generate characters from.</param>
        /// <param name="expected">The expected result.</param>
        [Test]
        [TestCase(@"Unicode character \u0041..\u0043", "A", "B", "C")]
        [TestCase(@"Unicode characters \u0041 and \u0042", "A", "B")]
        [TestCase("Unicode class zs", "\u0020", "\u00A0", "\u1680", "\u2000", "\u2001", "\u2002", "\u2003", "\u2004", "\u2005", "\u2006", "\u2007", "\u2008", "\u2009", "\u200A", "\u202F", "\u205F", "\u3000")]
        public void Generate_ValidUnicodeSequence_CharactersGenerated(string sequence, params string[] expected)
        {
            var generator = new UnicodeSequenceGenerator();
            var actual = generator.Generate(sequence);

            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}