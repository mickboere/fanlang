using System;
using System.Collections.Generic;
using System.Linq;

namespace FanLang
{
	/// <summary>
	/// The root data object of a FanLang.
	/// Contains basic meta data about the FanLang as well as a collection of <see cref="TranslateSheetData"/>s used for translation.
	/// </summary>
	[Serializable]
	public class TranslateData : ICloneable
	{
		/// <summary>
		/// Unique ID (often of type <see cref="Guid"/>) used for identifying the language data.
		/// </summary>
		public string ID;

		/// <summary>
		/// The name of the language.
		/// </summary>
		public string Name;

		/// <summary>
		/// A description of the language. Could contain some lore, how to use it, etc.
		/// </summary>
		public string Description;

		/// <summary>
		/// The actual data used to translate our input.
		/// </summary>
		public List<TranslateSheetData> TranslateSheets;

		public TranslateData(string id, string name, string description, List<TranslateSheetData> translateSheets)
		{
			ID = id;
			Name = name;
			Description = description;
			TranslateSheets = translateSheets;
		}

		public object Clone()
		{
			return new TranslateData(ID, Name, Description, TranslateSheets.Select((x) => (TranslateSheetData)x.Clone()).ToList());
		}
	}
}