﻿using Microsoft.Xna.Framework;


namespace CrystalCore.Model.Core
{
    public class SimulationManager
    {
        // oh, my oh my...
        // it begins.
        // yet another big project.



        private double targetFPS; // the target FPS of the game. I can only imagine that it's 60.

        private int _targetStepsPS; // the amount of times the simulation needs to update per second.
        private int _actualStepsPS; // the amount of times that the simulation is currently running per second.
                                    // Unless the game is lagging, it should be the same as the target.

        public const int MIN_STEPS_PER_SECOND = 5; // the minimum allowable steps per second.

        private double overdueSteps; // the progress/amount of steps that need to happen, but have not.
                                     // Note that overdue steps does not count the descrepancy between target and actual SPS.

        private bool _paused; // TODO: implement simulation pausing

        private List<Map> _maps; // The grids currently in existence.


        public bool Paused
        {
            get { return _paused; }
            set
            {
                if (value)
                {
                    _actualStepsPS = _targetStepsPS;
                }
                _paused = value;
            }

        }

        // public properties
        public int TargetStepsPS
        {
            get => _targetStepsPS;
            set
            {
                _targetStepsPS = value > MIN_STEPS_PER_SECOND ? value : MIN_STEPS_PER_SECOND;
                _actualStepsPS = _targetStepsPS;
            }
        }

        public int ActualStepsPS => _actualStepsPS;

        public List<Map> Grids => _maps;

        public SimulationManager(double secondsBetweenFrames)
        {
            // I feel like I should comment this, but I don't think anything here needs explaining...
            targetFPS = 1.0 / secondsBetweenFrames;

            _targetStepsPS = (int)Math.Round(targetFPS);
            _actualStepsPS = _targetStepsPS;

            overdueSteps = 0;

            _maps = new List<Map>();
            _paused = true;

        }

        // The expected step rate given the current simulation speed.
        private double StepsPerFrame()
        {
            return _actualStepsPS / targetFPS;
        }

        // returns the amount of simulation steps to be performed in the next frame.
        private int StepsNextFrame()
        {
            return (int)(StepsPerFrame() + overdueSteps);

        }

        // the amount of overdue steps that will be created/destroyed next frame.
        private double overdueStepsNextFrame()
        {
            return StepsPerFrame() - StepsNextFrame();
        }


        public void Update(GameTime time)
        {
            // adjust our current steprate, if needbe
            if (Paused)
            {
                _actualStepsPS = 0;
                return;
            }

            adjustActualSPS(time.IsRunningSlowly);


            for (int i = 0; i < StepsNextFrame(); i++)
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
            int SPSChange = 1;


            // this code feels dirty...
            // I don't know how to fix it though...

            /*if(isRunningSlowly)
            {
                // we want to prevent the steprate from decreasing below it's minimum.
                if( _actualStepsPS-SPSChange < MIN_STEPS_PER_SECOND )
                {
                    _actualStepsPS = MIN_STEPS_PER_SECOND;
                    return;
                }

                _actualStepsPS -= SPSChange;
                return;

            }*/

            // we can run the simulation faster again!
            if (_actualStepsPS < _targetStepsPS)
            {
                if (_actualStepsPS + SPSChange > _targetStepsPS)
                {
                    _actualStepsPS = _targetStepsPS;
                    return;
                }

                _actualStepsPS += SPSChange;
                return;

            }
        }

        public void Step()
        {
            // do a simulation step.


            foreach (Map g in Grids)
            {
                g.Step();
            }

        }

        // add a grid to the list of grids
        // I wanted to make this protected, but apparently in C# that means something slightly different than java.
        // Apparently internal is closer to what I wanted, but still isn't...
        public void addMap(Map g)
        {
            _maps.Add(g);
        }

        // remove a grid from the list of grids.
        public void removeMap(Map g)
        {
            _maps.Remove(g);
        }


        ///// <summary>
        ///// Perform a simulation step for this grid.
        ///// </summary>
        //internal void Step()
        //{


        //    // have each agent determine the state they will be in next step based on the state of the grid last step.
        //    foreach (Agent a in _agents)
        //    {
        //        a.CalculateNextStep();
        //    }

        //    // have each agent perform it's next step, no longer needing to look at the state of the grid.
        //    for (int i = 0; i < _agents.Count; i++)
        //    {
        //        Agent a = _agents[i];

        //        a.Update();

        //        // transformations applied to agents can destroy them.
        //        if (a.Destroyed)
        //        {
        //            i--;
        //        }


        //    }
        //}

    }
}
