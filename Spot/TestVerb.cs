using Pote.CommandLine;
using Spot.Ebnf;
using Spot.SrtL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Spot
{
    /// <summary>
    /// Performs tests on a syntax using SrtL.
    /// </summary>
    internal class TestVerb : Verb
    {
        [Option(MetaName = "syntax", IsRequired = true, Help = "Path to the ebnf syntax to test", Default = new string[0])]
        [ExtensionValidator("ebnf")]
        public IEnumerable<string> Syntaxes
        {
            get;
            set;
        }

        [Option(MetaName = "tests", IsRequired = true, Help = "The SrtL file paths", Default = new string[0])]
        [ExtensionValidator("srtl", IgnoreNonFilePaths = true)]
        [FileInterceptor("srtl")]
        public IEnumerable<string> Tests
        {
            get;
            set;
        }

        /// <summary>
        /// Description of how to use the test verb.
        /// </summary>
        public override string Help
        {
            get
            {
                return "Tests a syntax against a set of SrtL tests";
            }
        }

        /// <summary>
        /// The name of the verb.
        /// </summary>
        public override string Name
        {
            get
            {
                return "test";
            }
        }

        /// <summary>
        /// Executes the test verb.
        /// </summary>
        /// <returns>The exit code of the verb.</returns>
        public override int Execute()
        {
            var grammar = "";
            foreach (var file in Syntaxes)
            {
                if (grammar != "")
                {
                    Console.WriteLine("Multiple .ebnf files specified.");

                    return 0;
                }

                if (!File.Exists(file))
                {
                    Console.WriteLine("The file '" + file + "' doesn't exist.");

                    return 0;
                }

                grammar = file;
            }

            var tests = new List<string>();
            foreach (var test in Tests.Distinct())
            {
                if (!File.Exists(test))
                {
                    Console.WriteLine("The file '" + test + "' doesn't exist.");

                    return 0;
                }

                tests.Add(test);
            }

            var reader = new SyntaxReader();
            var syntax = reader.Read(grammar);

            var srtLReader = new SrtLReader();
            var testCollection = srtLReader.Read(tests);

            if (testCollection.Errors.Count != 0)
            {
                foreach (var error in testCollection.Errors)
                    Console.WriteLine(error);

                return 0;
            }

            var runner = new TestRunner(Console.Out, ThirdParty.GetSpecialSequenceValidators());
            runner.Run(testCollection, syntax);

            return 0;
        }
    }
}