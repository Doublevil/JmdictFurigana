using JmdictFurigana.Helpers;

namespace JmdictFurigana.Models
{
    /// <summary>
    /// Represents an entry of the word dictionary.
    /// </summary>
    public class VocabEntry
    {
        #region Properties

        /// <summary>
        /// Gets or sets the kanji reading of the dictionary entry.
        /// </summary>
        public string KanjiReading { get; set; }

        /// <summary>
        /// Gets or sets the kana reading of the dictionary entry.
        /// </summary>
        public string KanaReading { get; set; }

        #endregion

        #region Constructors

        public VocabEntry() { }

        public VocabEntry(string kanjiReading, string kanaReading)
        {
            KanjiReading = kanjiReading;
            KanaReading = kanaReading;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return string.Format("{0}{1}{2}", KanjiReading, SeparatorHelper.FileFieldSeparator, KanaReading);
        }

        #endregion
    }
}
