using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JmdictFurigana.Models
{
    public class FuriganaPart : IComparable<FuriganaPart>, ICloneable
    {
        #region Properties

        /// <summary>
        /// Gets the index of the first character this furigana part applies for, in the kanji string
        /// of the related vocab.
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// Gets the index of the last character this furigana part applies for, in the kanji string
        /// of the related vocab.
        /// </summary>
        public int EndIndex { get; set; }

        /// <summary>
        /// Gets the furigana kana string.
        /// </summary>
        public string Value { get; set; }

        #endregion

        #region Constructors

        public FuriganaPart() { }

        public FuriganaPart(string value, int startIndex)
            : this(value, startIndex, startIndex) { }

        public FuriganaPart(string value, int startIndex, int endIndex)
        {
            Value = value;
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            if (StartIndex == EndIndex)
            {
                return string.Format("{0}:{1}", StartIndex, Value);
            }

            return string.Format("{0}-{1}:{2}", StartIndex, EndIndex, Value);
        }

        public int CompareTo(FuriganaPart other)
        {
            return StartIndex.CompareTo(other.StartIndex);
        }

        public override bool Equals(object obj)
        {
            FuriganaPart other = obj as FuriganaPart;
            if (other != null)
            {
                // Compare both furigana parts.
                return StartIndex == other.StartIndex
                    && EndIndex == other.EndIndex
                    && Value == other.Value;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public object Clone()
        {
            return new FuriganaPart(Value, StartIndex, EndIndex);
        }

        #endregion
    }
}
