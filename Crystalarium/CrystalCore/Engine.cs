using CrystalCore.Input;
using CrystalCore.Model;
using CrystalCore.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using CrystalCore.View.Configs;
using CrystalCore.Model.Rules;
using CrystalCore.Model.Elements;
using CrystalCore.Util;

namespace CrystalCore
{



    // A central access point for setting up, drawing, and updating.
    public class Engine:InitializableObject
    {
        private MapSaver _saver;
        private SimulationManager _sim;
        private Controller _controller;
    
       
      
        private List<GridView> _viewports;
        private List<Map> _grids;

        private List<Ruleset> _rulesets;
        private List<SkinSet> _skinSets;
      
        public SimulationManager Sim
        {
            get => _sim;
        }
        public MapSaver saveManager
        {
            get => _saver;
        }
        public Controller Controller
        {
            get => _controller;
        }

        public List<Ruleset> Rulesets
        {
            get => _rulesets;
        }

        



        public Engine(TimeSpan timeBetweenFrames)
        {
            _sim = new SimulationManager(timeBetweenFrames.TotalSeconds);
            
            _controller = new Controller();

            _viewports = new List<GridView>();
            _grids = new List<Map>();

            _skinSets = new List<SkinSet>();
            _rulesets = new List<Ruleset>();

            _saver = new MapSaver(this);

     
            

        }

        public void Initialize() 
        {
            try 
            { 
                foreach(Ruleset rs in _rulesets)
                {
                    rs.Initialize();
                }

                foreach(SkinSet ss in _skinSets)
                {
                    ss.Initialize();
                }
            }
            catch(InitializationFailedException e)
            {
               
                throw new InitializationFailedException("Crystalarium's Engine was given an invalid setup configuration, and cannot initialize.\nDetailed description of the problem:" + Util.Util.Indent(e.Message));
            }

            base.Initialize();



        }


        public void ReportKeybindConflicts()
        {

            List<Keybind> conflicts = Controller.ConflictingKeybinds();

            if (conflicts.Count==0)
            {
                return;
            }

            Console.WriteLine("Warning! The following keybinds are conflicted, and will not trigger.");
            foreach (Keybind kb in conflicts)
            {
                Console.WriteLine(kb);
            }
        }

        // manual ways to create gridviews
        public GridView addView(Map g, int x, int y, int width, int height, SkinSet skinSet)
        {
            return addView(g,new Point(x,y), new Point(width,height), skinSet);
        }

        public GridView addView(Map g, Point location, Point size, SkinSet skinSet)
        {
            if(!Initialized)
            {
                throw new InvalidOperationException("CrystalCore must be initalized before gridviews can be created. call Engine.Initialize().");
            }
            return new GridView(_viewports, g, location, size, skinSet);
        }

        public Map addGrid( Ruleset r)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("CrystalCore must be initalized before grids can be created. call Engine.Initialize().");
            }

            _grids.Add(new Map(Sim, r));

            return _grids[_grids.Count - 1];
        }

        public SkinSet addSkinSet(string name)
        {
            if (Initialized)
            {
                throw new InvalidOperationException("CrystalCore has already been Initialized. No more modifications may be done to SkinSets.");
            }
            SkinSet skinSet = new SkinSet(name, this);
            _skinSets.Add(skinSet);
            return skinSet;
        }

        public Ruleset addRuleset(string name)
        {
            if (Initialized)
            {
                throw new InvalidOperationException("CrystalCore has already been Initialized. No more modifications may be done to Rulesets.");
            }

            Ruleset rs = new Ruleset(name);
            _rulesets.Add(rs);
            return rs;
        }

        public void Update(GameTime gameTime)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("CrystalCore must be initalized before it can be updated. Call Engine.Initialize().");
            }


            _sim.Update(gameTime);
          


            // update viewports. (Camera Controls, mostly)
            foreach (GridView v in _viewports)
            {
                v.Update();
            }


            _controller.Update();

        }


        public void Draw(SpriteBatch sb)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("CrystalCore must be initalized before it can be drawn. Call Engine.Initialize().");
            }
           
            
            // draw viewports
            foreach (GridView v in _viewports)
            {
                v.Draw(sb);
            }
           
        }
    }
}
