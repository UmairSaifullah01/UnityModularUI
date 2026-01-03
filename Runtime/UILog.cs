using UnityEngine;

namespace THEBADDEST.UI
{
	/// <summary>
	/// Centralized logging utility for UI system.
	/// Logging can be enabled/disabled via UIVolume's enableDebugLogs flag.
	/// </summary>
	public static class UILog
	{
		private static bool enableLogs = true;
		private static readonly string LogTag = "<color=orange>[UI]</color>";

		/// <summary>
		/// Sets whether logging is enabled or disabled.
		/// </summary>
		/// <param name="enabled">True to enable logging, false to disable.</param>
		public static void SetEnabled(bool enabled)
		{
			enableLogs = enabled;
		}

		/// <summary>
		/// Logs a message to the Unity console.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public static void Log(string message)
		{
			if (!enableLogs) return;
			Debug.Log($"{LogTag} {message}");
		}

		/// <summary>
		/// Logs a warning message to the Unity console.
		/// </summary>
		/// <param name="message">The warning message to log.</param>
		public static void LogWarning(string message)
		{
			if (!enableLogs) return;
			Debug.LogWarning($"{LogTag} {message}");
		}

		/// <summary>
		/// Logs an error message to the Unity console.
		/// </summary>
		/// <param name="message">The error message to log.</param>
		public static void LogError(string message)
		{
			if (!enableLogs) return;
			Debug.LogError($"{LogTag} {message}");
		}
	}
}

