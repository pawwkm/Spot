using Pote.Text;
using System;

namespace Spot.SrtL
{
    /// <summary>
    /// A test defined in SrtL.
    /// </summary>
    public sealed class Test
    {
        private Input input = new Input();

        private Validity validity = new Validity();

        private IncludedRules includedRules;

        private ExcludingAllRules excludingAllRules;

        private ExcludedRules excludedRules;

        private InputPosition definedAt = new InputPosition();

        /// <summary>
        /// The description of the test. 
        /// If the test doesn't have a description, this is null.
        /// </summary>
        public Description Description
        {
            get;
            set;
        }

        /// <summary>
        /// The rule to start the test from. 
        /// If null, the tests starts from the rule dictated by the meta syntax.
        /// </summary>
        public StartingPoint StartFrom
        {
            get;
            set;
        }

        /// <summary>
        /// The input to test against a given syntax.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public Input Input
        {
            get
            {
                return input;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                input = value;
            }
        }

        /// <summary>
        /// If null, the test have not been defined with the 
        /// "exclude all rules" keywords.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <see cref="IncludedRules"/> and <see cref="ExcludedRules"/> 
        /// must be null before setting this to a non-null value.
        /// </exception>
        public ExcludingAllRules ExcludingAllRules
        {
            get
            {
                return excludingAllRules;
            }
            set
            {
                if (IncludedRules != null && value != null)
                    throw new InvalidOperationException(nameof(IncludedRules) + " must be null before setting this to a non-null value.");
                if (ExcludedRules != null && value != null)
                    throw new InvalidOperationException(nameof(ExcludedRules) + " must be null before setting this to a non-null value.");

                excludingAllRules = value;
            }
        }

        /// <summary>
        /// If null, the test have not been defined with a
        /// set of excluded rules.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <see cref="ExcludingAllRules"/> and <see cref="IncludedRules"/> 
        /// must be null before setting this to a non-null value.
        /// </exception>
        public ExcludedRules ExcludedRules
        {
            get
            {
                return excludedRules;
            }
            set
            {
                if (IncludedRules != null &&  value != null)
                    throw new InvalidOperationException(nameof(IncludedRules) + " must be null before setting this to a non-null value.");
                if (ExcludingAllRules != null && value != null)
                    throw new InvalidOperationException(nameof(ExcludingAllRules) + " must be null before setting this to a non-null value.");

                excludedRules = value;
            }
        }

        /// <summary>
        /// If null, the test have not been defined with a
        /// set of included rules.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <see cref="ExcludingAllRules"/> and <see cref="ExcludedRules"/> 
        /// must be null before setting this to a non-null value.
        /// </exception>
        public IncludedRules IncludedRules
        {
            get
            {
                return includedRules;
            }
            set
            {
                if (ExcludingAllRules != null && value != null)
                    throw new InvalidOperationException(nameof(ExcludingAllRules) + " must be null before setting this to a non-null value.");
                if (ExcludedRules != null && value != null)
                    throw new InvalidOperationException(nameof(ExcludedRules) + " must be null before setting this to a non-null value.");

                includedRules = value;
            }
        }

        /// <summary>
        /// The validity of this test's <see cref="Input"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public Validity Validity
        {
            get
            {
                return validity;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                validity = value;
            }
        }

        /// <summary>
        /// The location where the test was defined.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public InputPosition DefinedAt
        {
            get
            {
                return definedAt;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                definedAt = value;
            }
        }
    }
}