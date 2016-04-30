using Pote;
using Pote.Text;
using System;

namespace Spot.SrtL
{
    /// <summary>
    /// The parser for SrtL.
    /// </summary>
    internal sealed class Parser
    {
        private TestCollection result;

        private LexicalAnalyzer<TokenType> analyzer;

        /// <summary>
        /// Parses SrtL input and turns it into <see cref="Test"/>.
        /// </summary>
        /// <param name="source">The lexical analyzer that provides the tokens of the syntax.</param>
        /// <returns>The result of parsing the <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.
        /// </exception>
        public TestCollection Parse(LexicalAnalyzer<TokenType> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            analyzer = source;
            result = new TestCollection();

            while (!source.EndOfInput)
                Test();

            return result;
        }

        /// <summary>
        /// Parses a whole test.
        /// </summary>
        private void Test()
        {
            IssueErrorUntil("test", "Expected 'test' keyword.");

            var token = analyzer.LookAhead();
            if (token.Type == TokenType.EndOfInput)
                return;

            analyzer.Next();

            Test test = new Test();
            test.DefinedAt = token.Position;
            if (analyzer.LookAhead().Text == "description")
                test.Description = Description();

            test.Input = Input();
            test.Validity = Validity();

            result.Add(test);
        }

        /// <summary>
        /// Parses the input of a test.
        /// </summary>
        /// <returns>The input of a test.</returns>
        private Input Input()
        {
            IssueErrorUntil("input", "Expected 'input' keyword.");

            var token = analyzer.LookAhead();
            if (token.Type != TokenType.EndOfInput)
            {
                Input input = new Input();
                input.DefinedAt = analyzer.Next().Position;
                input.Contents = ConcatenatedString();

                return input;
            }

            return new Input();
        }

        private Description Description()
        {
            IssueErrorUntil("description", "Expected 'description' keyword.");

            return new Description(analyzer.Next().Position, ConcatenatedString());
        }

        /// <summary>
        /// Parses the validity of a test.
        /// </summary>
        /// <returns>
        /// True if the test is declared to be valid; 
        /// otherwise false
        /// </returns>
        private Validity Validity()
        {
            IssueErrorUntil("is", "Expected 'is' keyword.");

            var token = analyzer.Next();
            if (token.Type == TokenType.EndOfInput)
            {
                result.Errors.Add(token.Position.ToString("Unexpected end of input."));
                return new Validity();
            }

            var validity = new Validity();
            validity.DefinedAt = token.Position;
            validity.IsValid = true;

            token = analyzer.LookAhead();
            if (token.Text == "not")
            {
                analyzer.Next();
                validity.IsValid = false;
            }

            IssueErrorUntil("valid", "Expected 'valid' keyword.");

            token = analyzer.Next();
            if (token.Type == TokenType.EndOfInput)
            {
                result.Errors.Add(token.Position.ToString("Unexpected end of input."));
                return new Validity();
            }

            return validity;
        }

        /// <summary>
        /// Parses a concatenated string.
        /// </summary>
        /// <returns>The concatenated string.</returns>
        private ConcatenatedString ConcatenatedString()
        {
            IssueErrorUntil(TokenType.String, "Expected a concatenated string.");

            var token = analyzer.LookAhead();
            if (token.Type == TokenType.EndOfInput)
            {
                result.Errors.Add(token.Position.ToString("Unexpected end of input."));
                return new ConcatenatedString();
            }

            var strings = new ConcatenatedString();
            strings.Strings.Add(String());

            while (analyzer.LookAhead().Type == TokenType.String)
                strings.Strings.Add(String());

            return strings;
        }

        /// <summary>
        /// Parses a single string.
        /// </summary>
        /// <returns>The parsed string.</returns>
        private String String()
        {
            var token = analyzer.Next();
            if (token.Type != TokenType.String)
            {
                result.Errors.Add(token.Position.ToString("Expected a string."));
                return new String();
            }

            var s = new String();
            s.Content = token.Text;
            s.DefinedAt = token.Position;

            return s;
        }

        /// <summary>
        /// Issues the given <paramref name="error"/> until 
        /// a token's text matches the given <paramref name="text"/>.
        /// To ensure this methods doesn't run to far, it will stop 
        /// at the next token that is the 'test' keyword.
        /// </summary>
        /// <param name="text">The text to match against tokens.</param>
        /// <param name="error">The error to issue.</param>
        private void IssueErrorUntil(string text, string error)
        {
            var token = analyzer.LookAhead();
            if (token.Type == TokenType.EndOfInput)
                return;

            while (!token.Text.IsOneOf(text, "test"))
            {
                if (token.Type == TokenType.EndOfInput)
                    return;

                result.Errors.Add(token.Position.ToString(error));

                analyzer.Next();
                token = analyzer.LookAhead();
            }
        }

        /// <summary>
        /// Issues the given <paramref name="error"/> until 
        /// a token's type matches the given <paramref name="type"/>.
        /// To ensure this methods doesn't run to far, it will stop 
        /// at the next token that is the 'test' keyword.
        /// </summary>
        /// <param name="type">The type to match against tokens.</param>
        /// <param name="error">The error to issue.</param>
        private void IssueErrorUntil(TokenType type, string error)
        {
            var token = analyzer.LookAhead();
            if (token.Type == TokenType.EndOfInput)
                return;

            while (token.Type != type && token.Text != "test")
            {
                if (token.Type == TokenType.EndOfInput)
                    return;

                result.Errors.Add(token.Position.ToString(error));

                analyzer.Next();
                token = analyzer.LookAhead();
            }
        }
    }
}