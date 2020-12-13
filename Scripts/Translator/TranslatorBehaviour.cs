using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace FanLang
{
	public class TranslatorBehaviour : MonoBehaviour
	{
		private const string INPUT_TEXT_PREF_KEY = "TranslatorInput";

		public bool AlwaysUpdate { get; set; } = true;
		public bool AllowEmptyHashes { get; set; } = false;
		private string SaveKey => $"{INPUT_TEXT_PREF_KEY}_{translateData.ID}";

		[SerializeField] private TMP_InputField inputField;
		[SerializeField] private TMP_InputField outputField;

		private TranslateData translateData;
		private List<Translator> translators = new List<Translator>();

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
			translators.Clear();
			foreach (TranslateSheetData sheet in translateData.TranslateSheets)
			{
				if (sheet.Enabled)
				{
					translators.Add(new Translator(sheet, AllowEmptyHashes));
				}
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
			foreach (Translator translator in translators)
			{
				output = translator.Translate(output);
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
			inputField.SetTextWithoutNotify(PlayerPrefs.GetString(SaveKey, ""));
		}

		private void SaveInputText()
		{
			PlayerPrefs.SetString(SaveKey, inputField.text);
			PlayerPrefs.Save();
		}
	}
}
