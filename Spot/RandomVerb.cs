using Pote.CommandLine;
using Spot.Ebnf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spot
{
    /// <summary>
    /// Generates a random sentence from a grammar.
    /// </summary>
    internal class RandomVerb : Verb
    {
        [Option(MetaName = "syntax", IsRequired = true, Help = "Path to the ebnf syntax to test", Default = new string[0])]
        [ExtensionValidator("ebnf")]
        public IEnumerable<string> Syntaxes
        {
            get;
            set;
        }

        [Option(IsRequired = true, Help = "The file to save the generated sentence.")]
        public string Destination
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
                return "Generates a random sentence from a grammar";
            }
        }

        /// <summary>
        /// The name of the verb.
        /// </summary>
        public override string Name
        {
            get
            {
                return "random";
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

            try
            {
                var reader = new SyntaxReader();
                var syntax = reader.Read(grammar);

                var random = new RandomPass();
                random.Visit(syntax);

                File.WriteAllText(Destination, random.Sentence);
            }
            catch (SyntaxException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return 0;
        }
    }
}
