using UnityEngine;

namespace FanLang
{
	/// <summary>
	/// Simple <see cref="MonoBehaviour"/> that forces the object to always be the last child in its parent's hierarchy.
	/// </summary>
	public class ForceLastChild : MonoBehaviour
	{
		protected void LateUpdate()
		{
			if (transform.GetSiblingIndex() != transform.parent.childCount - 1)
			{
				transform.SetSiblingIndex(transform.parent.childCount - 1);
			}
		}
	}
}