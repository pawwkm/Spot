using NUnit.Framework;
using Spot.Ebnf.Unicode;
using System.Linq;

namespace Spot.Ebnf
{
    /// <summary>
    /// Tests implementations using grammar based fuzzing.
    /// </summary>
    public class Fuzzer
    {
        /// <summary>
        /// Tests the <see cref="UnicodeSpecialSequenceValidator"/> 
        /// class using fuzzing.
        /// </summary>
        [Test]
        [Explicit]
        public void UnicodeSpecialSequences()
        {
            var validator = new UnicodeSpecialSequenceValidator();
            var reader = new SyntaxReader();
            var syntax = reader.Read("..\\..\\Spot.Ebnf\\Unicode\\Unicode Sequence.ebnf");
            var fuzzer = new FuzzyTestGenerator();
            fuzzer.SpecialSequenceGenerators.Add(new UnicodeSequenceGenerator());

            foreach (string sequence in fuzzer.Generate(syntax).Take(10000000))
            {
                UnicodeSequence.ClearCache();
                Assert.True(validator.IsValid(sequence), "Could not validate '" + sequence + "'");
            }
        }
    }
}