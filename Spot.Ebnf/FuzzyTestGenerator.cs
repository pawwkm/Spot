using Pote;
using Pote.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spot.Ebnf
{
    /// <summary>
    /// Fuzzy test generator for ebnf (iso 14977) compliant grammars.
    /// </summary>
    public sealed class FuzzyTestGenerator
    {
        private int recursionDepth = 3;

        private Range repetitions = new Range(0, 2);

        private Dictionary<Rule, int> depth = new Dictionary<Rule, int>();

        private Syntax source;

        private Stream stream;

        private IList<ISpecialSequenceGenerator> specialSequenceGenerator = new List<ISpecialSequenceGenerator>();

        /// <summary>
        /// The maximum level of recursive nesting for each rule.
        /// The default is 3.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is less than 1.
        /// </exception>
        public int RecursionDepth
        {
            get
            {
                return recursionDepth;
            }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(value), "The value is less than 1.");

                recursionDepth = value;
            }
        }

        /// <summary>
        /// This range defines the minimum and maximum times text 
        /// is generated for repeated sequences. Default range 0 - 2.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The minimum or maximum value is negative.
        /// </exception>
        public Range Repetitions
        {
            get
            {
                return repetitions;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (value.Minimum < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "The minimum value is negative.");

                if (value.Maximum < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "The maximum value is negative.");

                repetitions = value;
            }
        }

        /// <summary>
        /// The list of special sequence generators used by this test generator.
        /// </summary>
        public IList<ISpecialSequenceGenerator> SpecialSequenceGenerator
        {
            get
            {
                return specialSequenceGenerator;
            }
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="syntax"/>.
        /// </summary>
        /// <param name="output">Where the fuzz tests are written to.</param>
        /// <param name="syntax">The syntax to generate tests for.</param>
        /// <returns>The number of tests generated.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="output"/> or <paramref name="syntax"/> is null.
        /// </exception>
        /// <exception cref="SpecialSequenceException">
        /// The <paramref name="syntax"/> contains an erroneous special sequence.
        /// </exception>
        public ulong Generate(Stream output, Syntax syntax)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            if (syntax == null)
                throw new ArgumentNullException(nameof(syntax));

            return Generate(output, syntax, syntax.Start);
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="syntax"/> starting
        /// from the given <paramref name="rule"/>.
        /// </summary>
        /// <param name="output">Where the fuzz tests are written to.</param>
        /// <param name="syntax">The syntax to generate tests for.</param>
        /// <param name="rule">The rule to start generating test from.</param>
        /// <returns>The number of tests generated.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="output"/>, <paramref name="syntax"/> or 
        /// <paramref name="rule"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="rule"/> is not defined in the <paramref name="syntax"/>.
        /// </exception>
        /// <exception cref="SpecialSequenceException">
        /// The <paramref name="syntax"/> contains an erroneous special sequence.
        /// </exception>
        public ulong Generate(Stream output, Syntax syntax, Rule rule)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            if (syntax == null)
                throw new ArgumentNullException(nameof(syntax));
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            if (!syntax.Rules.Contains(rule))
                throw new ArgumentException("The rule is not part of the syntax");

            source = syntax;
            stream = output;

            ulong count = 0;
            foreach (var test in Generate(rule))
            {
                ushort length = (ushort)test.Length;

                var bytes = BitConverter.GetBytes(length);
                stream.Write(bytes, 0, bytes.Length);

                bytes = Encoding.UTF8.GetBytes(test);
                stream.Write(bytes, 0, bytes.Length);

                count++;
            }

            return count;
        }

        /// <summary>
        /// Generates a fuzzy test for the <paramref name="terminal"/>.
        /// </summary>
        /// <param name="terminal">The terminal to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        private static IEnumerable<string> Generate(TerminalString terminal)
        {
            yield return terminal.Value;
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="rule"/>.
        /// </summary>
        /// <param name="rule">The rule to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        private IEnumerable<string> Generate(Rule rule)
        {
            if (!depth.ContainsKey(rule))
                depth[rule] = 0;

            if (depth[rule] != RecursionDepth)
            {
                depth[rule]++;
                foreach (var fragment in Generate(rule.Branches))
                    yield return fragment;

                depth[rule]--;
            }
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="list"/>.
        /// </summary>
        /// <param name="list">The list to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        private IEnumerable<string> Generate(DefinitionList list)
        {
            foreach (var definition in list)
            {
                foreach (var fragment in Generate(definition))
                    yield return fragment;
            }
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="definition"/>.
        /// </summary>
        /// <param name="definition">The definition to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        private IEnumerable<string> Generate(SingleDefinition definition)
        {
            IEnumerable<string> fragments = null;
            foreach (var term in definition.SyntacticTerms.Reverse())
            {
                if (fragments == null)
                    fragments = Generate(term);
                else
                    fragments = Combine(Generate(term), fragments);
            }

            return fragments;
        }

        /// <summary>
        /// Calculates all the combinations of <paramref name="left"/> and <paramref name="right"/>
        /// but, they do not change position.
        /// </summary>
        /// <param name="left">The left hand side.</param>
        /// <param name="right">The right hand side.</param>
        /// <returns>All the combinations of <paramref name="left"/> and <paramref name="right"/>.</returns>
        private IEnumerable<string> Combine(IEnumerable<string> left, IEnumerable<string> right)
        {
            foreach (var l in left)
            {
                foreach (var r in right)
                    yield return l + r;
            }
        }

        /// <summary>
        /// Enumerates <paramref name="a"/> and <paramref name="b"/> as if they were
        /// one list consisting of <paramref name="a"/>'s elements then <paramref name="b"/>'s elements.
        /// </summary>
        /// <param name="a">The first list.</param>
        /// <param name="b">The second list.</param>
        /// <returns>A merged version of <paramref name="a"/> and <paramref name="b"/>.</returns>
        private IEnumerable<string> Merge(IEnumerable<string> a, IEnumerable<string> b)
        {
            foreach (var value in a)
                yield return value;

            foreach (var value in b)
                yield return value;
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="term"/>.
        /// </summary>
        /// <param name="term">The term to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        private IEnumerable<string> Generate(SyntacticTerm term)
        {
            if (term.Exception == null)
            {
                foreach (var fragment in Generate(term.Factor))
                    yield return fragment;
            }
            else
            {
                var exceptions = Generate(term.Exception);
                foreach (var fragment in Generate(term.Factor))
                {
                    if (!exceptions.Contains(fragment))
                        yield return fragment;
                }
            }
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="factor"/>.
        /// </summary>
        /// <param name="factor">The factor to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        private IEnumerable<string> Generate(SyntacticFactor factor)
        {
            var fragments = Generate(factor.SyntacticPrimary);
            for (var i = 1; i < factor.NumberOfRepetitions; i++)
                fragments = Combine(Generate(factor.SyntacticPrimary), fragments);

            return fragments;
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="definition"/>.
        /// </summary>
        /// <param name="definition">The branches to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        private IEnumerable<string> Generate(Definition definition)
        {
            return Generate((dynamic)definition);
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence">The sequence to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        private IEnumerable<string> Generate(GroupedSequence sequence)
        {
            return Generate(sequence.Branches);
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence">The sequence to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        private IEnumerable<string> Generate(OptionalSequence sequence)
        {
            // This is the path when the optional string is not generated.
            yield return "";

            foreach (var fragment in Generate(sequence.Branches))
                yield return fragment;
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence">The sequence to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        private IEnumerable<string> Generate(RepeatedSequence sequence)
        {
            IEnumerable<string> fragments = new string[0];
            foreach (var branch in sequence.Branches)
                fragments = Merge(fragments, Generate(branch));

            foreach (var combination in fragments.Combinations(Repetitions))
                yield return combination.Join();
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence">The sequence to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        /// <exception cref="SpecialSequenceException">
        /// There is an error in the given <paramref name="sequence"/>.
        /// </exception>
        private IEnumerable<string> Generate(SpecialSequence sequence)
        {
            ISpecialSequenceGenerator selected = null;
            foreach (var validator in SpecialSequenceGenerator)
            {
                if (validator.IsValid(sequence.Value))
                {
                    if (selected != null)
                        throw new SpecialSequenceException(sequence.DefinedAt.ToString("Ambiguous special sequence."));
                    else
                        selected = validator;
                }
            }

            if (selected == null)
                throw new SpecialSequenceException(sequence.DefinedAt.ToString("No generator defined for the special sequence."));

            return selected.Generate(sequence.Value);
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="identifier"/>.
        /// </summary>
        /// <param name="identifier">The identifier to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        private IEnumerable<string> Generate(MetaIdentifier identifier)
        {
            var rule = (from r in source
                        where r.Name == identifier.Name
                        select r).FirstOrDefault();

            if (rule == null)
            {
                string message = "Referencing an undefined rule.";
                throw new UndefinedRuleException(identifier.DefinedAt.ToString(message));
            }

            return Generate(rule);
        }
    }
}