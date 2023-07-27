using CrystalCore.Util.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;


namespace Crystalarium.Main
{
    struct Textures
    {

        // basic textures.
        internal static Texture2D testSquare;
        internal static Texture2D pixel;



        //crysm textures
        internal static Texture2D tile; // unused
        internal static Texture2D chunkGrid;
        internal static Texture2D altChunkGrid;
        internal static Texture2D sampleAgent;

        //
        internal static Texture2D emitter;
        internal static Texture2D channel;
        internal static Texture2D luminalGate;
        internal static Texture2D mirror;
        internal static Texture2D prism;
        internal static Texture2D stopper;
        internal static Texture2D notGate;
        internal static Texture2D splitter;

        // other textrues
        internal static Texture2D viewboxBG;


        internal static SpriteFont testFont;
        internal static FontFamily Consolas;


        // should this class be, you know, an actual class instead of this?
        // Yes!
        // but I'm lazy.
        internal static void LoadContent(ContentManager Content)
        {
            // initialize fonts
            Textures.testFont = Content.Load<SpriteFont>("Consolas12");
            Textures.Consolas = new FontFamily(
                new SpriteFont[]{
                Content.Load<SpriteFont>("Consolas48"),
                Content.Load<SpriteFont>("Consolas200")
                });

            // textures.
            Textures.pixel = Content.Load<Texture2D>("pixel");
            Textures.tile = Content.Load<Texture2D>("tile");
            Textures.testSquare = Content.Load<Texture2D>("testSquare");
            Textures.viewboxBG = Content.Load<Texture2D>("ViewportBG");
            Textures.chunkGrid = Content.Load<Texture2D>("chunkGrid");
            Textures.altChunkGrid = Content.Load<Texture2D>("AltChunkGrid");
            Textures.sampleAgent = Content.Load<Texture2D>("SampleAgent");

            // agent textures
            Textures.emitter = Content.Load<Texture2D>("Agents/emitter");
            Textures.channel = Content.Load<Texture2D>("Agents/channel");
            Textures.luminalGate = Content.Load<Texture2D>("Agents/luminalGate");
            Textures.mirror = Content.Load<Texture2D>("Agents/mirror");
            Textures.notGate = Content.Load<Texture2D>("Agents/notgate");
            Textures.prism = Content.Load<Texture2D>("Agents/prism");
            Textures.stopper = Content.Load<Texture2D>("Agents/stopper");
            splitter = Content.Load<Texture2D>("Agents/splitter");
        }

    }
}
