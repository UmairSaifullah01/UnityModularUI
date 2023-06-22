namespace THEBADDEST
{


	/// <summary>
	/// Extension methods for the <see cref="IState"/> interface.
	/// </summary>
	public static class StateMachineExtensionMethods
	{
		/// <summary>
		/// Gets the name of the state.
		/// </summary>
		/// <param name="state">The state.</param>
		/// <returns>The name of the state.</returns>
		public static string GetStateName(this IState state)
		{
			return state.GetType().Name;
		}
	}



}