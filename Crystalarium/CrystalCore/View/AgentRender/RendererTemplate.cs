using CrystalCore.Sim;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.View.AgentRender
{
    public class RendererTemplate
    {
        


        public Texture2D AgentBackground { get; set; }

        public Color BackgroundColor { get; set; }

        public RendererTemplate()
        {
            AgentBackground = null;
            BackgroundColor = Color.White;
        }

        internal BasicRenderer CreateRenderer(GridView v, Agent a, List<RendererBase> others)
        {
            BasicRenderer toReturn = new BasicRenderer(v, a, others)
            {
                Background = AgentBackground,
                BackgroundColor = BackgroundColor
            };
            return toReturn;
        }

    }
}
