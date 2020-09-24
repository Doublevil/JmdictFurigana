using System.Collections.Generic;
using System.IO;
using System.Linq;
using JmdictFurigana.Etl;
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
            DictionaryEtl dictionaryEtl = new DictionaryEtl(Path.Combine("Resources", "Waruguchi.xml"));
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
            CollectionAssert.AreEquivalent(wanted, resultsAsStrings);
        }
    }
}
