using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Spot.Ebnf
{
    /// <summary>
    /// A deﬁnitions-list consists of an ordered list of one or
    /// more single-deﬁnitions separated from each other by a
    /// deﬁnition separator symbol.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This maps to an Ebnf syntax rule.")]
    public sealed class DefinitionList : List<Definition>
    {
    }
}