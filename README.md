# FanLang

A tool for creating fictional languages by reading input text and replacing its characters through a data set.

[Quick video demonstration of everything below can be found here](https://www.youtube.com/watch?v=Vk4Ewopi_K4)

![](https://mickboere.com/wp-content/uploads/2021/01/ExampleLanguageScreenshot.png)

> Example project demonstrating how I go about creating languages. Download the example project [HERE](https://drive.google.com/file/d/1afVMiwYk5kExp4PT5wBZzKlSC8fi9DKt/view?usp=sharing).
> Note that the example project does not contain the example input, since input is saved in player prefs (per system).

## Who is this for?

You, if you are into worldbuilding and the like but are too lazy to figure out how to create proper languages.
Hell, even if you aren't into worldbuilding this software could probably be used for many other things than just creating languages. You could turn it into an UWU generator, replacing all the R's and L's with W's. You could turn it into any type of generator you want as long as it's possible to reach your desired result through reading and replacing strings of characters.

**The beauty of FanLang is that you don't have to make up complex grammar and lingual rules, since your input language already contains all the necessary logics.**

## How does it work?

Being a fan of simple things, I decided to to keep this application as simple as possible. FanLang operates on a simple read and replace principle. Character by character, the software tries to find a match from a list of input hashes. Should a matching input be found, we write away the corresponding output to our final translation. If no matching hash is found, we keep the original input.

By utilizing this simple set of provided tools, the complexity of the fantasy language is limited almost solely by the lengths the user is willing to go in defining the translate rules (hashes).

Find a more detailed breakdown of the exact steps the application follows to translate input to output below:

 1. From our current character index in the input, read ahead by the length of the longest possible match.
 2. Check the context of the input string in the following order: Is the input a word? (no letter before and no letter after) Is the input a prefix? (no letter before) Is the input a suffix? (no letter after)
 3. Until a match is found, keep decreasing our match check length.
 4. If no match is found at all, write away the current input character to our translation.
 5. If a match is found, write away the corresponding output to our translation and increase the current index by the length of the matching input hash.
 6. Repeat.

This set of steps is repeated for EACH defined *sheet*, using the output from the previous sheet as the input for the next.

The **order of priority** in terms of replacing is:

1. **Input length:** Longer hashes have priority over shorter hashes.
2. **Context** (Hash Type): Word > Prefix > Suffix > Default

## Features

 - Translate input to output through a definable data set.
 - Stackable translate sheets to allow for exponentially complex languages.
 - Enabling / disabling sheets / hashes.
 - Sorting by input / output / hash type.
 - Saving / loading projects.
 - Undo (will undo all changes by reloading the data set)
 - Automatically saving / loading input text on a per-project basis (saved in player prefs, not in the project itself)
 - Linked scrolling.
 - Highlighting translated text by selecting input text.

## Planned Features

A list of "planned" features that are by no means actually planned to be in the application some specific day but are rather nice-to-haves in case I ever have time to add them.
Or, since it's open source, if anyone else has time to add them ;)

- Multi threaded translation. (easy)
- Highlighting of hashes depending on similarity when hovering over an input or output field. (normal)
- Highlighting of hashes within the input and output when hovering over a hash. (hard)

## Known Issues

- Saving a new project will clear the input text, presumably because the project ID changes. (low prio)
- User is able to insert '<' and '>' unless when typing at the end of the input. The user should never be able to enter these characters since it could mess with the reading functionality. Either that, or the reader should be able to handle those characters while still being able to apply the < mark > tags used for highlighting the selected text.

## Open Source

This tool was created in Unity 3D (C#) for no other reason than that I'm very comfortable with Unity and didn't feel like figuring out Windows Forms. But since this project is open source, if you're willing to recreate the UI it should be very easy to convert it to whatever software you like since I decoupled the UI, data and functionality as much as possible.

To use this repo in a Unity project, you can simply add it as a submodule to your project repo.

## FAQish

> I found this issue.

Great! Please report it and I'll see if I can fix it anytime soon.

> I don't like the UI.

Feel free to change it.

> I'm missing feature X.

Feel free to create it and open a pull request.

> I have the most amazing idea for a new feature!

Awesome! Feel free to create it and open a pull request.

> I'm missing feature X but I'm not a programmer.

Feel free to request it, I can put it on the "planned" features list but I can't promise it will ever be made.

## License

> MIT License
> 
> Copyright (c) 2021 Mick Boere
> 
> Permission is hereby granted, free of charge, to any person obtaining
> a copy of this software and associated documentation files (the
> "Software"), to deal in the Software without restriction, including
> without limitation the rights to use, copy, modify, merge, publish,
> distribute, sublicense, and/or sell copies of the Software, and to
> permit persons to whom the Software is furnished to do so, subject to
> the following conditions:
> 
> The above copyright notice and this permission notice shall be
> included in all copies or substantial portions of the Software.
> 
> THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
> EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
> MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
> IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
> CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
> TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
> SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

Includes the [UnityStandaloneFileBrowser](https://github.com/gkngkc/UnityStandaloneFileBrowser).

