using Pote;
using Pote.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
        /// <param name="syntax">The syntax to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="syntax"/> is null.
        /// </exception>
        /// <exception cref="SpecialSequenceException">
        /// The <paramref name="syntax"/> contains an erroneous special sequence.
        /// </exception>
        public Collection<string> Generate(Syntax syntax)
        {
            if (syntax == null)
                throw new ArgumentNullException(nameof(syntax));

            return Generate(syntax, syntax.Start);
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="syntax"/> starting
        /// from the given <paramref name="rule"/>.
        /// </summary>
        /// <param name="syntax">The syntax to generate tests for.</param>
        /// <param name="rule">The rule to start generating test from.</param>
        /// <returns>The generated tests.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="syntax"/> or <paramref name="rule"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="rule"/> is not defined in the <paramref name="syntax"/>.
        /// </exception>
        /// <exception cref="SpecialSequenceException">
        /// The <paramref name="syntax"/> contains an erroneous special sequence.
        /// </exception>
        public Collection<string> Generate(Syntax syntax, Rule rule)
        {
            if (syntax == null)
                throw new ArgumentNullException(nameof(syntax));
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            if (!syntax.Rules.Contains(rule))
                throw new ArgumentException("The rule is not part of the syntax");

            source = syntax;
            return new Collection<string>(Generate(rule));
        }

        /// <summary>
        /// Generates a fuzzy test for the <paramref name="terminal"/>.
        /// </summary>
        /// <param name="terminal">The terminal to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        private static List<string> Generate(TerminalString terminal)
        {
            return new List<string>() { terminal.Value.Text };
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="rule"/>.
        /// </summary>
        /// <param name="rule">The rule to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        private List<string> Generate(Rule rule)
        {
            if (!depth.ContainsKey(rule))
                depth[rule] = 0;

            if (depth[rule] == RecursionDepth)
                return new List<string>();

            depth[rule]++;
            var strings = Generate(rule.Branches);
            depth[rule]--;

            return strings;
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="branches"/>.
        /// </summary>
        /// <param name="branches">The branches to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        private List<string> Generate(IList<DefinitionList> branches)
        {
            var strings = new List<string>();
            foreach (var branch in branches)
                strings.AddRange(Generate(branch));

            return strings;
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="list"/>.
        /// </summary>
        /// <param name="list">The list to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        private List<string> Generate(DefinitionList list)
        {
            var strings = new List<string>();
            foreach (var definition in list)
                strings.AddRange(Generate(definition));

            return strings;
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="definition"/>.
        /// </summary>
        /// <param name="definition">The definition to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        private List<string> Generate(SingleDefinition definition)
        {
            var strings = new List<string>();
            for (int i = definition.SyntacticTerms.Count; i > 0; i--)
            {
                var temp = new List<string>(strings);
                var generated = Generate(definition.SyntacticTerms[i - 1]);

                if (strings.Count == 0)
                    strings.AddRange(generated);
                else
                {
                    strings.Clear();
                    foreach (var t in temp)
                    {
                        foreach (var g in generated)
                            strings.Add(g + t);
                    }
                }
            }

            return strings;
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="term"/>.
        /// </summary>
        /// <param name="term">The term to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        private List<string> Generate(SyntacticTerm term)
        {
            var generated = Generate(term.Factor);
            if (term.Exception != null)
            {
                var exceptions = Generate(term.Exception);
                var excluded = new List<int>();

                for (var i = 0; i < generated.Count; i++)
                {
                    for (var k = 0; k < exceptions.Count; k++)
                    {
                        if (exceptions[k] == generated[i])
                        {
                            if (!excluded.Contains(i))
                                excluded.Add(i);
                        }
                    }
                }

                foreach (var index in excluded)
                    generated.RemoveAt(index);
            }

            return generated;
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="factor"/>.
        /// </summary>
        /// <param name="factor">The factor to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        private List<string> Generate(SyntacticFactor factor)
        {
            var generated = Generate(factor.SyntacticPrimary);

            var strings = new List<string>(generated);
            for (var i = 1; i < factor.NumberOfRepetitions; i++)
            {
                var temp = new List<string>(strings);
                strings.Clear();

                foreach (var t in temp)
                {
                    foreach (var g in generated)
                        strings.Add(t + g);
                }
            }

            return strings;
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="definition"/>.
        /// </summary>
        /// <param name="definition">The branches to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        private List<string> Generate(Definition definition)
        {
            return Generate((dynamic)definition);
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence">The sequence to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        private List<string> Generate(GroupedSequence sequence)
        {
            return Generate(sequence.Branches);
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence">The sequence to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        private List<string> Generate(OptionalSequence sequence)
        {
            var strings = new List<string>();

            // This is the path when the optional string is not generated.
            strings.Add("");
            strings.AddRange(Generate(sequence.Branches));

            return strings;
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence">The sequence to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        private List<string> Generate(RepeatedSequence sequence)
        {
            var strings = new List<string>();

            var generated = new List<string>();
            foreach (var branch in sequence.Branches)
                generated.AddRange(Generate(branch));

            foreach (var combination in generated.Combinations(Repetitions))
                strings.Add(combination.Join());

            return strings;
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence">The sequence to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        /// <exception cref="SpecialSequenceException">
        /// There is an error in the given <paramref name="sequence"/>.
        /// </exception>
        private List<string> Generate(SpecialSequence sequence)
        {
            ISpecialSequenceGenerator selected = null;
            foreach (var validator in SpecialSequenceGenerator)
            {
                if (validator.IsValid(sequence.Value.Text))
                {
                    if (selected != null)
                        throw new SpecialSequenceException(sequence.Value.Position.ToString("Ambiguous special sequence."));
                    else
                        selected = validator;
                }
            }

            if (selected == null)
                throw new SpecialSequenceException(sequence.Value.Position.ToString("No generator defined for the special sequence"));

            return new List<string>(selected.Generate(sequence.Value.Text));
        }

        /// <summary>
        /// Generates fuzzy tests for the <paramref name="identifier"/>.
        /// </summary>
        /// <param name="identifier">The identifier to generate tests for.</param>
        /// <returns>The generated tests.</returns>
        private List<string> Generate(MetaIdentifier identifier)
        {
            var rule = (from r in source
                        where r.MetaIdentifier.Text == identifier.Value.Text
                        select r).FirstOrDefault();

            if (rule == null)
            {
                string message = "Referencing an undefined rule.";
                throw new UndefinedRuleException(identifier.Value.Position.ToString(message));
            }

            return Generate(rule);
        }
    }
}