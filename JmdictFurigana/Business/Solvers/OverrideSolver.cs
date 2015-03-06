using JmdictFurigana.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JmdictFurigana.Business
{
    public class OverrideSolver : FuriganaSolver
    {
        #region Constructors

        public OverrideSolver()
        {
            Priority = 9999; // Critical hit.
        }

        #endregion

        /// <summary>
        /// Attempts to solve furigana by looking up for solutions in the override list.
        /// </summary>
        protected override IEnumerable<FuriganaSolution> DoSolve(FuriganaResourceSet r, VocabEntry v)
        {
            FuriganaSolution solution = r.GetOverride(v);
            if (solution != null)
            {
                yield return new FuriganaSolution()
                    {
                        Furigana = solution.Furigana,
                        Vocab = v
                    };
            }
        }
    }
}
