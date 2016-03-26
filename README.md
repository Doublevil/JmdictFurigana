# The JmdictFurigana project

[Download the latest release of the JmdictFurigana file.](https://github.com/Doublevil/JmdictFurigana/releases/latest)

## What is it?

This project aims to build an open-source furigana resource to complement the [Jmdict dictionary file](http://www.edrdg.org/jmdict/j_jmdict.html).

**Concretely, if you are building an application with the Jmdict file, you can use the output of this project to display beautiful furigana over your words instead of a plain kana string.**

### For non-initiates

The Jmdict is a Japanese word dictionary file that contains, for each entry:
- The **kanji reading** of the entry (e.g. "手を取る"), that you can consider like the "proper writing" of the entry. It contains kanji (ideographic) characters and may also contain kana (phonetic) characters.
- The **kana readings** of the entry (e.g. "てをとる" (tewotoru)), which is a kana (phonetic) string documenting the pronunciation of the entry. Each kanji character in the kanji reading has a matching pronunciation (one or more kana) that can vary depending on the expression it is used in. (e.g. 手 -> て (te) ; 取 -> と (to))
- (The definitions and other informations that are not relevant to this project)

Our goal is to attach the right parts of the kana reading to the right kanji in the kanji reading.

![Capture](http://houhou-srs.com/file/Furigana.png)

## How can I use it?

[Download the latest release of the furigana file.](https://github.com/Doublevil/JmdictFurigana/releases/latest)

The file is a text file containing lines of data following this format:
**&lt;kanji reading>|&lt;kana reading>|&lt;furigana string>**

The **&lt;furigana string>** itself consists of chains of the following pattern, separated by ';':

&lt;startIndex>(-&lt;endIndex>):&lt;kana string>

Indexes describe the position of the concerned characters in the kanji reading that are attached the kana string.
If the end index is not specified, the kana string applies only on the character at the start index.

### Let's take some examples

####頑張る|がんばる|0:がん;1:ば
- Our kanji reading is **頑張る**.
- Our kana reading is **がんばる**.
- Our furigana string is **0:がん;1:ば**. It contains two parts: **0:がん** and **1:ば**.
  - **0:がん** means that the がん applies to the character at index 0 in the kanji string, i.e. 頑.
  - **1:ば** means that the ば applies to the character at index 1 in the kanji string, i.e. 張.

####全日本|ぜんにほん|0:ぜん;1-2:にほん
- Our kanji reading is **全日本**.
- Our kana reading is **ぜんにほん**.
- Our furigana string is **0:ぜん;1-2:にほん**. It contains two parts: **0:ぜん** and **1-2:にほん**.
  - **0:ぜん** means that the ぜん applies to the character at index 0 in the kanji string, i.e. 全.
  - **1-2:にほん** means that the にほん applies to the characters between index 1 and 2 in the kanji string, i.e. 日本.

**Note:** In this last example, the expression "日本" uses a special reading: "にほん". This reading cannot be cut in に and ほん, and this is why our "にほん" furigana applies to the whole expression.

## How does it work?

The solver that finds out what kanji matches what kana string uses multiple algorithms that may solve specific cases.
The main algorithm uses the kanji readings read from the [kanjidic files](http://www.csse.monash.edu.au/~jwb/kanjidic.html). It browses the kanji reading and recursively tries to match the kana string using all possible combinations of readings.
This does not always work, because of special readings, missing readings and other oddities.

Other algorithms can solve entries with a kanji reading that contains only one kanji, entries where there are no consecutive kanji, and other specific cases.

These algorithms are run one after another and they all return the solutions found (if found). In the end, if there is only one solution, or if all solutions are equivalent, the single solution is retained.

There are also lists that contain exceptions and special readings. These lists are filled manually and will probably never be complete, given the massive amount of work that it represents.

The latest release of the Furigana file was built in **about two minutes and solved 173456 entries** out of 231625 (keep in mind that a lot of entries are not even possible to solve because they do not contain kanji).

## Fiability

While I do not guarantee that results are 100% accurate, they are verified with an algorithm that checks that no kanji is left without furigana and that the expression reads correctly.

I am aware of an issue that incorrectly cuts certain special expressions because of the same-length algorithm. I consider these issues minor in number and importance.

##Contribution and contact

If you have any questions or remarks regarding the project, or want to report errors, don't hesitate to file an issue or contact me through GitHub.

You can also contribute directly very easily if you notice an error with a special expression, by editing the *SpecialExpressions.txt* file.

##Release notes

1.1 (2016-03-26):
- Fixed [issue #2]https://github.com/Doublevil/JmdictFurigana/issues/2 (thank you stephenmac7)
- Updated the JMDict file to the latest version as of 2016-03-26
