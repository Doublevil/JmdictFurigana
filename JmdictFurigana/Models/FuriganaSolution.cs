using JmdictFurigana.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JmdictFurigana.Models
{
    /// <summary>
    /// A vocab entry with a furigana reading string.
    /// </summary>
    public class FuriganaSolution
    {
        #region Properties

        /// <summary>
        /// Gets or sets the base dictionary entry of this furigana entry.
        /// </summary>
        public VocabEntry Vocab { get; set; }

        /// <summary>
        /// Gets or sets the furigana.
        /// </summary>
        public List<FuriganaPart> Furigana { get; set; }

        #endregion

        #region Constructors

        public FuriganaSolution() : this(null, new List<FuriganaPart>()) { }

        public FuriganaSolution(VocabEntry vocab, params FuriganaPart[] parts)
            : this(vocab, parts.ToList()) { }

        public FuriganaSolution(VocabEntry vocab, List<FuriganaPart> furigana)
        {
            Vocab = vocab;
            Furigana = furigana;
        }

        #endregion

        #region Methods

        #region Static

        /// <summary>
        /// Attempts to parse a furigana solution from the given string.
        /// The expected format is the ToString format.
        /// </summary>
        /// <param name="s">String to parse.</param>
        /// <param name="v">Reference vocab entry.</param>
        /// <returns>The parsed solution if the operation was successful. Null otherwise.</returns>
        public static FuriganaSolution Parse(string s, VocabEntry v)
        {
            if (s == null)
            {
                return null;
            }

            List<FuriganaPart> parts = new List<FuriganaPart>();

            string[] partSplit = s.Split(SeparatorHelper.MultiValueSeparator);
            foreach (string partString in partSplit)
            {
                string[] fieldSeparator = partString.Split(SeparatorHelper.AssociationSeparator);
                if (fieldSeparator.Count() == 2)
                {
                    string indexesString = fieldSeparator[0];
                    string furiganaValue = fieldSeparator[1];

                    int? minIndex = 0;
                    int? maxIndex = 0;
                    string[] indexSplit = indexesString.Split(SeparatorHelper.RangeSeparator);
                    if (indexSplit.Count() == 2)
                    {
                        minIndex = ParsingHelper.ParseInt(indexSplit[0]);
                        maxIndex = ParsingHelper.ParseInt(indexSplit[1]);
                    }
                    else if (indexSplit.Count() == 1)
                    {
                        minIndex = ParsingHelper.ParseInt(indexSplit[0]);
                        maxIndex = minIndex;
                    }
                    else
                    {
                        // Malformed input.
                        return null;
                    }

                    if (minIndex.HasValue && maxIndex.HasValue && minIndex.Value <= maxIndex.Value)
                    {
                        parts.Add(new FuriganaPart(furiganaValue, minIndex.Value, maxIndex.Value));
                    }
                    else
                    {
                        // Malformed input.
                        return null;
                    }
                }
                else
                {
                    // Malformed input or just a simple reading.
                    // Treat it like a simple reading.
                    parts.Add(new FuriganaPart(partString, 0, v.KanjiReading.Length));
                }
            }

            // Everything went fine. Return the solution.
            return new FuriganaSolution(v, parts);
        }

        /// <summary>
        /// Checks if the solution is correctly solved for the given coupling of vocab and furigana.
        /// </summary>
        /// <param name="v">Vocab to check.</param>
        /// <param name="furigana">Furigana to check.</param>
        /// <returns>True if the furigana covers all characters of the vocab reading without
        /// overlapping.</returns>
        public static bool Check(VocabEntry v, List<FuriganaPart> furigana)
        {
            // There are three conditions to check:
            // 1. Furigana parts are not overlapping: for any given index in the kanji reading string,
            // there is between 0 and 1 matching furigana parts.
            // 2. All non-kana characters are covered by a furigana part.
            // 3. Reconstituting the kana reading from the kanji reading using the furigana parts when
            // available will give the kana reading of the vocab entry.
            
            // Keep in mind things like 真っ青 admit a correct "0-2:まっさお" solution. There can be
            // furigana parts covering kana.

            // Check condition 1.
            if (Enumerable.Range(0, v.KanjiReading.Length).Any(i => furigana.Count(f => i >= f.StartIndex && i <= f.EndIndex) > 1))
            {
                // There are multiple furigana parts that are appliable for a given index.
                // This constitutes an overlap and results in the check being negative.
                // Condition 1 failed.
                return false;
            }

            // Now try to reconstitute the reading using the furigana parts.
            // This will allow us to test both 2 and 3.
            StringBuilder reconstitutedReading = new StringBuilder();
            for (int i = 0; i < v.KanjiReading.Length; i++)
            {
                // Try to find a matching part.
                FuriganaPart matchingPart = furigana.FirstOrDefault(f => i >= f.StartIndex && i <= f.EndIndex);
                if (matchingPart != null)
                {
                    // We have a matching part. Add the furigana string to the reconstituted reading.
                    reconstitutedReading.Append(matchingPart.Value);

                    // Advance i to the end index and continue.
                    i = matchingPart.EndIndex;
                    continue;
                }

                // Characters that are not covered by a furigana part should be kana.
                char c = v.KanjiReading[i];
                if (KanaHelper.IsAllKana(c.ToString()))
                {
                    // It is kana. Add the character to the reconstituted reading.
                    reconstitutedReading.Append(c);
                }
                else
                {
                    // This is not kana and this is not covered by any furigana part.
                    // The solution is not complete and is therefore not valid.
                    // Condition 2 failed.
                    return false;
                }
            }

            // Our reconstituted reading should be the same as the kana reading of the vocab.
            if (!KanaHelper.AreEquivalent(reconstitutedReading.ToString(), v.KanaReading))
            {
                // It is different. Something is not correct in the furigana reading values.
                // Condition 3 failed.
                return false;
            }

            // Nothing has failed. Everything is good.
            return true;
        }

        #endregion

        /// <summary>
        /// Absorbs the specified solution's furigana parts.
        /// </summary>
        /// <param name="other">Solution to absorb.</param>
        /// <param name="index">Index to add to each of the other furigana part's indexes.</param>
        public void EatSubsolution(FuriganaSolution other, int index)
        {
            foreach (FuriganaPart part in other.Furigana)
            {
                Furigana.Add(new FuriganaPart(part.Value, part.StartIndex + index, part.EndIndex + index));
            }
        }

        /// <summary>
        /// Gets the parts covering the given index.
        /// Remember that an invalid solution may have several parts for a given index.
        /// </summary>
        /// <param name="index">Target index.</param>
        /// <returns>All parts covering the given index.</returns>
        public List<FuriganaPart> GetPartsForIndex(int index)
        {
            return Furigana.Where(f => index >= f.StartIndex && index <= f.EndIndex).ToList();
        }

        /// <summary>
        /// Checks if the solution is correctly solved.
        /// </summary>
        /// <returns>True if the furigana covers all characters without overlapping.
        /// False otherwise.</returns>
        public bool Check()
        {
            return FuriganaSolution.Check(Vocab, Furigana);
        }

        /// <summary>
        /// Sorts the furigana parts by start index.
        /// </summary>
        public void ReorderFurigana()
        {
            Furigana.Sort();
        }

        public override string ToString()
        {
            // Example output:
            // For 大人買い (おとながい):
            // 0-1:おとな,2:が
            StringBuilder result = new StringBuilder();

            result.Append(Vocab.KanjiReading);
            result.Append(SeparatorHelper.FileFieldSeparator);
            result.Append(Vocab.KanaReading);
            result.Append(SeparatorHelper.FileFieldSeparator);

            for (int i = 0; i < Furigana.Count; i++)
            {
                result.Append(Furigana[i]);
                if (i < Furigana.Count - 1)
                {
                    result.Append(SeparatorHelper.MultiValueSeparator);
                }
            }

            return result.ToString();
        }

        public override bool Equals(object obj)
        {
            FuriganaSolution other = obj as FuriganaSolution;
            if (other != null)
            {
                // Compare both solutions.
                if (Vocab != other.Vocab || Furigana.Count != other.Furigana.Count)
                {
                    // Not the same vocab or not the same count of furigana parts.
                    return false;
                }

                // If there is at least one furigana part that has no equivalent in the other
                // furigana solution, then the readings differ.
                return Furigana.All(f1 => other.Furigana.Any(f2 => f1.Equals(f2)))
                    && other.Furigana.All(f2 => Furigana.Any(f1 => f1.Equals(f2)));
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}
