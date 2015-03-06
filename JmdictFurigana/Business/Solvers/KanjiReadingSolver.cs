using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JmdictFurigana.Models;
using JmdictFurigana.Helpers;
using JmdictFurigana.Extensions;

namespace JmdictFurigana.Business
{
    public class KanjiReadingSolver : FuriganaSolver
    {
        /// <summary>
        /// Defines the maximal number of kana that can be attributed to a single kanji (performance trick).
        /// </summary>
        private static readonly int MaxKanaPerKanji = 4;

        /// <summary>
        /// Attempts to solve furigana by reading the kanji reading string and finding matching kanji
        /// kanji readings.
        /// </summary>
        protected override IEnumerable<FuriganaSolution> DoSolve(FuriganaResourceSet r, VocabEntry v)
        {
            foreach (FuriganaSolution solution in TryReading(r, v, 0, 0, new List<FuriganaPart>()))
            {
                yield return solution;
            }
        }

        /// <summary>
        /// Recursive method that reads the kanji reading string and attempts to find all the ways the
        /// kana reading could be cut by matching it with the potential kanji readings.
        /// </summary>
        /// <param name="r">Resource set.</param>
        /// <param name="v">Vocab to solve.</param>
        /// <param name="currentIndexKanji">Current position in the kanji string. Used for recursion.</param>
        /// <param name="currentIndexKana">Current position in the kana string. Used for recursion.</param>
        /// <param name="currentCut">Current furigana parts. Used for recursion.</param>
        private IEnumerable<FuriganaSolution> TryReading(FuriganaResourceSet r, VocabEntry v,
            int currentIndexKanji, int currentIndexKana, List<FuriganaPart> currentCut)
        {
            if (currentIndexKanji == v.KanjiReading.Length && currentIndexKana == v.KanaReading.Length)
            {
                // We successfuly read the word and stopped at the last character in both kanji and kana readings.
                // Our current cut is valid. Return it.
                yield return new FuriganaSolution(v, currentCut);
                yield break;
            }
            else if (currentIndexKanji >= v.KanjiReading.Length || currentIndexKana >= v.KanaReading.Length)
            {
                // Broken case. Do not return anything.
                yield break;
            }

            // Search for special expressions.
            bool foundSpecialExpressions = false;
            foreach (FuriganaSolution solution in FindSpecialExpressions(r, v, currentIndexKanji, currentIndexKana, currentCut))
            {
                foundSpecialExpressions = true;
                yield return solution;
            }

            if (foundSpecialExpressions)
            {
                yield break;
            }

            // General case. Get the current character and see if it is a kanji.
            char c = v.KanjiReading[currentIndexKanji];

            if (c == '々' && currentIndexKanji > 0)
            {
                // Special case: handle the repeater kanji by using the previous character instead.
                c = v.KanjiReading[currentIndexKanji - 1];
            }
            Kanji k = r.GetKanji(c);

            if (k != null)
            {
                // Read as kanji subpart.
                foreach (FuriganaSolution solution in ReadAsKanji(r, v, currentIndexKanji, currentIndexKana, currentCut, c, k))
                {
                    yield return solution;
                }
            }
            else
            {
                // Read as kana subpart.
                foreach (FuriganaSolution solution in ReadAsKana(r, v, currentIndexKanji, currentIndexKana, currentCut, c))
                {
                    yield return solution;
                }
            }
        }

