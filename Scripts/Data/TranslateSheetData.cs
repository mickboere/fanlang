using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FanLang
{
	/// <summary>
	/// Data object containing <see cref="TranslateHashData"/>s.
	/// You can use multiple <see cref="TranslateSheetData"/>s to perform several iterations over the output in order to generate more complex languages.
	/// </summary>
	[Serializable]
	public class TranslateSheetData : ICloneable
	{
		[SerializeField] public string SheetName;
		[SerializeField] public List<TranslateHashData> TranslateHashes;

		public TranslateSheetData(string sheetName, List<TranslateHashData> translateHashes)
		{
			SheetName = sheetName;
			TranslateHashes = translateHashes;
		}

		public object Clone()
		{
			return new TranslateSheetData(SheetName, TranslateHashes.Select((x) => (TranslateHashData)x.Clone()).ToList());
		}
	}
}