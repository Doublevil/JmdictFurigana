using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JmdictFurigana.Models
{
    public class SpecialReading : IEqualityComparer<SpecialReading>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the kana reading string of the special reading.
        /// </summary>
        public string KanaReading { get; set; }

        /// <summary>
        /// Gets or sets the furigana solution of the special reading.
        /// </summary>
        public FuriganaSolution Furigana { get; set; }

        #endregion

        #region Constructors

        public SpecialReading()
            : this(string.Empty, null) { }

        public SpecialReading(string kanaReading, FuriganaSolution furigana)
        {
            KanaReading = kanaReading;
            Furigana = furigana;
        }

        #endregion

        #region Methods

        public bool Equals(SpecialReading x, SpecialReading y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }

            return x.KanaReading == y.KanaReading
                && x.Furigana.Equals(y.Furigana);
        }

        public int GetHashCode(SpecialReading obj)
        {
            return GetHashCode();
        }

        #endregion
    }
}
