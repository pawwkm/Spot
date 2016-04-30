using Pote.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace Spot.Ebnf
{
    /// <summary>
    /// Reads syntax definitions specified using iso ebnf.
    /// </summary>
    public sealed class SyntaxReader
    {
        /// <summary>
        /// Reads the syntax defined in the <paramref name="file"/>.
        /// </summary>
        /// <param name="file">The path to the file that is to be read.</param>
        /// <returns>The read syntax.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="file"/> is null.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// <paramref name="file"/> doesn't exist.
        /// </exception>
        /// <exception cref="SyntaxException">
        /// There is one or more errors in the syntax.
        /// </exception>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = Justifications.InstanceAccessIsVeryLikelyNeeded)]
        public Syntax Read(string file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            if (!File.Exists(file))
                throw new FileNotFoundException("The file doesn't seem to exist.", file);

            using (Stream source = File.OpenRead(file))
                return Read(source, file);
        }

        /// <summary>
        /// Reads the syntax defined in the <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source of the defined syntax.</param>
        /// <returns>The read syntax.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.
        /// </exception>
        /// <exception cref="SyntaxException">
        /// There is one or more errors in the syntax.
        /// </exception>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = Justifications.InstanceAccessIsVeryLikelyNeeded)]
        public Syntax Read(Stream source)
        {
            return Read(source, "");
        }

        /// <summary>
        /// Reads the syntax defined in the <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source of the defined syntax.</param>
        /// <param name="origin">
        /// The origin of the <paramref name="source"/>.
        /// This is used to give more detailed information if errors occur.
        /// </param>
        /// <returns>The read syntax.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="origin"/> is null.
        /// </exception>
        /// <exception cref="SyntaxException">
        /// There is one or more errors in the syntax.
        /// </exception>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = Justifications.InstanceAccessIsVeryLikelyNeeded)]
        public Syntax Read(Stream source, string origin)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            using (StreamReader reader = new StreamReader(source))
            {
                LexicalAnalyzer analyzer = new LexicalAnalyzer(reader, origin);
                Parser parser = new Parser();

                Syntax syntax;
                try
                {
                    syntax = parser.Parse(analyzer);

                    RuleReferenceResolver resolver = new RuleReferenceResolver();
                    resolver.Resolve(syntax);
                }
                catch (ParsingException exception)
                {
                    throw new SyntaxException(exception.Message);
                }

                LeftRecursionChecker checker = new LeftRecursionChecker();
                List<Rule> leftRecursions = checker.Check(syntax).ToList();

                if (leftRecursions.Count != 0)
                {
                    StringBuilder builder = new StringBuilder();
                    foreach (Rule rule in leftRecursions)
                    {
                        string message = rule.Name + " is recursive.";
                        message = rule.DefinedAt.ToString(message);

                        builder.AppendLine(message);
                    }

                    throw new SyntaxException(builder.ToString());
                }

                return syntax;
            }
        }
    }
}