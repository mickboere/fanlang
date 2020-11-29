using UnityEngine;
using TMPro;

namespace FanLang
{
	public class TranslatorBehaviour : MonoBehaviour
	{
		private const string INPUT_TEXT_PREF_KEY = "TranslatorInput";

		public bool AlwaysUpdate { get; set; } = true;
		public bool AllowEmptyHashes { get; set; } = false;

		[SerializeField] private TMP_InputField inputField;
		[SerializeField] private TMP_InputField outputField;

		private TranslateData translateData;
		private Translator[] translators;

		protected void OnEnable()
		{
			inputField.onValueChanged.AddListener(OnInputChangedEvent);
		}

		protected void OnDisable()
		{
			inputField.onValueChanged.RemoveListener(OnInputChangedEvent);
		}

		public void Load(TranslateData translateData)
		{
			this.translateData = translateData;
			LoadInputText();
			Reload();
		}

		public void Reload()
		{
			translators = new Translator[translateData.TranslateSheets.Count];
			for (int i = 0; i < translators.Length; i++)
			{
				translators[i] = new Translator(translateData.TranslateSheets[i], AllowEmptyHashes);
			}
			Translate();
		}

		private void Translate()
		{
			outputField.text = Translate(inputField.text);
		}

		private string Translate(string input)
		{
			string output = input;
			for (int i = 0; i < translators.Length; i++)
			{
				output = translators[i].Translate(output);
			}

			return output;
		}

		private void OnInputChangedEvent(string input)
		{
			if (AlwaysUpdate)
			{
				Translate();
			}

			SaveInputText();
		}

		private void LoadInputText()
		{
			inputField.SetTextWithoutNotify(PlayerPrefs.GetString(INPUT_TEXT_PREF_KEY, ""));
		}

		private void SaveInputText()
		{
			PlayerPrefs.SetString(INPUT_TEXT_PREF_KEY, inputField.text);
			PlayerPrefs.Save();
		}
	}
}
