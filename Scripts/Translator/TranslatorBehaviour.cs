using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace FanLang
{
	/// <summary>
	/// <see cref="MonoBehaviour"/> responsible for handling the <see cref="Translator"/>s required to apply the loaded
	/// <see cref="TranslateData"/> to an input taken from an <see cref="TMP_InputField"/> and translate to an output <see cref="TMP_InputField"/>.
	/// </summary>
	public class TranslatorBehaviour : MonoBehaviour
	{
		private const string INPUT_TEXT_PREF_KEY = "TranslatorInput";

		public bool AlwaysUpdate { get; set; } = true;
		public bool AllowEmptyHashes { get; set; } = false;
		private string SaveKey => $"{INPUT_TEXT_PREF_KEY}_{translateData.ID}";
		private bool HasSelection => selectionStart != selectionEnd;

		[SerializeField] private TMP_InputField inputField;
		[SerializeField] private TMP_InputField outputField;
		[SerializeField] private Color selectionColor;

		private TranslateData translateData;
		private List<Translator> translators = new List<Translator>();

		private int selectionStart;
		private int selectionEnd;
		private bool requiresTranslationUpdate;

		protected void OnEnable()
		{
			inputField.onValueChanged.AddListener(OnInputChangedEvent);
			inputField.onTextSelection.AddListener(OnInputSelectionChangedEvent);
			inputField.onEndTextSelection.AddListener(OnInputSelectionChangedEvent);
			inputField.onDeselect.AddListener(OnInputDeselect);
		}

		protected void OnDisable()
		{
			inputField.onValueChanged.RemoveListener(OnInputChangedEvent);
			inputField.onTextSelection.RemoveListener(OnInputSelectionChangedEvent);
			inputField.onEndTextSelection.RemoveListener(OnInputSelectionChangedEvent);
			inputField.onDeselect.RemoveListener(OnInputDeselect);
		}

		protected void Update()
		{
			// If AlwaysUpdate is false, either Reload or Translate will need to be called externally to update the translation.
			if (AlwaysUpdate && requiresTranslationUpdate)
			{
				Translate();
				requiresTranslationUpdate = false;
			}
		}

		/// <summary>
		/// Loads the given <see cref="TranslateData"/> and creates the required <see cref="Translator"/>s.
		/// </summary>
		public void Load(TranslateData translateData)
		{
			this.translateData = translateData;
			LoadInputText();
			Reload();
		}

		/// <summary>
		/// Reloads the currently loaded <see cref="TranslateData"/> in case the data structure has changed.
		/// </summary>
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

		/// <summary>
		/// Takes input from the input field, applies selected text marking and translates it using the loaded <see cref="Translator"/>s.
		/// Wraps around <seealso cref="Translate(string)"/>.
		/// </summary>
		public void Translate()
		{
			string input = inputField.text;
			if (HasSelection)
			{
				string markingTag = $"<mark=#{ColorUtility.ToHtmlStringRGBA(selectionColor)}>";
				input = input.Insert(selectionStart, markingTag);
				input = input.Insert(selectionEnd + markingTag.Length, $"</mark>");
			}
			outputField.text = Translate(input);
		}

		/// <summary>
		/// Translates <paramref name="input"/> using the currently defined <see cref="Translator"/>s.
		/// </summary>
		/// <returns>The translated input.</returns>
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
			// Don't allow tags to be manually added by the user.
			input = input.Trim('<', '>');
			inputField.SetTextWithoutNotify(input);

			requiresTranslationUpdate = true;
			SaveInputText();
		}

		private void OnInputDeselect(string input)
		{
			selectionStart = 0;
			selectionEnd = 0;
			requiresTranslationUpdate = true;
		}

		private void OnInputSelectionChangedEvent(string text, int start, int end)
		{
			if (!inputField.isFocused)
			{
				return;
			}

			selectionStart = Mathf.Min(start, end);
			selectionEnd = Mathf.Max(start, end);
			requiresTranslationUpdate = true;
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
