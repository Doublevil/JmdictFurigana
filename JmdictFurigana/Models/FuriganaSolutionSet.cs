using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JmdictFurigana.Models
{
    /// <summary>
    /// Contains a set of furigana solutions that solves a vocab entry.
    /// </summary>
    public class FuriganaSolutionSet
    {
        #region Properties

        /// <summary>
        /// Gets or sets the solutions of this set.
        /// </summary>
        public List<FuriganaSolution> Solutions { get; set; }

        /// <summary>
        /// Gets or sets the vocab associated with this solution set.
        /// </summary>
        public VocabEntry Vocab { get; set; }

        #endregion

        #region Constructors

        public FuriganaSolutionSet(VocabEntry vocab)
        {
            Vocab = vocab;
            Solutions = new List<FuriganaSolution>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attempts to add the given solution to the set.
        /// </summary>
        /// <param name="solution">Solution to add.</param>
        /// <returns>True if the solution was added. False otherwise.</returns>
        public bool SafeAdd(FuriganaSolution solution)
        {
            if (!solution.Check())
            {
                // The specified solution is not valid.
                return false;
            }

            if (Solutions.Any(s => s.Equals(solution)))
            {
                // We already have an equivalent solution.
                return false;
            }

            // All is good.
            Solutions.Add(solution);
            return true;
        }

        /// <summary>
        /// SafeAdds several solutions.
        /// </summary>
        public int SafeAdd(IEnumerable<FuriganaSolution> solutions)
        {
            int count = 0;
            foreach (FuriganaSolution solution in solutions)
            {
                if (SafeAdd(solution))
                {
                    count++;
                }
            }

            return count;
        }

        public override string ToString()
        {
            if (!Solutions.Any())
            {
                return "???";
            }

            StringBuilder output = new StringBuilder();
            for (int i = 0; i < Solutions.Count; i++)
            {
                FuriganaSolution a = Solutions[i];
                if (i < Solutions.Count - 1)
                {
                    output.Append(string.Format("{0}, ", a));
                }
                else
                {
                    output.Append(a.ToString());
                }
            }

            return output.ToString();
        }

        /// <summary>
        /// Gets a value indicating if the set contains at least one solution.
        /// </summary>
        public bool Any()
        {
            return Solutions.Any();
        }

        /// <summary>
        /// Gets the single solution if there is only one solution, or null in
        /// any other case.
        /// </summary>
        public FuriganaSolution GetSingleSolution()
        {
            return Solutions.Count == 1 ? Solutions.First() : null;
        }

        #endregion
    }
}
