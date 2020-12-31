using System;
using System.Collections.Generic;
using System.Linq;

namespace FanLang
{
	/// <summary>
	/// Data object containing <see cref="TranslateHashData"/>s.
	/// You can use multiple <see cref="TranslateSheetData"/>s to perform several iterations over the output in order to generate more complex languages.
	/// </summary>
	[Serializable]
	public class TranslateSheetData : ICloneable
	{
		/// <summary>
		/// Should the data in this sheet be used?
		/// </summary>
		public bool Enabled;

		/// <summary>
		/// The name of this sheet.
		/// </summary>
		public string Name;

		/// <summary>
		/// List containing the translate hashes which are used for the actual translation of the input.
		/// </summary>
		public List<TranslateHashData> TranslateHashes;

		public TranslateSheetData()
		{
			Enabled = true;
			Name = "New Sheet";
			TranslateHashes = new List<TranslateHashData>();
			TranslateHashes.Add(new TranslateHashData());
		}

		public TranslateSheetData(bool enabled, string name, List<TranslateHashData> translateHashes)
		{
			Enabled = enabled;
			Name = name;
			TranslateHashes = translateHashes;
		}

		public object Clone()
		{
			return new TranslateSheetData(Enabled, Name, TranslateHashes.Select((x) => (TranslateHashData)x.Clone()).ToList());
		}
	}
}