        /// <summary>
        /// Subpart of TryReading. Attempts to find a matching special expression.
        /// If found, iterates on TryReading.
        /// </summary>
        private IEnumerable<FuriganaSolution> FindSpecialExpressions(FuriganaResourceSet r, VocabEntry v,
            int currentIndexKanji, int currentIndexKana, List<FuriganaPart> currentCut)
        {
            string lookup = string.Empty;
            for (int i = v.KanjiReading.Length - 1; i >= currentIndexKanji; i--)
            {
                lookup = v.KanjiReading.Substring(currentIndexKanji, (i - currentIndexKanji) + 1);
                SpecialExpression expression = r.GetExpression(lookup);
                if (expression != null)
                {
                    foreach (SpecialReading expressionReading in ReadingExpander.GetPotentialSpecialReadings(
                        expression, currentIndexKanji == 0, i == v.KanjiReading.Length - 1))
                    {
                        if (v.KanaReading.Length >= currentIndexKana + expressionReading.KanaReading.Length
                            && v.KanaReading.Substring(currentIndexKana, expressionReading.KanaReading.Length) == expressionReading.KanaReading)
                        {
                            // The reading matches. Iterate with this possibility.
                            List<FuriganaPart> newCut = currentCut.Clone();
                            newCut.AddRange(expressionReading.Furigana.Furigana
                                .Select(fp => new FuriganaPart(fp.Value, fp.StartIndex + currentIndexKanji, fp.EndIndex + currentIndexKanji)));

                            foreach (FuriganaSolution result in TryReading(r, v, i + 1,
                                currentIndexKana + expressionReading.KanaReading.Length, newCut))
                            {
                                yield return result;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Subpart of TryReading. Finds all matching kanji readings for the current situation,
        /// and iterates on TryReading when found.
        /// </summary>
        private IEnumerable<FuriganaSolution> ReadAsKanji(FuriganaResourceSet r, VocabEntry v,
            int currentIndexKanji, int currentIndexKana, List<FuriganaPart> currentCut, char c, Kanji k)
        {
            // Our character is a kanji. Try to consume kana strings that match that kanji.
            int remainingKanjiLength = v.KanjiReading.Length - currentIndexKanji - 1;
            List<string> kanjiReadings = ReadingExpander.GetPotentialKanjiReadings(k,
                currentIndexKanji == 0, currentIndexKanji == v.KanjiReading.Length - 1);

            // Iterate on the kana reading.
            for (int i = currentIndexKana; i < v.KanaReading.Length && i < currentIndexKana + MaxKanaPerKanji; i++)
            {
                int remainingKanaLength = v.KanaReading.Length - i - 1;
                if (remainingKanaLength < remainingKanjiLength)
                {
                    // We consumed too many characters: not enough kana remaining for the number of kanji.
                    // Stop here. There are no more solutions.
                    yield break;
                }

                // Get the kana string between currentIndexKana and i.
                string testedString = v.KanaReading.Substring(currentIndexKana, (i - currentIndexKana) + 1);

                // Now try to match that string against one of the potential readings of our kanji.
                foreach (string reading in kanjiReadings)
                {
                    if (reading == testedString)
                    {
                        // We have a match.
                        // Create our new cut and iterate with it.
                        List<FuriganaPart> newCut = currentCut.Clone();
                        newCut.Add(new FuriganaPart(reading, currentIndexKanji));

                        foreach (FuriganaSolution result in TryReading(r, v, currentIndexKanji + 1, i + 1, newCut))
                        {
                            yield return result;
                        }
                    }
                }

                // Continue to expand our testedString to try and follow other potential reading paths.
            }
        }

        /// <summary>
        /// Subpart of TryReading. Attempts to find a match between the current kanji reading character
        /// and the current kana reading character. If found, iterates on TryReading.
        /// </summary>
        private IEnumerable<FuriganaSolution> ReadAsKana(FuriganaResourceSet r, VocabEntry v,
            int currentIndexKanji, int currentIndexKana, List<FuriganaPart> currentCut, char c)
        {
            char kc = v.KanaReading[currentIndexKana];
            if (c == kc || KanaHelper.ToHiragana(c.ToString()) == KanaHelper.ToHiragana(kc.ToString()))
            {
                // What we are reading in the kanji reading matches the kana reading.
                // We can iterate with the same cut (no added furigana) because we are reading kana.
                foreach (FuriganaSolution result in TryReading(r, v, currentIndexKanji + 1, currentIndexKana + 1, currentCut))
                {
                    yield return result;
                }
            }
        }
    }
}
