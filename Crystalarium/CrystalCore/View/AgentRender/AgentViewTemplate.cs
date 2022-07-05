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

        public float Shrinkage { get; set; }

        public Texture2D DefaultTexture { get; set; }

        public Color Color { get; set; }

        public bool DoBackgroundShrinkage{ get; set; }

        public bool DoBackgroundRotation { get; set; }  

        public AgentViewTemplate()
        {
            // defaults for stuff.
            AgentBackground = null;
            BackgroundColor = Color.White;
            Shrinkage = 0;
            Color = Color.White;
            DefaultTexture = null;
            DoBackgroundShrinkage = false;
            DoBackgroundRotation = false;   
        }

        public AgentViewTemplate(AgentViewTemplate from)
        {
            AgentBackground = from.AgentBackground;
            BackgroundColor = from.BackgroundColor;
            Shrinkage = from.Shrinkage;
            Color = from.Color;
            DefaultTexture = from.DefaultTexture;
            DoBackgroundShrinkage = from.DoBackgroundShrinkage;
            DoBackgroundRotation = from.DoBackgroundRotation;

        }

        internal AgentView CreateRenderer(GridView v, Agent a, List<Subview> others)
        {
            AgentView toReturn = new AgentView(v, a, others)
            {
                Background = AgentBackground,
                BackgroundColor = BackgroundColor,
                Shrinkage = Shrinkage,
                DefaultTexture = DefaultTexture,
                Color = Color,
                DoBackgroundShrinkage = DoBackgroundShrinkage,
                DoBackgroundRotation = DoBackgroundRotation
            };
            return toReturn;
        }

    }
}
