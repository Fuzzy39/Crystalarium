using CrystalCore.Input;
using CrystalCore.Sim;
using CrystalCore.View;
using CrystalCore.View.ChunkRender;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore
{
    // A central access point for setting up, drawing, and updating.
    public class CrystalCore
    {

        private SimulationManager _sim;
        private Controller _controller;

        private List<GridView> _viewports;
      
        public SimulationManager Sim
        {
            get => _sim;
        }

        public Controller Controller
        {
            get => _controller;
        }


        public CrystalCore(TimeSpan timeBetweenFrames)
        {
            _sim = new SimulationManager(timeBetweenFrames.TotalSeconds);
            _controller = new Controller();

            _viewports = new List<GridView>();
            
        }


        // manual ways to create gridviews
        public GridView addView(Grid g, int x, int y, int width, int height, RendererTemplate renderConfig )
        {
            return new GridView(_viewports, g, x, y, width, height, renderConfig);
        }

        public GridView addView(Grid g, Point location, Point size, RendererTemplate renderConfig)
        {
            return new GridView(_viewports, g, location, size, renderConfig);
        }


        public void Update(GameTime gameTime)
        {
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
            // draw viewports
            foreach (GridView v in _viewports)
            {
                v.Draw(sb);
            }

        }
    }
}
