using JmdictFurigana.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JmdictFurigana.Business;
using Newtonsoft.Json;

namespace JmdictFurigana
{
    /// <summary>
    /// Writes the furigana file.
    /// </summary>
    class FuriganaFileWriter
    {
        /// <summary>
        /// Gets or sets the value defining whether to write or not words for which a valid
        /// furigana string could not be determined.
        /// </summary>
        public bool WriteUnsuccessfulWords { get; set; }

        public HashSet<string> AlreadyWritten { get; set; } = new HashSet<string>();

        /// <summary>
        /// Gets or sets the path where the file is written.
        /// </summary>
        public string OutputPath { get; set; }

        public FuriganaFileWriter(string outputPath)
        {
            OutputPath = outputPath;
        }

        public void Write(IEnumerable<FuriganaSolutionSet> solutions)
        {
            int success = 0, total = 0;
            log4net.ILog logger = log4net.LogManager.GetLogger("Writer");
            DateTime start = DateTime.Now;

            string jsonFileName = $"{Path.GetFileNameWithoutExtension(OutputPath)}.json";

            using (var stream = new StreamWriter(OutputPath, false, Encoding.UTF8))
            using (var jsonStream = new StreamWriter(jsonFileName, false, Encoding.UTF8))
            using (var jsonWriter = new JsonTextWriter(jsonStream))
            {
                jsonWriter.WriteStartArray();
                var jsonSerializer = new JsonSerializer();
                jsonSerializer.Converters.Add(new FuriganaSolutionJsonSerializer());
                foreach (FuriganaSolutionSet solution in solutions)
                {
                    FuriganaSolution singleSolution = solution.GetSingleSolution();

                    if (solution.Any())
                    {
                        if (singleSolution == null)
                        {
                            logger.InfoFormat("➕   {0}", solution);
                        }
                        else
                        {
                            logger.InfoFormat("◯   {0}", solution);
                        }
                    }
                    else
                    {
                        logger.InfoFormat("X    {0}|{1}|???", solution.Vocab.KanjiReading, solution.Vocab.KanaReading);
                    }

                    if (singleSolution != null && !AlreadyWritten.Contains(singleSolution.ToString()))
                    {
                        stream.WriteLine(singleSolution.ToString());
                        AlreadyWritten.Add(singleSolution.ToString());
                        jsonSerializer.Serialize(jsonWriter, singleSolution);
                    }

                    if (singleSolution != null)
                    {
                        success++;
                    }

                    total++;
                }
                jsonWriter.WriteEndArray();
            }

            TimeSpan duration = DateTime.Now - start;
            logger.InfoFormat("Successfuly ended process with {0} out of {1} successfuly found furigana strings.", success, total);
            logger.InfoFormat("Process took {0} seconds.", duration.TotalSeconds);
        }
    }
}
