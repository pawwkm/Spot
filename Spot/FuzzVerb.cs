using Pote.CommandLine;
using Spot.Ebnf;
using System;
using System.IO;

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
        [Option('s', "start-from", Help = "Start from a specific rule", Default = "")]
        public string StartFrom
        {
            get;
            set;
        }

        /// <summary>
        /// The max recursion depth of each rule.
        /// </summary>
        [Option('d', "recursion-depth", Help = "The max depth of recursion for a single rule", Default = 3)]
        public int MaxRecursionDepth
        {
            get;
            set;
        }
        
        /// <summary>
        /// The file where the output is written to.
        /// </summary>
        [Option('o', "output", Help = "The file where the fuzzy tests are written to", Default = "")]
        public string OutputFile
        {
            get;
            set;
        }

        /// <summary>
        /// The file containing the syntax definition.
        /// </summary>
        [Option(MetaName = "syntax file", Help = "The syntax to generate fuzzy tests for")]
        public string SyntaxFile
        {
            get;
            set;
        }

        /// <summary>
        /// If true the number of tests generated is displayed.
        /// </summary>
        [Option('c', "count", Help = "Displays the number of tests generated")]
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
            foreach (var g in ThirdParty.GetSpecialSequenceGenerators())
                generator.SpecialSequenceGenerators.Add(g);

            if (MaxRecursionDepth < 1)
            {
                Console.WriteLine("The max recursion depth is below 1");

                return 0;
            }
            else
                generator.RecursionDepth = MaxRecursionDepth;

            var rule = syntax.Start;
            if (StartFrom != null && StartFrom.Length != 0)
            {
                rule = syntax.GetRuleBy(StartFrom);
                if (rule == null)
                {
                    Console.WriteLine("The '{0}' rule doesn't exist in the given syntax", StartFrom);

                    return 0;
                }
            }

            if (OutputFile == "")
                OutputFile = Path.Combine(Path.GetDirectoryName(SyntaxFile), Path.GetFileNameWithoutExtension(SyntaxFile) + ".fuzz");

            ulong count = 0;
            using (FileStream stream = new FileStream(OutputFile, FileMode.Create, FileAccess.Write, FileShare.None, 4194304, FileOptions.None))
                count = generator.Generate(stream, syntax, rule);

            if (DisplayTestCount)
            {
                if (count == 0)
                    Console.WriteLine("No tests generated");
                else if (count == 1)
                    Console.WriteLine("1 test generated");
                else
                    Console.WriteLine("{0} tests generated", count);
            }

            return 0;
        }
    }
}