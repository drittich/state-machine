using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachine
{
	public enum ProcessState
	{
		Inactive,
		Active,
		Paused,
		Terminated
	}

	public enum Event
	{
		Exit,
		Begin,
		End,
		Pause,
		Resume
	}
}
