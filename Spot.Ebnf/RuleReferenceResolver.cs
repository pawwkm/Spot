using Pote.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Spot.Ebnf
{
    /// <summary>
    /// Resolves references between rules.
    /// </summary>
    public sealed class RuleReferenceResolver
    {
        private Syntax source;

        /// <summary>
        /// Resolves the references between the rules of the <paramref name="syntax"/>.
        /// </summary>
        /// <param name="syntax">
        /// The syntax to resolve the the rules of.
        /// </param>
        /// <exception cref="ParsingException">
        /// An undeclared rule is referenced.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="syntax"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="syntax"/> contains more than one start rule.
        /// </exception>
        public void Resolve(Syntax syntax)
        {
            if (syntax == null)
                throw new ArgumentNullException(nameof(syntax));

            source = syntax;

            var references = FindReferences();
            ResolveReferencedBy(references);
            ResolveRulesReferenced();

            foreach (Rule rule in syntax)
            {
                if (rule.ReferencedBy.Any(x => x != rule))
                    continue;

                if (syntax.Start != null)
                    throw new ArgumentException("Contains more than one start rule.", "syntax");

                syntax.Start = rule;
            }
        }

        /// <summary>
        /// Resolves the <see cref="Rule.ReferencedBy"/> property
        /// of the given <paramref name="references"/>.
        /// </summary>
        /// <param name="references">
        /// The key is a rule and its value is the names of the rules that
        /// it references.
        /// </param>
        /// <exception cref="ParsingException">
        /// An undeclared rule is referenced.
        /// </exception>
        private static void ResolveReferencedBy(Dictionary<Rule, List<string>> references)
        {
            foreach (var pair in references)
            {
                foreach (string name in pair.Value)
                {
                    var rule = (from r in references.Keys
                                where r.Name == name
                                select r).FirstOrDefault();

                    if (rule == null)
                        throw new ParsingException("The '" + name + "' rule is not declared.");

                    pair.Key.ReferencedBy.Add(rule);
                }
            }
        }

        /// <summary>
        /// Resolves the <see cref="Rule.RulesReferenced"/> property
        /// of the given <see cref="source"/>.
        /// </summary>
        private void ResolveRulesReferenced()
        {
            foreach (Rule referencee in source)
            {
                foreach (var referencer in source)
                {
                    foreach (var rule in referencee.ReferencedBy)
                    {
                        if (referencer == rule)
                            referencer.RulesReferenced.Add(referencee);
                    }
                }
            }
        }

        /// <summary>
        /// Finds all the references that a set of rules themselves reference.
        /// </summary>
        /// <returns>
        /// The key is a rule and its value is the names of the rules that
        /// it references.
        /// </returns>
        /// <exception cref="ParsingException">
        /// An undefined rule has been referenced.
        /// </exception>
        private Dictionary<Rule, List<string>> FindReferences()
        {
            var references = new Dictionary<Rule, List<string>>();
            foreach (Rule rule in source)
            {
                references.Add(rule, new List<string>());
                foreach (var r in source)
                {
                    foreach (var l in r.Branches)
                    {
                        if (FindReferences(l, rule.Name))
                            references[rule].Add(r.Name);
                    }
                }
            }

            return references;
        }

        /// <summary>
        /// Checks if the rule with the given <paramref name="name"/>
        /// is referenced by the <paramref name="list"/>.
        /// </summary>
        /// <param name="list">The list to search.</param>
        /// <param name="name">The name of the rule's meta identifier.</param>
        /// <returns>
        /// True if the <paramref name="name"/> was referenced by 
        /// the <paramref name="list"/>.
        /// </returns>
        /// <exception cref="ParsingException">
        /// An undefined rule has been referenced.
        /// </exception>
        private bool FindReferences(DefinitionList list, string name)
        {
            foreach (Definition definition in list)
            {
                if (FindReferences(definition, name))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the rule with the given <paramref name="name"/>
        /// is referenced by the <paramref name="definition"/>.
        /// </summary>
        /// <param name="definition">The definition to search.</param>
        /// <param name="name">The name of the rule's meta identifier.</param>
        /// <returns>
        /// True if the <paramref name="name"/> was referenced by 
        /// the <paramref name="definition"/>.
        /// </returns>
        private bool FindReferences(SingleDefinition definition, string name)
        {
            foreach (var term in definition.SyntacticTerms)
            {
                if (FindReferences(term, name))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the rule with the given <paramref name="name"/>
        /// is referenced by the <paramref name="term"/>.
        /// </summary>
        /// <param name="term">The term to search.</param>
        /// <param name="name">The name of the rule's meta identifier.</param>
        /// <returns>
        /// True if the <paramref name="name"/> was referenced by 
        /// the <paramref name="term"/>.
        /// </returns>
        /// <exception cref="ParsingException">
        /// An undefined rule has been referenced.
        /// </exception>
        /// <exception cref="ParsingException">
        /// An undefined rule has been referenced.
        /// </exception>
        private bool FindReferences(SyntacticTerm term, string name)
        {
            if (FindReferences(term.Factor.SyntacticPrimary, name))
                return true;

            return false;
        }

        /// <summary>
        /// Checks if the rule with the given <paramref name="name"/>
        /// is referenced by the <paramref name="definition"/>.
        /// </summary>
        /// <param name="definition">The definition to search.</param>
        /// <param name="name">The name of the rule's meta identifier.</param>
        /// <returns>
        /// True if the <paramref name="name"/> was referenced by 
        /// the <paramref name="definition"/>.
        /// </returns>
        /// <exception cref="ParsingException">
        /// An undefined rule has been referenced.
        /// </exception>
        private bool FindReferences(Definition definition, string name)
        {
            if (definition is OptionalSequence)
                return FindReferences(definition as OptionalSequence, name);
            if (definition is RepeatedSequence)
                return FindReferences(definition as RepeatedSequence, name);
            if (definition is GroupedSequence)
                return FindReferences(definition as GroupedSequence, name);
            if (definition is MetaIdentifier)
                return FindReferences(definition as MetaIdentifier, name);
            if (definition is SyntacticTerm)
                return FindReferences(definition as SyntacticTerm, name);
            if (definition is SyntacticFactor)
                return FindReferences(definition as SyntacticFactor, name);
            if (definition is SingleDefinition)
                return FindReferences(definition as SingleDefinition, name);

            return false;
        }

        /// <summary>
        /// Checks if the rule with the given <paramref name="name"/>
        /// is referenced by the <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence">The sequence to search.</param>
        /// <param name="name">The name of the rule's meta identifier.</param>
        /// <returns>
        /// True if the <paramref name="name"/> was referenced by 
        /// the <paramref name="sequence"/>.
        /// </returns>
        private bool FindReferences(Sequence sequence, string name)
        {
            foreach (var branch in sequence.Branches)
            {
                if (FindReferences(branch, name))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the rule with the given <paramref name="name"/>
        /// is referenced by the <paramref name="identifier"/>.
        /// </summary>
        /// <param name="identifier">The identifier to check.</param>
        /// <param name="name">The name of the rule's meta identifier.</param>
        /// <returns>
        /// True if the <paramref name="name"/> was referenced by 
        /// the <paramref name="identifier"/>.
        /// </returns>
        /// <exception cref="ParsingException">
        /// An undefined rule has been referenced.
        /// </exception>
        private bool FindReferences(MetaIdentifier identifier, string name)
        {
            if (IsRuleUndefined(identifier))
            {
                string message = "The '" + identifier.Name + "' rule is not declared.";
                throw new ParsingException(identifier.DefinedAt.ToString(message));
            }

            if (identifier.Name == name)
                return true;

            return false;
        }

        /// <summary>
        /// Checks if the given <paramref name="identifier"/> is a 
        /// reference to an undefined rule.
        /// </summary>
        /// <param name="identifier">The reference to check.</param>
        /// <returns>True if there is no rule defined with the given <paramref name="identifier"/>.</returns>
        private bool IsRuleUndefined(MetaIdentifier identifier)
        {
            var r = (from rule in source
                     where rule.Name == identifier.Name
                     select rule).FirstOrDefault();

            return r == null;
        }
    }
}