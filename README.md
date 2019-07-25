
# The JmdictFurigana project

[Download the latest release of the JmdictFurigana file.](https://github.com/Doublevil/JmdictFurigana/releases/latest)

## What is it?

This project aims to build an open-source furigana resource to complement the [EDICT/Jmdict](http://www.edrdg.org/jmdict/j_jmdict.html) and [ENAMDICT/Jmnedict](http://www.edrdg.org/enamdict/enamdict_doc.html) dictionary files.
What it does is provide a link between kanji reading and kana reading by attaching the kana portions on the right kanji characters in individual dictionary words.

**Concretely, if you are building an application with the EDICT/Jmdict file, you can use the output of this project to display beautiful furigana over your words instead of a plain kana string.**

## What it is *NOT*

JmdictFurigana is not a lexical parser. It is designed around individual words, not for sentences.

In other words, where lexical parsers are identifying words in a sentence or an expression, JmdictFurigana aims to identify individual kanji readings in a word.

As such, it is discouraged to use it in tools that provide furigana over entire sentences.

## For non-initiates

The EDICT (or Jmdict) and ENAMDICT (or Jmnedict) files are Japanese word dictionary files that contain, for each entry:
- The **kanji reading** of the entry (e.g. "頑張り屋"), that you can consider like the "proper writing" of the entry. It contains kanji (ideographic) characters and may also contain kana (phonetic) characters.
- The **kana readings** of the entry (e.g. "がんばりや" (ganbariya)), which is a kana (phonetic) string documenting the pronunciation of the entry. Each kanji character in the kanji reading has a matching pronunciation (one or more kana) that can vary depending on the expression it is used in. (e.g. 頑 -> がん (gan) ; 張 -> ば (ba) ; 屋 -> や (ya))
- (The definitions and other informations that are not relevant to this project)

Our goal is to attach the right parts of the kana reading to the right kanji in the kanji reading.

![Capture](http://houhou-srs.com/file/Furigana2.png)

## How can I use it?

[Download the latest release of the furigana files.](https://github.com/Doublevil/JmdictFurigana/releases/latest)
There are two sets of files you can use: either the json files, or the compact plain text format.

### How to use the Json files
There are two files you can use:
- **JmdictFurigana.json** provides furigana for the EDICT (or JMDict) dictionary file entries.
- **JmnedictFurigana.json** provides furigana for the ENAMDICT (or JMnedict) dictionary file entries. **Use this one for proper names only.**

Please note that the json files available in the releases are zipped using gzip (hence the .gz file extension), because they are very large. You may need a third-party zip utility to unzip them.

Both files are formatted in the exact same way: they are a json array containing entries as objects in the following format:
- **text**: **string** containing the kanji reading of the entry.
- **reading**: **string** containing the kana reading of the entry.
- **furigana**: **array** containing each individual reading part, in order of reading, as objects containing:
	- **ruby**: **string** containing the text of the reading part, which may contain kanji.
	- **rt**: **optional string** containing the furigana for the text in the *ruby* field, when applicable. Will be left out in plain kana reading parts.

#### Example Json entry
Here is an example entry from the JmdictFurigana.json file:

```
{
  "text": "大人買い",
  "reading": "おとながい",
  "furigana": [
    {
      "ruby": "大人",
      "rt": "おとな"
    }, {
      "ruby": "買",
      "rt": "が"
    }, {
      "ruby": "い"
    }
  ]
}
```

In this example, the word is 大人買い, read as おとながい, and the *furigana* array breaks it down in 3 parts:
- 大人 read as おとな
- 買 read as が
- い which is plain kana and thus does not need furigana (no *rt* value).

**Note:** In this example, the expression "大人" uses a special reading: "おとな". This reading cannot be cut in お and とな or おと and な. This is why the "おとな" furigana applies to the whole expression.

### How to use the plain text format
Please note that this format is historical. It's probably a better idea to use the json files instead.

There are two files you can use:
- **JmdictFurigana.txt** provides furigana for the EDICT (or JMDict) dictionary file entries.
- **JmnedictFurigana.txt** provides furigana for the ENAMDICT (or JMnedict) dictionary file entries. **Use this one for proper names only.**

Both files are text files containing lines of data following this format:
**&lt;kanji reading>|&lt;kana reading>|&lt;furigana string>**

The **&lt;furigana string>** itself consists of chains of the following pattern, separated by ';':

&lt;startIndex>(-&lt;endIndex>):&lt;kana string>

Indexes describe the position of the concerned characters in the kanji reading that are attached the kana string.
If the end index is not specified, the kana string applies only on the character at the start index.

### Let's take some examples

#### 頑張る|がんばる|0:がん;1:ば
- Our kanji reading is **頑張る**.
- Our kana reading is **がんばる**.
- Our furigana string is **0:がん;1:ば**. It contains two parts: **0:がん** and **1:ば**.
  - **0:がん** means that the がん applies to the character at index 0 in the kanji string, i.e. 頑.
  - **1:ば** means that the ば applies to the character at index 1 in the kanji string, i.e. 張.

#### 大人買い|おとながい|0-1:おとな;2:が
- Our kanji reading is **大人買い**.
- Our kana reading is **おとながい**.
- Our furigana string is **0-1:おとな;2:が**. It contains two parts: **0-1:おとな** and **2:が**.
  - **0-1:おとな** means that the おとな furigana applies to the characters between index 0 and 1 in the kanji string, i.e. 大人.
  - **2:が** means that the が furigana applies to the character at index 2 in the kanji string, i.e. 買.

**Note:** In this last example, the expression "大人" uses a special reading: "おとな". This reading cannot be cut in お and とな or おと and な. This is why our "おとな" furigana applies to the whole expression.

## How does it work?

The solver that finds out what kanji matches what kana string uses multiple algorithms that may solve specific cases.
The main algorithm uses the kanji readings read from the [kanjidic files](http://www.csse.monash.edu.au/~jwb/kanjidic.html). It browses the kanji reading and recursively tries to match the kana string using all possible combinations of readings.
This does not always work, because of special readings, missing readings and other oddities.

Other algorithms can solve entries with a kanji reading that contains only one kanji, entries where there are no consecutive kanji, and other specific cases.

These algorithms are run one after another and they all return the solutions found (if found). In the end, if there is only one solution, or if all solutions are equivalent, the single solution is retained.

There are also lists that contain exceptions and special readings. These lists are filled manually and will probably never be complete, given the massive amount of work that it represents.

The latest release of the Furigana file for the Jmdict was built in **about two minutes and solved 177702 entries** out of 234814 (keep in mind that a lot of entries are not even possible to "solve" because they do not contain kanji).

The latest Jmnedict file solved 583619 out of 740802 entries in about 5 minutes.

## Fiability

While results are not 100% accurate, they are verified with an algorithm that checks that no kanji is left without furigana and that the expression reads correctly.

I am aware of an issue that incorrectly cuts certain special expressions because of the same-length algorithm. I consider these issues minor in number and importance.

The JmnedictFurigana file is a bit experimental. Quick checks show it seems to work, but don't hesitate to report issues with it.

## Running the solution

The solution is missing the ./JmdictFurigana/Resources/JMnedict.xml file because it is too big to commit here. You can download it on [the ENAMDICT/Jmnedict project page](http://www.edrdg.org/enamdict/enamdict_doc.html).

## Contribution and contact

If you have any questions or remarks regarding the project, or want to report errors, don't hesitate to file an issue or contact me through GitHub.

You can also contribute directly very easily if you notice an error with a special expression, by editing the *SpecialExpressions.txt* file.

## Licence

This resource is distributed under the same licence as JMDict (Creative Commons Attribution-ShareAlike Licence).

## Release notes

2.1 (2019-07-24):
- Implemented [issue #11](https://github.com/Doublevil/JmdictFurigana/issues/11): the project now also outputs a json file with data presented in an alternative way, which should make it a lot easier to parse. Both formats will continue to be supported. Thanks, fasiha.

2.0 (2017-07-16):
- Implemented [issue #8](https://github.com/Doublevil/JmdictFurigana/issues/8), which means we now have a separate furigana file for the ENAMDICT/Jmnedict proper name dictionary file.

1.4 (2016-11-13):
- Fixed [issue #5](https://github.com/Doublevil/JmdictFurigana/issues/5) (thank you yayoo1971)

1.3 (2016-08-21):
- Fixed [issue #4](https://github.com/Doublevil/JmdictFurigana/issues/4) (thank you again fasiha)
- Added lots of special readings to cover for other potential cases of missing readings. This brought up the number of solved entries by a few thousand.

1.2 (2016-04-10):
- Fixed [issue #3](https://github.com/Doublevil/JmdictFurigana/issues/3) (thank you fasiha)

1.1 (2016-03-26):
- Fixed [issue #2](https://github.com/Doublevil/JmdictFurigana/issues/2) (thank you stephenmac7)
- Updated the JMDict file to the latest version as of 2016-03-26
