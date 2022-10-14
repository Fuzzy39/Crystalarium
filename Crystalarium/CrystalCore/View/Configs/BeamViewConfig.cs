using CrystalCore.Model.Communication;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.View.Configs
{
    public class BeamViewConfig : InitializableObject
    {

        // See the BeamView class for descriptions of these fields.

        private Texture2D beamTexture;
        private Color color;
        private float beamWidth;

        public Texture2D BeamTexture // the texture for use as the chunk's background.
        {
            get => beamTexture;
            set
            {
                if (!Initialized) { beamTexture = value; return; }
                throw new InvalidOperationException("Cannot modify skin config after engine initialization.");
            }

        }

        public Color Color // the default color for a chunk. default is white.
        {
            get => color;
            set
            {
                if (!Initialized)
                {
                    color = value;
                    return;
                }
                throw new InvalidOperationException("Cannot modify skin config after engine initialization.");
            }

        }


        public float BeamWidth
        {
            get => beamWidth;
            set
            {
                if (Initialized)
                {
                    throw new InvalidOperationException("Cannot modify skin config after engine initialization.");
                }

                if (value >= .01f & value <= .5f)
                {
                    beamWidth = value;
                    return;
                }

                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        public BeamViewConfig() : base()
        {
            BeamTexture = null;
            Color = Color.White;
            BeamWidth = .25f;
        }

        public BeamViewConfig(BeamViewConfig from) : base()
        {
            BeamTexture = from.beamTexture;
            Color = from.Color;
            BeamWidth = from.beamWidth;
        }

        internal override void Initialize()
        {
         
            base.Initialize();
        }


    }
}
