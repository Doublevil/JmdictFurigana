namespace JmdictFurigana.Models
{
    /// <summary>
    /// Represents an individual part of the reading of a word or expression.
    /// </summary>
    public class ReadingPart
    {
        /// <summary>
        /// Gets or sets the text of the part (with kanji when applicable).
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the furigana of the part, if necessary (will be empty when the text is plain kana).
        /// </summary>
        public string Furigana { get; set; }
    }
}
