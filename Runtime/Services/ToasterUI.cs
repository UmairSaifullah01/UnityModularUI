using System.Collections;
using THEBADDEST.MVVM;
using UnityEngine;

namespace THEBADDEST.UI
{
	/// <summary>
	/// UI component for displaying toast notifications.
	/// Inherits from ViewModelMono for MVVM pattern integration.
	/// </summary>
	public class ToasterUI : ViewModelMono
	{
		[SerializeField] private Canvas canvas;
		
		/// <summary>
		/// Camera to use for the canvas. Set this before calling InitViewModel().
		/// </summary>
		public Camera UICamera { get; set; }
		
		/// <summary>
		/// Duration for show animation in seconds.
		/// </summary>
		public float AnimationDuration { get; set; } = 0.5f;
		
		/// <summary>
		/// View ID for the inner popup element that will be animated.
		/// </summary>
		public string InnerPopupViewId { get; set; } = "InnerPopup";
		
		private Coroutine animationCoroutine;

		/// <summary>
		/// Initializes the ViewModel and sets up the canvas camera.
		/// </summary>
		public override void InitViewModel()
		{
			if (canvas == null)
			{
				canvas = GetComponent<Canvas>();
				if (canvas == null)
				{
					canvas = gameObject.AddComponent<Canvas>();
					canvas.renderMode = RenderMode.ScreenSpaceOverlay;
				}
			}
			
			if (UICamera != null)
			{
				canvas.renderMode = RenderMode.ScreenSpaceCamera;
				canvas.worldCamera = UICamera;
			}
			
			base.InitViewModel();
		}

		/// <summary>
		/// Plays the show animation.
		/// </summary>
		public virtual void PlayShowAnimation()
		{
			if (!views.ContainsKey(InnerPopupViewId))
			{
				return;
			}

			var innerPopup = views[InnerPopupViewId];
			if (innerPopup == null) return;

			innerPopup.transformObject.localScale = Vector3.zero;
			
			if (animationCoroutine != null)
			{
				StopCoroutine(animationCoroutine);
			}

			animationCoroutine = StartCoroutine(ShowAnimation(innerPopup.transformObject));
		}

		/// <summary>
		/// Plays the hide animation.
		/// </summary>
		public virtual void PlayHideAnimation()
		{
			if (!views.ContainsKey(InnerPopupViewId))
			{
				gameObject.SetActive(false);
				return;
			}

			var innerPopup = views[InnerPopupViewId];
			if (innerPopup == null)
			{
				gameObject.SetActive(false);
				return;
			}

			if (animationCoroutine != null)
			{
				StopCoroutine(animationCoroutine);
			}

			animationCoroutine = StartCoroutine(HideAnimation(innerPopup.transformObject));
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
			
			while (elapsed < AnimationDuration)
			{
				elapsed += Time.deltaTime;
				float t = Mathf.Clamp01(elapsed / AnimationDuration);
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
			float hideDuration = AnimationDuration * 0.5f;
			
			while (elapsed < hideDuration)
			{
				elapsed += Time.deltaTime;
				float t = Mathf.Clamp01(elapsed / hideDuration);
				float easedT = EaseOut(t);
				
				target.localScale = Vector3.Lerp(start, end, easedT);
				yield return null;
			}
			
			target.localScale = end;
			gameObject.SetActive(false);
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
	}
}

