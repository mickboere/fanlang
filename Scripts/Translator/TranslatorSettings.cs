using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace FanLang
{
	[RequireComponent(typeof(TranslatorBehaviour))]
	public class TranslatorSettings : MonoBehaviour
	{
		[SerializeField] private string dataPath;
		[SerializeField] private TranslateDataObject preset;

		[Header("Translator Settings")]
		[SerializeField] private Toggle alwaysUpdateToggle;

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

		private TranslateData translateData;
		private string projectName = "";
		private bool changes;

		private Dictionary<TranslateSheetData, TranslateSheetUIElement> spawnedSheets = new Dictionary<TranslateSheetData, TranslateSheetUIElement>();

		protected void OnEnable()
		{
			translatorBehaviour = GetComponent<TranslatorBehaviour>();
			Subscribe();
		}

		protected void OnDisable()
		{
			Unsubscribe();
		}

		protected void Start()
		{
			Initialize();
		}

		#region Button Events
		private void Subscribe()
		{
			alwaysUpdateToggle.onValueChanged.AddListener(OnAlwaysUpdateToggleValueChanged);
			addTranslateSheetButton.onClick.AddListener(OnAddTranslateSheetEvent);
		}

		private void Unsubscribe()
		{
			alwaysUpdateToggle.onValueChanged.RemoveListener(OnAlwaysUpdateToggleValueChanged);
			addTranslateSheetButton.onClick.RemoveListener(OnAddTranslateSheetEvent);
		}
		#endregion

		private void Initialize()
		{
			// First check if there is a dataPath, if there isn't check if there is a preset (editor only) then finally create a new project with default settings.
			if (!string.IsNullOrEmpty(dataPath))
			{
				Debug.LogError("NOT SUPPORTED YET");
			}
			else if (preset != null)
			{
				projectName = preset.name;
				LoadData(preset.GetTranslateDataClone());
			}
			else
			{
				projectName = "New FanLang Project";
				List<TranslateSheetData> sheets = new List<TranslateSheetData>();
				List<TranslateHashData> hashes = new List<TranslateHashData>();
				hashes.Add(default);
				sheets.Add(new TranslateSheetData("New Sheet", hashes));
				LoadData(new TranslateData(sheets));
			}

			UpdateProjectName();
			alwaysUpdateToggle.SetIsOnWithoutNotify(translatorBehaviour.AlwaysUpdate);
		}

		private void OnTranslateDataChangedEvent()
		{
			changes = true;
			UpdateProjectName();
			translatorBehaviour.Reload();
		}

		private void UpdateProjectName()
		{
			projectNameField.text = $"{projectName}{(changes ? "*" : "")}";
		}

		private void LoadData(TranslateData translateData)
		{
			this.translateData = translateData;
			translatorBehaviour.Load(translateData);
			RebuildUI();
		}

		private void OnAlwaysUpdateToggleValueChanged(bool value)
		{
			translatorBehaviour.AlwaysUpdate = value;
		}

		#region Translate Sheets Management
		private void OnAddTranslateSheetEvent()
		{
			List<TranslateHashData> hashes = new List<TranslateHashData>();
			hashes.Add(new TranslateHashData("", "", TranslateHashType.Default));
			TranslateSheetData sheetData = new TranslateSheetData("New Sheet", hashes);
			translateData.TranslateSheets.Add(sheetData);
			AddTranslateSheetUIElement(sheetData);
		}

		private void OnRequestDeleteTranslateSheetEvent(TranslateSheetData sheetData)
		{
			// perform confirmation check, then if true:
			Destroy(spawnedSheets[sheetData].gameObject);
			spawnedSheets.Remove(sheetData);
			translateData.TranslateSheets.Remove(sheetData);
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
				sheetElement.Value.TranslateDataChangedEvent -= OnTranslateDataChangedEvent;
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
			element.TranslateDataChangedEvent += OnTranslateDataChangedEvent;
			element.RequestDeleteEvent += OnRequestDeleteTranslateSheetEvent;
		}
		#endregion
	}
}