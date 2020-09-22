using System.IO;
using System.IO.Compression;
using System.Net;
using JmdictFurigana.Helpers;

namespace JmdictFurigana.Business
{
    /// <summary>
    /// Downloads the dictionary files used by the program.
    /// </summary>
    public class ResourceDownloader
    {
        private const string Kanjidic2Uri = "http://www.edrdg.org/kanjidic/kanjidic2.xml.gz";

        // Note that we use the English-only version of the Jmdict file, because it's lighter and we don't need translations
        private const string JmdictUri = "ftp://ftp.monash.edu.au/pub/nihongo/JMdict_e.gz";

        private const string JmnedictUri = "http://ftp.monash.edu/pub/nihongo/JMnedict.xml.gz";

        /// <summary>
        /// Downloads the Kanjidic2 XML file to its resource path.
        /// </summary>
        public void DownloadKanjidic()
        {
            DownloadGzFile(Kanjidic2Uri, PathHelper.KanjiDic2Path);
        }

        /// <summary>
        /// Downloads the JMdict file to its resource path.
        /// </summary>
        public void DownloadJmdict()
        {
            DownloadGzFile(JmdictUri, PathHelper.JmDictPath);
        }

        /// <summary>
        /// Downloads the JMnedict file to its resource path.
        /// </summary>
        public void DownloadJmnedict()
        {
            DownloadGzFile(JmnedictUri, PathHelper.JmneDictPath);
        }

        /// <summary>
        /// Downloads and unzips the gzipped file at the given URI, and stores it to the given path.
        /// </summary>
        /// <param name="uri">URI of the file to obtain.</param>
        /// <param name="targetPath">Path of the resulting file.</param>
        private void DownloadGzFile(string uri, string targetPath)
        {
            using var webClient = new WebClient();
            using var httpStream = webClient.OpenRead(uri);
            using var gzipStream = new GZipStream(httpStream, CompressionMode.Decompress);
            using var fileStream = new FileStream(targetPath, FileMode.Create);
            gzipStream.CopyTo(fileStream);
        }
    }
}
