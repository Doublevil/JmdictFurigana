using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JmdictFurigana.Models
{
    /// <summary>
    /// Represents a special reading expression.
    /// For example, 大人 - おとな can't be cut as おと.な or お.とな.
    /// These readings are loaded from the SpecialReadings.txt file.
    /// </summary>
    public class SpecialExpression
    {
        #region Properties

        /// <summary>
        /// Gets or sets the kanji reading of the expression.
        /// </summary>
        public string KanjiReading { get; set; }

        /// <summary>
        /// Gets or sets the readings of the expression.
        /// </summary>
        public List<SpecialReading> Readings { get; set; }

        #endregion

        #region Constructors

        public SpecialExpression()
            : this(string.Empty, new List<SpecialReading>()) { }

        public SpecialExpression(string kanjiReading, params SpecialReading[] readings)
            : this(kanjiReading, readings.ToList()) { }

        public SpecialExpression(string kanjiReading, List<SpecialReading> readings)
        {
            KanjiReading = kanjiReading;
            Readings = readings;
        }

        #endregion
    }
}
