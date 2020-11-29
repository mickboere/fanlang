using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FanLang
{
	public class TranslateHashUIElement : MonoBehaviour
	{
		public event Action<object> TranslateDataChangedEvent;
		public event Action<TranslateHashData> RequestDeleteEvent;

		[SerializeField] private Toggle enabledToggle;
		[SerializeField] private TMP_InputField inputField;
		[SerializeField] private TMP_InputField outputField;
		[SerializeField] private TMP_Dropdown hashTypeDropdown;
		[SerializeField] private Button deleteHashButton;

		private TranslateHashData hashData;
		private List<IDisposable> disposables = new List<IDisposable>();

		public void Initialize(TranslateHashData hashData)
		{
			CleanUp();
			this.hashData = hashData;

			disposables.Add(new ToggleBoolDataBinding(
				enabledToggle,
				() => hashData.Enabled,
				delegate (bool v)
				{
					hashData.Enabled = v;
					OnDataChange();
				}));

			disposables.Add(new InputFieldTextDataBinding(
				inputField,
				() => hashData.Input,
				delegate (string t)
				{
					hashData.Input = t;
					OnDataChange();
				}));

			disposables.Add(new InputFieldTextDataBinding(
				outputField,
				() => hashData.Output,
				delegate (string t)
				{
					hashData.Output = t;
					OnDataChange();
				}));

			deleteHashButton.onClick.AddListener(OnDeleteHashButtonPressed);

			hashTypeDropdown.options = new List<TMP_Dropdown.OptionData>(((TranslateHashType[])Enum.GetValues(typeof(TranslateHashType))).Select((x) => new TMP_Dropdown.OptionData(x.ToString())));
			hashTypeDropdown.value = (int)hashData.HashType;
			hashTypeDropdown.onValueChanged.AddListener(OnHashTypeChanged);
		}

		protected void OnDestroy()
		{
			CleanUp();
		}

		private void OnDataChange()
		{
			TranslateDataChangedEvent?.Invoke(this);
		}

		private void OnDeleteHashButtonPressed()
		{
			RequestDeleteEvent?.Invoke(hashData);
		}

		private void OnHashTypeChanged(int dropdownIndex)
		{
			hashData.HashType = (TranslateHashType)dropdownIndex;
			OnDataChange();
		}

		private void CleanUp()
		{
			foreach (IDisposable disposable in disposables)
			{
				disposable.Dispose();
			}
			disposables.Clear();

			deleteHashButton.onClick.RemoveListener(OnDeleteHashButtonPressed);
			hashTypeDropdown.onValueChanged.RemoveListener(OnHashTypeChanged);
		}
	}
}