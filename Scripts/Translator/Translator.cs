using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FanLang
{
	public class Translator
	{
		private Dictionary<string, List<TranslateHashData>> table;
		private int longestHash = 0;

		public Translator(TranslateSheetData sheet)
		{
			// We convert the translate sheet to a dictionary for easy access.
			table = new Dictionary<string, List<TranslateHashData>>();
			for (int i = 0; i < sheet.TranslateHashes.Count; i++)
			{
				string input = sheet.TranslateHashes[i].Input;
				if (!table.ContainsKey(input))
				{
					table.Add(input, new List<TranslateHashData>());
				}
				table[input].Add(sheet.TranslateHashes[i]);

				// We store the longest available hash to get the max-check-length.
				if (input.Length > longestHash)
				{
					longestHash = sheet.TranslateHashes[i].Input.Length;
				}
			}
		}

		public string Translate(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return "";
			}

			StringBuilder translated = new StringBuilder();
			for (int index = 0; index < text.Length; index++)
			{
				string output = null;
				int checkLength = Mathf.Min(text.Length - index, longestHash);
				while (checkLength > 0)
				{
					bool prefix = index == 0 || !char.IsLetter(text[index - 1]);
					bool suffix = index + checkLength >= text.Length - 1 || !char.IsLetter(text[index + checkLength]);

					string selection = text.Substring(index, checkLength);
					if (table.ContainsKey(selection.ToLower()))
					{
						output = TransferCases(selection, GetOutput(selection.ToLower(), prefix, suffix).ToLower());
						break;
					}

					checkLength--;
				}

				if (output != null)
				{
					translated.Append(output);
					index += checkLength - 1;
				}
				else
				{
					translated.Append(text[index]);
				}
			}

			return translated.ToString();
		}

		private string TransferCases(string from, string to)
		{
			if (string.IsNullOrEmpty(from))
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
			var hashes = table[input];
			if (prefix && hashes.Any((x) => x.HashType == TranslateHashType.Prefix))
			{
				return hashes.First((x) => x.HashType == TranslateHashType.Prefix).Output;
			}
			else if (suffix && hashes.Any((x) => x.HashType == TranslateHashType.Suffix))
			{
				return hashes.First((x) => x.HashType == TranslateHashType.Suffix).Output;
			}
			else
			{
				return hashes.First((x) => x.HashType == TranslateHashType.Default).Output;
			}
		}
	}
}
