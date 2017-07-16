using JmdictFurigana.Helpers;
using JmdictFurigana.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace JmdictFurigana.Etl
{
    /// <summary>
    /// Parses the kanji data file and produces instances of the Kanji model.
    /// </summary>
    public class KanjiEtl
    {
        #region Constants

        private static readonly string XmlNode_Character = "character";
        private static readonly string XmlNode_Literal = "literal";
        private static readonly string XmlNode_ReadingMeaning = "reading_meaning";
        private static readonly string XmlNode_ReadingMeaningGroup = "rmgroup";
        private static readonly string XmlNode_Reading = "reading";
        private static readonly string XmlNode_Nanori = "nanori";

        private static readonly string XmlAttribute_ReadingType = "r_type";

        private static readonly string XmlAttributeValue_KunYomiReading = "ja_kun";
        private static readonly string XmlAttributeValue_OnYomiReading = "ja_on";

        #endregion

        #region Fields

        private log4net.ILog _log;

        #endregion

        #region Constructors

        public KanjiEtl()
        {
            _log = log4net.LogManager.GetLogger(this.GetType());
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reads and returns Kanji models.
        /// </summary>
        public IEnumerable<Kanji> Execute()
        {
            // Load the supplement file.
            List<Kanji> supplementaryKanji = new List<Kanji>();
            foreach (string line in File.ReadAllLines(PathHelper.SupplementaryKanjiPath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.First() == ';')
                    continue;

                char c = line.First();
                string[] split = line.Split(SeparatorHelper.FileFieldSeparator);
                string[] readings = split[1].Split(SeparatorHelper.FileReadingSeparator);

                supplementaryKanji.Add(new Kanji()
                {
                    Character = c,
                    Readings = readings.ToList(),
                    ReadingsWithNanori = readings.ToList(),
                    IsRealKanji = false
                });
            }

            // Load the KanjiDic2 file.
            XDocument xdoc = XDocument.Load(PathHelper.KanjiDic2Path);

            // Browse kanji nodes.
            foreach (XElement xkanji in xdoc.Root.Elements(XmlNode_Character))
            {
                // For each kanji node, read values.
                Kanji kanji = new Kanji();

                // Read the kanji character.
                kanji.Character = xkanji.Element(XmlNode_Literal).Value.First();

                // In the reading/meaning node...
                XElement xreadingMeaning = xkanji.Element(XmlNode_ReadingMeaning);
                if (xreadingMeaning != null)
                {
                    // Browse the reading group...
                    XElement xrmGroup = xreadingMeaning.Element(XmlNode_ReadingMeaningGroup);
                    if (xrmGroup != null)
                    {
                        // Read the readings and add them to the readings of the kanji.
                        foreach (XElement xreading in xrmGroup.Elements(XmlNode_Reading)
                            .Where(x => x.Attribute(XmlAttribute_ReadingType).Value == XmlAttributeValue_OnYomiReading
                                || x.Attribute(XmlAttribute_ReadingType).Value == XmlAttributeValue_KunYomiReading))
                        {
                            kanji.Readings.Add(KanaHelper.ToHiragana(xreading.Value));
                        }
                    }
                }

                // See if there's a supplementary entry for this kanji.
                Kanji supp = supplementaryKanji.FirstOrDefault(k => k.Character == kanji.Character);
                if (supp != null)
                {
                    // Supplementary entry found. Remove it from the list and add its readings to our current entry.
                    kanji.Readings.AddRange(supp.Readings);
                    supplementaryKanji.Remove(supp);
                }

                // Read the nanori readings
                var nanoriReadings = xreadingMeaning?.Elements(XmlNode_Nanori).Select(n => n.Value).ToList() ?? new List<string>();
                kanji.ReadingsWithNanori = kanji.Readings.Union(nanoriReadings).Distinct().ToList();

                // Return the kanji read and go to the next kanji node.
                yield return kanji;

                xkanji.RemoveAll();
            }

            // Return the remaining supplementary kanji as new kanji.
            foreach (Kanji k in supplementaryKanji)
            {
                yield return k;
            }
        }

        #endregion
    }
}
