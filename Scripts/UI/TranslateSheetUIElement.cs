using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

namespace FanLang
{
	public class TranslateSheetUIElement : MonoBehaviour
	{
		public event Action<object> TranslateDataChangedEvent;
		public event Action<TranslateSheetData> RequestDeleteEvent;

		[SerializeField] private Toggle enabledToggle;
		[SerializeField] private TMP_InputField sheetNameField;
		[SerializeField] private Button deleteSheetButton;
		[SerializeField] private Button addHashButton;
		[SerializeField] private RectTransform hashParent;
		[SerializeField] private TranslateHashUIElement translateHashUIElementPrefab;

		private TranslateSheetData sheetData;
		private List<IDisposable> disposables = new List<IDisposable>();
		private Dictionary<TranslateHashData, TranslateHashUIElement> spawnedHashes = new Dictionary<TranslateHashData, TranslateHashUIElement>();

		public void Initialize(TranslateSheetData sheetData)
		{
			CleanUp();
			this.sheetData = sheetData;

			disposables.Add(new ToggleBoolDataBinding(enabledToggle, () => sheetData.Enabled, delegate (bool v) { sheetData.Enabled = v; TranslateDataChangedEvent?.Invoke(this); }));
			disposables.Add(new InputFieldTextDataBinding(sheetNameField, () => sheetData.Name, (string t) => sheetData.Name = t));

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
			TranslateHashData hash = new TranslateHashData();
			sheetData.TranslateHashes.Add(hash);
			AddTranslateHashUIElement(hash);
		}

		private void OnTranslateHashDataChangedEvent(object hash)
		{
			TranslateDataChangedEvent?.Invoke(hash);
		}

		private void OnRequestDeleteTranslateHashEvent(TranslateHashData hashData)
		{
			Destroy(spawnedHashes[hashData].gameObject);
			spawnedHashes.Remove(hashData);
			sheetData.TranslateHashes.Remove(hashData);
			TranslateDataChangedEvent?.Invoke(this);
		}

		private void OnDeleteSheetButtonPressed()
		{
			RequestDeleteEvent?.Invoke(sheetData);
		}

		private void CleanUp()
		{
			foreach (IDisposable disposable in disposables)
			{
				disposable.Dispose();
			}
			disposables.Clear();

			foreach (KeyValuePair<TranslateHashData, TranslateHashUIElement> translateHashElement in spawnedHashes)
			{
				if (translateHashElement.Value != null)
				{
					translateHashElement.Value.TranslateDataChangedEvent -= OnTranslateHashDataChangedEvent;
					translateHashElement.Value.RequestDeleteEvent -= OnRequestDeleteTranslateHashEvent;
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
			element.RequestDeleteEvent += OnRequestDeleteTranslateHashEvent;
		}
	}
}