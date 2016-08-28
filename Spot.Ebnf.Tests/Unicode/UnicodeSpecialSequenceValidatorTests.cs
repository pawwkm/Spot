using NSubstitute;
using NUnit.Framework;
using Pote.Text;
using System.Linq;

namespace Spot.Ebnf.Unicode
{
    /// <summary>
    /// Provides tests for the <see cref="UnicodeSpecialSequenceValidator"/> class.
    /// </summary>
    [TestFixture]
    public class UnicodeSpecialSequenceValidatorTests
    {
        /// <summary>
        /// Calls the <see cref="UnicodeSequence.ClearCache()"/> method before 
        /// any test is run.
        /// </summary>
        [TestFixtureSetUp]
        public void ClearValidatorCache()
        {
            UnicodeSequence.ClearCache();
        }

        /// <summary>
        /// Tests that <see cref="UnicodeSpecialSequenceValidator.IsValid(string)"/>
        /// validates all possible variations of valid sequences.
        /// </summary>
        [Test]
        public void IsValid_ValidSequences_ReturnsTrue()
        {
            string[] sequences = 
            { 
                @"Unicode class zs",
                @" Unicode class zs ",
                
                @"Unicode classes zs and cc",
                @" Unicode classes zs and cc ",

                @"Unicode classes zs,cn and cc",
                @" Unicode classes zs, cn and cc ",

                @"Unicode character \u0041",
                @" Unicode character \u0041 ",

                @"Unicode characters \u0041 and \u0042",
                @" Unicode characters \u0041 and \u0042 ",

                @"Unicode class nd and character \u0041",
                @" Unicode class nd and character \u0041 ",

                @"Unicode classes nd and ll and characters \u0041 and \u0042",
                @" Unicode classes nd and ll and characters \u0041 and \u0042 ",

                @"Unicode class zs except character \u0020",
                @" Unicode class zs except character \u0020 ",

                @"Unicode classes zs and cc except characters \u0020 and \u0021",
                @" Unicode classes zs and cc except characters \u0020 and \u0021 ",

                @"Unicode classes zs,cn and cc except characters \u0020 and \u0021",
                @" Unicode classes zs, cn and cc except characters \u0020 and \u0021 ",
            };

            var validator = new UnicodeSpecialSequenceValidator();
            foreach (string sequence in sequences)
                Assert.True(validator.IsValid(sequence), "Could not validate '" + sequence + "'");
        }

        /// <summary>
        /// Tests that <see cref="UnicodeSpecialSequenceValidator.IsValid(string)"/>
        /// invalidates all possible variations of invalid sequences.
        /// </summary>
        [Test]
        public void IsValid_InvalidSequences_ReturnsFalse()
        {
            string[] sequences = 
            { 
                @"",
                @" ",

                @"UNICODE classes zs",
                @"Unicode CLASSES zs",

                @"UNICODE characters \u0041",
                @"Unicode CHARACTERS \u0041",

                @"Unicode classes zs, \u0041",
                @"Unicode characters \u0041, zs",

                @"Unicode classes \u0041",
                @"Unicode characters zs",

                @"Unicode \u0041",
                @"Unicode zs",

                @"\u0041",
                @"[zs]",

                @"Unicode classes zs cc",
                @"Unicode classes zs cc",
                @"Unicode characters \u0041\u0042",
                @"Unicode characters \u0041 \u0042",
            };

            var validator = new UnicodeSpecialSequenceValidator();
            foreach (string sequence in sequences)
                Assert.False(validator.IsValid(sequence), "This must not be valid '" + sequence + "'");
        }

        /// <summary>
        /// Tests that <see cref="UnicodeSpecialSequenceValidator.Consume(string, string, InputPosition)"/>
        /// can consume a character that is defined in the sequence.
        /// </summary>
        [Test]
        public void Consume_InputContainedInClass_ReturnsTrue()
        {
            var validator = new UnicodeSpecialSequenceValidator();
            InputPosition position = new InputPosition();

            Assert.True(validator.Consume("a", "Unicode class ll", position));
            Assert.AreEqual(1, position.Index);
        }

        /// <summary>
        /// Tests that <see cref="UnicodeSpecialSequenceValidator.Consume(string, string, InputPosition)"/>
        /// can not consume a character that is not defined in the sequence.
        /// </summary>
        [Test]
        public void Consume_InputNotContainedInClass_ReturnsFalse()
        {
            var validator = new UnicodeSpecialSequenceValidator();
            InputPosition position = new InputPosition();

            Assert.False(validator.Consume("A", "Unicode class ll", position));
            Assert.AreEqual(0, position.Index);
        }

        /// <summary>
        /// Tests that <see cref="UnicodeSpecialSequenceValidator.Consume(string, string, InputPosition)"/>
        /// can consume a character that is defined in the sequence.
        /// </summary>
        [Test]
        public void Consume_CharacterContainedInSequence_ReturnsTrue()
        {
            var validator = new UnicodeSpecialSequenceValidator();
            InputPosition position = new InputPosition();

            Assert.True(validator.Consume("a", @"Unicode character \u0061", position));
            Assert.AreEqual(1, position.Index);
        }

        /// <summary>
        /// Tests that <see cref="UnicodeSpecialSequenceValidator.Consume(string, string, InputPosition)"/>
        /// can not consume a character that is not defined in the sequence.
        /// </summary>
        [Test]
        public void Consume_CharacterNotContainedInSequence_ReturnsFalse()
        {
            var validator = new UnicodeSpecialSequenceValidator();
            InputPosition position = new InputPosition();

            Assert.False(validator.Consume("A", @"Unicode character \u0061", position));
            Assert.AreEqual(0, position.Index);
        }

        /// <summary>
        /// Tests that <see cref="UnicodeSpecialSequenceValidator.Consume(string, string, InputPosition)"/>
        /// can not consume a character that is mentioned in the except clause.
        /// </summary>
        [Test]
        public void Consume_ConsumeClassExceptCharacter_Success()
        {
            var validator = new UnicodeSpecialSequenceValidator();
            var position = new InputPosition();

            Assert.False(validator.Consume("\u0020", @"Unicode class zs except character \u0020", position));
            Assert.AreEqual(0, position.Index);

            Assert.True(validator.Consume("\u00A0", @"Unicode class zs except character \u0020", position));
            Assert.AreEqual(1, position.Index);
        }
    }
}