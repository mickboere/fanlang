using UnityEngine.UI;
using System;

namespace FanLang
{
	/// <summary>
	/// <see cref="Button"/> binding that makes it easier to manage onClick subscriptions.
	/// </summary>
	public class ButtonBinding : IDisposable
	{
		private Button button;
		private Action callback;

		public ButtonBinding(Button button, Action callback)
		{
			this.button = button;
			this.callback = callback;

			button.onClick.AddListener(OnClickedButton);
		}

		public void Dispose()
		{
			if (button != null)
			{
				button.onClick.RemoveListener(OnClickedButton);
			}
		}

		private void OnClickedButton()
		{
			callback();
		}
	}
}