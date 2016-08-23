using System;
using System.Collections.Generic;
using System.Linq;

namespace Spot.Ebnf
{
    /// <summary>
    /// Generates a random sentence from a EBNF (ISO 14977) compliant grammar.
    /// </summary>
    public class RandomPass : Pass
    {
        private IList<ISpecialSequenceGenerator> specialSequenceGenerator = new List<ISpecialSequenceGenerator>();

        private static Random random = new Random();

        private Dictionary<string, int> depth = new Dictionary<string, int>();

        private int recursionDepth = 3;

        /// <summary>
        /// The randomly generated sentence.
        /// </summary>
        public string Sentence
        {
            get;
            private set;
        }

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
        /// The list of special sequence generators used by this pass.
        /// </summary>
        public IList<ISpecialSequenceGenerator> SpecialSequenceGenerators
        {
            get
            {
                return specialSequenceGenerator;
            }
        }

        /// <summary>
        /// Visits a syntax.
        /// </summary>
        /// <param name="syntax">The syntax to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="syntax"/> is null.
        /// </exception>
        public override void Visit(Syntax syntax)
        {
            Sentence = "";
            base.Visit(syntax);
        }

        /// <summary>
        /// Visits a rule.
        /// </summary>
        /// <param name="rule">The rule to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rule"/> is null.
        /// </exception>
        public override void Visit(Rule rule)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            if (!depth.ContainsKey(rule.Name))
                depth[rule.Name] = 0;

            if (depth[rule.Name] == RecursionDepth)
                return;

            depth[rule.Name]++;
            base.Visit(rule);
            depth[rule.Name]--;
        }

        /// <summary>
        /// Visits a definition list.
        /// </summary>
        /// <param name="list">The list to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is null.
        /// </exception>
        public override void Visit(DefinitionList list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (list.Count == 0)
                return;

            var index = random.Next(0, list.Count);

            Visit((dynamic)list[index]);
        }

        /// <summary>
        /// Visits a syntactic term.
        /// </summary>
        /// <param name="term">The term to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="term"/> is null.
        /// </exception>
        public override void Visit(SyntacticTerm term)
        {
            if (term == null)
                throw new ArgumentNullException(nameof(term));

            if (term.Exception == null)
                Visit(term.Factor);
            else
            {
                var index = Sentence.Length;
                var exception = "a";

                do
                {
                    Visit(term.Factor);
                    if (Sentence.Substring(index).StartsWith(exception))
                        Sentence = Sentence.Substring(0, index);
                    else
                        break;
                }
                while (true);
            }
        }

        /// <summary>
        /// Visits a sequence.
        /// </summary>
        /// <param name="sequence">The sequence to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sequence"/> is null.
        /// </exception>
        public override void Visit(OptionalSequence sequence)
        {
            if (random.Bool())
                base.Visit(sequence);
        }

        /// <summary>
        /// Visits a sequence.
        /// </summary>
        /// <param name="sequence">The sequence to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sequence"/> is null.
        /// </exception>
        public override void Visit(RepeatedSequence sequence)
        {
            while (random.Bool())
                base.Visit(sequence);
        }

        /// <summary>
        /// Visits a sequence.
        /// </summary>
        /// <param name="sequence">The sequence to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sequence"/> is null.
        /// </exception>
        public override void Visit(SpecialSequence sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            ISpecialSequenceGenerator selected = null;
            foreach (var validator in SpecialSequenceGenerators)
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

            var generated = selected.Generate(sequence.Value).ToArray();
            var index = random.Next(0, generated.Length);

            Sentence += generated[index];
        }

        /// <summary>
        /// Visits a terminal.
        /// </summary>
        /// <param name="terminal">The terminal to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="terminal"/> is null.
        /// </exception>
        public override void Visit(TerminalString terminal)
        {
            if (terminal == null)
                throw new ArgumentNullException(nameof(terminal));

            Sentence += terminal.Value;
        }
    }
}