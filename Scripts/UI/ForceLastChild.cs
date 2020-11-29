using UnityEngine;

namespace FanLang
{
	[ExecuteInEditMode]
	public class ForceLastChild : MonoBehaviour
	{
		[SerializeField] private bool editor;

		protected void LateUpdate()
		{
			if (editor || Application.isPlaying)
			{
				transform.SetSiblingIndex(transform.parent.childCount);
			}
		}
	}
}