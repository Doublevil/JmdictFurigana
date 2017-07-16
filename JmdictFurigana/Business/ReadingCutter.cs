using JmdictFurigana.Helpers;
using JmdictFurigana.Models;
using System.Collections.Generic;
using System.Linq;

namespace JmdictFurigana.Business
{
    public static class ReadingCutter
    {
        private static readonly int MaxKanaPerKanji = 4;

        private static readonly char[] ImpossibleCutStart = new char[]
            {
                'っ',
                'ょ',
                'ゅ',
                'ん'
            };

        /// <summary>
        /// Gets all possible reading cuts of the kana reading of the specified vocab entry, considering
        /// the length of the kanji reading string.
        /// </summary>
        /// <param name="v">Vocab entry to cut.</param>
        /// <returns>List of all possible reading cuts.</returns>
        /// <example>
        /// 頑張る (がんばる)
        /// => が.ん.ばる,
        ///    が.んば.る,
        ///    がん.ば.る
        /// </example>
        public static IEnumerable<string> GetAllPossibleCuts(VocabEntry v)
        {
            int cutCount = v.KanjiReading.Length;
            return GetCuts(v.KanaReading, v.KanjiReading.Length);
        }

        /// <summary>
        /// Recursive method that, given an input string and a number of cuts
        /// wanted, finds and returns all the cut possibilities.
        /// </summary>
        /// <param name="input">Input string to cut.</param>
        /// <param name="cutCount">Number of cuts to make.</param>
        /// <returns>The exhaustive list of possible cuts.</returns>
        private static IEnumerable<string> GetCuts(string input, int cutCount)
        {
            // Recursive exit condition.
            if (cutCount == 1)
            {
                // If we have only one cut to make, the only possible cut is our input string itself.

                if (input.Length <= MaxKanaPerKanji)
                {
                    // Performance fix.
                    // Return nothing if the final cut is more than xx characters long,
                    // because a single kanji associated with xx hiragana is not gonna happen anyway.
                    yield return input;
                }
            }
            else
            {
                // When we have more than one cut to make...
                string firstCut = string.Empty;
                while (input.Length >= cutCount)
                {
                    // Pop the first character of the input string to the firstCut.
                    firstCut += input.First();
                    input = input.Remove(0, 1);

                    // Performance fix. Check the characters to see if it is worth going on.
                    if ((firstCut.Length == 1 && ImpossibleCutStart.Contains(firstCut.First()))
                        || (firstCut.Length == 2 && KanaHelper.IsAllKatakana(firstCut)))
                    {
                        break;
                    }

                    // Recursively call this method with our input (keep in mind its first character has been removed)
                    // and with one less cut, because our currentString is the first cut in our context.
                    foreach (string subcut in GetCuts(input, cutCount - 1))
                    {
                        // This gives us all possible cuts after our currentString cut.
                        // Output the results.
                        yield return string.Format("{0}{1}{2}", firstCut, SeparatorHelper.FuriganaSeparator, subcut);
                    }

                    // If, after removing its first character, our input string is now too short,
                    // i.e. we could not produce any valid cut with a firstCut longer than it is now,
                    // stop looping. We're done.

                    // Example: for (がんばる, 3), we are stopping when firstCut = がん, because a
                    // firstCut of がんば couldn't produce any valid output, considering that we want
                    // 2 more cuts and there's only 1 character left.

                    if (firstCut.Length >= MaxKanaPerKanji)
                    {
                        // Performance fix. Exit the loop if our first cut is too long.
                        break;
                    }
                }
            }
        }
    }
}
