using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spot.Ebnf
{
    /// <summary>
    /// Passes all the nodes of a <see cref="Syntax"/>.
    /// </summary>
    public abstract class Pass : IPass
    {
        private Syntax source;

        /// <summary>
        /// Visits a syntax.
        /// </summary>
        /// <param name="syntax">The syntax to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="syntax"/> is null.
        /// </exception>
        [DebuggerStepThrough]
        public virtual void Visit(Syntax syntax)
        {
            if (syntax == null)
                throw new ArgumentNullException(nameof(syntax));

            source = syntax;
            Visit(syntax.Start);
        }

        /// <summary>
        /// Visits a rule.
        /// </summary>
        /// <param name="rule">The rule to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rule"/> is null.
        /// </exception>
        [DebuggerStepThrough]
        public virtual void Visit(Rule rule)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            Visit(rule.Branches);
        }

        /// <summary>
        /// Visits a definition list.
        /// </summary>
        /// <param name="list">The list to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is null.
        /// </exception>
        [DebuggerStepThrough]
        public virtual void Visit(DefinitionList list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            foreach (dynamic definition in list)
                Visit(definition);
        }

        /// <summary>
        /// Visits a single definition.
        /// </summary>
        /// <param name="definition">The single definition to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="definition"/> is null.
        /// </exception>
        [DebuggerStepThrough]
        public virtual void Visit(SingleDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            foreach (var term in definition.SyntacticTerms)
                Visit(term);
        }

        /// <summary>
        /// Visits a syntactic term.
        /// </summary>
        /// <param name="term">The term to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="term"/> is null.
        /// </exception>
        [DebuggerStepThrough]
        public virtual void Visit(SyntacticTerm term)
        {
            if (term == null)
                throw new ArgumentNullException(nameof(term));

            Visit(term.Factor);
        }

        /// <summary>
        /// Visits a syntactic factor.
        /// </summary>
        /// <param name="factor">The factor to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factor"/> is null.
        /// </exception>
        [DebuggerStepThrough]
        public virtual void Visit(SyntacticFactor factor)
        {
            if (factor == null)
                throw new ArgumentNullException(nameof(factor));

            for (int i = 0; i < factor.NumberOfRepetitions; i++)
                Visit((dynamic)factor.SyntacticPrimary);
        }

        /// <summary>
        /// Visits an identifier.
        /// </summary>
        /// <param name="identifier">The identifier to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="identifier"/> is null.
        /// </exception>
        [DebuggerStepThrough]
        public virtual void Visit(MetaIdentifier identifier)
        {
            var rule = source.GetRuleBy(identifier.Name);
            if (rule == null)
                throw new UndefinedRuleException(identifier.DefinedAt.ToString($"The rule '{identifier.Name}' is undefined."));          

            Visit(rule);
        }

        /// <summary>
        /// Visits a sequence.
        /// </summary>
        /// <param name="sequence">The sequence to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sequence"/> is null.
        /// </exception>
        [DebuggerStepThrough]
        public virtual void Visit(OptionalSequence sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            Visit(sequence.Branches);
        }

        /// <summary>
        /// Visits a sequence.
        /// </summary>
        /// <param name="sequence">The sequence to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sequence"/> is null.
        /// </exception>
        [DebuggerStepThrough]
        public virtual void Visit(RepeatedSequence sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            Visit(sequence.Branches);
        }

        /// <summary>
        /// Visits a sequence.
        /// </summary>
        /// <param name="sequence">The sequence to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sequence"/> is null.
        /// </exception>
        [DebuggerStepThrough]
        public virtual void Visit(GroupedSequence sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            Visit(sequence.Branches);
        }

        /// <summary>
        /// Visits a sequence.
        /// </summary>
        /// <param name="sequence">The sequence to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sequence"/> is null.
        /// </exception>
        [DebuggerStepThrough]
        public virtual void Visit(SpecialSequence sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));
        }

        /// <summary>
        /// Visits a terminal.
        /// </summary>
        /// <param name="terminal">The terminal to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="terminal"/> is null.
        /// </exception>
        [DebuggerStepThrough]
        public virtual void Visit(TerminalString terminal)
        {
            if (terminal == null)
                throw new ArgumentNullException(nameof(terminal));
        }
    }
}