using CrystalCore.Util;
using CrystalCore.Util.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrystalCore.View.Configs
{
    public class SignalViewConfig : InitializableObject
    {

        // See the SignalView class for descriptions of these fields.

        private Texture2D beamTexture;
        private Gradient colors;
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

        public Gradient Colors // the default color for a signal. default is white.
        {
            get => colors;
            set
            {
                if (!Initialized)
                {
                    colors = value;
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
            Colors = new Gradient(new Gradient.ColorStop(Color.White, 0f));
            SignalWidth = .25f;
        }

        public SignalViewConfig(SignalViewConfig from) : base()
        {
            SignalTexture = from.beamTexture;
            Colors = from.Colors;
            SignalWidth = from.beamWidth;
        }

        public override void Initialize()
        {

            base.Initialize();
        }


    }
}
