using CrystalCore.Model.Communication;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.View.SignalRender
{
    public class BeamViewTemplate
    {


        public Texture2D BeamTexture { get; set; } // the texture for use as the chunk's background.

        public Color Color { get; set; } // the default color for a chunk. default is white.

        public float BeamWidth { get; set; }

        public BeamViewTemplate()
        {
            BeamTexture = null;
            Color = Color.White;
            BeamWidth = .25f;
        }

        internal BeamView CreateRenderer(GridView v, Beam b, List<Subview> others)
        {
            // set up a new renderer
            BeamView toReturn = new BeamView(v, b, others, BeamTexture, Color)
            {
                BeamWidth = BeamWidth

            };

            return toReturn;
        }


        
    }
}
