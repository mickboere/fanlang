using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FanLang
{
	/// <summary>
	/// Base translator class, able to load a <see cref="TranslateSheetData"/> object which will
	/// be used to translate input text passed through <see cref="Translate(string)"/>.
	/// </summary>
	public class Translator
	{
		private Dictionary<string, List<TranslateHashData>> table;
		private int longestHash = 0;

		/// <summary>
		/// Creates a new <see cref="Translator"/> object.
		/// </summary>
		/// <param name="sheet">The data which will be used to translate the given input.</param>
		public Translator(TranslateSheetData sheet)
		{
			// We convert the translate sheet to a dictionary for easy access.
			table = new Dictionary<string, List<TranslateHashData>>();
			for (int i = 0; i < sheet.TranslateHashes.Count; i++)
			{
				TranslateHashData hash = sheet.TranslateHashes[i];
				if (!hash.Enabled || IsEmpty(hash.Output) || IsEmpty(hash.Input))
				{
					continue;
				}

				if (!table.ContainsKey(hash.Input))
				{
					table.Add(hash.Input, new List<TranslateHashData>());
				}
				table[hash.Input].Add(sheet.TranslateHashes[i]);

				// We store the longest available hash to get the max-check-length.
				if (hash.Input.Length > longestHash)
				{
					longestHash = sheet.TranslateHashes[i].Input.Length;
				}
			}
		}

		/// <summary>
		/// Replace the given string hash by hash.
		/// </summary>
		public string Translate(string text)
		{
			if (IsEmpty(text))
			{
				return "";
			}

			StringBuilder translated = new StringBuilder();
			for (int index = 0; index < text.Length; index++)
			{
				string output = null;

				// 1: If we encounter a tag, append the tag and continue to the next iteration.
				if (TryGetTag(text, index, out string tag, out _))
				{
					index += tag.Length - 1;
					translated.Append(tag);
					continue;
				}

				// 2: See if we can find a matching hash starting from the current character, starting with the longest possible.
				int checkLength = Mathf.Min(text.Length - index, longestHash);
				int nextIndex = index + checkLength;
				while (checkLength > 0)
				{
					RichSubstring(text, index, checkLength, out string input, out string openingTags, out string closingTags);
					nextIndex = index + openingTags.Length + input.Length + closingTags.Length;

					// Is the current input a prefix? Check whether the previous character is a letter.
					bool foundPreviousCharacter = TryGetPreviousChar(text, index, out char previousCharacter);
					bool prefix = index == 0 || (foundPreviousCharacter && !char.IsLetter(previousCharacter));

					// Is the current input a suffix? Check whether the next character is a letter.
					RichSubstring(text, nextIndex, 1, out string nextCharacter, out _, out _);
					bool suffix = nextIndex >= text.Length || (nextCharacter.Length > 0 && !char.IsLetter(nextCharacter[0]));

					if (table.ContainsKey(input.ToLower()))
					{
						string rawOutput = GetOutput(input.ToLower(), prefix, suffix);
						if (!IsEmpty(rawOutput))
						{
							output = openingTags + TransferCases(input, rawOutput.ToLower()) + closingTags;
							break;
						}
					}

					checkLength--;
				}

				if (output != null)
				{
					// A matching hash was found, append and continue.
					translated.Append(output);
					index = nextIndex - 1;
				}
				else
				{
					// No matching hash was found, append the current character and continue.
					translated.Append(text[index]);
				}
			}

			return translated.ToString();
		}

		/// <summary>
		/// Tries to get the previous character while reading over HTML tags.
		/// </summary>
		private bool TryGetPreviousChar(string text, int current, out char previousCharacter)
		{
			int index = current - 1;
			bool inTag = false;
			while (index >= 0)
			{
				char c = text[index];
				if (c == '>')
				{
					inTag = true;
				}
				else if (inTag && c == '<')
				{
					inTag = false;
				}
				else if (!inTag)
				{
					previousCharacter = c;
					return true;
				}

				index--;
			}

			previousCharacter = ' ';
			return false;
		}

		/// <summary>
		/// <see cref="string.Substring(int, int)"/> variant that reads over HTML tags and stores them separately.
		/// </summary>
		private void RichSubstring(string text, int startIndex, int length, out string input, out string openingTags, out string closingTags)
		{
			input = "";
			openingTags = "";
			closingTags = "";

			int index = startIndex;
			while (input.Length < length && index < text.Length)
			{
				if (TryGetTag(text, index, out string tag, out bool isClosingTag))
				{
					if (isClosingTag)
					{
						closingTags += tag;
					}
					else
					{
						openingTags += tag;
					}

					index += tag.Length;
				}
				else
				{
					input += text[index];
					index++;
				}
			}
		}

		private bool TryGetTag(string text, int start, out string tag, out bool isClosingTag)
		{
			if (text[start] != '<')
			{
				// We're definetely not at a tag.
				tag = null;
				isClosingTag = false;
				return false;
			}

			// Is the tag a closing tag or an opening tag?
			isClosingTag = start + 1 < text.Length && text.Substring(start, 2) == "</";

			tag = "";
			for (int i = start; i < text.Length; i++)
			{
				char c = text[i];
				tag += c;

				if (c == '>')
				{
					// Tag closed
					return true;
				}
				else if (i > start && c == '<')
				{
					// Opening another tag - return false
					return false;
				}
			}

			// Out of range; incomplete tag.
			return false;
		}

		private string TransferCases(string from, string to)
		{
			if (IsEmpty(from) || IsEmpty(to))
			{
				return to;
			}

			if (char.IsUpper(from[0]))
			{
				if (to.Length > 1)
				{
					return to[0].ToString().ToUpper() + to.Substring(1);
				}
				else
				{
					return to[0].ToString().ToUpper();
				}
			}

			return to;
		}

		private string GetOutput(string input, bool prefix, bool suffix)
		{
			List<TranslateHashData> hashes = table[input];
			if (prefix && suffix && HasHash(TranslateHashType.Word))
			{
				return GetHash(TranslateHashType.Word);
			}
			else if (prefix && HasHash(TranslateHashType.Prefix))
			{
				return GetHash(TranslateHashType.Prefix);
			}
			else if (suffix && HasHash(TranslateHashType.Suffix))
			{
				return GetHash(TranslateHashType.Suffix);
			}
			else if (HasHash(TranslateHashType.Default))
			{
				return GetHash(TranslateHashType.Default);
			}
			else
			{
				return null;
			}

			bool HasHash(TranslateHashType type)
			{
				return hashes.Any((x) => x.HashType == type);
			}

			string GetHash(TranslateHashType type)
			{
				return hashes.First((x) => x.HashType == type).Output;
			}
		}

		private bool IsEmpty(string s)
		{
			return string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s);
		}
	}
}
