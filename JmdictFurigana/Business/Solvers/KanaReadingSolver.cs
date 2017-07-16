using System.Collections.Generic;
using System.Linq;
using JmdictFurigana.Models;
using JmdictFurigana.Helpers;

namespace JmdictFurigana.Business
{
    public class KanaReadingSolver : FuriganaSolver
    {
        private static readonly char[] ImpossibleCutStart = new char[] { 'っ', 'ょ', 'ゃ', 'ゅ', 'ん' };

        /// <summary>
        /// Attempts to solve furigana by reading the kana string and attributing kanji a reading based
        /// not on the readings of the kanji, but on the kana characters that come up.
        /// </summary>
        protected override IEnumerable<FuriganaSolution> DoSolve(FuriganaResourceSet r, VocabEntry v)
        {
            // Basically, we are reading the kanji reading character by character, eating the kana from
            // the kana reading and associating each kanji the piece of kana that comes next.
            // The thing is, we are taking advantage that kanji readings cannot start with certain
            // kana (ん and the small characters).
            // If we just stumbled upon a kanji and the next characters of the kana string are of these
            // impossible start kana, we can automatically associate them with the kanji.
            // Now this will work only for a number of vocab, but it does significantly improve the results.
            // It is especially good for 2-characters compounds that use unusual readings.

            /// Example: 阿呆陀羅 (あほんだら)
            /// Read the あ for 阿;
            /// Read the ほ for 呆;
            /// Read the ん: it's an impossible start character, so it goes with 呆 as well;
            /// Read the だ for 陀;
            /// Read the ら for 羅.

            string kana = v.KanaReading;
            List<FuriganaPart> furigana = new List<FuriganaPart>();
            for (int i = 0; i < v.KanjiReading.Length; i++)
            {
                if (kana.Length == 0)
                {
                    // We still have characters to browse in our kanji reading, but
                    // there are no more kana to consume. Cannot solve.
                    yield break;
                }

                char c = v.KanjiReading[i];
                // Check for special expressions
                bool foundExpression = false;
                for (int j = v.KanjiReading.Length - 1; j >= i; j--)
                {
                    string lookup = v.KanjiReading.Substring(i, (j - i) + 1);
                    SpecialExpression expression = r.GetExpression(lookup);
                    if (expression != null)
                    {
                        // We found an expression.
                        foreach (SpecialReading expressionReading in ReadingExpander.GetPotentialSpecialReadings(
                            expression, i == 0, j == v.KanjiReading.Length - 1))
                        {
                            if (kana.Length >= expressionReading.KanaReading.Length
                                && kana.Substring(0, expressionReading.KanaReading.Length) == expressionReading.KanaReading)
                            {
                                // The reading matches.
                                // Eat the kana chain.
                                furigana.AddRange(expressionReading.Furigana.Furigana
                                    .Select(fp => new FuriganaPart(fp.Value, fp.StartIndex + i, fp.EndIndex + i)));
                                kana = kana.Substring(expressionReading.KanaReading.Length);
                                i = j;
                                foundExpression = true;
                                break;
                            }
                        }

                        if (foundExpression)
                        {
                            break;
                        }
                    }
                }

                if (foundExpression)
                {
                    continue;
                }

                // Normal process: eat the first character of our kana string.
                string eaten = kana.First().ToString();
                kana = kana.Substring(1);
                Kanji k = r.GetKanji(c);
                if (k != null)
                {
                    // On a kanji case, also eat consecutive "impossible start characters"
                    // (ん, ょ, ゃ, ゅ, っ)
                    while (kana.Length > 0 && ImpossibleCutStart.Contains(kana.First()))
                    {
                        eaten += kana.First();
                        kana = kana.Substring(1);
                    }

                    furigana.Add(new FuriganaPart(eaten, i));
                }
                else if (!KanaHelper.IsAllKana(c.ToString()))
                {
                    // The character is neither a kanji or a kana.
                    // Cannot solve.
                    yield break;
                }
                else
                {
                    if (eaten != c.ToString())
                    {
                        // The character browsed is a kana but is not the
                        // character that we just ate. We made a mistake
                        // in one of the kanji readings, meaning that we...
                        // Cannot solve.
                        yield break;
                    }
                }
            }

            if (kana.Length == 0)
            {
                // We consumed the whole kana string.
                // The case is solved.
                yield return new FuriganaSolution(v, furigana);
            }
        }
    }
}
