using System.Collections.Generic;
using JmdictFurigana.Models;
using System.Text.RegularExpressions;

namespace JmdictFurigana.Business
{
    public class NoConsecutiveKanjiSolver : FuriganaSolver
    {
        /// <summary>
        /// Attempts to solve furigana in cases where there are no consecutive kanji in the kanji string,
        /// using regular expressions.
        /// </summary>
        protected override IEnumerable<FuriganaSolution> DoSolve(FuriganaResourceSet r, VocabEntry v)
        {
            // We are using both a greedy expression and a lazy expression because we want to make sure
            // there is only one way to read them. If the result differs with a greedy or a lazy expression,
            // it means that we have no idea how to read the damn thing.
            string regGreedy = "^";
            string regLazy = "^";
            bool consecutiveMarker = false;
            List<int> kanjiIndexes = new List<int>(4);
            for (int i = 0; i < v.KanjiReading.Length; i++)
            {
                char c = v.KanjiReading[i];
                Kanji k = r.GetKanji(c);
                if (k == null)
                {
                    // Add the characters to the string. No capture group for kana.
                    regGreedy += string.Format(c.ToString());
                    regLazy += string.Format(c.ToString());
                    consecutiveMarker = false;
                }
                else if (consecutiveMarker)
                {
                    // Consecutive kanji. The vocab entry is not eligible for this solution.
                    yield break;
                }
                else
                {
                    // Add the characters inside a capture group for kanji.
                    regGreedy += "(.+)";
                    regLazy += "(.+?)";
                    consecutiveMarker = true;
                    kanjiIndexes.Add(i);
                }
            }
            regGreedy += "$";
            regLazy += "$";

            // Example regex:
            // For 持ち運ぶ (もちはこぶ)
            // The regexes would be:
            // ^(.+)ち(.+)ぶ$
            // ^(.+?)ち(.+?)ぶ$

            Regex regexGreedy = new Regex(regGreedy);
            Regex regexLazy = new Regex(regLazy);
            Match matchGreedy = regexGreedy.Match(v.KanaReading);
            Match matchLazy = regexLazy.Match(v.KanaReading);

            if (matchGreedy.Success && matchLazy.Success)
            {
                // Obtain both solutions.
                FuriganaSolution greedySolution = MakeSolutionFromMatch(v, matchGreedy, kanjiIndexes);
                FuriganaSolution lazySolution = MakeSolutionFromMatch(v, matchLazy, kanjiIndexes);

                // Are both solutions non-null and equivalent?
                if (greedySolution != null && lazySolution != null && greedySolution.Equals(lazySolution))
                {
                    // Yes they are! Return only one of them of course.
                    // Greedy wins obviously.
                    yield return greedySolution;
                }
            }
        }

        /// <summary>
        /// Creates a furigana solution from a regex match computed in the DoSolve method.
        /// </summary>
        private FuriganaSolution MakeSolutionFromMatch(VocabEntry v, Match match, List<int> kanjiIndexes)
        {
            if (match.Groups.Count != kanjiIndexes.Count + 1)
            {
                return null;
            }

            List<FuriganaPart> parts = new List<FuriganaPart>(match.Groups.Count - 1);
            for (int i = 1; i < match.Groups.Count; i++)
            {
                Group group = match.Groups[i];
                parts.Add(new FuriganaPart(group.Value, kanjiIndexes[i - 1]));
            }

            return new FuriganaSolution(v, parts);
        }
    }
}
