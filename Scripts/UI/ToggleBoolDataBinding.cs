using UnityEngine.UI;
using System;

namespace FanLang
{
	public class ToggleBoolDataBinding : IDisposable
	{
		private Toggle toggle;
		private Func<bool> getData;
		private Action<bool> setData;

		public ToggleBoolDataBinding(Toggle toggle, Func<bool> getData, Action<bool> setData)
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