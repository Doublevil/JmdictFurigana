using JmdictFurigana.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JmdictFurigana.Business
{
    /// <summary>
    /// A class that aims to provide the furigana business with solutions for tested
    /// vocab entries.
    /// </summary>
    public abstract class FuriganaSolver : IComparable<FuriganaSolver>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the priority of the solutions given by this solver.
        /// Only results found with the maximal priority should be taken in account.
        /// </summary>
        public int Priority { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Attempts to solve the given vocab entry.
        /// </summary>
        /// <param name="r">Set of resources required by solvers.</param>
        /// <param name="v">Entry to attempt to solve.</param>
        /// <returns>The solutions found, if any.</returns>
        public IEnumerable<FuriganaSolution> Solve(FuriganaResourceSet r, VocabEntry v)
        {
            foreach (FuriganaSolution solution in DoSolve(r, v))
            {
                if (!solution.Check())
                {
                    throw new Exception("The solution did not pass the check test.");
                }

                yield return solution;
            }
        }

        protected abstract IEnumerable<FuriganaSolution> DoSolve(FuriganaResourceSet r, VocabEntry v);

        /// <summary>
        /// Comparator for override solvers.
        /// Provided to sort furigana solvers by priority.
        /// </summary>
        public int CompareTo(FuriganaSolver other)
        {
            return Priority.CompareTo(other.Priority);
        }

        #endregion
    }
}
