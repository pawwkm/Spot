using Spot.Ebnf;
using System;
using System.Collections.Generic;
using System.IO;

namespace Spot.SrtL
{
    /// <summary>
    /// Runs a syntax against tests.
    /// </summary>
    public class TestRunner
    {
        private TextWriter output;

        private SyntaxValidator validator;

        private List<ISpecialSequenceValidator> specialValidators;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestRunner"/> class.
        /// </summary>
        /// <param name="writer">The output to write to.</param>
        /// <param name="validators">
        /// The special sequence validators that are available to the test runner.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="writer"/> or <paramref name="validators"/> is null.
        /// </exception>
        public TestRunner(TextWriter writer, IEnumerable<ISpecialSequenceValidator> validators)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            if (validators == null)
                throw new ArgumentNullException(nameof(validators));

            output = writer;
            specialValidators = new List<ISpecialSequenceValidator>(validators);
        }

        /// <summary>
        /// Tests a set of rules against a syntax.
        /// </summary>
        /// <param name="tests">The tests to run against a syntax.</param>
        /// <param name="syntax">The syntax to run the tests against.</param>
        public void Run(TestCollection tests, Syntax syntax)
        {
            if (tests == null)
                throw new ArgumentNullException(nameof(tests));

            if (tests.Errors.Count != 0)
                throw new ArgumentException("The tests have errors. Check " + nameof(tests.Errors), nameof(tests));

            validator = new SyntaxValidator(syntax);
            foreach (var v in specialValidators)
                validator.SpecialSequenceValidators.Add(v);

            foreach (var t in tests)
            {
                var result = Validate(t);
                if (t.Validity.IsValid != result.IsSyntaxValid)
                {
                    output.WriteLine(t.DefinedAt.ToString("The test's validity assumption doesn't match the syntax."));
                    output.WriteLine("Rule trace:");

                    foreach (var frame in result.RuleTrace)
                    {
                        if (frame.ExitPoint == null)
                            output.WriteLine("\tRule \"{0}\" entered at {1} ecountered an error at {2}", frame.Rule, frame.EntryPoint, frame.ErrorPoint);
                        else
                            output.WriteLine("\tRule \"{0}\" entered at {1} and exited at {2}", frame.Rule, frame.EntryPoint, frame.ExitPoint);
                    }
                }
            }
        }

        /// <summary>
        /// Calls the appropriate method on the validator.
        /// </summary>
        /// <param name="test">The test to run.</param>
        /// <returns>The result of the validation.</returns>
        private SyntaxValidationResult Validate(Test test)
        {
            var rule = validator.Syntax.Start.Name;
            if (test.StartFrom != null)
                rule = test.StartFrom.Rule.Content;

            var input = test.Input.Contents.Concatenate();
            if (test.ExcludingAllRules != null)
                return validator.Validate(input, rule, new Ebnf.IncludedRules());
            else if (test.IncludedRules != null)
                return validator.Validate(input, rule, new Ebnf.IncludedRules(test.IncludedRules.Rules.ToRawStrings()));
            else if (test.ExcludedRules != null)
                return validator.Validate(input, rule, new Ebnf.ExcludedRules(test.ExcludedRules.Rules.ToRawStrings()));
            else
                return validator.Validate(input, rule);
        }
    }
}