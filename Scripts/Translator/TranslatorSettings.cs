using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using SFB;
using Newtonsoft.Json;
using System.IO;
using System;

namespace FanLang
{
	[RequireComponent(typeof(TranslatorBehaviour))]
	[RequireComponent(typeof(ConfirmationPopupBehaviour))]
	public class TranslatorSettings : MonoBehaviour
	{
		[SerializeField] private string dataPath;
		[SerializeField] private TranslateDataUIElement translateDataUI;

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

		private TranslatorBehaviour translatorBehaviour;
		private ConfirmationPopupBehaviour confirmation;
		private TranslateData loadedTranslateData;
		private DisposableContainer disposables = new DisposableContainer();

		protected void OnEnable()
		{
			translatorBehaviour = GetComponent<TranslatorBehaviour>();
			confirmation = GetComponent<ConfirmationPopupBehaviour>();
			translateDataUI = GetComponent<TranslateDataUIElement>();
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
			saveButton.interactable = translateDataUI.Dirty;
			undoAllButton.interactable = translateDataUI.Dirty;
		}

		private void Load()
		{
			if (!string.IsNullOrEmpty(dataPath))
			{
				LoadFromPath(dataPath);
			}
			else
			{
				LoadData(CreateDefaultProject());
			}

			alwaysUpdateToggle.SetIsOnWithoutNotify(translatorBehaviour.AlwaysUpdate);
			updateTranslationButtonContainer.SetActive(!translatorBehaviour.AlwaysUpdate);
			allowEmptyHashesToggle.SetIsOnWithoutNotify(translatorBehaviour.AllowEmptyHashes);
		}

		private void LoadFromPath(string path)
		{
			if (File.Exists(path))
			{
				dataPath = path;
				TranslateData data = JsonConvert.DeserializeObject<TranslateData>(File.ReadAllText(path));
				LoadData(data);
			}
			else
			{
				Debug.LogError($"File at path '{path}' does not exist.");
			}
		}

		private void LoadData(TranslateData translateData)
		{
			loadedTranslateData = translateData;
			CleanData();
			translatorBehaviour.Load(translateData);
			translateDataUI.Initialize(translateData, confirmation);
		}

		private void SaveData(TranslateData translateData, string path, bool reload)
		{
			string json = JsonConvert.SerializeObject(translateData, Formatting.Indented);
			File.WriteAllText(path, json);
			if (reload)
			{
				LoadFromPath(path);
			}
			translateDataUI.SetDirty(false);
		}

		private void SaveAs()
		{
			StandaloneFileBrowser.SaveFilePanelAsync("Save FanLangData file", "", loadedTranslateData.Name, "fld", delegate (string path)
			{
				if (path != dataPath)
				{
					loadedTranslateData.ID = Guid.NewGuid().ToString();
				}
				SaveData(loadedTranslateData, path, true);
			});
		}

		private void CleanData()
		{
			// Make sure we have an ID, if not, generate it.
			if (string.IsNullOrEmpty(loadedTranslateData.ID))
			{
				loadedTranslateData.ID = Guid.NewGuid().ToString();
			}

			// Remove all empty data from the loaded TranslateData.
			for (int s = 0; s < loadedTranslateData.TranslateSheets.Count; s++)
			{
				TranslateSheetData sheet = loadedTranslateData.TranslateSheets[s];
				if (sheet.TranslateHashes.Count == 0)
				{
					loadedTranslateData.TranslateSheets.Remove(sheet);
					s--;
				}
				else
				{
					for (int h = 0; h < sheet.TranslateHashes.Count; h++)
					{
						TranslateHashData hash = sheet.TranslateHashes[h];
						if (string.IsNullOrEmpty(hash.Input) ||
							string.IsNullOrEmpty(hash.Output))
						{
							sheet.TranslateHashes.Remove(hash);
							h--;
						}
					}
				}
			}
		}

		private TranslateData CreateDefaultProject()
		{
			List<TranslateSheetData> sheets = new List<TranslateSheetData>();
			sheets.Add(new TranslateSheetData());
			return new TranslateData(Guid.NewGuid().ToString(), "New FanLang Project", "", sheets);
		}

		private void OnTranslateDataChanged(object data)
		{
			if (translatorBehaviour.AlwaysUpdate)
			{
				translatorBehaviour.Reload();
			}
		}

		private void Subscribe()
		{
			translateDataUI.TranslateDataChangedEvent += OnTranslateDataChanged;

			// Set up UI bindings.
			disposables.Add(new ToggleBinding(alwaysUpdateToggle, () => translatorBehaviour.AlwaysUpdate, delegate (bool value)
			{
				translatorBehaviour.AlwaysUpdate = value;
				updateTranslationButtonContainer.SetActive(!translatorBehaviour.AlwaysUpdate);
				translatorBehaviour.Reload();
			}));
			disposables.Add(new ToggleBinding(allowEmptyHashesToggle, () => translatorBehaviour.AllowEmptyHashes, delegate (bool value)
			{
				translatorBehaviour.AllowEmptyHashes = value;
				translatorBehaviour.Reload();
			}));
			disposables.Add(new ButtonBinding(updateTranslationButton, () => translatorBehaviour.Reload()));
			disposables.Add(new ButtonBinding(loadButton, delegate
			{
				StandaloneFileBrowser.OpenFilePanelAsync("Select FanLangData file to load", "", "fld", false, delegate (string[] paths)
				{
					if (paths != null && paths.Length > 0)
					{
						LoadFromPath(paths[0]);
					}
				});
			}));
			disposables.Add(new ButtonBinding(saveButton, delegate
			{
				if (!string.IsNullOrEmpty(dataPath) && File.Exists(dataPath))
				{
					SaveData(loadedTranslateData, dataPath, false);
				}
				else
				{
					SaveAs();
				}
			}));
			disposables.Add(new ButtonBinding(saveAsButton, SaveAs));
			disposables.Add(new ButtonBinding(undoAllButton, delegate
			{
				confirmation.Popup("Please Confirm", "Are you sure you want to undo all unsaved changes?", delegate (bool result)
				{
					if (result)
					{
						Load();
					}
				});
			}));
		}

		private void Unsubscribe()
		{
			translateDataUI.TranslateDataChangedEvent -= OnTranslateDataChanged;
			// Dispose of all UI bindings.
			disposables.Dispose();
		}
	}
}