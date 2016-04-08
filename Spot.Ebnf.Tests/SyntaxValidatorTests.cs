using NSubstitute;
using NUnit.Framework;
using Pote;
using Pote.Text;
using System.IO;
using System.Linq;
using System.Text;

namespace Spot.Ebnf
{
    /// <summary>
    /// Provides tests for the <see cref="SyntaxValidator"/> class.
    /// </summary>
    public class SyntaxValidatorTests
    {
        /// <summary>
        /// Tests that <see cref="SyntaxValidator.Validate(string)"/> is able to 
        /// validate an empty syntax.
        /// </summary>
        public void Validate_EmptySyntax_Success()
        {
            string text = "syntax = ;";

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            SyntaxValidator validator = new SyntaxValidator(syntax);

            SyntaxValidationResult a = validator.Validate("");
            Assert.True(a.IsSyntaxValid, a.Message);
        }

        /// <summary>
        /// Tests that <see cref="SyntaxValidator.Validate(string)"/> is able to 
        /// validate a syntax complex that makes use of all kinds of sequences.
        /// </summary>
        [Test]
        public void Validate_ComplexSyntax_Success()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("syntax = ( word | integer ) ;");
            text.AppendLine("word = letter, { letter } ;");
            text.AppendLine("integer = digit, { digit } ;");
            text.AppendLine("letter = 'A' | 'B' | 'C' ;");
            text.AppendLine("digit = '0' | '1' | '2' ;");

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            SyntaxValidator validator = new SyntaxValidator(syntax);

            SyntaxValidationResult a = validator.Validate("ABC");
            Assert.True(a.IsSyntaxValid, a.Message);
            Assert.AreEqual(5, a.RuleTrace.Length);

