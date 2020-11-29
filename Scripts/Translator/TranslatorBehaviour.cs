using UnityEngine;
using TMPro;

namespace FanLang
{
	public class TranslatorBehaviour : MonoBehaviour
	{
		public bool AlwaysUpdate { get; set; } = true;

		[SerializeField] private TMP_InputField inputField;
		[SerializeField] private TMP_InputField outputField;

		private Translator[] translators;

		protected void OnEnable()
		{
			inputField.onValueChanged.AddListener(OnInputChangedEvent);
		}

		protected void OnDisable()
		{
			inputField.onValueChanged.RemoveListener(OnInputChangedEvent);
		}

		public void Load(TranslateData data)
		{
			translators = new Translator[data.TranslateSheets.Count];
			for (int i = 0; i < data.TranslateSheets.Count; i++)
			{
				translators[i] = new Translator(data.TranslateSheets[i]);
			}
			Reload();
		}

		public void Reload()
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
				Reload();
			}
		}
	}
}
