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
		[SerializeField] public string Input;
		[SerializeField] public string Output;
		[SerializeField] public TranslateHashType HashType;

		public TranslateHashData(string input, string output, TranslateHashType hashType)
		{
			Input = input;
			Output = output;
			HashType = hashType;
		}

		public object Clone()
		{
			return new TranslateHashData(Input, Output, HashType);
		}
	}
}