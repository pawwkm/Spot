using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// generates all the characters in a unicode character class.
        /// </summary>
        [Test]
        public void Generate_SequenceOfClasses_Success()
        {
            string[] expected = 
            { 
                "\u0020", "\u00A0", "\u1680", "\u2000", "\u2001",
                "\u2002", "\u2003", "\u2004", "\u2005", "\u2006",
                "\u2007", "\u2008", "\u2009", "\u200A", "\u202F",
                "\u205F", "\u3000"
            };

            var generator = new UnicodeSequenceGenerator();
            var generated = generator.Generate("Unicode class zs");

            CollectionAssert.AreEquivalent(expected, generated);
        }

        /// <summary>
        /// Tests that <see cref="UnicodeSequenceGenerator.Generate(string)"/>
        /// generates all the characters in the defined sequence.
        /// </summary>
        [Test]
        public void Generate_SequenceOfCcharacters_Success()
        {
            string[] expected = 
            {
                "A", "B"
            };

            var generator = new UnicodeSequenceGenerator();
            var generated = generator.Generate(@"Unicode characters \u0041 and \u0042");

            CollectionAssert.AreEquivalent(expected, generated);
        }
    }
}