using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace THEBADDEST.UI
{


	/// <summary>
	/// Component that adjusts the anchored positions of a RectTransform to fit within the safe area of the screen.
	/// </summary>
	public class SafeArea : ViewBase
	{

		Canvas            canvas;
		RectTransform     panelSafeArea;
		Rect              currentSafeArea;
		ScreenOrientation currentOrientation;

		/// <summary>
		/// Initializes the SafeArea component by retrieving the necessary references and applying the initial safe area.
		/// </summary>
		void Start()
		{
			// Get the parent Canvas component
			canvas = GetComponentInParent<Canvas>();

			// Get the RectTransform component of the panel
			panelSafeArea = GetComponent<RectTransform>();

			// Store the initial screen orientation and safe area
			currentOrientation = Screen.orientation;
			currentSafeArea    = Screen.safeArea;

			// Apply the safe area
			ApplySafeArea();
		}

		/// <summary>
		/// Applies the safe area by calculating the anchor positions based on the current safe area and setting them on the panel.
		/// </summary>
		void ApplySafeArea()
		{
			// Calculate the anchor positions based on the current safe area
			Vector2 anchorMin = currentSafeArea.position;
			Vector2 anchorMax = currentSafeArea.position + currentSafeArea.size;
			anchorMin.x /= canvas.pixelRect.width;
			anchorMin.y /= canvas.pixelRect.height;
			anchorMax.x /= canvas.pixelRect.width;
			anchorMax.y /= canvas.pixelRect.height;

			// Set the anchor positions of the panel
			panelSafeArea.anchorMin = anchorMin;
			panelSafeArea.anchorMax = anchorMax;
		}

		/// <summary>
		/// Checks if the screen orientation or safe area has changed and applies the updated safe area if necessary.
		/// </summary>
		void Update()
		{
			// Check if the screen orientation or safe area has changed
			if (currentOrientation != Screen.orientation && currentSafeArea != Screen.safeArea)
			{
				// Update the current orientation and safe area
				currentOrientation = Screen.orientation;
				currentSafeArea    = Screen.safeArea;

				// Apply the updated safe area
				ApplySafeArea();
			}
		}

	}


}