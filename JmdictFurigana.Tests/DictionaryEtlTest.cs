using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            DictionaryEtl dictionaryEtl = new DictionaryEtl(PathHelper.JmDictPath);
            List<string> wanted = new List<string>()
            {
                "ˆ«Œû|‚ ‚Á‚±‚¤",
                "ˆ«Œû|‚í‚é‚­‚¿",
                "ˆ«Œû|‚í‚é‚®‚¿",
                "œ¦Œû|‚ ‚Á‚±‚¤",
                "œ¦Œû|‚í‚é‚­‚¿",
                "œ¦Œû|‚í‚é‚®‚¿"
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
