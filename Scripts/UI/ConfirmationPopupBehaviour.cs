using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace FanLang
{
	/// <summary>
	/// <see cref="MonoBehaviour"/> able to show a confirmation popup to the user and return the user response through a callback.
	/// </summary>
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

		/// <summary>
		/// Shows the user a new confirmation popup.
		/// </summary>
		/// <param name="header">The header text of the popup.</param>
		/// <param name="body">The body text of the popup.</param>
		/// <param name="callback">The callback to be called once the user has confirmed his input. </param>
		/// <param name="cancelText">The text to be displayed on the cancel button.</param>
		/// <param name="confirmText">The text to be displayed on the confirm button.</param>
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

		/// <summary>
		/// Force the current popup to close.
		/// </summary>
		/// <param name="result">The result to pass into the callback passed into the last
		/// open <see cref="Popup(string, string, Action{bool}, string, string)"/></param> call.
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