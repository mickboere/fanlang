using UnityEngine.UI;
using System;

namespace FanLang
{
	/// <summary>
	/// <see cref="Toggle"/> binding that makes it easier to manage toggle value change subscriptions.
	/// </summary>
	public class ToggleBinding : IDisposable
	{
		private Toggle toggle;
		private Func<bool> getData;
		private Action<bool> setData;

		public ToggleBinding(Toggle toggle, Func<bool> getData, Action<bool> setData)
		{
			this.toggle = toggle;
			this.getData = getData;
			this.setData = setData;

			toggle.SetIsOnWithoutNotify(getData());
			toggle.onValueChanged.AddListener(OnInputChanged);
		}

		public void Dispose()
		{
			if (toggle != null)
			{
				toggle.onValueChanged.RemoveListener(OnInputChanged);
			}
		}

		private void OnInputChanged(bool value)
		{
			setData(value);
		}
	}
}