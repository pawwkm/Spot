using Pote;
using Pote.Text;
using System;
using System.Linq;

namespace Spot.Ebnf
{
    /// <summary>
    /// Tracks a specific path taken by the validator.
    /// </summary>
    internal sealed class SyntaxPath : IDeepCopy<SyntaxPath>
    {
        private InputPosition position = new InputPosition();

        private RuleTrace ruleTrace = new RuleTrace();

        private string message = "";

        /// <summary>
        /// The message describing the status of this path.
        /// </summary>
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                message = value;
            }
        }

        /// <summary>
        /// The current position in the source measured by characters.
        /// </summary>
        public InputPosition Position
        {
            get
            {
                return position;
            }
        }

        /// <summary>
        /// The raw position in the stream measured in bytes.
        /// </summary>
        public long ByteIndex
        {
            get;
            set;
        }

        /// <summary>
        /// The rule trace of the validation.
        /// </summary>
        public RuleTrace RuleTrace
        {
            get
            {
                return ruleTrace;
            }
        }

        /// <summary>
        /// The parsing state of the path.
        /// </summary>
        public PathState State
        {
            get;
            set;
        }

        /// <summary>
        /// Determines if the <paramref name="left"/> hand side is equal to the 
        /// <paramref name="right"/> hand side.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>True if the two paths are equal.</returns>
        public static bool operator ==(SyntaxPath left, SyntaxPath right)
        {
            if (object.ReferenceEquals(left, right))
                return true;

            if ((object)left == null || (object)right == null)
                return false;

            if (left.ByteIndex != right.ByteIndex)
                return false;
            if (left.Message != right.Message)
                return false;
            if (left.Position != right.Position)
                return false;
            if (!left.RuleTrace.SequenceEqual(right.RuleTrace))
                return false;
            if (left.State != right.State)
                return false;

            return true;
        }

        /// <summary>
        /// Determines if the <paramref name="left"/> hand side is not equal to the 
        /// <paramref name="right"/> hand side.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>True if the two paths are not equal.</returns>
        public static bool operator !=(SyntaxPath left, SyntaxPath right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Creates a deep copy of the path.
        /// </summary>
        /// <returns>The copy of the path.</returns>
        public SyntaxPath DeepCopy()
        {
            return new SyntaxPath()
            {
                Message = this.Message,
                ByteIndex = this.ByteIndex,
                position = this.position.DeepCopy(),
                ruleTrace = this.RuleTrace.DeepCopy()
            };
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SyntaxPath path = obj as SyntaxPath;
            if ((object)path == null)
                return false;

            return this == path;
        }

        /// <summary>
        /// Returns a hash code for the path.
        /// </summary>
        /// <returns>A hash code for the path.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 0;
                hash += 17 * this.ByteIndex.GetHashCode();
                hash += 17 * this.Message.GetHashCode();
                hash += 17 * this.Position.GetHashCode();
                hash += 17 * this.State.GetHashCode();

                return hash;
            }
        }
    }
}