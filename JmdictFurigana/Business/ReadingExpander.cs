using JmdictFurigana.Helpers;
using JmdictFurigana.Models;
using JmdictFurigana.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JmdictFurigana.Business
{
    /// <summary>
    /// Provides a process that expands a given list of readings by adding rendaku versions and stuff like this.
    /// </summary>
    public static class ReadingExpander
    {
        private static readonly Dictionary<char, char[]> RendakuDictionary = new Dictionary<char, char[]>()
        {
            {'か', new char[]{'が'}},
            {'き', new char[]{'ぎ'}},
            {'く', new char[]{'ぐ'}},
            {'け', new char[]{'げ'}},
            {'こ', new char[]{'ご'}},
            {'さ', new char[]{'ざ'}},
            {'し', new char[]{'じ'}},
            {'す', new char[]{'ず'}},
            {'せ', new char[]{'ぜ'}},
            {'そ', new char[]{'ぞ'}},
            {'た', new char[]{'だ'}},
            {'ち', new char[]{'ぢ','じ'}},
            {'つ', new char[]{'づ','ず'}},
            {'て', new char[]{'で'}},
            {'と', new char[]{'ど'}},
            {'は', new char[]{'ば','ぱ'}},
            {'ひ', new char[]{'び','ぴ'}},
            {'ふ', new char[]{'ぶ','ぷ'}},
            {'へ', new char[]{'べ','ぺ'}},
            {'ほ', new char[]{'ぼ','ぽ'}},
        };

        private static readonly Dictionary<string, string> AfterDotKunYomiTransformDictionary = new Dictionary<string, string>()
        {
            {"く", "き"},
            {"ぐ", "ぎ"},
            {"す", "し"},
            {"ず", "じ"},
            {"む", "み"},
            {"る", "り"},
            {"ぶ", "び"},
            {"う", "い"}
        };

        private static readonly char[] SmallTsuRendakuList = new char[]
        {
            'つ',
            'く',
            'き',
            'ち'
        };

        /// <summary>
        /// Given a kanji, finds and returns all potential readings that it could take in a string.
        /// </summary>
        /// <param name="k">Kanji to evaluate.</param>
        /// <param name="isFirstChar">Set to true if this kanji is the first character of the string
        /// that the kanji is found in.</param>
        /// <param name="isLastChar">Set to true if this kanji is the last character of the string
        /// that the kanji is found in.</param>
        /// <returns>A list containing all potential readings that the kanji could take.</returns>
        public static List<string> GetPotentialKanjiReadings(Kanji k, bool isFirstChar, bool isLastChar)
        {
            List<string> output = new List<string>();
            foreach (string reading in k.Readings)
            {
                string r = reading.Replace("-", string.Empty);
                if (!KanaHelper.IsAllKatakana(r))
                {
                    r = r.Replace("ー", string.Empty);
                }

                string[] dotSplit = r.Split('.');
                if (dotSplit.Count() == 1)
                {
                    output.Add(r);
                }
                else if (dotSplit.Count() == 2)
                {
                    output.Add(dotSplit[0]);
                    output.Add(r.Replace(".", string.Empty));

                    if (AfterDotKunYomiTransformDictionary.ContainsKey(dotSplit[1]))
                    {
                        string newTerm = AfterDotKunYomiTransformDictionary[dotSplit[1]];
                        string newReading = r.Replace(".", string.Empty);
                        newReading = newReading.Substring(0, newReading.Length - dotSplit[1].Length);
                        newReading += newTerm;
                        output.Add(newReading);
                    }

                    if (dotSplit[1].Length >= 2 && dotSplit[1][1] == 'る')
                    {
                        // Add variant without the ending る.
                        string newReading = r.Replace(".", string.Empty);
                        newReading = newReading.Substring(0, newReading.Length - 1);
                        output.Add(newReading);
                    }
                }
                else
                {
                    throw new Exception(string.Format("Weird reading: {0} for kanji {1}.", reading, k.Character));
                }
            }

            // Add final small tsu rendaku
            if (!isLastChar)
            {
                output.AddRange(GetSmallTsuRendaku(output));
            }

            // Rendaku
            if (!isFirstChar)
            {
                output.AddRange(GetAllRendaku(output));
            }

            return output.Distinct().ToList();
        }

        /// <summary>
        /// Given a special reading expression, returns all potential kana readings the expression could use.
        /// </summary>
        /// <param name="sp">Target special reading expression.</param>
        /// <param name="isFirstChar">Set to true if the first character of the expression is the first
        /// character of the string that the expression is found in.</param>
        /// <param name="isLastChar">Set to true if the last character of the expression is the last
        /// character of the string that the expression is found in.</param>
        /// <returns>A list containing all potential readings the expression could assume.</returns>
        public static List<SpecialReading> GetPotentialSpecialReadings(SpecialExpression sp, bool isFirstChar, bool isLastChar)
        {
            // Aaargh that's a mess.
            List<SpecialReading> output = new List<SpecialReading>(sp.Readings);

            // Add final small tsu rendaku
            if (!isLastChar)
            {
                List<SpecialReading> add = new List<SpecialReading>();
                foreach (SpecialReading r in output)
                {
                    if (SmallTsuRendakuList.Contains(r.KanaReading.Last()))
                    {
                        string newKanaReading = r.KanaReading.Substring(0, r.KanaReading.Length - 1) + "っ";
                        SpecialReading newReading = new SpecialReading(newKanaReading, new FuriganaSolution(r.Furigana.Vocab,
                            r.Furigana.Furigana.Clone()));

                        List<FuriganaPart> affectedParts = newReading.Furigana.GetPartsForIndex(
                            newReading.Furigana.Vocab.KanjiReading.Length - 1);
                        foreach (FuriganaPart part in affectedParts)
                        {
                            part.Value = part.Value.Remove(part.Value.Length - 1) + "っ";
                        }

                        add.Add(newReading);
                    }
                }
                output.AddRange(add);
            }

            // Rendaku
            if (!isFirstChar)
            {
                List<SpecialReading> add = new List<SpecialReading>();
                foreach (SpecialReading r in output)
                {
                    if (RendakuDictionary.ContainsKey(r.KanaReading.First()))
                    {
                        foreach (char ren in RendakuDictionary[r.KanaReading.First()])
                        {
                            string newKanaReading = ren.ToString() + r.KanaReading.Substring(1);
                            SpecialReading newReading = new SpecialReading(newKanaReading, new FuriganaSolution(r.Furigana.Vocab,
                                r.Furigana.Furigana.Clone()));

                            List<FuriganaPart> affectedParts = newReading.Furigana.GetPartsForIndex(0);
                            foreach (FuriganaPart part in affectedParts)
                            {
                                part.Value = ren.ToString() + part.Value.Substring(1);
                            }

                            add.Add(newReading);
                        }
                    }
                }
                output.AddRange(add);
            }

            return output.Distinct().ToList();
        }

        private static List<string> GetSmallTsuRendaku(List<string> readings)
        {
            List<string> addedOutput = new List<string>();
            foreach (string r in readings)
            {
                if (SmallTsuRendakuList.Contains(r.Last()))
                {
                    addedOutput.Add(r.Substring(0, r.Length - 1) + "っ");
                }
            }

            return addedOutput;
        }

        private static List<string> GetAllRendaku(List<string> readings)
        {
            List<string> rendakuOutput = new List<string>();
            foreach (string r in readings)
            {
                if (RendakuDictionary.ContainsKey(r.First()))
                {
                    foreach (char ren in RendakuDictionary[r.First()])
                    {
                        rendakuOutput.Add(ren.ToString() + r.Substring(1, r.Length - 1));
                    }
                }
            }

            return rendakuOutput;
        }
    }
}
