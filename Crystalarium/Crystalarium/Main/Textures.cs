using CrystalCore.Util.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


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

        // this is an ugly and untennable system
        // it will be fixed by the time Milestone 9 is done, surely.


        // crystalarium
        internal static Texture2D channel;
        internal static Texture2D luminalGate;

        // general
        internal static Texture2D stopper;
        internal static Texture2D mirror;

        internal static Texture2D prism;
        internal static Texture2D splitter;

        // gates
        internal static Texture2D notGate;
        internal static Texture2D and;
        internal static Texture2D or;

        // triangles
        internal static Texture2D emitter;
        internal static Texture2D increment;
        internal static Texture2D decrement;

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
            Textures.pixel =        Content.Load<Texture2D>("pixel");
            Textures.tile =         Content.Load<Texture2D>("tile");
            Textures.testSquare =   Content.Load<Texture2D>("testSquare");
            Textures.viewboxBG =    Content.Load<Texture2D>("viewportBG");
            Textures.chunkGrid =    Content.Load<Texture2D>("chunkGrid");
            Textures.altChunkGrid = Content.Load<Texture2D>("altChunkGrid");
            Textures.sampleAgent =  Content.Load<Texture2D>("sampleAgent");

            // agent textures
            Textures.emitter =      Content.Load<Texture2D>("Agents/emitter");
            Textures.channel =      Content.Load<Texture2D>("Agents/channel");
            Textures.luminalGate =  Content.Load<Texture2D>("Agents/luminalGate");
            Textures.mirror =       Content.Load<Texture2D>("Agents/mirror");
            Textures.notGate =      Content.Load<Texture2D>("Agents/not");
            Textures.prism =        Content.Load<Texture2D>("Agents/prism");
            Textures.stopper =      Content.Load<Texture2D>("Agents/stopper");
            splitter =              Content.Load<Texture2D>("Agents/splitter");
            and =                   Content.Load<Texture2D>("Agents/and");
            or =                    Content.Load<Texture2D>("Agents/or");
            increment =             Content.Load<Texture2D>("Agents/increment");
            decrement =             Content.Load<Texture2D>("Agents/decrement");


        }

    }
}
