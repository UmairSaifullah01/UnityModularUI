using UnityEngine;


namespace THEBADDEST.UI
{


	public class UIView : ViewBase
	{

		[SerializeField, HideInInspector] protected RectTransform cachedRectTransform;
		/// <summary>
		/// Retrieves the transform component of the view.
		/// </summary>
		public RectTransform rectTransformObject
		{
			get
			{
				if (cachedRectTransform == null) cachedRectTransform = GetComponent<RectTransform>();
				return cachedRectTransform;
			}
		}

	}


}