using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

namespace FanLang
{
	public class TranslateSheetUIElement : MonoBehaviour
	{
		public event Action TranslateDataChangedEvent;
		public event Action<TranslateSheetData> RequestDeleteEvent;

		[SerializeField] private TMP_InputField sheetNameField;
		[SerializeField] private Button deleteSheetButton;
		[SerializeField] private Button addHashButton;
		[SerializeField] private RectTransform hashParent;
		[SerializeField] private TranslateHashUIElement translateHashUIElementPrefab;

		private TranslateSheetData sheetData;
		private InputFieldTextDataBinding sheetNameBinding;
		private Dictionary<TranslateHashData, TranslateHashUIElement> spawnedHashes = new Dictionary<TranslateHashData, TranslateHashUIElement>();

		public void Initialize(TranslateSheetData sheetData)
		{
			CleanUp();
			this.sheetData = sheetData;

			sheetNameBinding = new InputFieldTextDataBinding(sheetNameField, () => sheetData.SheetName, (string t) => sheetData.SheetName = t);

			deleteSheetButton.onClick.AddListener(OnDeleteSheetButtonPressed);
			addHashButton.onClick.AddListener(OnAddHashButton);

			foreach (TranslateHashData hashData in sheetData.TranslateHashes)
			{
				AddTranslateHashUIElement(hashData);
			}
		}

		protected void OnDestroy()
		{
			CleanUp();
		}

		private void OnAddHashButton()
		{
			AddTranslateHashUIElement(new TranslateHashData("", "", TranslateHashType.Default));
		}

		private void OnTranslateHashDataChangedEvent()
		{
			TranslateDataChangedEvent?.Invoke();
		}

		private void OnRequestDeleteTranslateHashEvent(TranslateHashData hashData)
		{
			Destroy(spawnedHashes[hashData].gameObject);
			spawnedHashes.Remove(hashData);
			sheetData.TranslateHashes.Remove(hashData);
			TranslateDataChangedEvent?.Invoke();
		}

		private void OnDeleteSheetButtonPressed()
		{
			RequestDeleteEvent?.Invoke(sheetData);
		}

		private void CleanUp()
		{
			if (sheetNameBinding != null)
			{
				sheetNameBinding.Dispose();
			}

			foreach (KeyValuePair<TranslateHashData, TranslateHashUIElement> translateHashElement in spawnedHashes)
			{
				if (translateHashElement.Value != null)
				{
					translateHashElement.Value.TranslateDataChangedEvent -= OnTranslateHashDataChangedEvent;
					GameObject.Destroy(translateHashElement.Value.gameObject);
				}
			}
			spawnedHashes.Clear();

			deleteSheetButton.onClick.RemoveListener(OnDeleteSheetButtonPressed);
		}

		private void AddTranslateHashUIElement(TranslateHashData hashData)
		{
			TranslateHashUIElement element = Instantiate(translateHashUIElementPrefab, hashParent);
			element.Initialize(hashData);
			spawnedHashes.Add(hashData, element);
			element.TranslateDataChangedEvent += OnTranslateHashDataChangedEvent;
		}
	}
}