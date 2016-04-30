using Pote.CommandLine;
using System;
using System.Diagnostics;

namespace Spot
{
    /// <summary>
    /// The main class of the program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Decides what verb or command to run.
        /// </summary>
        /// <param name="args">The arguments the method uses to make a decision.</param>
        /// <returns>The exit code of the verb or command that was executed.</returns>
        private static int Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            var executor = new CommandExecutor();
            executor.AddVerb<TestVerb>()
                    .AddVerb<FuzzVerb>();

            watch.Start();
            var result = executor.Execute(args);
            watch.Stop();

            Console.WriteLine("Elapsed time: " + watch.Elapsed);

            return result;
        }
    }
}