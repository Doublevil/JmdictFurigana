using JmdictFurigana.Etl;
using JmdictFurigana.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JmdictFurigana
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Initialize log4net.
            log4net.Config.XmlConfigurator.Configure();

            DictionaryEtl dictionaryEtl = new DictionaryEtl();
            FuriganaBusiness furigana = new FuriganaBusiness();

            FuriganaFileWriter writer = new FuriganaFileWriter();
            //writer.WriteUnsuccessfulWords = true;
            writer.Write(furigana.Execute(dictionaryEtl.Execute()));

            Console.ReadKey(false);
        }
    }
}
