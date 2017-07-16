using JmdictFurigana.Etl;
using JmdictFurigana.Helpers;
using JmdictFurigana.Models;

namespace JmdictFurigana
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Initialize log4net.
            log4net.Config.XmlConfigurator.Configure();

            // Jmdict
            DictionaryEtl jmdictEtl = new DictionaryEtl(PathHelper.JmDictPath);
            FuriganaBusiness furiganaJmdict = new FuriganaBusiness(DictionaryFile.Jmdict);
            FuriganaFileWriter jmdictWriter = new FuriganaFileWriter(PathHelper.JmdictOutFilePath);
            jmdictWriter.Write(furiganaJmdict.Execute(jmdictEtl.Execute()));

            // Jmnedict
            DictionaryEtl jmnedictEtl = new DictionaryEtl(PathHelper.JmneDictPath);
            FuriganaBusiness furiganaJmnedict = new FuriganaBusiness(DictionaryFile.Jmnedict);
            FuriganaFileWriter jmnedictWriter = new FuriganaFileWriter(PathHelper.JmnedictOutFilePath);
            jmnedictWriter.Write(furiganaJmnedict.Execute(jmnedictEtl.Execute()));
        }
    }
}
