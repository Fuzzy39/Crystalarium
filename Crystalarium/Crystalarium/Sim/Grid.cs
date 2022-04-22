using System;
using System.Collections.Generic;
using System.Text;

namespace Crystalarium.Sim
{
    class Grid
    {
        /* the grid class represents a grid.
        * In Crystalarium, a grid is a 2d plane where devices can be built using a number of systems,
        * with the primary system called Crysm.
        */

        SimulationManager sim;

        public Grid(SimulationManager sim)
        {
            this.sim = sim;
            sim.addGrid(this);
        }

        public void destroy()
        {
            sim.removeGrid(this);
        }

    }
}
