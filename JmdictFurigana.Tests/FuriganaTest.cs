using System.Linq;
using JmdictFurigana.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JmdictFurigana.Tests
{
    [TestClass]
    public class FuriganaTest
    {
        [TestMethod]
        public void Test_Furigana_Ganbaru()
        {
            Test_Furigana("頑張る", "がんばる", "0:がん;1:ば");
        }

        [TestMethod]
        public void Test_Furigana_Ikkagetsu()
        {
            Test_Furigana("一ヶ月", "いっかげつ", "0:いっ;1:か;2:げつ");
        }

        [TestMethod]
        public void Test_Furigana_Obocchan()
        {
            Test_Furigana("御坊っちゃん", "おぼっちゃん", "0:お;1:ぼ");
        }

        [TestMethod]
        public void Test_Furigana_Ara()
        {
            // Will fail. This is a weird kanji. The string containing only the kanji is Length == 2.
            // Would be cool to find a solution but don't worry too much about it.
            Test_Furigana("𩺊", "あら", "0:あら");
        }

        [TestMethod]
        public void Test_Furigana_Ijirimawasu()
        {
            Test_Furigana("弄り回す", "いじりまわす", "0:いじ;2:まわ");
        }

        [TestMethod]
        public void Test_Furigana_Kassarau()
        {
            Test_Furigana("掻っ攫う", "かっさらう", "0:か;2:さら");
        }

        [TestMethod]
        public void Test_Furigana_Oneesan()
        {
            Test_Furigana("御姉さん", "おねえさん", "0:お;1:ねえ");
        }

        [TestMethod]
        public void Test_Furigana_Hakabakashii()
        {
            Test_Furigana("捗捗しい", "はかばかしい", "0:はか;1:ばか");
        }

        [TestMethod]
        public void Test_Furigana_Issue5()
        {
            Test_Furigana("御兄さん", "おにいさん", "0:お;1:にい");
            Test_Furigana("御姉さん", "おねえさん", "0:お;1:ねえ");
            Test_Furigana("御母さん", "おかあさん", "0:お;1:かあ");
            Test_Furigana("抑抑", "そもそも", "0:そも;1:そも");
            Test_Furigana("犇犇", "ひしひし", "0:ひし;1:ひし");
            Test_Furigana("険しい路", "けわしいみち", "0:けわ;3:みち");
            Test_Furigana("芝生", "しばふ", "0-1:しばふ");
            Test_Furigana("純日本風", "じゅんにほんふう", "0:じゅん;1-2:にほん;3:ふう");
            Test_Furigana("真珠湾", "しんじゅわん", "0:しん;1:じゅ;2:わん");
            Test_Furigana("草履", "ぞうり", "0-1:ぞうり");
            Test_Furigana("大和魂", "やまとだましい", "0-1:やまと;2:だましい");
            Test_Furigana("竹刀", "しない", "0-1:しない");
            Test_Furigana("東京湾", "とうきょうわん", "0:とう;1:きょう;2:わん");
            Test_Furigana("日本学者", "にほんがくしゃ", "0-1:にほん;2:がく;3:しゃ");
            Test_Furigana("日本製", "にほんせい", "0-1:にほん;2:せい");
            Test_Furigana("日本側", "にほんがわ", "0-1:にほん;2:がわ");
            Test_Furigana("日本刀", "にほんとう", "0-1:にほん;2:とう");
            Test_Furigana("日本風", "にほんふう", "0-1:にほん;2:ふう");
            Test_Furigana("木ノ葉", "このは", "0:こ;2:は");
            Test_Furigana("木ノ葉", "きのは", "0:き;2:は");
            Test_Furigana("余所見", "よそみ", "0:よ;1:そ;2:み");
            Test_Furigana("嗹", "れん", "0:れん");
            Test_Furigana("愈愈", "いよいよ", "0:いよ;1:いよ");
            Test_Furigana("偶偶", "たまたま", "0:たま;1:たま");
            Test_Furigana("益益", "ますます", "0:ます;1:ます");
            Test_Furigana("風邪薬", "かぜぐすり", "0-1:かぜ;2:ぐすり");
            Test_Furigana("日独協会", "にちどくきょうかい", "0:にち;1:どく;2:きょう;3:かい");
        }
        
        public void Test_Furigana(string kanjiReading, string kanaReading, string expectedFurigana)
        {
            VocabEntry v = new VocabEntry(kanjiReading, kanaReading);
            FuriganaBusiness business = new FuriganaBusiness(DictionaryFile.Jmdict);
            FuriganaSolutionSet result = business.Execute(v);

            if (result.GetSingleSolution() == null)
            {
                Assert.Fail();
            }
            else
            {
                Assert.AreEqual(FuriganaSolution.Parse(expectedFurigana, v), result.GetSingleSolution());
            }
        }

        [TestMethod]
        public void Test_BreakIntoParts_Akagaeruka()
        {
            var vocab = new VocabEntry("アカガエル科", "アカガエルか");
            var solution = new FuriganaSolution(vocab, new FuriganaPart("か", 5));

            var parts = solution.BreakIntoParts().ToList();

            Assert.AreEqual(2, parts.Count);
            Assert.AreEqual("アカガエル", parts[0].Text);
            Assert.IsNull(parts[0].Furigana);
            Assert.AreEqual("科", parts[1].Text);
            Assert.AreEqual("か", parts[1].Furigana);
        }

        [TestMethod]
        public void Test_BreakIntoParts_Otonagai()
        {
            var vocab = new VocabEntry("大人買い", "おとながい");
            var solution = new FuriganaSolution(vocab, new FuriganaPart("おとな", 0, 1), new FuriganaPart("が", 2));

            var parts = solution.BreakIntoParts().ToList();

            Assert.AreEqual(3, parts.Count);
            Assert.AreEqual("大人", parts[0].Text);
            Assert.AreEqual("おとな", parts[0].Furigana);
            Assert.AreEqual("買", parts[1].Text);
            Assert.AreEqual("が", parts[1].Furigana);
            Assert.AreEqual("い", parts[2].Text);
            Assert.IsNull(parts[2].Furigana);
        }
    }
}
