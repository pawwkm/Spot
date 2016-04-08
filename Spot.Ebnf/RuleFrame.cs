using Pote;
using Pote.Text;
using System;

namespace Spot.Ebnf
{
    /// <summary>
    /// Provides information about a rule call.
    /// </summary>
    public sealed class RuleFrame : IDeepCopy<RuleFrame>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleFrame"/> class.
        /// </summary>
        /// <param name="rule">The name of the rule that was called.</param>
        /// <param name="entryPoint">Where the rule was called in the input.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rule"/> or <paramref name="entryPoint"/> is null.
        /// </exception>
        public RuleFrame(string rule, InputPosition entryPoint)
        {
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));
            if (entryPoint == null)
                throw new ArgumentNullException(nameof(entryPoint));

            Rule = rule;
            EntryPoint = entryPoint;
        }

        /// <summary>
        /// The name of the rule that was called.
        /// </summary>
        public string Rule
        {
            get;
            private set;
        }

        /// <summary>
        /// Where the rule was called in the input.
        /// </summary>
        public InputPosition EntryPoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Where the rule exited in the input.
        /// </summary>
        /// <remarks>Might be null.</remarks>
        public InputPosition ExitPoint
        {
            get;
            set;
        }

        /// <summary>
        /// Where an error has occurred.
        /// </summary>
        /// <remarks>Might be null.</remarks>
        public InputPosition ErrorPoint
        {
            get;
            set;
        }

        /// <summary>
        /// Deep copies all the data of the rule frame.
        /// </summary>
        /// <returns>A deep copy of the rule frame.</returns>
        public RuleFrame DeepCopy()
        {
            var frame = new RuleFrame(Rule, EntryPoint.DeepCopy());

            if (ErrorPoint != null)
                frame.ErrorPoint = ErrorPoint.DeepCopy();
            if (ExitPoint != null)
                frame.ExitPoint = ExitPoint.DeepCopy();

            return frame;
        }
    }
}