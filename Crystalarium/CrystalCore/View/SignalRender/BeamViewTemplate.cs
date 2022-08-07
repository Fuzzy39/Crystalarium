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

        // See the BeamView class for descriptions of these fields.
        
        private Texture2D beamTexture;
        private Color color;
        private float beamWidth;

        public Texture2D BeamTexture // the texture for use as the chunk's background.
        { 
            get => beamTexture;
            set => beamTexture = value; 
        } 

        public Color Color // the default color for a chunk. default is white.
        { 
            get => color;
            set => color = value;
        } 

        public float BeamWidth 
        { 
            get => beamWidth; 
            set => beamWidth = value; 
        }

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
