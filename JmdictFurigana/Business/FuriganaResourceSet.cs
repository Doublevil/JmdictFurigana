using JmdictFurigana.Etl;
using JmdictFurigana.Helpers;
using JmdictFurigana.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JmdictFurigana.Business
{
    public class FuriganaResourceSet
    {
        #region Fields

        private Dictionary<char, Kanji> _kanjiDictionary;
        private Dictionary<string, FuriganaSolution> _overrideList;
        private Dictionary<string, SpecialExpression> _specialExpressions;

        #endregion

        #region Constructors

        public FuriganaResourceSet()
        {
            _kanjiDictionary = new Dictionary<char, Kanji>();
            _overrideList = new Dictionary<string, FuriganaSolution>();
            _specialExpressions = new Dictionary<string, SpecialExpression>();
        }

        #endregion

        #region Methods


        #region Loading

        /// <summary>
        /// Loads the resources. Should be done before using any accessor method.
        /// </summary>
        public void Load()
        {
            LoadKanjiDictionary();
            LoadOverrideList();
            LoadSpecialExpressions();
        }

        /// <summary>
        /// Loads the furigana override list.
        /// </summary>
        private void LoadOverrideList()
        {
            _overrideList = new Dictionary<string, FuriganaSolution>();
            foreach (string line in File.ReadAllLines(PathHelper.OverrideFuriganaPath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.First() == ';')
                    continue;

                string[] split = line.Split(SeparatorHelper.FileFieldSeparator);
                _overrideList.Add(new VocabEntry(split[0], split[1]).ToString(), FuriganaSolution.Parse(split[2], null));
            }
        }

        /// <summary>
        /// Loads the special expressions dictionary.
        /// </summary>
        private void LoadSpecialExpressions()
        {
            _specialExpressions = new Dictionary<string, SpecialExpression>();
            foreach (string line in File.ReadAllLines(PathHelper.SpecialReadingsPath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.First() == ';')
                    continue;

                string[] split = line.Split(SeparatorHelper.FileFieldSeparator);
                string kanjiReading = split[0];
                string kanaReading = split[1];

                VocabEntry v = new VocabEntry(kanjiReading, kanaReading);

                // Read the solution if it is explicitly written. Compute it otherwise.
                FuriganaSolution solution = split.Count() == 3 ?
                    FuriganaSolution.Parse(split[2], v)
                    : new FuriganaSolution(v, new FuriganaPart(kanaReading, 0, kanjiReading.Length - 1));

                // Add the special reading or special expression.
                SpecialReading specialReading = new SpecialReading(kanaReading, solution);
                if (_specialExpressions.ContainsKey(kanjiReading))
                {
                    _specialExpressions[kanjiReading].Readings.Add(specialReading);
                }
                else
                {
                    _specialExpressions.Add(kanjiReading, new SpecialExpression(kanjiReading, specialReading));
                }
            }
        }

        /// <summary>
        /// Loads the kanji dictionary using resource files.
        /// </summary>
        private void LoadKanjiDictionary()
        {
            _kanjiDictionary = new Dictionary<char, Kanji>();
            KanjiEtl etl = new KanjiEtl();
            foreach (Kanji kanji in etl.Execute())
            {
                AddKanjiEntry(kanji);
            }
        }

        /// <summary>
        /// Adds or merge a kanji to the kanji dictionary.
        /// </summary>
        /// <param name="k">Kanji to add or merge.</param>
        private void AddKanjiEntry(Kanji k)
        {
            if (_kanjiDictionary.ContainsKey(k.Character))
            {
                _kanjiDictionary[k.Character].Readings.AddRange(k.Readings);
                _kanjiDictionary[k.Character].Readings = _kanjiDictionary[k.Character].Readings.Distinct().ToList();

                _kanjiDictionary[k.Character].ReadingsWithNanori.AddRange(k.ReadingsWithNanori);
                _kanjiDictionary[k.Character].ReadingsWithNanori = _kanjiDictionary[k.Character].ReadingsWithNanori.Distinct().ToList();
            }
            else
            {
                _kanjiDictionary.Add(k.Character, k);
            }
        }

        #endregion

        #region Getters

        /// <summary>
        /// Gets the kanji matching the given character from the dictionary.
        /// </summary>
        /// <param name="c">Character to look for in the kanji dictionary.</param>
        /// <returns>The kanji matching the given character, or null if it does not exist.</returns>
        public Kanji GetKanji(char c)
        {
            return _kanjiDictionary.ContainsKey(c) ? _kanjiDictionary[c] : null;
        }

        /// <summary>
        /// Gets the special expression matching the given string from the dictionary.
        /// </summary>
        /// <param name="s">String to look for in the special expression dictionary.</param>
        /// <returns>The expression matching the given string, or null if it does not exist.</returns>
        public SpecialExpression GetExpression(string s)
        {
            return _specialExpressions.ContainsKey(s) ? _specialExpressions[s] : null;
        }

        /// <summary>
        /// Gets the override solution matching the given vocab entry.
        /// </summary>
        /// <param name="v">Entry to look for in the override list.</param>
        /// <returns>The matching solution if found. Null otherwise.</returns>
        public FuriganaSolution GetOverride(VocabEntry v)
        {
            string s = v.ToString();
            return _overrideList.ContainsKey(s) ? _overrideList[s] : null;
        }

        #endregion

        #endregion
    }
}
