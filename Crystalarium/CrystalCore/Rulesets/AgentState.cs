using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Rulesets
{
    internal class AgentState
    {
        private AgentType type; // the type this state belongs to. this is important, because it determines the ports we have.

        // the conditions required for this state to occur.
        private int minNeighbors; // the minimum number of ports active for this state.
        private int maxNeighbors; // the maximum number of ports active for this state.




    }
}
