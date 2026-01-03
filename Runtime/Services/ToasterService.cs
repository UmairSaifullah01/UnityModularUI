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
		private Coroutine animationCoroutine;
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
			PlayShowAnimation();
			
			toasterInstance.gameObject.SetActive(true);
			
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

			if (animationCoroutine != null)
			{
				toasterInstance.StopCoroutine(animationCoroutine);
			}

			PlayHideAnimation();
		}

		/// <summary>
		/// Plays the show animation.
		/// </summary>
		protected virtual void PlayShowAnimation()
		{
			if (toasterInstance == null || !toasterInstance.views.ContainsKey(innerPopupViewId))
			{
				return;
			}

			var innerPopup = toasterInstance.views[innerPopupViewId];
			if (innerPopup == null) return;

			innerPopup.transformObject.localScale = Vector3.zero;
			
			if (animationCoroutine != null)
			{
				toasterInstance.StopCoroutine(animationCoroutine);
			}

			animationCoroutine = toasterInstance.StartCoroutine(ShowAnimation(innerPopup.transformObject));
		}

		/// <summary>
		/// Plays the hide animation.
		/// </summary>
		protected virtual void PlayHideAnimation()
		{
			if (toasterInstance == null || !toasterInstance.views.ContainsKey(innerPopupViewId))
			{
				if (toasterInstance != null)
				{
					toasterInstance.gameObject.SetActive(false);
				}
				return;
			}

			var innerPopup = toasterInstance.views[innerPopupViewId];
			if (innerPopup == null)
			{
				if (toasterInstance != null)
				{
					toasterInstance.gameObject.SetActive(false);
				}
				return;
			}

			if (animationCoroutine != null)
			{
				toasterInstance.StopCoroutine(animationCoroutine);
			}

			animationCoroutine = toasterInstance.StartCoroutine(HideAnimation(innerPopup.transformObject));
		}

		/// <summary>
		/// Virtual coroutine for show animation. Override this to create custom show animations.
		/// </summary>
		/// <param name="target">The transform to animate.</param>
		/// <returns>Coroutine for the animation.</returns>
		protected virtual IEnumerator ShowAnimation(Transform target)
		{
			if (target == null) yield break;
			
			float elapsed = 0f;
			Vector3 start = Vector3.zero;
			Vector3 end = Vector3.one;
			
			while (elapsed < animationDuration)
			{
				elapsed += Time.deltaTime;
				float t = Mathf.Clamp01(elapsed / animationDuration);
				float easedT = EaseOutElastic(t);
				
				target.localScale = Vector3.Lerp(start, end, easedT);
				yield return null;
			}
			
			target.localScale = end;
		}

		/// <summary>
		/// Virtual coroutine for hide animation. Override this to create custom hide animations.
		/// </summary>
		/// <param name="target">The transform to animate.</param>
		/// <returns>Coroutine for the animation.</returns>
		protected virtual IEnumerator HideAnimation(Transform target)
		{
			if (target == null) yield break;
			
			float elapsed = 0f;
			Vector3 start = target.localScale;
			Vector3 end = Vector3.zero;
			float hideDuration = animationDuration * 0.5f;
			
			while (elapsed < hideDuration)
			{
				elapsed += Time.deltaTime;
				float t = Mathf.Clamp01(elapsed / hideDuration);
				float easedT = EaseOut(t);
				
				target.localScale = Vector3.Lerp(start, end, easedT);
				yield return null;
			}
			
			target.localScale = end;
			
			if (toasterInstance != null)
			{
				toasterInstance.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Ease out elastic function for animations.
		/// </summary>
		protected virtual float EaseOutElastic(float t)
		{
			const float c4 = (2f * Mathf.PI) / 3f;
			return t == 0f ? 0f : t == 1f ? 1f : Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
		}

		/// <summary>
		/// Ease out function for animations.
		/// </summary>
		protected virtual float EaseOut(float t)
		{
			return 1f - Mathf.Pow(1f - t, 3f);
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

	/// <summary>
	/// Static helper class for easy access to toaster functionality.
	/// </summary>
	public static class Toaster
	{
		/// <summary>
		/// Shows a toast message using the registered ToasterService.
		/// </summary>
		/// <param name="message">The message to display.</param>
		/// <param name="duration">How long to display the message.</param>
		public static void Show(string message, float? duration = null)
		{
			var service = ServiceLocator.Global.GetService<ToasterService>();
			if (service == null)
			{
				UILog.LogWarning("ToasterService is not registered. Make sure to initialize it first.");
				return;
			}
			service.Show(message, duration);
		}

		/// <summary>
		/// Hides the current toast notification.
		/// </summary>
		public static void Hide()
		{
			var service = ServiceLocator.Global.GetService<ToasterService>();
			if (service == null)
			{
				UILog.LogWarning("ToasterService is not registered. Make sure to initialize it first.");
				return;
			}
			service.Hide();
		}
	}
}

