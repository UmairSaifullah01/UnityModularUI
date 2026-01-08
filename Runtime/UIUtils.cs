using UnityEngine;

namespace THEBADDEST.UI
{
	public static class UIUtils
	{
		private static float previousClickTime = 0;
		private static int currentOpenIndex = 0;
		private static float delay = 0;

		public static void SetDelayBetweenClick(float value)
		{
			delay = value;
		}

		public static bool WaitBetweenClick()
		{
			if (Time.time - previousClickTime < delay)
			{
				return true;
			}

			previousClickTime = Time.time;
			return false;
		}

		public static void SetOpenIndex(int value)
		{
			currentOpenIndex = value;
		}

		public static bool IsDisableButtons(int value)
		{
			if (currentOpenIndex == 0)
				return false;
			return currentOpenIndex != value;
		}
	}
}

