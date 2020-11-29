﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace FanLang
{
	public class TranslateHashUIElement : MonoBehaviour
	{
		public event Action<object> TranslateDataChangedEvent;
		public event Action<TranslateHashData> RequestDeleteEvent;

		[SerializeField] private TMP_InputField inputField;
		[SerializeField] private TMP_InputField outputField;
		[SerializeField] private TMP_Dropdown hashTypeDropdown;
		[SerializeField] private Button deleteHashButton;

		private TranslateHashData hashData;
		private InputFieldTextDataBinding inputBinding;
		private InputFieldTextDataBinding outputBinding;

		public void Initialize(TranslateHashData hashData)
		{
			CleanUp();
			this.hashData = hashData;

			inputBinding = new InputFieldTextDataBinding(
				inputField,
				() => hashData.Input,
				delegate (string t)
				{
					hashData.Input = t;
					OnDataChange();
				});

			outputBinding = new InputFieldTextDataBinding(
				outputField,
				() => hashData.Output,
				delegate (string t)
				{
					hashData.Output = t;
					OnDataChange();
				});

			deleteHashButton.onClick.AddListener(OnDeleteHashButtonPressed);

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
			if (inputBinding != null)
			{
				inputBinding.Dispose();
			}
			if (outputBinding != null)
			{
				outputBinding.Dispose();
			}
			deleteHashButton.onClick.RemoveListener(OnDeleteHashButtonPressed);
			hashTypeDropdown.onValueChanged.RemoveListener(OnHashTypeChanged);
		}
	}
}