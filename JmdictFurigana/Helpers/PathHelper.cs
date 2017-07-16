using System.IO;

namespace JmdictFurigana.Helpers
{
    /// <summary>
    /// Provides paths to the external resources.
    /// </summary>
    public static class PathHelper
    {
        public static readonly string ResourcesBasePath = "Resources";
        public static readonly string JmDictPath = Path.Combine(ResourcesBasePath, "JMdict.xml");
        public static readonly string JmneDictPath = Path.Combine(ResourcesBasePath, "JMnedict.xml");
        public static readonly string KanjiDic2Path = Path.Combine(ResourcesBasePath, "kanjidic2.xml");
        public static readonly string OverrideFuriganaPath = Path.Combine(ResourcesBasePath, "OverrideFurigana.txt");
        public static readonly string SupplementaryKanjiPath = Path.Combine(ResourcesBasePath, "SupplementaryKanji.txt");
        public static readonly string SpecialReadingsPath = Path.Combine(ResourcesBasePath, "SpecialReadings.txt");

        public static readonly string JmdictOutFilePath = "JmdictFurigana.txt";
        public static readonly string JmnedictOutFilePath = "JmnedictFurigana.txt";
    }
}
