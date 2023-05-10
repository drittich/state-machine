using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace state_machine_poc
{
	public class StateTransition<T>
	{
		public ProcessState CurrentState { get; }
		public Event Command { get; }
		public Action<T> MethodToExecute { get; }
		public ProcessState Inactive { get; }
		public Event Exit { get; }
		//public Action<BattleStateDto> SomeMethodToExecute { get; }

		public StateTransition(ProcessState currentState, Event command, Action<T> methodToExecute)
		{
			CurrentState = currentState;
			Command = command;
			MethodToExecute = methodToExecute;
		}

		public override int GetHashCode()
		{
			return 17 + 31 * CurrentState.GetHashCode() + 31 * Command.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			StateTransition<T> other = obj as StateTransition<T>;
			return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command;
		}
	}
}
