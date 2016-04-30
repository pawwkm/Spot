using Pote;
using Pote.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Spot.Ebnf
{
    /// <summary>
    /// Uses a specified syntax to validate text.
    /// </summary>
    public sealed class SyntaxValidator
    {
        private Stream source;

        private List<ISpecialSequenceValidator> specialSequenceValidators = new List<ISpecialSequenceValidator>();

        private List<SyntaxPath> paths = new List<SyntaxPath>();

        private SyntaxPath path;

        private IncludedRules includedRules;

        private ExcludedRules excludedRules;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxValidator"/> class.
        /// </summary>
        /// <param name="syntax">The syntax used for validating text.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="syntax"/> is null.
        /// </exception>
        public SyntaxValidator(Syntax syntax)
        {
            if (syntax == null)
                throw new ArgumentNullException(nameof(syntax));

            Syntax = syntax;
        }

        /// <summary>
        /// The syntax used for validating text.
        /// </summary>
        public Syntax Syntax
        {
            get;
            private set;
        }

        /// <summary>
        /// The special sequence validators the syntax validator uses 
        /// when validating special sequences.
        /// </summary>
        public IList<ISpecialSequenceValidator> SpecialSequenceValidators
        {
            get
            {
                return specialSequenceValidators;
            }
        }

        /// <summary>
        /// The current syntax path.
        /// </summary>
        private SyntaxPath Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
                source.Position = value.ByteIndex;
            }
        }

        /// <summary>
        /// Validates that the <paramref name="text"/> conforms 
        /// to the <see cref="Syntax"/>.
        /// </summary>
        /// <param name="text">The text to validate.</param>
        /// <returns>The result of the validation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="text"/> is null.
        /// </exception>
        public SyntaxValidationResult Validate(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            return Validate(text.ToStream(), "");
        }

        /// <summary>
        /// Validates that the <paramref name="text"/> conforms 
        /// to the <see cref="Syntax"/>.
        /// </summary>
        /// <param name="text">The text to validate.</param>
        /// <param name="rule">
        /// The name of the rule to start from. 
        /// If left empty, the validation start from the rule that no other 
        /// rule references.
        /// </param>
        /// <returns>The result of the validation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="text"/> or <paramref name="rule"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The given <paramref name="rule"/> doens't exist in the syntax.
        /// </exception>
        public SyntaxValidationResult Validate(string text, string rule)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            return Validate(text.ToStream(), rule);
        }

        /// <summary>
        /// Validates that the <paramref name="text"/> conforms 
        /// to the <see cref="Syntax"/>.
        /// </summary>
        /// <param name="text">The text to validate.</param>
        /// <param name="rule">
        /// The name of the rule to start from. 
        /// If left empty, the validation start from the rule that no other 
        /// rule references.
        /// </param>
        /// <param name="included">
        /// The only rules to be called in the validation.
        /// </param>
        /// <returns>The result of the validation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="text"/> or <paramref name="rule"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The given <paramref name="rule"/> doens't exist in the syntax.
        /// <paramref name="included"/> lists a rule that is not defined in the syntax.
        /// </exception>
        public SyntaxValidationResult Validate(string text, string rule, IncludedRules included)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));
            if (included == null)
                throw new ArgumentNullException(nameof(included));

            return Validate(text.ToStream(), rule, included);
        }

        /// <summary>
        /// Validates that the <paramref name="text"/> conforms 
        /// to the <see cref="Syntax"/>.
        /// </summary>
        /// <param name="text">The text to validate.</param>
        /// <param name="rule">
        /// The name of the rule to start from. 
        /// If left empty, the validation start from the rule that no other 
        /// rule references.
        /// </param>
        /// <param name="excluded">
        /// The rules not to be called in the validation.
        /// </param>
        /// <returns>The result of the validation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="text"/> or <paramref name="rule"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The given <paramref name="rule"/> doens't exist in the syntax.
        /// <paramref name="excluded"/> lists a rule that is not defined in the syntax.
        /// </exception>
        public SyntaxValidationResult Validate(string text, string rule, ExcludedRules excluded)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));
            if (excluded == null)
                throw new ArgumentNullException(nameof(excluded));

            return Validate(text.ToStream(), rule, excluded);
        }

        /// <summary>
        /// Validates that the <paramref name="stream"/> conforms 
        /// to the <see cref="Syntax"/>.
        /// </summary>
        /// <param name="stream">The input to validate.</param>
        /// <returns>The result of the validation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="stream"/> is not readable.
        /// The <paramref name="stream"/> is not seekable.
        /// </exception>
        public SyntaxValidationResult Validate(Stream stream)
        {
            return Validate(stream, "");
        }

        /// <summary>
        /// Validates that the <paramref name="stream"/> conforms 
        /// to the <see cref="Syntax"/>.
        /// </summary>
        /// <param name="stream">The input to validate.</param>
        /// <param name="rule">
        /// The name of the rule to start from. 
        /// If left empty, the validation start from the rule that no other 
        /// rule references.
        /// </param>
        /// <returns>The result of the validation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> or <paramref name="rule"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="stream"/> is not readable.
        /// The <paramref name="stream"/> is not seekable.
        /// The given <paramref name="rule"/> doens't exist in the syntax.
        /// </exception>
        public SyntaxValidationResult Validate(Stream stream, string rule)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead)
                throw new ArgumentException("The stream is not readable.", "stream");
            if (!stream.CanSeek)
                throw new ArgumentException("The stream is not seekable.", "stream");
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            Rule ruleToStartFrom = Syntax.Start;
            if (rule.Length != 0)
            {
                ruleToStartFrom = Syntax.GetRuleBy(rule);
                if (ruleToStartFrom == null)
                    throw new ArgumentException("There doesn't exist a rule with the given name in the syntax.", nameof(rule));
            }

            source = stream;
            long start = stream.Position;

            for (int i = 0; i < Syntax.Start.Branches.Count; i++)
            {
                Path = new SyntaxPath();
                Path.ByteIndex = start;

                InputPosition startPosition = Path.Position.DeepCopy();

                RuleFrame frame = new RuleFrame(ruleToStartFrom.Name, startPosition.DeepCopy());
                Path.RuleTrace.Add(frame);

                if (Validate(ruleToStartFrom.Branches[i]))
                {
                    Path.State = PathState.Success;
                    Path.RuleTrace[0].ExitPoint = Path.Position.DeepCopy();
                }
                else
                {
                    Path.State = PathState.Error;
                    Path.RuleTrace[0].ErrorPoint = startPosition.DeepCopy();
                }

                if (!paths.Contains(Path))
                    paths.Add(Path);
            }

            var result = BuildValidationResult(source.Length);

            paths.Clear();
            source = null;

            return result;
        }

        /// <summary>
        /// Validates that the <paramref name="stream"/> conforms 
        /// to the <see cref="Syntax"/>.
        /// </summary>
        /// <param name="stream">The input to validate.</param>
        /// <param name="rule">
        /// The name of the rule to start from. 
        /// If left empty, the validation start from the rule that no other 
        /// rule references.
        /// </param>
        /// <param name="included">
        /// The only rules to be called in the validation.
        /// </param>
        /// <returns>The result of the validation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> or <paramref name="rule"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="stream"/> is not readable.
        /// The <paramref name="stream"/> is not seekable.
        /// The given <paramref name="rule"/> doens't exist in the syntax.
        /// <paramref name="included"/> lists a rule that is not defined in the syntax.
        /// </exception>
        public SyntaxValidationResult Validate(Stream stream, string rule, IncludedRules included)
        {
            if (included == null)
                throw new ArgumentNullException(nameof(included));

            foreach (var r in included)
            {
                if (Syntax.GetRuleBy(r) == null)
                    throw new ArgumentException("Rule '" + r + "' is not defined in the syntax.");
            }

            includedRules = included;
            var result = Validate(stream, rule);

            includedRules = null;

            return result;
        }

        /// <summary>
        /// Validates that the <paramref name="stream"/> conforms 
        /// to the <see cref="Syntax"/>.
        /// </summary>
        /// <param name="stream">The input to validate.</param>
        /// <param name="rule">
        /// The name of the rule to start from. 
        /// If left empty, the validation start from the rule that no other 
        /// rule references.
        /// </param>
        /// <param name="excluded">
        /// The only rules to be called in the validation.
        /// </param>
        /// <returns>The result of the validation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> or <paramref name="rule"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="stream"/> is not readable.
        /// The <paramref name="stream"/> is not seekable.
        /// The given <paramref name="rule"/> doens't exist in the syntax.
        /// <paramref name="excluded"/> lists a rule that is not defined in the syntax.
        /// </exception>
        public SyntaxValidationResult Validate(Stream stream, string rule, ExcludedRules excluded)
        {
            if (excluded == null)
                throw new ArgumentNullException(nameof(excluded));

            foreach (var r in excluded)
            {
                if (Syntax.GetRuleBy(r) == null)
                    throw new ArgumentException("Rule '" + r + "' is not defined in the syntax.");
            }

            excludedRules = excluded;
            var result = Validate(stream, rule);

            excludedRules = null;

            return result;
        }

        /// <summary>
        /// Builds a validation result from all paths using the total <paramref name="length"/> of the source.
        /// </summary>
        /// <param name="length">The total length of the source.</param>
        /// <returns>The result from the validation.</returns>
        private SyntaxValidationResult BuildValidationResult(long length)
        {
            var search = (from p in paths
                          where p.ByteIndex == length &&
                              p.State == PathState.Success
                          select p).ToArray();

            switch (search.Length)
            {
                case 0:
                    var failure = (from p in paths
                                   where p.State != PathState.Parsing
                                   orderby p.Position.Index descending, p.State descending
                                   select p).FirstOrDefault();
                    
                    if (failure != null)
                        return new SyntaxValidationResult(false, failure.Message, failure.RuleTrace);

                    return new SyntaxValidationResult(false, "There is no matches.", new RuleTrace());
                case 1:
                    return new SyntaxValidationResult(true, search[0].Message, search[0].RuleTrace);
                default:
                    return new SyntaxValidationResult(false, "The syntax is ambiguous.", search[0].RuleTrace);
            }
        }

        /// <summary>
        /// Validates the source against the given <paramref name="rule"/>.
        /// </summary>
        /// <param name="rule">The rule to use for validating the source.</param>
        /// <returns>True if the source conforms to the <paramref name="rule"/>; otherwise false.</returns>
        private bool Validate(Rule rule)
        {
            var original = Path.DeepCopy();
            var temp = new List<SyntaxPath>();

            var isValid = false;
            foreach (var branch in rule.Branches)
            {
                RuleFrame frame = new RuleFrame(rule.Name, Path.Position.DeepCopy());
                Path.RuleTrace.Add(frame);

                int index = Path.RuleTrace.IndexOf(frame);

                if (Validate(branch))
                {
                    isValid = true;
                    temp.Add(Path);
                }

                if (isValid)
                    Path.RuleTrace[index].ExitPoint = Path.Position.DeepCopy();
                else
                    Path.RuleTrace[index].ErrorPoint = Path.Position.DeepCopy();

                Path = original.DeepCopy();
            }

            if (temp.Count != 0)
                Path = temp.OrderBy(x => x.Position.Index).First();

            return isValid;
        }

        /// <summary>
        /// Validates the source against the given <paramref name="list"/> of definitions.
        /// </summary>
        /// <param name="list">The list of definitions to use for validating the source.</param>
        /// <returns>True if the source conforms to the <paramref name="list"/>; otherwise false.</returns>
        private bool Validate(DefinitionList list)
        {
            var isValid = false;
            foreach (var definition in list)
            {
                if (Validate(definition))
                    isValid = true;
            }

            return isValid;
        }

        /// <summary>
        /// Validates the source against the given <paramref name="definition"/>.
        /// </summary>
        /// <param name="definition">The definition to validate.</param>
        /// <returns>True if the source conforms to the <paramref name="definition"/>; otherwise false.</returns>
        private bool Validate(SingleDefinition definition)
        {
            foreach (var term in definition.SyntacticTerms)
            {
                if (!Validate(term))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Validates the source against the given syntactic <paramref name="term"/>.
        /// </summary>
        /// <param name="term">The syntactic term to validate.</param>
        /// <returns>True if the source conforms to the syntactic <paramref name="term"/>; otherwise false.</returns>
        private bool Validate(SyntacticTerm term)
        {
            var original = Path.DeepCopy();
            if (Validate(term.Factor))
            {
                if (term.Exception == null)
                    return true;
                else
                {
                    var validated = Path.DeepCopy();
                    Path = original.DeepCopy();

                    if (Validate(term.Exception) && validated.ByteIndex == Path.ByteIndex)
                        Path = original;
                    else
                    {
                        Path = validated;

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Validates the source against the given syntactic <paramref name="factor"/>.
        /// </summary>
        /// <param name="factor">The syntactic factor to validate.</param>
        /// <returns>True if the source conforms to the syntactic <paramref name="factor"/>; otherwise false.</returns>
        private bool Validate(SyntacticFactor factor)
        {
            for (int i = 0; i < factor.NumberOfRepetitions; i++)
            {
                if (!Validate(factor.SyntacticPrimary))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Validates the source against the given <paramref name="definition"/>.
        /// </summary>
        /// <param name="definition">The definition to validate.</param>
        /// <returns>True if the source conforms to the <paramref name="definition"/>; otherwise false.</returns>
        private bool Validate(Definition definition)
        {
            return Validate((dynamic)definition);
        }

        /// <summary>
        /// Validates the source against the given <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence">The sequence to validate.</param>
        /// <returns>True if the source conforms to the <paramref name="sequence"/>; otherwise false.</returns>
        private bool Validate(GroupedSequence sequence)
        {
            var original = Path.DeepCopy();
            var temp = new List<SyntaxPath>();

            var isValid = false;
            foreach (var branch in sequence.Branches)
            {
                if (Validate(branch))
                {
                    isValid = true;
                    temp.Add(Path);
                }

                Path = original.DeepCopy();
            }

            if (temp.Count != 0)
            {
                Path = (from t in temp
                        orderby t.Position.Index descending
                        select t).First();
            }

            return isValid;
        }

        /// <summary>
        /// Validates the source against the given <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence">The sequence to validate.</param>
        /// <returns>True if the source conforms to the <paramref name="sequence"/>; otherwise false.</returns>
        private bool Validate(RepeatedSequence sequence)
        {
            var dictionary = new Dictionary<Definition, SyntaxPath>();
            foreach (var branch in sequence.Branches)
                dictionary.Add(branch, Path.DeepCopy());

            bool isValid;
            do
            {
                isValid = false;
                foreach (var pair in dictionary)
                {
                    if (pair.Value.State == PathState.Parsing)
                    {
                        if (Validate(pair.Key))
                            isValid = true;
                    }
                }
            }
            while (isValid);

            if (dictionary.Count != 0)
            {
                var temp = (from pair in dictionary
                            orderby pair.Value.Position.Index descending
                            select pair.Value).First();
                
                if (!paths.Contains(temp))
                    paths.Add(temp);
            }

            return true;
        }

        /// <summary>
        /// Validates the source against the given <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence">The sequence to validate.</param>
        /// <returns>True if the source conforms to the <paramref name="sequence"/>; otherwise false.</returns>
        private bool Validate(OptionalSequence sequence)
        {
            var original = Path.DeepCopy();
            var temp = new List<SyntaxPath>();

            foreach (var branch in sequence.Branches)
            {
                if (Validate(branch))
                    temp.Add(Path);

                Path = original.DeepCopy();
            }

            if (temp.Count != 0)
            {
                Path = (from t in temp
                        orderby t.Position.Index descending
                        select t).First();
            }

            return true;
        }

        /// <summary>
        /// Validates the source against the given <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence">The sequence to validate.</param>
        /// <returns>True if the source conforms to the <paramref name="sequence"/>; otherwise false.</returns>
        private bool Validate(SpecialSequence sequence)
        {
            ISpecialSequenceValidator selected = null;
            foreach (var validator in SpecialSequenceValidators)
            {
                if (validator.IsValid(sequence.Value))
                {
                    if (selected != null)
                    {
                        string message = "Ambiguous special sequence.";
                        Path.Message = sequence.DefinedAt.ToString(message);
                        Path.State = PathState.Error;

                        return false; 
                    }
                    else
                        selected = validator;
                }
            }

            if (selected != null)
            {
                bool characterConsumed = selected.Consume(source, sequence.Value, Path.Position);
                if (characterConsumed)
                    Path.ByteIndex = source.Position;

                return characterConsumed;
            }
            else
                return false;
        }

        /// <summary>
        /// Validates the source against the rule referenced by the given <paramref name="identifier"/>.
        /// </summary>
        /// <param name="identifier">The rule reference to validate.</param>
        /// <returns>True if the source conforms to referenced rule.</returns>
        private bool Validate(MetaIdentifier identifier)
        {
            if (includedRules != null && !includedRules.Contains(identifier.Name))
                return true;

            if (excludedRules != null && excludedRules.Contains(identifier.Name))
                return true;

            var rule = (from r in Syntax
                        where r.Name == identifier.Name
                        select r).FirstOrDefault();

            if (rule == null)
            {
                string message = "Referencing an undefined rule.";
                Path.Message = identifier.DefinedAt.ToString(message);

                return false;
            }

            return Validate(rule);
        }

        /// <summary>
        /// Validates the source against the given <paramref name="terminal"/>.
        /// </summary>
        /// <param name="terminal">The terminal to validate.</param>
        /// <returns>True if the source conforms to the <paramref name="terminal"/>.</returns>
        private bool Validate(TerminalString terminal)
        {
            return Consume(terminal.Value);
        }

        /// <summary>
        /// Consumes the given <paramref name="text"/> if it occurs 
        /// at the current position in the source.
        /// </summary>
        /// <param name="text">The text to consume.</param>
        /// <returns>True if the <paramref name="text"/> was consumed; otherwise false.</returns>
        private bool Consume(string text)
        {
            long start = source.Position;
            byte[] expected = Encoding.UTF8.GetBytes(text);
            byte[] actual = new byte[expected.Length];

            source.Read(actual, 0, actual.Length);

            if (actual.SequenceEqual(expected))
            {
                Path.ByteIndex = source.Position;
                foreach (char c in text)
                    Path.Position.Advance(c);

                return true;
            }
            else
            {
                source.Position = start;
                return false;
            }
        }
    }
}