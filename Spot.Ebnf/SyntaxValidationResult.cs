using System;

namespace Spot.Ebnf
{
    /// <summary>
    /// The result of a syntax validation
    /// </summary>
    public sealed class SyntaxValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxValidationResult"/> class.
        /// </summary>
        /// <param name="isSyntaxValid">True if the syntax is valid</param>
        /// <param name="message">A message that describes the outcome of the validation.</param>
        /// <param name="ruleTrace">The rule trace of the validation.</param>
        public SyntaxValidationResult(bool isSyntaxValid, string message, RuleTrace ruleTrace)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (ruleTrace == null)
                throw new ArgumentNullException(nameof(ruleTrace));

            IsSyntaxValid = isSyntaxValid;
            Message = message;
            RuleTrace = ruleTrace;
        }

        /// <summary>
        /// True if the syntax is valid.
        /// </summary>
        public bool IsSyntaxValid
        {
            get;
            private set;
        }

        /// <summary>
        /// A message that describes the outcome of the validation.
        /// </summary>
        public string Message
        {
            get;
            private set;
        }

        /// <summary>
        /// The rule trace of the validation.
        /// </summary>
        public RuleTrace RuleTrace
        {
            get;
            private set;
        }
    }
}