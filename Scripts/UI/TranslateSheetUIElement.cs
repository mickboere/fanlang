using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using System.Collections.Generic;

namespace FanLang
{
	/// <summary>
	/// UI element handling the showing and updating of a <see cref="TranslateSheetData"/>'s data.
	/// </summary>
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
		[SerializeField] private Button sortByInputButton;
		[SerializeField] private Button sortByOutputButton;
		[SerializeField] private Button sortByHashTypeButton;

		private TranslateSheetData sheetData;
		private DisposableContainer disposables = new DisposableContainer();
		private Dictionary<TranslateHashData, TranslateHashUIElement> spawnedHashes = new Dictionary<TranslateHashData, TranslateHashUIElement>();

		public void Initialize(TranslateSheetData sheetData)
		{
			CleanUp();
			this.sheetData = sheetData;

			// Set up bindings.
			disposables.Add(new ToggleBinding(enabledToggle, () => sheetData.Enabled, delegate (bool v) { sheetData.Enabled = v; TranslateDataChangedEvent?.Invoke(this); }));
			disposables.Add(new InputFieldBinding(sheetNameField, () => sheetData.Name, delegate (string t) { sheetData.Name = t; TranslateDataChangedEvent?.Invoke(this); }));
			disposables.Add(new ButtonBinding(deleteSheetButton, () => RequestDeleteEvent?.Invoke(sheetData)));
			disposables.Add(new ButtonBinding(addHashButton, delegate { TranslateHashData hash = new TranslateHashData(); sheetData.TranslateHashes.Add(hash); AddTranslateHashUIElement(hash); }));
			disposables.Add(new ButtonBinding(sortByInputButton, delegate { List<TranslateHashData> orderedList = sheetData.TranslateHashes.OrderBy((x) => x.Input).ToList(); SortHashes(orderedList); }));
			disposables.Add(new ButtonBinding(sortByOutputButton, delegate { List<TranslateHashData> orderedList = sheetData.TranslateHashes.OrderBy((x) => x.Output).ToList(); SortHashes(orderedList); }));
			disposables.Add(new ButtonBinding(sortByHashTypeButton, delegate { List<TranslateHashData> orderedList = sheetData.TranslateHashes.OrderBy((x) => x.HashType.ToString()).ToList(); SortHashes(orderedList); }));

			// Create child translate hash UI elements.
			RebuildUI();
		}

		protected void OnDestroy()
		{
			CleanUp();
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

		private void CleanUp()
		{
			disposables.Dispose();
			CleanUpUI();
		}

		private void RebuildUI()
		{
			CleanUpUI();
			foreach (TranslateHashData hashData in sheetData.TranslateHashes)
			{
				AddTranslateHashUIElement(hashData);
			}
		}

		private void CleanUpUI()
		{
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
		}

		private void AddTranslateHashUIElement(TranslateHashData hashData)
		{
			TranslateHashUIElement element = Instantiate(translateHashUIElementPrefab, hashParent);
			element.Initialize(hashData);
			spawnedHashes.Add(hashData, element);
			element.TranslateDataChangedEvent += OnTranslateHashDataChangedEvent;
			element.RequestDeleteEvent += OnRequestDeleteTranslateHashEvent;
		}

		private void SortHashes(List<TranslateHashData> orderedList)
		{
			// If it's already ordered, reverse the ordering to have a descending list instead of ascending.
			if (sheetData.TranslateHashes.SequenceEqual(orderedList))
			{
				orderedList.Reverse();
			}

			sheetData.TranslateHashes = orderedList;
			TranslateDataChangedEvent(this);
			RebuildUI();
		}
	}
}