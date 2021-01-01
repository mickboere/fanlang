namespace FanLang
{
	/// <summary>
	/// Defines what type of translate hash we're dealing with,
	/// has consequences for how the <see cref="Translator"/> handles specific situations when replacing characters.
	/// </summary>
	public enum TranslateHashType
	{
		Default = 0,
		Prefix = 1,
		Suffix = 2,
		Word = 3
	}
}