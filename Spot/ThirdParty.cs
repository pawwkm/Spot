using Spot.Ebnf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Spot
{
    /// <summary>
    /// Provides to third party implementations.
    /// </summary>
    internal static class ThirdParty
    {
        private static readonly string ThirdPartyFolder = "ThirdParty";

        private static List<ISpecialSequenceValidator> validators;

        /// <summary>
        /// Gets all third party implementations of the <see cref="ISpecialSequenceValidator"/> interface.
        /// </summary>
        /// <returns>All third party implementations of the <see cref="ISpecialSequenceValidator"/> interface.</returns>
        public static IEnumerable<ISpecialSequenceValidator> GetSpecialSequenceValidators()
        {
            if (validators == null)
            {
                validators = new List<ISpecialSequenceValidator>();
                foreach (var path in Directory.GetFiles(ThirdPartyFolder, "*.dll", SearchOption.AllDirectories))
                {
                    var assembly = Assembly.LoadFile(Path.GetFullPath(path));
                    var types = assembly.GetTypes().Select(t => t).Where(t => typeof(ISpecialSequenceValidator).IsAssignableFrom(t) && t.GetConstructor(Type.EmptyTypes) != null);

                    foreach (var type in types)
                        validators.Add((ISpecialSequenceValidator)Activator.CreateInstance(type));
                }
            }

            return validators;
        }
    }
}