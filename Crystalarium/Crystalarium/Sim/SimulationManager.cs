using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;


namespace Crystalarium.Sim
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

        private int _targetStepsPS; // the amount of times the simulation needs to update per second.
        private int _actualStepsPS; // the amount of times that the simulation is currently running per second.
                                    // Unless the game is lagging, it should be the same as the target.

        public const int MIN_STEPS_PER_SECOND = 10; // the minimum allowable steps per second.

        private double overdueSteps; // the progress/amount of steps that need to happen, but have not.
                                     // Note that overdue steps does not count the descrepancy between target and actual SPS.

        private bool _paused; // TODO: implement simulation pausing

        private List<Grid> _grids; // The grids currently in existence.



        // public properties
        public int TargetStepsPS
        {
            get => _targetStepsPS;
            set 
            {
                _targetStepsPS = (value > MIN_STEPS_PER_SECOND) ? value : MIN_STEPS_PER_SECOND;
                _actualStepsPS = _targetStepsPS;
            }
        }

        public int ActualStepsPS => _actualStepsPS;

        public List<Grid> Grids => _grids;

        public SimulationManager( double secondsBetweenFrames )
        {
            // I feel like I should comment this, but I don't think anything here needs explaining...
            targetFPS = 1.0 / secondsBetweenFrames;

            _targetStepsPS = (int)Math.Round(targetFPS);
            _actualStepsPS = _targetStepsPS;
            
            overdueSteps = 0;

            _grids = new List<Grid>();
       
        }

        // The expected step rate given the current simulation speed.
        private double StepsPerFrame()
        {
            return (double)_actualStepsPS / targetFPS;
        }

        // returns the amount of simulation steps to be performed in the next frame.
        private int StepsNextFrame()
        {
            return (int)(StepsPerFrame()+overdueSteps);
          
        }

        // the amount of overdue steps that will be created/destroyed next frame.
        private double overdueStepsNextFrame()
        {
            return StepsPerFrame() - StepsNextFrame();
        }


        public void Update( GameTime time)
        {
            // adjust our current steprate, if needbe

            adjustActualSPS(time.IsRunningSlowly);

         
            for(int i=0; i<StepsNextFrame(); i++)
            {
                // do a step.
                Step();

            }

            // update overdue steps.
            overdueSteps += overdueStepsNextFrame();
        }

        private void adjustActualSPS(bool isRunningSlowly)
        {

            // the rate that SPS will change in a frame. Fairly arbitrary.
            int SPSChange = 10;
             

            // this code feels dirty...
            // I don't know how to fix it though...

            if(isRunningSlowly)
            {
                // we want to prevent the steprate from decreasing below it's minimum.
                if( _actualStepsPS-SPSChange < MIN_STEPS_PER_SECOND )
                {
                    _actualStepsPS = MIN_STEPS_PER_SECOND;
                    return;
                }

                _actualStepsPS -= SPSChange;
                return;

            }

            // we can run the simulation faster again!
            if(_actualStepsPS<_targetStepsPS)
            {
                if(_actualStepsPS+SPSChange > _targetStepsPS)
                {
                    _actualStepsPS = _targetStepsPS;
                    return;
                }

                _actualStepsPS += SPSChange;
                return;

            }
        }

        private void Step()
        {
            // do a simulation step.
            // this is currently empty, but I imagine it would look like this:

            /* foreach(Grid g in Grids)
             * {
             *      g.generateFutureStates();
             * }
             * foreach(Grid g in Grids)
             * {
             *     g.updateStates();
             * }
             */
        }

        // add a grid to the list of grids
        // I wanted to make this protected, but apparently in C# that means something slightly different than java.
        // Apparently internal is closer to what I wanted, but still isn't...
        internal void addGrid(Grid g)
        {
            _grids.Add(g);
        }

        // remove a grid from the list of grids.
        internal void removeGrid(Grid g)
        {
            _grids.Remove(g);
        }

    }
}
