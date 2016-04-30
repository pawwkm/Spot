using System;
using System.Collections.Generic;

namespace Spot.Ebnf
{
    /// <summary>
    /// Use to determine if a <see cref="Syntax"/> contains rules
    /// that are left recursive.
    /// </summary>
    internal sealed class LeftRecursionChecker
    {
        private Syntax source;

        /// <summary>
        /// Checks if the given <paramref name="syntax"/> contains rules
        /// that are left recursive.
        /// </summary>
        /// <param name="syntax">The syntax to check.</param>
        /// <returns>The rules that are left recursive.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="syntax"/> is null.
        /// </exception>
        public IEnumerable<Rule> Check(Syntax syntax)
        {
            if (syntax == null)
                throw new ArgumentNullException(nameof(syntax));

            source = syntax;

            List<Rule> recursiveRules = new List<Rule>();
            foreach (Rule rule in syntax)
            {
                if (IsLeftRecursive(rule))
                {
                    if (!recursiveRules.Contains(rule))
                        recursiveRules.Add(rule);
                }
            }

            return recursiveRules;
        }

        /// <summary>
        /// Searches for a meta identifiers in the <paramref name="definition"/>.
        /// </summary>
        /// <param name="definition">The definition to search in.</param>
        /// <returns>All the meta identifiers in the <paramref name="definition"/>.</returns>
        private static IEnumerable<MetaIdentifier> FindReferences(SingleDefinition definition)
        {
            var identifier = definition.SyntacticTerms[0].Factor.SyntacticPrimary as MetaIdentifier;
            if (identifier != null)
                yield return identifier;

            var sequence = definition.SyntacticTerms[0].Factor.SyntacticPrimary as Sequence;
            if (sequence != null)
            {
                foreach (var branch in sequence.Branches)
                {
                    if (branch is SingleDefinition)
                    {
                        foreach (var meta in FindReferences(branch as SingleDefinition))
                            yield return meta;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the given <paramref name="rule"/> is is left 
        /// recursive, directly or indirectly.
        /// </summary>
        /// <param name="rule">The rule to check for left recursiveness.</param>
        /// <returns>True if the rule is left recursive.</returns>
        private bool IsLeftRecursive(Rule rule)
        {
            List<Rule> checkedRules = new List<Rule>();
            Stack<Rule> rulesToCheck = new Stack<Rule>();
            rulesToCheck.Push(rule);

            Rule current;
            do
            {
                current = rulesToCheck.Pop();
                if (checkedRules.Contains(current))
                    continue;

                checkedRules.Add(current);
                foreach (var definition in current.Branches)
                {
                    if (definition is SingleDefinition)
                    {
                        foreach (var meta in FindReferences(definition as SingleDefinition))
                        {
                            if (rule.Name == meta.Name)
                                return true;

                            rulesToCheck.Push(source.GetRuleBy(meta.Name));
                        }
                    }
                }
            }
            while (rulesToCheck.Count != 0);

            return false;
        }
    }
}