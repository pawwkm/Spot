namespace Spot.Ebnf
{
    /// <summary>
    /// Justifications for suppressing static 
    /// analysis warnings.
    /// </summary>
    internal static class Justifications
    {
        /// <summary>
        /// It makes more sense to have the few types in this
        /// name space than to combine it with another.
        /// </summary>
        public const string NamespacesShouldRemainSeparated = "It makes more sense to have the few types in this namespace than to combine it with another.";

        /// <summary>
        /// The member name would not make sense if it was suffixed by 'collection'.
        /// </summary>
        public const string MissingCollectionSuffix = "The member name would not make sense if it was suffixed by 'collection'.";

        /// <summary>
        /// The library is not cls compliant.
        /// </summary>
        public const string NotClsCompliant = "The libary is not cls compliant.";

        /// <summary>
        /// Instance access is very likely to be needed later on.
        /// </summary>
        public const string InstanceAccessIsVeryLikelyNeeded = "Instance access is very likely to be needed later on and that change would not break any code if the code is instance based now.";

        /// <summary>
        /// The nested generics are unavoidable.
        /// </summary>
        public const string NestedGenericsAreUnavoidable = "The nested generics are unavoidable.";
    }
}