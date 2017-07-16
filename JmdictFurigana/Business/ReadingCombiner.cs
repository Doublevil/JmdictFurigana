using JmdictFurigana.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace JmdictFurigana.Business
{
    /// <summary>
    /// Provides a method that combines all potential readings between them
    /// and returns the list of possible combinations.
    /// </summary>
    public static class ReadingCombiner
    {
        /// <summary>
        /// Finds and returns all possible combinations of the given list of strings that respect
        /// the order.
        /// </summary>
        /// <param name="readings">List containing string lists that can be combined with others.</param>
        /// <param name="solutions">List containing all possible solutions that should be searched.</param>
        /// <returns>All possible combinations.</returns>
        public static List<string> CombineReadings(List<List<string>> readings, IEnumerable<string> solutions)
        {
            return CombineReadings(readings, string.Empty, solutions);
        }

        private static List<string> CombineReadings(List<List<string>> readings, string prefix, IEnumerable<string> solutions)
        {
            // Recursion exit conditions.
            if (!solutions.Where(s => s.StartsWith(prefix)).Any())
            {
                return new List<string>();
            }
            if (readings.Count == 1)
            {
                // If we only have one list of readings, just return these readings prepended
                // with the prefix.
                return readings.First().Select(s => prefix + s).Intersect(solutions).ToList();
            }

            // Recursion general behavior.
            List<string> output = new List<string>();
            // Take the first reading list.
            List<string> firstReadingList = readings.First();
            foreach (string reading in firstReadingList)
            {
                // For each reading of that first list, create a matching prefix and
                // make a recursive call without the first reading list.
                output.AddRange(CombineReadings(readings.Skip(1).ToList(), prefix + reading + SeparatorHelper.FuriganaSeparator, solutions));
            }

            return output;
        }
    }
}
