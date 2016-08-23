using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spot.Ebnf
{
    /// <summary>
    /// Passes all the nodes of a <see cref="Syntax"/>.
    /// </summary>
    internal interface IPass
    {
        /// <summary>
        /// Visits a syntax.
        /// </summary>
        /// <param name="syntax">The syntax to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="syntax"/> is null.
        /// </exception>
        void Visit(Syntax syntax);

        /// <summary>
        /// Visits a rule.
        /// </summary>
        /// <param name="rule">The rule to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rule"/> is null.
        /// </exception>
        void Visit(Rule rule);

        /// <summary>
        /// Visits a definition list.
        /// </summary>
        /// <param name="list">The list to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is null.
        /// </exception>
        void Visit(DefinitionList list);

        /// <summary>
        /// Visits a single definition.
        /// </summary>
        /// <param name="definition">The single definition to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="definition"/> is null.
        /// </exception>
        void Visit(SingleDefinition definition);

        /// <summary>
        /// Visits a syntactic term.
        /// </summary>
        /// <param name="term">The term to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="term"/> is null.
        /// </exception>
        void Visit(SyntacticTerm term);

        /// <summary>
        /// Visits a syntactic factor.
        /// </summary>
        /// <param name="factor">The factor to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factor"/> is null.
        /// </exception>
        void Visit(SyntacticFactor factor);

        /// <summary>
        /// Visits an identifier.
        /// </summary>
        /// <param name="identifier">The identifier to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="identifier"/> is null.
        /// </exception>
        void Visit(MetaIdentifier identifier);

        /// <summary>
        /// Visits a sequence.
        /// </summary>
        /// <param name="sequence">The sequence to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sequence"/> is null.
        /// </exception>
        void Visit(OptionalSequence sequence);

        /// <summary>
        /// Visits a sequence.
        /// </summary>
        /// <param name="sequence">The sequence to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sequence"/> is null.
        /// </exception>
        void Visit(RepeatedSequence sequence);

        /// <summary>
        /// Visits a sequence.
        /// </summary>
        /// <param name="sequence">The sequence to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sequence"/> is null.
        /// </exception>
        void Visit(GroupedSequence sequence);

        /// <summary>
        /// Visits a sequence.
        /// </summary>
        /// <param name="sequence">The sequence to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sequence"/> is null.
        /// </exception>
        void Visit(SpecialSequence sequence);

        /// <summary>
        /// Visits a terminal.
        /// </summary>
        /// <param name="terminal">The terminal to visit.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="terminal"/> is null.
        /// </exception>
        void Visit(TerminalString terminal);
    }
}