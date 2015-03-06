# The JmdictFurigana project

(Latest furigana file release to be linked here)

## What is it?

This project aims to build an open-source furigana resource to complement the Jmdict dictionary file.
**Concretely, if you are building an application with the Jmdict file, you can use the output of this project to display beautiful furigana over your words instead of a plain kana string.**

### For non-initiates

The Jmdict is a Japanese word dictionary file that contains, for each entry:
- The kanji reading of the entry (e.g. "手を取る"), that you can consider like the "proper writing" of the entry. It contains kanji (ideographic) characters and may also contain kana (phonetic) characters.
- The kana readings of the entry (e.g. "てをとる" (tewotoru)), which is a kana (phonetic) string documenting the pronunciation of the entry. Each kanji character in the kanji reading has a matching pronunciation (one or more kana) that can vary depending on the expression it is used in. (e.g. 手 -> て (te) ; 取 -> と (to))
- (The definitions and other informations that are not relevant to this project)

Our goal is to attach the right parts of the kana reading to the kanji in the kanji reading.

![Capture](http://houhou-srs.com/file/Furigana.png)

## How does it work?

(Currently being redacted)
