using System;

namespace FanLang
{
	/// <summary>
	/// The data that decides the output of the FanLang.
	/// Contains a <see cref="string"/> Input, a <see cref="string"/> Output and a <see cref="TranslateHashType"/> to allow for a range of configurable FanLangs.
	/// </summary>
	[Serializable]
	public class TranslateHashData : ICloneable
	{
		/// <summary>
		/// Should this hash be used?
		/// </summary>
		public bool Enabled;

		/// <summary>
		/// The input characters required to produce the defined output.
		/// </summary>
		public string Input;

		/// <summary>
		/// The ouput used to translate the defined input.
		/// </summary>
		public string Output;

		/// <summary>
		/// Defines the context in which the output is allowed to be used.
		/// </summary>
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