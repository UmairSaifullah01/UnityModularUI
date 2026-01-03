using System;
using System.Collections;
using UnityEngine;

namespace THEBADDEST.UI
{
	/// <summary>
	/// Service for managing toast notifications.
	/// Provides global access to show/hide toast messages.
	/// </summary>
	[CreateAssetMenu(fileName = "ToasterService", menuName = "THEBADDEST/UI/Toaster Service")]
	public class ToasterService : ScriptableObject
	{
		[SerializeField] private ToasterUI toasterCanvasPrefab;
		[SerializeField] private float defaultDisplayDuration = 2f;
		[SerializeField] private float animationDuration = 0.5f;
		[SerializeField] private string messageViewId = "MessageText";
		[SerializeField] private string innerPopupViewId = "InnerPopup";

		private ToasterUI toasterInstance;
		private Coroutine hideCoroutine;
		private bool isInitialized;

		/// <summary>
		/// Gets whether the service is initialized.
		/// </summary>
		public bool IsInitialized => isInitialized;

		/// <summary>
		/// Initializes the toaster service.
		/// </summary>
		/// <param name="uiCamera">Optional camera for the toaster canvas.</param>
		public void Initialize(Camera uiCamera = null)
		{
			if (toasterCanvasPrefab == null)
			{
				UILog.LogError("ToasterCanvas prefab is not assigned in ToasterService!");
				return;
			}

			if (toasterInstance != null)
			{
				UILog.LogWarning("ToasterService is already initialized.");
				return;
			}

			toasterInstance = Instantiate(toasterCanvasPrefab);
			toasterInstance.name = "ToasterUI";
			DontDestroyOnLoad(toasterInstance.gameObject);
			
			toasterInstance.UICamera = uiCamera;
			toasterInstance.AnimationDuration = animationDuration;
			toasterInstance.InnerPopupViewId = innerPopupViewId;
			toasterInstance.InitViewModel();
			Hide();
			isInitialized = true;
			
			ServiceLocator.Global.RegisterService<ToasterService>(this);
			UILog.Log("ToasterService initialized successfully.");
		}

		/// <summary>
		/// Shows a toast message.
		/// </summary>
		/// <param name="message">The message to display.</param>
		/// <param name="duration">How long to display the message (uses default if not specified).</param>
		public void Show(string message, float? duration = null)
		{
			if (!isInitialized || toasterInstance == null)
			{
				UILog.LogWarning("ToasterService is not initialized. Call Initialize() first.");
				return;
			}

			Hide();
			
			toasterInstance.StringBinder(messageViewId, message);
			toasterInstance.gameObject.SetActive(true);
			toasterInstance.PlayShowAnimation();
			
			float displayDuration = duration ?? defaultDisplayDuration;
			if (hideCoroutine != null && toasterInstance != null)
			{
				toasterInstance.StopCoroutine(hideCoroutine);
			}
			
			hideCoroutine = toasterInstance.StartCoroutine(HideAfterDelay(displayDuration));
		}

		/// <summary>
		/// Hides the toast notification.
		/// </summary>
		public void Hide()
		{
			if (toasterInstance == null) return;

			if (hideCoroutine != null)
			{
				toasterInstance.StopCoroutine(hideCoroutine);
				hideCoroutine = null;
			}

			toasterInstance.PlayHideAnimation();
		}

		/// <summary>
		/// Coroutine to hide the toast after a delay.
		/// </summary>
		private IEnumerator HideAfterDelay(float delay)
		{
			yield return new WaitForSeconds(delay);
			Hide();
		}
	}


}

