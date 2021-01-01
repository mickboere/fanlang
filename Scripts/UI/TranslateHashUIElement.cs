using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FanLang
{
	/// <summary>
	/// UI element handling the showing and updating of a <see cref="TranslateHashData"/>'s data.
	/// </summary>
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
		private DisposableContainer disposables = new DisposableContainer();

		public void Initialize(TranslateHashData hashData)
		{
			CleanUp();
			this.hashData = hashData;

			// Set up bindings.
			disposables.Add(new ToggleBinding(
				enabledToggle,
				() => hashData.Enabled,
				delegate (bool v)
				{
					hashData.Enabled = v;
					OnDataChange();
				}));
			disposables.Add(new InputFieldBinding(
				inputField,
				() => hashData.Input,
				delegate (string t)
				{
					hashData.Input = t;
					OnDataChange();
				}));
			disposables.Add(new InputFieldBinding(
				outputField,
				() => hashData.Output,
				delegate (string t)
				{
					hashData.Output = t;
					OnDataChange();
				}));
			disposables.Add(new ButtonBinding(deleteHashButton, () => RequestDeleteEvent?.Invoke(hashData)));

			// Fill the dropdown with all available TranslateHashTypes.
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

		private void OnHashTypeChanged(int dropdownIndex)
		{
			hashData.HashType = (TranslateHashType)dropdownIndex;
			OnDataChange();
		}

		private void CleanUp()
		{
			disposables.Dispose();
			hashTypeDropdown.onValueChanged.RemoveListener(OnHashTypeChanged);
		}
	}
}