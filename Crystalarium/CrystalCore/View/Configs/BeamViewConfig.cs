using CrystalCore.Model.Communication;
using CrystalCore.View.Subviews;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.View.Configs
{
    public class BeamViewConfig
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
            set
            {

                if (value >= .01f & value <= .5f)
                {
                    beamWidth = value;
                    return;
                }

                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        public BeamViewConfig()
        {
            BeamTexture = null;
            Color = Color.White;
            BeamWidth = .25f;
        }


    }
}
