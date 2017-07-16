using System.Collections.Generic;
using System.Linq;
using JmdictFurigana.Models;
using JmdictFurigana.Helpers;

namespace JmdictFurigana.Business
{
    public class LengthMatchSolver : FuriganaSolver
    {
        public LengthMatchSolver()
        {
            // Priority down because it's not good with special expressions.
            Priority = -1;
        }

        /// <summary>
        /// Attempts to solve cases where the length of the kanji reading matches the length of the
        /// kana reading.
        /// </summary>
        protected override IEnumerable<FuriganaSolution> DoSolve(FuriganaResourceSet r, VocabEntry v)
        {
            if (v.KanjiReading.Length == v.KanaReading.Length)
            {
                List<FuriganaPart> parts = new List<FuriganaPart>();
                for (int i = 0; i < v.KanjiReading.Length; i++)
                {
                    if (r.GetKanji(v.KanjiReading[i]) != null)
                    {
                        parts.Add(new FuriganaPart(v.KanaReading[i].ToString(), i));
                    }
                    else if (!KanaHelper.IsAllKana(v.KanjiReading[i].ToString()))
                    {
                        // Our character is not a kanji and apparently not a kana either.
                        // Stop right there. It's probably a trap.
                        yield break;
                    }
                    else
                    {
                        if (!KanaHelper.AreEquivalent(v.KanjiReading[i].ToString(), v.KanaReading[i].ToString()))
                        {
                            // We are reading kana characters that are not equivalent. Stop.
                            yield break;
                        }
                    }
                }

                if (parts.Any())
                {
                    yield return new FuriganaSolution(v, parts);
                }
            }
        }
    }
}
