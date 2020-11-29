using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace FanLang
{
	public class ConfirmationPopupBehaviour : MonoBehaviour
	{
		[SerializeField] private GameObject blocker;
		[SerializeField] private GameObject popup;
		[SerializeField] private TMP_Text headerLabel;
		[SerializeField] private TMP_Text bodyLabel;
		[SerializeField] private Button cancelButton;
		[SerializeField] private TMP_Text cancelButtonLabel;
		[SerializeField] private Button confirmButton;
		[SerializeField] private TMP_Text confirmButtonLabel;

		private Action<bool> callback;

		protected void OnEnable()
		{
			blocker.SetActive(false);
			popup.SetActive(false);

			cancelButton.onClick.AddListener(OnCancelButton);
			confirmButton.onClick.AddListener(OnConfirmButton);
		}

		protected void OnDisable()
		{
			cancelButton.onClick.RemoveListener(OnCancelButton);
			confirmButton.onClick.RemoveListener(OnConfirmButton);
		}

		public void Popup(string header, string body, Action<bool> callback, string cancelText = "Cancel", string confirmText = "Confirm")
		{
			this.callback = callback;

			headerLabel.text = header;
			bodyLabel.text = body;
			cancelButtonLabel.text = cancelText;
			confirmButtonLabel.text = confirmText;

			blocker.SetActive(true);
			popup.SetActive(true);
		}

		public void ClosePopup(bool result)
		{
			blocker.SetActive(false);
			popup.SetActive(false);
			callback?.Invoke(result);
		}

		private void OnCancelButton()
		{
			ClosePopup(false);
		}

		private void OnConfirmButton()
		{
			ClosePopup(true);
		}
	}
}