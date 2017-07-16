using System.Collections.Generic;
using JmdictFurigana.Models;

namespace JmdictFurigana.Business
{
    public class RepeatedKanjiSolver : FuriganaSolver
    {
        /// <summary>
        /// Solves cases where the kanji reading consists in a repeated kanji.
        /// </summary>
        protected override IEnumerable<FuriganaSolution> DoSolve(FuriganaResourceSet r, VocabEntry v)
        {
            if (v.KanjiReading.Length == 2 && v.KanaReading.Length % 2 == 0
                && (v.KanjiReading[1] == '々' || v.KanjiReading[1] == v.KanjiReading[0]))
            {
                // We have a case where the kanji string is composed of kanji repeated (e.g. 中々),
                // and our kana string can be cut in two. Just do that.

                yield return new FuriganaSolution(v,
                    new FuriganaPart(v.KanaReading.Substring(0, v.KanaReading.Length / 2), 0),
                    new FuriganaPart(v.KanaReading.Substring(v.KanaReading.Length / 2), 1));
            }
        }
    }
}
