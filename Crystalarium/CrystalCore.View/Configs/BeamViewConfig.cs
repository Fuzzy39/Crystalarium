using CrystalCore.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.View.Configs
{
    public class SignalViewConfig : InitializableObject
    {

        // See the SignalView class for descriptions of these fields.

        private Texture2D beamTexture;
        private Color color;
        private float beamWidth;

        public Texture2D SignalTexture // the texture for use as the chunk's background.
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


        public float SignalWidth
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

        public SignalViewConfig() : base()
        {
            SignalTexture = null;
            Color = Color.White;
            SignalWidth = .25f;
        }

        public SignalViewConfig(SignalViewConfig from) : base()
        {
            SignalTexture = from.beamTexture;
            Color = from.Color;
            SignalWidth = from.beamWidth;
        }

        public override void Initialize()
        {
         
            base.Initialize();
        }


    }
}
