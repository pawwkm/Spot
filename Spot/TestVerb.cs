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
        private string grammar = "";

        private List<string> tests = new List<string>();

        /// <summary>
        /// The files handed to the verb.
        /// </summary>
        [RangedOption("files", Help = "The .srtl files and a .ebnf file.", Default = new string[] { })]
        public IEnumerable<string> Files
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
                return "";
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
            var code = GetFiles();
            if (code != 0)
                return code;

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

            var runner = new TestRunner(Console.Out);
            runner.Run(testCollection, syntax);

            return 0;
        }

        /// <summary>
        /// Extracts the paths from <see cref="Files"/>.
        /// </summary>
        /// <returns>Zero if the paths are valid; otherwise non-zero.</returns>
        private int GetFiles()
        {
            foreach (var file in Files)
            {
                if (Path.GetExtension(file) == ".ebnf")
                {
                    if (grammar == "")
                        grammar = file;
                    else
                    {
                        Console.WriteLine("Multiple .ebnf files specified.");

                        return 1;
                    }
                }
                else if (Path.GetExtension(file) == ".srtl")
                {
                    var files = Directory.GetFiles(Directory.GetCurrentDirectory(), file, SearchOption.AllDirectories);
                    foreach (var f in files)
                    {
                        if (tests.Contains(f))
                        {
                            Console.WriteLine("The file '" + file + "' has already been added.");

                            return 1;
                        }
                        else
                            tests.Add(f);
                    }
                }
                else
                {
                    Console.WriteLine("The file '" + file + "' is not a syntax or .srtl file.");

                    return 1;
                }
            }

            if (grammar == "")
            {
                var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.ebnf", SearchOption.AllDirectories).ToList();
                if (files.Count == 0)
                {
                    Console.WriteLine("No .ebnf file was found in the current directory or in its sub directories.");

                    return 1;
                }
                else if (files.Count > 1)
                {
                    Console.WriteLine("Multiple .ebnf files where found.");

                    return 1;
                }

                grammar = files[0];
            }

            if (tests.Count == 0)
            {
                tests = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.srtl", SearchOption.AllDirectories).ToList();
                if (tests.Count == 0)
                {
                    Console.WriteLine("No .srtl files was found in the current directory or in its sub directories.");

                    return 1;
                }
            }

            if (!File.Exists(grammar))
            {
                Console.WriteLine("The file '" + grammar + "' doesn't exist.");

                return 1;
            }

            for (var i = 0; i < tests.Count; i++)
            {
                if (tests[i].StartsWith(Directory.GetCurrentDirectory()))
                    tests[i] = tests[i].Substring(Directory.GetCurrentDirectory().Length + 1);

                if (!File.Exists(tests[i]))
                {
                    Console.WriteLine("The file '" + tests[i] + "' doesn't exist.");

                    return 1;
                }
            }

            return 0;
        }
    }
}