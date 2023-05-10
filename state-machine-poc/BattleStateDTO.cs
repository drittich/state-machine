using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace state_machine_poc
{
	public class BattleStateDto
	{
        public int BattleId { get; set; }
		public int AttackerMonsterId { get; set; }

		public int AttackerPlayerId { get; set; }
		public int VictimMonsterId { get; set; }

		public int VictimPlayerId { get; set; }
	}
}
