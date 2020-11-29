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
		[SerializeField] public bool Enabled;
		[SerializeField] public string Name;
		[SerializeField] public List<TranslateHashData> TranslateHashes;

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