using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace FanLang
{
	[RequireComponent(typeof(TranslatorBehaviour))]
	[RequireComponent(typeof(ConfirmationPopupBehaviour))]
	public class TranslatorSettings : MonoBehaviour
	{
		[SerializeField] private string dataPath;
		[SerializeField] private TranslateDataObject dataObject;

		[Header("Translator Settings")]
		[SerializeField] private Toggle alwaysUpdateToggle;
		[SerializeField] private Button updateTranslationButton;
		[SerializeField] private GameObject updateTranslationButtonContainer;
		[SerializeField] private Toggle allowEmptyHashesToggle;

		[Header("Project Settings")]
		[SerializeField] private Button loadButton;
		[SerializeField] private Button saveButton;
		[SerializeField] private Button saveAsButton;
		[SerializeField] private Button undoAllButton;
		[SerializeField] private TMP_InputField projectNameField;

		[Header("Project Data")]
		[SerializeField] private RectTransform contentParent;
		[SerializeField] private TranslateSheetUIElement translateSheetUIElementPrefab;
		[SerializeField] private Button addTranslateSheetButton;

		private TranslatorBehaviour translatorBehaviour;
		private ConfirmationPopupBehaviour confirmation;

		private TranslateData translateData;
		private string projectName = "";
		private bool dirty;

		private Dictionary<TranslateSheetData, TranslateSheetUIElement> spawnedSheets = new Dictionary<TranslateSheetData, TranslateSheetUIElement>();

		protected void OnEnable()
		{
			translatorBehaviour = GetComponent<TranslatorBehaviour>();
			confirmation = GetComponent<ConfirmationPopupBehaviour>();
			Subscribe();
		}

		protected void OnDisable()
		{
			Unsubscribe();
		}

		protected void Start()
		{
			Load();
		}

		protected void Update()
		{
			saveButton.interactable = dirty;
			undoAllButton.interactable = dirty;
		}

		private void SetDirty(bool dirty)
		{
			this.dirty = dirty;
			UpdateProjectName();
		}

		private void Load()
		{
			// First check if there is a dataPath, if there isn't check if there is a preset, then finally create a new project with default settings.
			if (!string.IsNullOrEmpty(dataPath))
			{
				Debug.LogError("NOT SUPPORTED YET");
			}
			else if (dataObject != null)
			{
				projectName = dataObject.name;
				LoadData(dataObject.GetTranslateDataClone());
			}
			else
			{
				LoadData(CreateDefaultProject());
			}

			SetDirty(false);

			alwaysUpdateToggle.SetIsOnWithoutNotify(translatorBehaviour.AlwaysUpdate);
			updateTranslationButtonContainer.SetActive(!translatorBehaviour.AlwaysUpdate);
			allowEmptyHashesToggle.SetIsOnWithoutNotify(translatorBehaviour.AllowEmptyHashes);
		}

		private void LoadData(TranslateData translateData)
		{
			this.translateData = translateData;
			translatorBehaviour.Load(translateData);
			RebuildUI();
		}

		private void OnTranslateDataChanged(object data)
		{
			SetDirty(true);

			if (translatorBehaviour.AlwaysUpdate)
			{
				translatorBehaviour.Reload();
			}
		}

		#region Subscriptions
		private void Subscribe()
		{
			updateTranslationButton.onClick.AddListener(OnUpdateTranslationButtonEvent);
			alwaysUpdateToggle.onValueChanged.AddListener(OnAlwaysUpdateToggleValueChanged);
			allowEmptyHashesToggle.onValueChanged.AddListener(OnAllowEmptyHashesToggleValueChanged);
			addTranslateSheetButton.onClick.AddListener(OnAddTranslateSheetEvent);

			loadButton.onClick.AddListener(OnLoadButton);
			saveButton.onClick.AddListener(OnSaveButton);
			saveAsButton.onClick.AddListener(OnSaveAsButton);
			undoAllButton.onClick.AddListener(OnUndoAllButton);
		}

		private void Unsubscribe()
		{
			updateTranslationButton.onClick.RemoveListener(OnUpdateTranslationButtonEvent);
			alwaysUpdateToggle.onValueChanged.RemoveListener(OnAlwaysUpdateToggleValueChanged);
			allowEmptyHashesToggle.onValueChanged.RemoveListener(OnAllowEmptyHashesToggleValueChanged);
			addTranslateSheetButton.onClick.RemoveListener(OnAddTranslateSheetEvent);

			loadButton.onClick.RemoveListener(OnLoadButton);
			saveButton.onClick.RemoveListener(OnSaveButton);
			saveAsButton.onClick.RemoveListener(OnSaveAsButton);
			undoAllButton.onClick.RemoveListener(OnUndoAllButton);
		}

		private void OnUpdateTranslationButtonEvent()
		{
			translatorBehaviour.Reload();
		}

		private void OnAlwaysUpdateToggleValueChanged(bool value)
		{
			translatorBehaviour.AlwaysUpdate = value;
			updateTranslationButtonContainer.SetActive(!translatorBehaviour.AlwaysUpdate);
			translatorBehaviour.Reload();
		}

		private void OnAllowEmptyHashesToggleValueChanged(bool value)
		{
			translatorBehaviour.AllowEmptyHashes = value;
			translatorBehaviour.Reload();
		}

		private void OnLoadButton()
		{

		}

		private void OnSaveButton()
		{
			if (dataObject != null)
			{
#if UNITY_EDITOR
				dataObject.OverwriteTranslateData(translateData);
				SetDirty(true);
#else
				OnSaveAsButton();
#endif
			}
		}

		private void OnSaveAsButton()
		{
			if (dataObject != null)
			{
#if UNITY_EDITOR

#endif
			}
		}

		private void OnUndoAllButton()
		{
			confirmation.Popup("Please Confirm", "Are you sure you want to undo all unsaved changes?", delegate (bool result)
			{
				if (result)
				{
					Load();
				}
			});
		}
		#endregion

		#region Building UI
		private void RebuildUI()
		{
			CleanUpUI();
			BuildUI();
		}

		private void CleanUpUI()
		{
			foreach (KeyValuePair<TranslateSheetData, TranslateSheetUIElement> sheetElement in spawnedSheets)
			{
				sheetElement.Value.TranslateDataChangedEvent -= OnTranslateDataChanged;
				sheetElement.Value.RequestDeleteEvent -= OnRequestDeleteTranslateSheetEvent;
				Destroy(sheetElement.Value.gameObject);
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

		private void OnAddTranslateSheetEvent()
		{
			TranslateSheetData sheetData = new TranslateSheetData();
			translateData.TranslateSheets.Add(sheetData);
			AddTranslateSheetUIElement(sheetData);
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
					translatorBehaviour.Reload();
					SetDirty(true);
				}
			});
		}
		#endregion

		private TranslateData CreateDefaultProject()
		{
			projectName = "New FanLang Project";
			List<TranslateSheetData> sheets = new List<TranslateSheetData>();
			sheets.Add(new TranslateSheetData());
			return new TranslateData(sheets);
		}

		private void UpdateProjectName()
		{
			projectNameField.text = $"{projectName}{(dirty ? "*" : "")}";
		}
	}
}