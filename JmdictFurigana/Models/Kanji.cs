using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JmdictFurigana.Models
{
    /// <summary>
    /// Represents a kanji.
    /// </summary>
    public class Kanji
    {
        /// <summary>
        /// Gets or sets the character representing the kanji.
        /// </summary>
        public char Character { get; set; }

        /// <summary>
        /// Gets or sets the list of possible readings of the kanji.
        /// </summary>
        public List<string> Readings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if this character should make the
        /// vocabulary entry containing it count as kanji strings.
        /// Default is true.
        /// </summary>
        public bool IsRealKanji { get; set; }

        public Kanji()
        {
            Readings = new List<string>();
            IsRealKanji = true;
        }

        public override string ToString()
        {
            return Character.ToString();
        }
    }
}
