using NUnit.Framework;
using Pote;
using System.Linq;
using System.Text;

namespace Spot.Ebnf
{
    /// <summary>
    /// Provides tests for the <see cref="LeftRecursionChecker"/> class.
    /// </summary>
    [TestFixture]
    public class LeftRecursionCheckerTests
    {
        /// <summary>
        /// Tests that <see cref="LeftRecursionChecker.Check(Syntax)"/> 
        /// is able to detect direct left recursion.
        /// </summary>
        [Test]
        public void Check_DirectLeftRecursion_Success()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("syntax = rule ;");
            text.AppendLine("rule = rule ;");

            LexicalAnalyzer analyzer = new LexicalAnalyzer(text.ToStreamReader());
            Parser parser = new Parser();

            Syntax syntax = parser.Parse(analyzer);
            RuleReferenceResolver resolver = new RuleReferenceResolver();
            resolver.Resolve(syntax);

            LeftRecursionChecker checker = new LeftRecursionChecker();
            Rule[] rules = checker.Check(syntax).ToArray();

            Assert.AreEqual(1, rules.Length);
            Assert.AreEqual("rule", rules[0].MetaIdentifier.Text);
        }

        /// <summary>
        /// Tests that <see cref="LeftRecursionChecker.Check(Syntax)"/> 
        /// is able to detect indirect left recursion.
        /// </summary>
        [Test]
        public void Check_IndirectLeftRecursion_Success()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("syntax = rule1 ;");
            text.AppendLine("rule1 = rule2 ;");
            text.AppendLine("rule2 = rule1 ;");

            LexicalAnalyzer analyzer = new LexicalAnalyzer(text.ToStreamReader());
            Parser parser = new Parser();

            Syntax syntax = parser.Parse(analyzer);
            RuleReferenceResolver resolver = new RuleReferenceResolver();
            resolver.Resolve(syntax);

            LeftRecursionChecker checker = new LeftRecursionChecker();
            Rule[] rules = checker.Check(syntax).ToArray();

            Assert.AreEqual(2, rules.Length);
            Assert.AreEqual("rule1", rules[0].MetaIdentifier.Text);
            Assert.AreEqual("rule2", rules[1].MetaIdentifier.Text);
        }

        /// <summary>
        /// Tests that <see cref="LeftRecursionChecker.Check(Syntax)"/> 
        /// is able to detect indirect left recursion in sequences.
        /// </summary>
        [Test]
        public void Check_IndirectLeftRecursionInSequences_Success()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("syntax = rule1 ;");
            text.AppendLine("rule1 = ( ( rule2 ) ) ;");
            text.AppendLine("rule2 = rule1 ;");

            LexicalAnalyzer analyzer = new LexicalAnalyzer(text.ToStreamReader());
            Parser parser = new Parser();

            Syntax syntax = parser.Parse(analyzer);
            RuleReferenceResolver resolver = new RuleReferenceResolver();
            resolver.Resolve(syntax);

            LeftRecursionChecker checker = new LeftRecursionChecker();
            Rule[] rules = checker.Check(syntax).ToArray();

            Assert.AreEqual(2, rules.Length);
            Assert.AreEqual("rule1", rules[0].MetaIdentifier.Text);
            Assert.AreEqual("rule2", rules[1].MetaIdentifier.Text);
        }

        /// <summary>
        /// Tests that <see cref="LeftRecursionChecker.Check(Syntax)"/> 
        /// is able to detect indirect left recursion in branches.
        /// </summary>
        [Test]
        public void Check_IndirectLeftRecursionInBranches_Success()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("syntax = rule1 ;");
            text.AppendLine("rule1 = 'abc' | rule2 ;");
            text.AppendLine("rule2 = rule1 ;");

            LexicalAnalyzer analyzer = new LexicalAnalyzer(text.ToStreamReader());
            Parser parser = new Parser();

            Syntax syntax = parser.Parse(analyzer);
            RuleReferenceResolver resolver = new RuleReferenceResolver();
            resolver.Resolve(syntax);

            LeftRecursionChecker checker = new LeftRecursionChecker();
            Rule[] rules = checker.Check(syntax).ToArray();

            Assert.AreEqual(2, rules.Length);
            Assert.AreEqual("rule1", rules[0].MetaIdentifier.Text);
            Assert.AreEqual("rule2", rules[1].MetaIdentifier.Text);
        }

        /// <summary>
        /// Tests that <see cref="LeftRecursionChecker.Check(Syntax)"/> 
        /// doesn't report non left recursive rules.
        /// </summary>
        [Test]
        public void Check_NoLeftRecursiveRules_NoRulesReported()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("syntax = rule1 ;");
            text.AppendLine("rule1 = rule2 ;");
            text.AppendLine("rule2 = 'abc' ;");

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            LeftRecursionChecker checker = new LeftRecursionChecker();
            Rule[] rules = checker.Check(syntax).ToArray();

            Assert.AreEqual(0, rules.Length);
        }

        /// <summary>
        /// Tests that <see cref="LeftRecursionChecker.Check(Syntax)"/> 
        /// doesn't report anything if the syntax is empty.
        /// </summary>
        [Test]
        public void Check_EmptySyntax_NoRulesReported()
        {
            string text = "syntax = ;";

            SyntaxReader reader = new SyntaxReader();
            Syntax syntax = reader.Read(text.ToStream());

            LeftRecursionChecker checker = new LeftRecursionChecker();
            Rule[] rules = checker.Check(syntax).ToArray();

            Assert.AreEqual(0, rules.Length);
        }
    }
}