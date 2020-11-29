using System;
using UnityEngine;

namespace FanLang
{
	/// <summary>
	/// The data that decides the output of the FanLang.
	/// Contains a <see cref="string"/> Input, a <see cref="string"/> Output and a <see cref="TranslateHashType"/> to allow for a range of configurable FanLangs.
	/// </summary>
	[Serializable]
	public class TranslateHashData : ICloneable
	{
		public bool Enabled;
		public string Input;
		public string Output;
		public TranslateHashType HashType;

		public TranslateHashData()
		{
			Enabled = true;
			Input = "";
			Output = "";
			HashType = TranslateHashType.Default;
		}

		public TranslateHashData(bool enabled, string input, string output, TranslateHashType hashType)
		{
			Enabled = enabled;
			Input = input;
			Output = output;
			HashType = hashType;
		}

		public object Clone()
		{
			return new TranslateHashData(Enabled, Input, Output, HashType);
		}
	}
}