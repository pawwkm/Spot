using System;
using System.Collections;
using System.Collections.Generic;

namespace Spot.SrtL
{
    /// <summary>
    /// A collection of SrtL tests.
    /// </summary>
    public class TestCollection : IEnumerable<Test>
    {
        private List<Test> list = new List<Test>();

        private List<string> errors = new List<string>();

        /// <summary>
        /// Error messages associated with the tests in 
        /// the collection.
        /// </summary>
        public IList<string> Errors
        {
            get
            {
                return errors;
            }
        }

        /// <summary>
        /// Adds a test to the collection.
        /// </summary>
        /// <param name="test">The test to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="test"/> is null.
        /// </exception>
        public void Add(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));

            list.Add(test);
        }

        /// <summary>
        /// Adds the <paramref name="tests"/> to this collection.
        /// </summary>
        /// <param name="tests">The collection of tests to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="tests"/> is null.
        /// </exception>
        public void Add(TestCollection tests)
        {
            if (tests == null)
                throw new ArgumentNullException(nameof(tests));

            list.AddRange(tests);
            errors.AddRange(tests.Errors);
        }

        /// <summary>
        /// Removes a test from the collection.
        /// </summary>
        /// <param name="test">The test to remove.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="test"/> is null.
        /// </exception>
        public void Remove(Test test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));

            list.Remove(test);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the tests.
        /// </summary>
        /// <returns>An enumerator that iterates through the tests.</returns>
        public IEnumerator<Test> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the tests.
        /// </summary>
        /// <returns>An enumerator that iterates through the tests.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}