            Assert.AreEqual(a.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(new InputPosition(), a.RuleTrace[0].EntryPoint);
            Assert.AreEqual(new InputPosition(1, 4, 3), a.RuleTrace[0].ExitPoint);

            Assert.AreEqual(a.RuleTrace[1].Rule, "word");
            Assert.AreEqual(new InputPosition(), a.RuleTrace[1].EntryPoint);
            Assert.AreEqual(new InputPosition(1, 4, 3), a.RuleTrace[1].ExitPoint);

            Assert.AreEqual(a.RuleTrace[2].Rule, "letter");
            Assert.AreEqual(new InputPosition(), a.RuleTrace[2].EntryPoint);
            Assert.AreEqual(new InputPosition(1, 2, 1), a.RuleTrace[2].ExitPoint);

            Assert.AreEqual(a.RuleTrace[3].Rule, "letter");
            Assert.AreEqual(new InputPosition(1, 2, 1), a.RuleTrace[3].EntryPoint);
            Assert.AreEqual(new InputPosition(1, 3, 2), a.RuleTrace[3].ExitPoint);

            Assert.AreEqual(a.RuleTrace[4].Rule, "letter");
            Assert.AreEqual(new InputPosition(1, 3, 2), a.RuleTrace[4].EntryPoint);
            Assert.AreEqual(new InputPosition(1, 4, 3), a.RuleTrace[4].ExitPoint);

            SyntaxValidationResult b = validator.Validate("012");
            Assert.True(b.IsSyntaxValid, b.Message);
            Assert.AreEqual(5, b.RuleTrace.Length);

            Assert.AreEqual(b.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(new InputPosition(), b.RuleTrace[0].EntryPoint);
            Assert.AreEqual(new InputPosition(1, 4, 3), b.RuleTrace[0].ExitPoint);

            Assert.AreEqual(b.RuleTrace[1].Rule, "integer");
            Assert.AreEqual(new InputPosition(), b.RuleTrace[1].EntryPoint);
            Assert.AreEqual(new InputPosition(1, 4, 3), b.RuleTrace[1].ExitPoint);

            Assert.AreEqual(b.RuleTrace[2].Rule, "digit");
            Assert.AreEqual(new InputPosition(), b.RuleTrace[2].EntryPoint);
            Assert.AreEqual(new InputPosition(1, 2, 1), b.RuleTrace[2].ExitPoint);

            Assert.AreEqual(b.RuleTrace[3].Rule, "digit");
            Assert.AreEqual(new InputPosition(1, 2, 1), b.RuleTrace[3].EntryPoint);
            Assert.AreEqual(new InputPosition(1, 3, 2), b.RuleTrace[3].ExitPoint);

            Assert.AreEqual(b.RuleTrace[4].Rule, "digit");
            Assert.AreEqual(new InputPosition(1, 3, 2), b.RuleTrace[4].EntryPoint);
            Assert.AreEqual(new InputPosition(1, 4, 3), b.RuleTrace[4].ExitPoint);
        }

        /// <summary>
        /// Tests that <see cref="SyntaxValidator.Validate(string)"/> is able to 
        /// validate a syntax consisting of optional root terminals.
        /// </summary>
        [Test]
        public void Validate_SyntaxWithOptionalRootBranches_Success()
        {
            InputPosition errorPoint = new InputPosition();
            InputPosition entryPoint = new InputPosition();
            InputPosition exitPoint = new InputPosition();
            exitPoint.Column = 4;
            exitPoint.Index = 3;

            string text = "syntax = 'abc' | 'def' | 'ghi' ;";
            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            SyntaxValidator validator = new SyntaxValidator(syntax);

            SyntaxValidationResult a = validator.Validate("abc");
            Assert.True(a.IsSyntaxValid, a.Message);
            Assert.AreEqual(1, a.RuleTrace.Length);
            Assert.AreEqual(a.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(entryPoint, a.RuleTrace[0].EntryPoint);
            Assert.AreEqual(exitPoint, a.RuleTrace[0].ExitPoint);

            SyntaxValidationResult b = validator.Validate("def");
            Assert.True(b.IsSyntaxValid, b.Message);
            Assert.AreEqual(b.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(entryPoint, b.RuleTrace[0].EntryPoint);
            Assert.AreEqual(exitPoint, b.RuleTrace[0].ExitPoint);

            SyntaxValidationResult c = validator.Validate("ghi");
            Assert.True(c.IsSyntaxValid, c.Message);
            Assert.AreEqual(c.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(entryPoint, c.RuleTrace[0].EntryPoint);
            Assert.AreEqual(exitPoint, c.RuleTrace[0].ExitPoint);

            SyntaxValidationResult d = validator.Validate("jkl");
            Assert.False(d.IsSyntaxValid, d.Message);
            Assert.AreEqual(d.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(entryPoint, d.RuleTrace[0].EntryPoint);
            Assert.AreEqual(errorPoint, d.RuleTrace[0].ErrorPoint);
        }

        /// <summary>
        /// Tests that <see cref="SyntaxValidator.Validate(string)"/> is able to 
        /// validate a syntax consisting of a single terminal.
        /// </summary>
        [Test]
        public void Validate_SyntaxConsistsOf1Terminal_Success()
        {
            InputPosition entryPoint = new InputPosition();
            InputPosition exitPoint = new InputPosition();
            exitPoint.Column = 4;
            exitPoint.Index = 3;

            string text = "syntax = 'abc' ;";

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            SyntaxValidator validator = new SyntaxValidator(syntax);

            SyntaxValidationResult result = validator.Validate("abc");
            Assert.True(result.IsSyntaxValid);
            Assert.AreEqual(1, result.RuleTrace.Length);
            Assert.AreEqual(result.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(entryPoint, result.RuleTrace[0].EntryPoint);
            Assert.AreEqual(exitPoint, result.RuleTrace[0].ExitPoint);
        }

        /// <summary>
        /// Tests that <see cref="SyntaxValidator.Validate(string)"/> is able to 
        /// validate a syntax consisting of concatenated terminals.
        /// </summary>
        [Test]
        public void Validate_SyntaxConsistsOfConcantatedTerminals_Success()
        {
            InputPosition entryPoint = new InputPosition();
            InputPosition exitPoint = new InputPosition();
            exitPoint.Column = 8;
            exitPoint.Index = 7;

            string text = "syntax = 'abc', ' ', 'def' ;";

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            SyntaxValidator validator = new SyntaxValidator(syntax);

            SyntaxValidationResult result = validator.Validate("abc def");
            Assert.True(result.IsSyntaxValid);
            Assert.AreEqual(1, result.RuleTrace.Length);
            Assert.AreEqual(result.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(entryPoint, result.RuleTrace[0].EntryPoint);
            Assert.AreEqual(exitPoint, result.RuleTrace[0].ExitPoint);
        }

        /// <summary>
        /// Tests that <see cref="SyntaxValidator.Validate(string)"/> is able to 
        /// validate a syntax consisting of grouped sequence of terminals.
        /// </summary>
        [Test]
        public void Validate_SyntaxConsistsOfAGroupedSequenceOfTerminals_Success()
        {
            InputPosition errorPoint = new InputPosition();
            InputPosition entryPoint = new InputPosition();
            InputPosition exitPoint = new InputPosition();
            exitPoint.Column = 4;
            exitPoint.Index = 3;

            string text = "syntax = ( 'abc' | 'def' ) ;";

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            SyntaxValidator validator = new SyntaxValidator(syntax);

            SyntaxValidationResult a = validator.Validate("abc");
            Assert.True(a.IsSyntaxValid, a.Message);
            Assert.AreEqual(1, a.RuleTrace.Length);
            Assert.AreEqual(a.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(entryPoint, a.RuleTrace[0].EntryPoint);
            Assert.AreEqual(exitPoint, a.RuleTrace[0].ExitPoint);

            SyntaxValidationResult b = validator.Validate("def");
            Assert.True(b.IsSyntaxValid, b.Message);
            Assert.AreEqual(1, b.RuleTrace.Length);
            Assert.AreEqual(b.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(entryPoint, b.RuleTrace[0].EntryPoint);
            Assert.AreEqual(exitPoint, b.RuleTrace[0].ExitPoint);

            SyntaxValidationResult c = validator.Validate("ghi");
            Assert.False(c.IsSyntaxValid, c.Message);
            Assert.AreEqual(1, c.RuleTrace.Length);
            Assert.AreEqual(c.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(entryPoint, c.RuleTrace[0].EntryPoint);
            Assert.AreEqual(errorPoint, c.RuleTrace[0].ErrorPoint);
        }

        /// <summary>
        /// Tests that <see cref="SyntaxValidator.Validate(string)"/> is able to 
        /// validate a syntax consisting of a repeated sequence of terminals.
        /// </summary>
        [Test]
        public void Validate_SyntaxConsistsOfARepeatedSequenceOfTerminals_Success()
        {
            InputPosition entryPoint = new InputPosition();
            InputPosition exitPoint = new InputPosition();
            exitPoint.Column = 2;
            exitPoint.Index = 1;

            string text = "syntax = { 'a' | 'b' } ;";

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            SyntaxValidator validator = new SyntaxValidator(syntax);

            SyntaxValidationResult a = validator.Validate("a");
            Assert.True(a.IsSyntaxValid, a.Message);
            Assert.AreEqual(a.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(entryPoint, a.RuleTrace[0].EntryPoint);
            Assert.AreEqual(exitPoint, a.RuleTrace[0].ExitPoint);

            SyntaxValidationResult b = validator.Validate("b");
            Assert.True(b.IsSyntaxValid, b.Message);
            Assert.AreEqual(b.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(entryPoint, b.RuleTrace[0].EntryPoint);
            Assert.AreEqual(exitPoint, b.RuleTrace[0].ExitPoint);

            exitPoint.Column = 5;
            exitPoint.Index = 4;

            SyntaxValidationResult c = validator.Validate("abba");
            Assert.True(c.IsSyntaxValid, c.Message);
            Assert.AreEqual(c.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(entryPoint, c.RuleTrace[0].EntryPoint);
            Assert.AreEqual(exitPoint, c.RuleTrace[0].ExitPoint);

            SyntaxValidationResult d = validator.Validate("baab");
            Assert.True(d.IsSyntaxValid, d.Message);
            Assert.AreEqual(d.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(entryPoint, d.RuleTrace[0].EntryPoint);
            Assert.AreEqual(exitPoint, d.RuleTrace[0].ExitPoint);
        }

        /// <summary>
        /// Tests that <see cref="SyntaxValidator.Validate(string)"/> is able to 
        /// validate a syntax consisting of an optional sequence of terminals.
        /// </summary>
        [Test]
        public void Validate_SyntaxConsistsOfAnOptionalSequenceOfTerminals_Success()
        {
            InputPosition entryPoint = new InputPosition();
            InputPosition aExitPoint = new InputPosition();
            aExitPoint.Column = 2;
            aExitPoint.Index = 1;

            InputPosition bExitPoint = new InputPosition();
            bExitPoint.Column = 4;
            bExitPoint.Index = 3;

            string text = "syntax = [ 'a', ' ' ], 'b' ;";

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            SyntaxValidator validator = new SyntaxValidator(syntax);

            SyntaxValidationResult a = validator.Validate("b");
            Assert.True(a.IsSyntaxValid, a.Message);
            Assert.AreEqual(1, a.RuleTrace.Length);
            Assert.AreEqual(a.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(entryPoint, a.RuleTrace[0].EntryPoint);
            Assert.AreEqual(aExitPoint, a.RuleTrace[0].ExitPoint);

            SyntaxValidationResult b = validator.Validate("a b");
            Assert.True(b.IsSyntaxValid, b.Message);
            Assert.AreEqual(1, b.RuleTrace.Length);
            Assert.AreEqual(b.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(entryPoint, b.RuleTrace[0].EntryPoint);
            Assert.AreEqual(bExitPoint, b.RuleTrace[0].ExitPoint);
        }

        /// <summary>
        /// Tests that <see cref="SyntaxValidator.Validate(string)"/> is able to 
        /// validate a syntax with repetitions.
        /// </summary>
        [Test]
        public void Validate_SyntaxWithRepetitions_Success()
        {
            InputPosition errorPoint = new InputPosition();
            InputPosition entryPoint = new InputPosition();
            InputPosition exitPoint = new InputPosition();
            exitPoint.Column = 4;
            exitPoint.Index = 3;

            string text = "syntax = 3 * 'a' ;";

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            SyntaxValidator validator = new SyntaxValidator(syntax);

            SyntaxValidationResult a = validator.Validate("aa");
            Assert.False(a.IsSyntaxValid, a.Message);
            Assert.AreEqual(1, a.RuleTrace.Length);
            Assert.AreEqual(a.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(entryPoint, a.RuleTrace[0].EntryPoint);
            Assert.AreEqual(errorPoint, a.RuleTrace[0].ErrorPoint);

            SyntaxValidationResult b = validator.Validate("aaa");
            Assert.True(b.IsSyntaxValid, a.Message);
            Assert.AreEqual(1, b.RuleTrace.Length);
            Assert.AreEqual(b.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(entryPoint, b.RuleTrace[0].EntryPoint);
            Assert.AreEqual(exitPoint, b.RuleTrace[0].ExitPoint);

            SyntaxValidationResult c = validator.Validate("aaaa");
            Assert.False(c.IsSyntaxValid, a.Message);
            Assert.AreEqual(1, c.RuleTrace.Length);
            Assert.AreEqual(c.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(entryPoint, c.RuleTrace[0].EntryPoint);
            Assert.AreEqual(exitPoint, c.RuleTrace[0].ExitPoint);
        }

        /// <summary>
        /// Tests that <see cref="SyntaxValidator.Validate(string)"/> is able to 
        /// validate a an exception.
        /// </summary>
        [Test]
        public void Validate_SyntaxWithException_Success()
        {
            InputPosition errorPoint = new InputPosition();
            InputPosition entryPoint = new InputPosition();
            InputPosition bExitPoint = new InputPosition();
            bExitPoint.Column = 3;
            bExitPoint.Index = 2;

            InputPosition cExitPoint = new InputPosition();
            cExitPoint.Column = 5;
            cExitPoint.Index = 4;

            InputPosition dExitPoint = new InputPosition();
            dExitPoint.Column = 4;
            dExitPoint.Index = 3;

            string text = "syntax = { 'A' | 'B' | 'C' } - 'ABC' ;";

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            SyntaxValidator validator = new SyntaxValidator(syntax);

            SyntaxValidationResult a = validator.Validate("ABC");
            Assert.False(a.IsSyntaxValid, a.Message);
            Assert.AreEqual(1, a.RuleTrace.Length);
            Assert.AreEqual(a.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(entryPoint, a.RuleTrace[0].EntryPoint);
            Assert.AreEqual(errorPoint, a.RuleTrace[0].ErrorPoint);

            SyntaxValidationResult b = validator.Validate("AB");
            Assert.True(b.IsSyntaxValid, b.Message);
            Assert.AreEqual(1, b.RuleTrace.Length);
            Assert.AreEqual(b.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(entryPoint, b.RuleTrace[0].EntryPoint);
            Assert.AreEqual(bExitPoint, b.RuleTrace[0].ExitPoint);

            SyntaxValidationResult c = validator.Validate("ABCA");
            Assert.True(c.IsSyntaxValid, c.Message);
            Assert.AreEqual(1, c.RuleTrace.Length);
            Assert.AreEqual(c.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(entryPoint, c.RuleTrace[0].EntryPoint);
            Assert.AreEqual(cExitPoint, c.RuleTrace[0].ExitPoint);

            SyntaxValidationResult d = validator.Validate("AAA");
            Assert.True(d.IsSyntaxValid, d.Message);
            Assert.AreEqual(1, d.RuleTrace.Length);
            Assert.AreEqual(d.RuleTrace[0].Rule, "syntax");
            Assert.AreEqual(entryPoint, d.RuleTrace[0].EntryPoint);
            Assert.AreEqual(dExitPoint, d.RuleTrace[0].ExitPoint);
        }

        /// <summary>
        /// Tests that <see cref="SyntaxValidator.Validate(string)"/> is able to 
        /// use <see cref="ISpecialSequenceValidator"/>.
        /// </summary>
        [Test]
        public void Validate_SpecialSequenceEncounteredAndHasAppropriateValidator_Success()
        {
            ISpecialSequenceValidator special = Substitute.For<ISpecialSequenceValidator>();
            special.IsValid(" Special ").Returns(true);
            special.Consume(Arg.Any<Stream>(), " Special ", Arg.Any<InputPosition>()).Returns(x => 
            {
                Stream stream = x.Arg<Stream>();
                long start = stream.Position;

                byte[] expected = { 0x61, 0x62, 0x63 }; // abc
                byte[] actual = new byte[3];

                stream.Read(actual, 0, 3);
                if (!actual.SequenceEqual(expected))
                {
                    stream.Position = start;
                    return false;
                }

                InputPosition position = x.Arg<InputPosition>();
                position.Column += 3;
                position.Index += 3;

                return true; 
            });

            string text = "syntax = ? Special ?";

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            SyntaxValidator validator = new SyntaxValidator(syntax);
            validator.SpecialSequenceValidators.Add(special);

            SyntaxValidationResult a = validator.Validate("abc");
            Assert.True(a.IsSyntaxValid, a.Message);

            SyntaxValidationResult b = validator.Validate("def");
            Assert.False(b.IsSyntaxValid, b.Message);
        }

        /// <summary>
        /// Tests that <see cref="SyntaxValidator.Validate(string)"/> sets an error message
        /// if there are multiple possible special sequences.
        /// </summary>
        [Test]
        public void Validate_AmbiguousSpecialSequences_ErrorReported()
        {
            ISpecialSequenceValidator special = Substitute.For<ISpecialSequenceValidator>();
            special.IsValid(Arg.Any<string>()).Returns(true);

            string text = "syntax = ? Special ?";
            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            SyntaxValidator validator = new SyntaxValidator(syntax);
            validator.SpecialSequenceValidators.Add(special);
            validator.SpecialSequenceValidators.Add(special);

            SyntaxValidationResult a = validator.Validate("abc");
            Assert.False(a.IsSyntaxValid);
            Assert.AreEqual("1:10: Ambiguous special sequence.", a.Message);
        }

        /// <summary>
        /// Tests that <see cref="SyntaxValidator.Validate(string, string)"/> start from the
        /// specified rule.
        /// </summary>
        [Test]
        public void Validate_StartingRuleSpecified_Success()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("syntax = 'a', rule ;");
            text.AppendLine("rule = 'b' ;");

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            SyntaxValidator validator = new SyntaxValidator(syntax);

            SyntaxValidationResult a = validator.Validate("b", "rule");
            Assert.True(a.IsSyntaxValid, a.Message);

            SyntaxValidationResult b = validator.Validate("b", "");
            Assert.False(b.IsSyntaxValid, b.Message);
        }

        /// <summary>
        /// Tests that <see cref="SyntaxValidator.Validate(string, string, IncludedRules)"/>
        /// only calls the specified rules and ignores the others.
        /// </summary>
        [Test]
        public void Validate_IncludedRulesSpecified_Success()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("syntax = rule1, rule2 ;");
            text.AppendLine("rule1 = 'a' ;");
            text.AppendLine("rule2 = 'b' ;");

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            IncludedRules included = new IncludedRules("rule2");
            SyntaxValidator validator = new SyntaxValidator(syntax);

            SyntaxValidationResult result = validator.Validate("b", "syntax", included);
            Assert.True(result.IsSyntaxValid, result.Message);
        }

        /// <summary>
        /// Tests that <see cref="SyntaxValidator.Validate(string, string, ExcludedRules)"/>
        /// only calls the rules that are not specified.
        /// </summary>
        [Test]
        public void Validate_ExcludedRulesSpecified_Success()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("syntax = rule1, rule2 ;");
            text.AppendLine("rule1 = 'a' ;");
            text.AppendLine("rule2 = 'b' ;");

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            ExcludedRules excluded = new ExcludedRules("rule1");
            SyntaxValidator validator = new SyntaxValidator(syntax);

            SyntaxValidationResult result = validator.Validate("b", "syntax", excluded);
            Assert.True(result.IsSyntaxValid, result.Message);
        }
    }
}