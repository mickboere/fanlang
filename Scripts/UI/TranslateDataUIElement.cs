using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FanLang
{
	/// <summary>
	/// UI element handling the showing and updating of a <see cref="TranslateData"/>'s data.
	/// </summary>
	public class TranslateDataUIElement : MonoBehaviour
	{
		public event Action<object> TranslateDataChangedEvent;

		public bool Dirty { get; private set; }

		[SerializeField] private RectTransform contentParent;
		[SerializeField] private TMP_InputField projectNameField;
		[SerializeField] private GameObject projectDirtyMarker;
		[SerializeField] private TMP_InputField projectDescriptionField;
		[SerializeField] private Button projectDescriptionCollapseButton;
		[SerializeField] private Button addTranslateSheetButton;
		[SerializeField] private TranslateSheetUIElement translateSheetUIElementPrefab;

		private TranslateData translateData;
		private ConfirmationPopupBehaviour confirmation;

		private Dictionary<TranslateSheetData, TranslateSheetUIElement> spawnedSheets = new Dictionary<TranslateSheetData, TranslateSheetUIElement>();
		private DisposableContainer disposables = new DisposableContainer();

		public void Initialize(TranslateData translateData, ConfirmationPopupBehaviour confirmationPopupBehaviour)
		{
			CleanUp();

			this.translateData = translateData;
			this.confirmation = confirmationPopupBehaviour;

			// Set up bindings.
			disposables.Add(new InputFieldBinding(projectNameField, () => translateData.Name, delegate (string value)
			{
				value = value.Trim(' ');
				UpdateProjectName(value);
				SetDirty(true);
			}));
			disposables.Add(new InputFieldBinding(projectDescriptionField, () => translateData.Description, delegate (string value) { translateData.Description = value; SetDirty(true); }));
			disposables.Add(new ButtonBinding(projectDescriptionCollapseButton, () => projectDescriptionField.gameObject.SetActive(!projectDescriptionField.gameObject.activeSelf)));
			disposables.Add(new ButtonBinding(addTranslateSheetButton, delegate
			{
				TranslateSheetData sheetData = new TranslateSheetData();
				translateData.TranslateSheets.Add(sheetData);
				AddTranslateSheetUIElement(sheetData);
			}));

			// Set up UI.
			SetDirty(false);
			UpdateProjectName();
			BuildUI();
		}

		protected void OnDestroy()
		{
			CleanUp();
		}

		public void SetDirty(bool dirty)
		{
			Dirty = dirty;
			projectDirtyMarker.SetActive(Dirty);
		}

		private void CleanUp()
		{
			disposables.Dispose();
			CleanUpUI();
		}

		private void UpdateProjectName(string name = null)
		{
			if (!string.IsNullOrEmpty(name))
			{
				translateData.Name = name;
			}

			projectNameField.SetTextWithoutNotify(translateData.Name);
		}

		private void CleanUpUI()
		{
			foreach (KeyValuePair<TranslateSheetData, TranslateSheetUIElement> sheetElement in spawnedSheets)
			{
				if (sheetElement.Value != null)
				{
					sheetElement.Value.TranslateDataChangedEvent -= OnTranslateDataChanged;
					sheetElement.Value.RequestDeleteEvent -= OnRequestDeleteTranslateSheetEvent;
					Destroy(sheetElement.Value.gameObject);
				}
			}
			spawnedSheets.Clear();
		}

		private void BuildUI()
		{
			foreach (TranslateSheetData sheetData in translateData.TranslateSheets)
			{
				AddTranslateSheetUIElement(sheetData);
			}
		}

		private void AddTranslateSheetUIElement(TranslateSheetData sheetData)
		{
			TranslateSheetUIElement element = Instantiate(translateSheetUIElementPrefab, contentParent);
			element.Initialize(sheetData);
			spawnedSheets.Add(sheetData, element);
			element.TranslateDataChangedEvent += OnTranslateDataChanged;
			element.RequestDeleteEvent += OnRequestDeleteTranslateSheetEvent;
		}

		private void OnRequestDeleteTranslateSheetEvent(TranslateSheetData sheetData)
		{
			confirmation.Popup("Please Confirm", "Are you sure you want to delete this translate sheet?", delegate (bool result)
			{
				if (result)
				{
					Destroy(spawnedSheets[sheetData].gameObject);
					spawnedSheets.Remove(sheetData);
					translateData.TranslateSheets.Remove(sheetData);
					OnTranslateDataChanged(this);
					SetDirty(true);
				}
			});
		}

		private void OnTranslateDataChanged(object data)
		{
			SetDirty(true);
			TranslateDataChangedEvent?.Invoke(data);
		}
	}
}
