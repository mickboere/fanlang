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
		public string ID;
		public string Name;
		public string Description;

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