using JmdictFurigana.Models;
using JmdictFurigana.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace JmdictFurigana.Etl
{
    /// <summary>
    /// Parses the dictionary file and produces VocabEntry model instances.
    /// </summary>
    public class DictionaryEtl
    {
        #region Constants

        private static readonly XNamespace XmlNs = "http://www.w3.org/XML/1998/namespace";

        private const string XmlNode_Entry = "entry";
        private const string XmlNode_KanjiElement = "k_ele";
        private const string XmlNode_KanjiReading = "keb";
        private const string XmlNode_ReadingElement = "r_ele";
        private const string XmlNode_KanaReading = "reb";
        private const string XmlNode_ReadingConstraint = "re_restr";
        private const string XmlNode_NoKanji = "re_nokanji";

        #endregion

        #region Fields

        private log4net.ILog _log;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the path to the XML dictionary file to be parsed.
        /// </summary>
        public string DictionaryFilePath { get; set; }

        #endregion

        #region Constructors

        public DictionaryEtl(string dictionaryFilePath)
        {
            _log = log4net.LogManager.GetLogger(this.GetType());
            DictionaryFilePath = dictionaryFilePath;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parses the dictionary file and returns entries.
        /// </summary>
        public IEnumerable<VocabEntry> Execute()
        {
            // Load the file as an XML document
            XDocument xdoc;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(File.ReadAllText(DictionaryFilePath))))
            {
                var settings = new XmlReaderSettings();
                settings.DtdProcessing = DtdProcessing.Parse;
                using (var reader = XmlReader.Create(stream, settings))
                {
                    xdoc = XDocument.Load(reader);
                }
            }

            // Load and return vocab items:
            // Browse each vocab entry.
            foreach (XElement xentry in xdoc.Root.Elements(XmlNode_Entry))
            {
                List<VocabEntry> vocabList = new List<VocabEntry>();

                // For each kanji element node
                foreach (XElement xkanjiElement in xentry.Elements(XmlNode_KanjiElement))
                {
                    // Parse the kanji element. The list will be expanded with new elements.
                    // Create a new vocab with the associated writing.
                    VocabEntry vocab = new VocabEntry();
                    vocab.KanjiReading = xkanjiElement.Element(XmlNode_KanjiReading).Value;

                    // Add the created vocab to the list.
                    vocabList.Add(vocab);
                }

                // For each kanji reading node
                var xreadingElements = xentry.Elements(XmlNode_ReadingElement);
                foreach (XElement xreadingElement in xreadingElements)
                {
                    // Exclude the node if it contains the no kanji node, and is not the only reading.
                    // This is a behavior that seems to be implemented in Jisho (example word: 台詞).
                    if (xreadingElement.HasElement(XmlNode_NoKanji) && xreadingElements.Count() > 1)
                    {
                        continue;
                    }

                    // Parse the reading. The list will be expanded and/or its elements filled with
                    // the available info.
                    ParseReading(xreadingElement, vocabList);
                }

                // Yield return all vocab entries parsed from this entry.
                foreach (VocabEntry entry in vocabList)
                {
                    yield return entry;
                }
            }
        }

        /// <summary>
        /// Parses a reading element node.
        /// Updates the list with the available info.
        /// </summary>
        /// <param name="xreadingElement">Element to parse.</param>
        /// <param name="vocabList">Vocab list to be updated.</param>
        private void ParseReading(XElement xreadingElement, List<VocabEntry> vocabList)
        {
            // First, we have to determine the target of the reading node.
            // Two possible cases:
            // - Scenario 1: There were no kanji readings. In that case, the reading should
            //   add a new vocab element which has no kanji reading.
            // - Scenario 2: There was at least one kanji reading. In that case, the reading
            //   node targets a set of existing vocabs. They may be filtered by kanji reading
            //   with the reading constraint nodes.

            VocabEntry[] targets;
            if (!vocabList.Any())
            {
                // Scenario 1. Create a new kanji reading, add it to the list, and set it as target.
                VocabEntry newVocab = new VocabEntry();
                vocabList.Add(newVocab);
                targets = new VocabEntry[] { newVocab };
            }
            else
            {
                // Scenario 2. Check constraint nodes to filter the targets.

                // Get all reading constraints in an array.
                string[] readingConstraints = xreadingElement.Elements(XmlNode_ReadingConstraint)
                    .Select(x => x.Value).ToArray();

                // Filter from the vocab list.
                if (readingConstraints.Any())
                {
                    targets = vocabList.Where(v => readingConstraints.Contains(v.KanjiReading)).ToArray();
                }
                else
                {
                    targets = vocabList.ToArray();
                }
            }

            // Now that we have the target vocabs, we can get the proper information from the node.
            string kanaReading = xreadingElement.Element(XmlNode_KanaReading).Value;

            // We have the info. Now we can apply it to the targets.
            // For each target
            foreach (VocabEntry target in targets)
            {
                // Set the kana reading if not already set.
                if (string.IsNullOrEmpty(target.KanaReading))
                {
                    target.KanaReading = kanaReading;
                }
                else if (vocabList.All(v => !(v.KanjiReading == target.KanjiReading && v.KanaReading == kanaReading)))
                {
                    // If a target already has a kana reading, we need to create a new vocab.
                    vocabList.Add(new VocabEntry()
                    {
                        KanjiReading = target.KanjiReading,
                        KanaReading = kanaReading
                    });
                }
            }
        }

        #endregion
    }
}
