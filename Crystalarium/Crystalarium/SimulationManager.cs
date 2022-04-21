using System;
using System.Collections.Generic;
using System.Text;

namespace Crystalarium
{
    class SimulationManager
    {
        // oh, my oh my...
        // it begins.
        // yet another big project.

        /*
         * Simulation Manager's job is to figure out how many simulation steps should occur in each frame, and to do them.
         * It's probably not best OOP practice, but I would rather not have this code in the main game class, I guess.
         * It's not incredibly Object Oriented either, I don't think...
         * But what do I know?
         * 
         */
        private double targetFPS; // the target FPS of the game. I can only imagine that it's 60.
        private double stepsPerSecond; // the amount of times the simulation needs to update per second.
        private arraylist? <Grid> activeGrids;

        public SimulationManager(double secondsBetweenFrames )
        {
            targetFPS = 1.0 / secondsBetweenFrames;

        }
    }
}
