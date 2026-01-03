namespace THEBADDEST.UI
{


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