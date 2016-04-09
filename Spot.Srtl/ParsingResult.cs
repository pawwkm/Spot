using System.Collections.Generic;

namespace Spot.SrtL
{
    /// <summary>
    /// The result of parsing SrtL input.
    /// </summary>
    internal class ParsingResult
    {
        private List<Test> tests = new List<Test>();

        private List<string> errors = new List<string>();

        /// <summary>
        /// The tests found during parsing.
        /// </summary>
        public IList<Test> Tests
        {
            get
            {
                return tests;
            }
        }

        /// <summary>
        /// The errors accumulated during parsing.
        /// </summary>
        public IList<string> Errors
        {
            get
            {
                return errors;
            }
        }
    }
}
