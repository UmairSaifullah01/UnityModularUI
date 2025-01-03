using UnityEngine;
using UnityEngine.Rendering.Universal;


namespace THEBADDEST
{


	public static class CameraExtensions
	{

		/// <summary>
		/// Adds the specified overlay camera to the main camera's stack.
		/// </summary>
		/// <param name="mainCamera">The main camera to which the overlay camera will be added.</param>
		/// <param name="overlayCamera">The overlay camera to add.</param>
		public static void AddToCameraStack(this Camera mainCamera, Camera overlayCamera)
		{
			if (mainCamera == null || overlayCamera == null)
			{
				Debug.LogError("Main camera or overlay camera is null.");
				return;
			}

			if (mainCamera.TryGetComponent(out UniversalAdditionalCameraData mainCameraData))
			{
				if (!mainCameraData.cameraStack.Contains(overlayCamera))
				{
					mainCameraData.cameraStack.Add(overlayCamera);
				}
				else
				{
					Debug.LogWarning("Overlay camera is already in the stack.");
				}
			}
			else
			{
				Debug.LogError("Main camera does not have UniversalAdditionalCameraData component.");
			}
		}

		/// <summary>
		/// Removes the specified overlay camera from the main camera's stack.
		/// </summary>
		/// <param name="mainCamera">The main camera from which the overlay camera will be removed.</param>
		/// <param name="overlayCamera">The overlay camera to remove.</param>
		public static void RemoveFromCameraStack(this Camera mainCamera, Camera overlayCamera)
		{
			if (mainCamera == null || overlayCamera == null)
			{
				Debug.LogError("Main camera or overlay camera is null.");
				return;
			}

			if (mainCamera.TryGetComponent(out UniversalAdditionalCameraData mainCameraData))
			{
				if (mainCameraData.cameraStack.Contains(overlayCamera))
				{
					mainCameraData.cameraStack.Remove(overlayCamera);
				}
				else
				{
					Debug.LogWarning("Overlay camera is not in the stack.");
				}
			}
			else
			{
				Debug.LogError("Main camera does not have UniversalAdditionalCameraData component.");
			}
		}

	}

	[RequireComponent(typeof(Camera))]
	public class CameraStackComponent : MonoBehaviour
	{

		[SerializeField] private Camera overlayCamera; // Assign your overlay camera in the inspector

		[SerializeField] private Camera mainCamera;

		void Awake()
		{
			overlayCamera = GetComponent<Camera>();
			// Find the main camera in the scene
			if (mainCamera == null)
				mainCamera = Camera.main;
		}

		void OnEnable()
		{
			if (mainCamera != null)
			{
				// Add the overlay camera to the main camera's stack
				mainCamera.AddToCameraStack(overlayCamera);
			}
			else
			{
				Debug.LogError("No main camera found in the scene.");
			}
		}

		void OnDisable()
		{
			if (mainCamera != null)
			{
				// Remove the overlay camera from the main camera's stack
				mainCamera.RemoveFromCameraStack(overlayCamera);
			}
		}

	}


}