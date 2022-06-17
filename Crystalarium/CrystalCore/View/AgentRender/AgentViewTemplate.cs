using CrystalCore.Sim;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.View.AgentRender
{
    public class AgentViewTemplate
    {
        


        public Texture2D AgentBackground { get; set; }

        public Color BackgroundColor { get; set; }

        public AgentViewTemplate()
        {
            AgentBackground = null;
            BackgroundColor = Color.White;
        }

        internal AgentView CreateRenderer(SubviewManager m, Agent a, List<Subview> others)
        {
            AgentView toReturn = new AgentView(m, a, others)
            {
                Background = AgentBackground,
                BackgroundColor = BackgroundColor
            };
            return toReturn;
        }

    }
}
