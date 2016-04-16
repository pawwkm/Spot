using Newtonsoft.Json;
using Pote.CommandLine;
using Spot.Ebnf;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Spot
{
    /// <summary>
    /// Generates fuzz tests for a given syntax.
    /// </summary>
    public class FuzzVerb : Verb
    {
        /// <summary>
        /// Changes the rule to start from.
        /// </summary>
        [SingleOption('s', "start-from", Help = "Start from a specific rule", Default = "")]
        public string StartFrom
        {
            get;
            set;
        }

        /// <summary>
        /// The max recursion depth of each rule.
        /// </summary>
        [SingleOption('d', "recursion-depth", Help = "The max depth of recursion for a single rule", Default = 3)]
        public int MaxRecursionDepth
        {
            get;
            set;
        }
        
        /// <summary>
        /// The file where the output is written to.
        /// </summary>
        [SingleOption('o', "output", Help = "The file where the fuzzy tests are written to", Default = "")]
        public string OutputFile
        {
            get;
            set;
        }

        /// <summary>
        /// The file containing the syntax definition.
        /// </summary>
        [FileOption(Help = "The syntax to generate fuzzy tests for", Pattern = @".*\.ebnf")]
        public string SyntaxFile
        {
            get;
            set;
        }

        /// <summary>
        /// If true the number of tests generated is displayed.
        /// </summary>
        [SingleOption('c', "count", Help = "Displays the number of tests generated")]
        public bool DisplayTestCount
        {
            get;
            set;
        }

        /// <summary>
        /// A short description of this verb. Usually a sentence summary.
        /// </summary>
        public override string Help
        {
            get
            {
                return "Generates fuzzy tests for a syntax.";
            }
        }

        /// <summary>
        /// The verb associated with the command.
        /// </summary>
        public override string Name
        {
            get
            {
                return "fuzz";
            }
        }

        /// <summary>
        /// Executes some behavior and returns an exit code.
        /// </summary>
        /// <returns>
        /// The exit code of the method. 0 means that the method executed 
        /// successfully. Non zero means something went wrong.
        /// </returns>
        public override int Execute()
        {
            var reader = new SyntaxReader();
            var syntax = reader.Read(SyntaxFile);

            var generator = new FuzzyTestGenerator();
            if (MaxRecursionDepth < 1)
            {
                Console.WriteLine("The max recursion depth is below 1");

                return 0;
            }
            else
                generator.RecursionDepth = MaxRecursionDepth;

            Collection<string> tests = null;
            if (StartFrom.Length == 0)
                tests = generator.Generate(syntax);
            else
            {
                var rule = syntax.GetRuleBy(StartFrom);
                if (rule == null)
                {
                    Console.WriteLine("The '{0}' rule doesn't exist in the given syntax.", StartFrom);

                    return 0;
                }

                tests = generator.Generate(syntax, rule);
            }

            if (OutputFile == "")
                OutputFile = Path.Combine(Path.GetDirectoryName(SyntaxFile), Path.GetFileNameWithoutExtension(SyntaxFile) + ".fuzz.json");

            using (FileStream stream = File.Open(OutputFile, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    using (JsonWriter jw = new JsonTextWriter(writer))
                    {
                        jw.Formatting = Formatting.None;

                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(jw, tests);
                    }
                }
            }

            if (DisplayTestCount)
            {
                if (tests.Count == 0)
                    Console.WriteLine("No tests generated");
                else if (tests.Count == 1)
                    Console.WriteLine("1 test generated");
                else
                    Console.WriteLine("{0} tests generated", tests.Count);
            }

            return 0;
        }
    }
}