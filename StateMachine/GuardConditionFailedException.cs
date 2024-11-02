using System;

namespace drittich.StateMachine
{
	/// <summary>
	/// Exception thrown when a guard condition fails.
	/// </summary>
	public class GuardConditionFailedException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GuardConditionFailedException"/> class with a specified error message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public GuardConditionFailedException(string message) : base(message) { }
	}
}
