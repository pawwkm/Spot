using Pote.Text;
using System;

namespace Spot.SrtL
{
    /// <summary>
    /// Declares the validity of a test's input.
    /// </summary>
    public class Validity
    {
        private InputPosition definedAt = new InputPosition();

        /// <summary>
        /// If true the test that this is associated with 
        /// is assumed to provide input that is valid for 
        /// a given syntax. If false the input is assumed
        /// to be invalid for a given syntax.
        /// </summary>
        public bool IsValid
        {
            get;
            set;
        }

        /// <summary>
        /// The location where the validity was defined.
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