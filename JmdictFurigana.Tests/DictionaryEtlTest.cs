using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JmdictFurigana.Etl;
using JmdictFurigana.Helpers;
using JmdictFurigana.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JmdictFurigana.Tests
{
    [TestClass]
    public class DictionaryEtlTest
    {
        [TestMethod]
        public void ExecuteTest_Waruguchi_FourReadings()
        {
            // Arrange
            File.Copy("Resources/Waruguchi.xml", PathHelper.JmDictPath, true);
            DictionaryEtl dictionaryEtl = new DictionaryEtl();
            List<string> wanted = new List<string>()
            {
                "悪口|あっこう",
                "悪口|わるくち",
                "悪口|わるぐち",
                "惡口|あっこう",
                "惡口|わるくち",
                "惡口|わるぐち"
            };

            // Act
            List<VocabEntry> results = dictionaryEtl.Execute().ToList();
            List<string> resultsAsStrings = results.Select(r => r.ToString()).ToList();

            // Assert
            Assert.AreEqual(6, results.Count);
            CollectionAssert.AreEquivalent(wanted, resultsAsStrings);
        }
    }
}
