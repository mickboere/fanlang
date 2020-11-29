using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FanLang
{
	/// <summary>
	/// The root data object of a FanLang.
	/// Contains basic meta data about the FanLang as well as a collection of <see cref="TranslateSheetData"/>s used for translation.
	/// </summary>
	[Serializable]
	public class TranslateData : ICloneable
	{
		//[SerializeField] public string LanguageName; <- For the name we just use the file name.
		[SerializeField] public List<TranslateSheetData> TranslateSheets;

		public TranslateData(List<TranslateSheetData> translateSheets)
		{
			TranslateSheets = translateSheets;
		}

		public object Clone()
		{
			return new TranslateData(TranslateSheets.Select((x) => (TranslateSheetData)x.Clone()).ToList());
		}
	}
}