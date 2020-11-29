using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FanLang
{
	/// <summary>
	/// <see cref="ScriptableObject"/> containing a <see cref="TranslateData"/> object to make it configurable in the Unity Editor.
	/// </summary>
	[CreateAssetMenu(fileName = "FanLang_", menuName = "Data/TranslateData", order = 1)]
	public class TranslateDataObject : ScriptableObject
	{
		[SerializeField] private TranslateData translateData;

		/// <summary>
		/// Returns a clone of the configured <see cref="TranslateData"/> so the user can safely edit its data.
		/// </summary>
		public TranslateData GetTranslateDataClone()
		{
			return (TranslateData)translateData.Clone();
		}

		public void OverwriteTranslateData(TranslateData translateData)
		{
#if UNITY_EDITOR
			this.translateData = (TranslateData)translateData.Clone();
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
#else
			Debug.LogError("Overwriting the data in a scriptable object is only meant to be done in the editor!");
#endif
		}
	}
}