using Pote;
using Pote.Text;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Spot.Ebnf.Unicode
{
    /// <summary>
    /// Parser for the unicode special sequence dsl.
    /// </summary>
    internal sealed class UnicodeSequenceParser
    {
        private static readonly Dictionary<string, UnicodeCategory> Dictionary = new Dictionary<string, UnicodeCategory>()
        {
            { "cc", UnicodeCategory.Control },
            { "cf", UnicodeCategory.Format },
            { "cn", UnicodeCategory.OtherNotAssigned },
            { "co", UnicodeCategory.PrivateUse },
            { "cs", UnicodeCategory.Surrogate },
            { "ll", UnicodeCategory.LowercaseLetter },
            { "lm", UnicodeCategory.ModifierLetter },
            { "lo", UnicodeCategory.OtherLetter },
            { "lt", UnicodeCategory.TitlecaseLetter },
            { "lu", UnicodeCategory.UppercaseLetter },
            { "mc", UnicodeCategory.SpacingCombiningMark },
            { "me", UnicodeCategory.EnclosingMark },
            { "mn", UnicodeCategory.NonSpacingMark },
            { "nd", UnicodeCategory.DecimalDigitNumber },
            { "nl", UnicodeCategory.LetterNumber },
            { "no", UnicodeCategory.OtherNumber },
            { "pc", UnicodeCategory.ConnectorPunctuation },
            { "pd", UnicodeCategory.DashPunctuation },
            { "pe", UnicodeCategory.ClosePunctuation },
            { "pf", UnicodeCategory.FinalQuotePunctuation },
            { "pi", UnicodeCategory.InitialQuotePunctuation },
            { "po", UnicodeCategory.OtherPunctuation },
            { "ps", UnicodeCategory.OpenPunctuation },
            { "sc", UnicodeCategory.CurrencySymbol },
            { "sk", UnicodeCategory.ModifierSymbol },
            { "sm", UnicodeCategory.MathSymbol },
            { "so", UnicodeCategory.OtherSymbol },
            { "zi", UnicodeCategory.LineSeparator },
            { "zp", UnicodeCategory.ParagraphSeparator },
            { "zs", UnicodeCategory.SpaceSeparator }
        };

        private UnicodeSequenceLexicalAnalyzer analyzer;

        private UnicodeSequence sequence;

        /// <summary>
        /// Parses a unicode special sequence.
        /// </summary>
        /// <param name="text">The sequence to parse.</param>
        /// <returns>The parsed sequence.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="text"/> is null.
        /// </exception>
        public UnicodeSequence Parse(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            analyzer = new UnicodeSequenceLexicalAnalyzer(text);
            sequence = new UnicodeSequence();

            Sequence();

            return sequence;
        }

        /// <summary>
        /// Parses a unicode special sequence.
        /// </summary>
        private void Sequence()
        {
            Token<TokenType> token = analyzer.LookAhead();
            if (token.Text == "Unicode")
                SomeElementsExcept();
            else if (token.Text == "All")
                AnyElementExcept();
            else
                sequence.IsValidSequence = false;

            if (analyzer.LookAhead().Type != TokenType.EndOfInput)
                sequence.IsValidSequence = false;
        }

        /// <summary>
        /// Parses a sequences that allows some elements
        /// with possible exceptions.
        /// </summary>
        private void SomeElementsExcept()
        {
            Token<TokenType> token = analyzer.Next();
            if (token.Text != "Unicode")
            {
                sequence.IsValidSequence = false;
                return;
            }

            Elements(false);

            token = analyzer.LookAhead();
            if (token.Text == "except")
                Except();
        }

        /// <summary>
        /// Parses a sequences that allows all elements
        /// with possible exceptions.
        /// </summary>
        private void AnyElementExcept()
        {
            Token<TokenType> token = analyzer.Next();
            if (token.Text != "All")
            {
                sequence.IsValidSequence = false;
                return;
            }

            token = analyzer.Next();
            if (token.Text != "Unicode")
            {
                sequence.IsValidSequence = false;
                return;
            }

            token = analyzer.Next();
            if (token.Text != "characters")
            {
                sequence.IsValidSequence = false;
                return;
            }

            foreach (UnicodeCategory category in Enum.GetValues(typeof(UnicodeCategory)))
                sequence.Categories.Add(category);

            token = analyzer.LookAhead();
            if (token.Text == "except")
                Except();
        }

        /// <summary>
        /// Parses the exception clause.
        /// </summary>
        private void Except()
        {
            Token<TokenType> token = analyzer.Next();
            if (token.Text != "except")
                sequence.IsValidSequence = false;
            else
                Elements(true);
        }

        /// <summary>
        /// Parses the class and character elements of a sequence.
        /// </summary>
        /// <param name="excludeElements">
        /// If true the parsed elements are not allowed by the sequence;
        /// otherwise the elements are allowed. 
        /// </param>
        private void Elements(bool excludeElements)
        {
            Token<TokenType> token = analyzer.Next();
            if (token.Text.IsOneOf("classes", "class"))
            {
                if (token.Text == "class")
                    Class(excludeElements);
                else
                    Classes(excludeElements);

                if (!sequence.IsValidSequence)
                    return;

                if (analyzer.LookAhead().Text == "and")
                {
                    analyzer.Next();
                    token = analyzer.LookAhead();

                    if (token.Text == "characters")
                    {
                        analyzer.Next();
                        Characters(excludeElements);
                    }
                    else if (token.Text == "character")
                    {
                        analyzer.Next();
                        Character(excludeElements);
                    }
                }
            }
            else if (token.Text == "characters")
                Characters(excludeElements);
            else if (token.Text == "character")
                Character(excludeElements);
            else
                sequence.IsValidSequence = false;
        }

        /// <summary>
        /// Parses the next set of classes.
        /// </summary>
        /// <param name="excludeClasses">
        /// If true the parsed classes are not allowed by the sequence;
        /// otherwise the classes are allowed. 
        /// </param>
        private void Classes(bool excludeClasses)
        {
            Class(excludeClasses);
            int count = 1;

            Token<TokenType> token = analyzer.LookAhead();
            while (token.Text == ",")
            {
                analyzer.Next();

                Class(excludeClasses);
                count++;

                token = analyzer.LookAhead();
                if (!sequence.IsValidSequence)
                    return;
            }

            if (token.Text != "and")
                sequence.IsValidSequence = true;

            analyzer.Next();

            Class(excludeClasses);
            if (!sequence.IsValidSequence)
                return;

            count++;
            if (count <= 1)
                sequence.IsValidSequence = false;
        }

        /// <summary>
        /// Parses the next class.
        /// </summary>
        /// <param name="excludeClass">
        /// If true the parsed class is not allowed by the sequence;
        /// otherwise the class is allowed.
        /// </param>
        private void Class(bool excludeClass)
        {
            Token<TokenType> token = analyzer.Next();
            if (token.Type != TokenType.ClassLiteral)
            {
                sequence.IsValidSequence = false;
                return;
            }

            UnicodeCategory category = Dictionary[token.Text];
            if (excludeClass)
            {
                sequence.Categories.Remove(category);

                List<char> list = new List<char>();
                foreach (char c in sequence.Characters)
                {
                    if (char.GetUnicodeCategory(c) == category)
                        list.Add(c);
                }

                foreach (char c in list)
                    sequence.Characters.Remove(c);
            }
            else if (!sequence.Categories.Contains(category))
                sequence.Categories.Add(category);
        }

        /// <summary>
        /// Parses the next set of characters.
        /// </summary>
        /// <param name="excludeCharacters">
        /// If true the parsed characters are not allowed by the sequence;
        /// otherwise the character are allowed. 
        /// </param>
        private void Characters(bool excludeCharacters)
        {
            Character(excludeCharacters);
            int count = 1;

            Token<TokenType> token = analyzer.LookAhead();
            while (token.Text == ",")
            {
                analyzer.Next();

                Character(excludeCharacters);
                count++;

                token = analyzer.LookAhead();
                if (!sequence.IsValidSequence)
                    return;
            }

            if (token.Text != "and")
                sequence.IsValidSequence = true;

            analyzer.Next();

            Character(excludeCharacters);
            if (!sequence.IsValidSequence)
                return;

            count++;
            if (count <= 1)
                sequence.IsValidSequence = false;
        }

        /// <summary>
        /// Parses the next character.
        /// </summary>
        /// <param name="excludeCharacter">
        /// If true the parsed character is not allowed by the sequence;
        /// otherwise the character is allowed.
        /// </param>
        private void Character(bool excludeCharacter)
        {
            Token<TokenType> token = analyzer.Next();
            if (token.Type != TokenType.Character)
            {
                sequence.IsValidSequence = false;
                return;
            }

            char parsedCharecter = (char)Convert.ToInt32(token.Text.Substring(2), 16);
            if (excludeCharacter)
            {
                UnicodeCategory category = char.GetUnicodeCategory(parsedCharecter);
                sequence.Characters.Remove(parsedCharecter);

                if (sequence.Categories.Contains(category))
                {
                    sequence.Categories.Remove(category);
                    for (char c = '\0'; c < char.MaxValue; c++)
                    {
                        if (parsedCharecter == c)
                            continue;

                        if (category == char.GetUnicodeCategory(c))
                        {
                            if (!sequence.Characters.Contains(c))
                                sequence.Characters.Add(c);
                        }
                    }
                }
            }
            else if (!sequence.Characters.Contains(parsedCharecter))
                sequence.Characters.Add(parsedCharecter);
        }
    }
}