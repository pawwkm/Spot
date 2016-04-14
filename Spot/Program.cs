using Pote.CommandLine;

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
        /// <param name="args">The arguments the method uses to make a desision.</param>
        /// <returns>The exit code of the verb or command that was executed.</returns>
        private static int Main(string[] args)
        {
            var executor = new CommandExecutor();
            executor.AddVerb<TestVerb>()
                    .AddVerb<FuzzVerb>();

            return executor.Execute(args);
        }
    }
}