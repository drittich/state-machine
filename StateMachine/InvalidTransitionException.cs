using System;

namespace drittich.StateMachine
{
	/// <summary>
	/// Exception thrown when an invalid transition is attempted.
	/// </summary>
	public class InvalidTransitionException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidTransitionException"/> class with a specified error message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public InvalidTransitionException(string message) : base(message) { }
	}

}
