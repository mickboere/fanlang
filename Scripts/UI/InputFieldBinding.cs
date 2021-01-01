using TMPro;
using System;

namespace FanLang
{
	/// <summary>
	/// <see cref="TMP_InputField"/> binding that makes it easier to manage input data changes.
	/// </summary>
	public class InputFieldBinding : IDisposable
	{
		private TMP_InputField inputField;
		private Func<string> getData;
		private Action<string> setData;

		public InputFieldBinding(TMP_InputField inputField, Func<string> getData, Action<string> setData)
		{
			this.inputField = inputField;
			this.getData = getData;
			this.setData = setData;

			inputField.SetTextWithoutNotify(getData());
			inputField.onValueChanged.AddListener(OnInputChanged);
		}

		public void Dispose()
		{
			if (inputField != null)
			{
				inputField.onValueChanged.RemoveListener(OnInputChanged);
			}
		}

		private void OnInputChanged(string text)
		{
			setData(text);
		}
	}
}