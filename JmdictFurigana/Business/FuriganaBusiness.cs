using JmdictFurigana.Business;
using JmdictFurigana.Helpers;
using JmdictFurigana.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace JmdictFurigana
{
    /// <summary>
    /// Works with kanji and dictionary entries to attach each entry a furigana string.
    /// </summary>
    public class FuriganaBusiness
    {
        #region Fields

        private log4net.ILog _log;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the resource set that will be sent to solvers.
        /// </summary>
        public FuriganaResourceSet ResourceSet { get; set; }

        /// <summary>
        /// Gets or sets the furigana solver list.
        /// </summary>
        public List<FuriganaSolver> Solvers { get; set; }

        #endregion

        #region Constructors

        public FuriganaBusiness()
        {
            _log = log4net.LogManager.GetLogger(this.GetType());
            Initialize();
        }

        public FuriganaBusiness(FuriganaResourceSet resourceSet)
            : this()
        {
            ResourceSet = resourceSet;
        }

        public FuriganaBusiness(FuriganaResourceSet resourceSet, List<FuriganaSolver> solvers)
            : this(resourceSet)
        {
            Solvers = solvers;
        }

        #endregion

        #region Methods

        protected void Initialize()
        {
            if (ResourceSet == null)
            {
                ResourceSet = new FuriganaResourceSet();
                ResourceSet.Load();
            }

            if (Solvers == null)
            {
                Solvers = new List<FuriganaSolver>()
                {
                    new KanaReadingSolver(),
                    new KanjiReadingSolver(),
                    new LengthMatchSolver(),
                    new NoConsecutiveKanjiSolver(),
                    new OverrideSolver(),
                    new RepeatedKanjiSolver(),
                    new SingleCharacterSolver(),
                    new SingleKanjiSolver()
                };
            }
            Solvers.Sort();
            Solvers.Reverse();
        }

        /// <summary>
        /// Starts the process of associating a furigana string to vocab.
        /// </summary>
        /// <returns>The furigana vocab entries.</returns>
        public IEnumerable<FuriganaSolutionSet> Execute(IEnumerable<VocabEntry> vocab)
        {
            foreach (VocabEntry v in vocab)
            {
                yield return Execute(v);
            }
        }

        public FuriganaSolutionSet Execute(VocabEntry v)
        {
            if (v.KanjiReading == null || v.KanaReading == null || string.IsNullOrWhiteSpace(v.KanjiReading))
            {
                // Cannot solve when we do not have a kanji or kana reading.
                return new FuriganaSolutionSet(v);
            }

            FuriganaSolutionSet result = Process(v);
            if (!result.Any() && v.KanjiReading.StartsWith("御"))
            {
                // When a word starts with 御 (honorific, often used), try to override the
                // result by replacing it with an お or a ご. It will sometimes bring a
                // result where the kanji form wouldn't.

                result = Process(new VocabEntry(v.KanaReading, "お" + v.KanjiReading.Substring(1)));

                if (!result.Any())
                {
                    result = Process(new VocabEntry(v.KanaReading, "ご" + v.KanjiReading.Substring(1)));
                }

                result.Vocab = v;
            }

            return result;
        }

        private FuriganaSolutionSet Process(VocabEntry v)
        {
            FuriganaSolutionSet solutionSet = new FuriganaSolutionSet(v);

            int priority = Solvers.First().Priority;
            foreach (FuriganaSolver solver in Solvers)
            {
                if (solver.Priority < priority)
                {
                    if (solutionSet.Any())
                    {
                        // Priority goes down and we already have solutions.
                        // Stop solving.
                        break;
                    }

                    // No solutions yet. Continue with the next level of priority.
                    priority = solver.Priority;
                }

                // Add all solutions if they are correct and unique.
                solutionSet.SafeAdd(solver.Solve(ResourceSet, v));
            }

            return solutionSet;
        }

        #endregion
    }
}
