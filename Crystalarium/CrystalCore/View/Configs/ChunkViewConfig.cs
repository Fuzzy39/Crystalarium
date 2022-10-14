using CrystalCore.Model.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.View.Configs
{
    public class ChunkViewConfig:InitializableObject
    {

        // A renderer template is created by the user and given to gridviews to use so that they can create chunk renderers.
        // the user configures how they want chunk renderers to behave here.
        // this kinda implements the builder pattern, I think?

        private Texture2D _chunkBG; // the texture for use as the chunk's background.

        private Color _BGColor; // the default color for a chunk. default is white.

        private int _brightenAmount; // the amount of increase in every RGB channel, should a chunk be brightened.
                                     // any brightening events will stack with each other.

        private bool _doCheckerBoardColoring; // if true, every other chunk will be brighter, in a checkerboard pattern.


        private Color? _originChunkColor; // if not null, the chunk with coords 0,0 will have this color.

        private GridView _viewCastTarget; // if not null, chunks viewed by this gridview (assuming it has the same grid) will be brightened.
                                          // this gridview should not be the same as the gridview that this renderer is part of.

        private Texture2D _viewCastOverlay; // if not null, this image (opaque and dark) will be plastered on the Gridview where the _viewCastTarget's
                                            // view lies.



        //properties

        public Texture2D ChunkBackground
        {
            get { return _chunkBG; }
            set
            {
                if (!Initialized) { _chunkBG = value; return; }
                throw new InvalidOperationException("Cannot modify skin config after engine initialization.");
            }
        }


        public Color BackgroundColor
        {
            get { return _BGColor; }
            set
            {
                if (!Initialized) { _BGColor = value; return; }
                throw new InvalidOperationException("Cannot modify skin config after engine initialization.");
            }
        }


        public int BrightenAmmount
        {
            get { return _brightenAmount; }
            set
            {
                if (!Initialized) { _brightenAmount = value; return; }
                throw new InvalidOperationException("Cannot modify skin config after engine initialization.");
            }
        }


        public bool DoCheckerBoardColoring
        {
            get { return _doCheckerBoardColoring; }
            set 
            { 
                if (!Initialized) {  _doCheckerBoardColoring = value; return; }
                throw new InvalidOperationException("Cannot modify skin config after engine initialization.");
            }
        }

        public Color? OriginChunkColor
        {
            get { return _originChunkColor; }
            set 
            {
                if (!Initialized) { _originChunkColor = value; return; }
                throw new InvalidOperationException("Cannot modify skin config after engine initialization.");
            }
        }

       

        public ChunkViewConfig(): base()
        {
            _chunkBG = null;
            _BGColor = Color.White;
            _brightenAmount = 30;
            _doCheckerBoardColoring = false;
            _originChunkColor = null;
            _viewCastTarget = null;

    
            
        }

        public ChunkViewConfig(ChunkViewConfig from) : base()
        {
            _chunkBG = from.ChunkBackground;
            _BGColor = from.BackgroundColor;
            _brightenAmount = from._brightenAmount;
            _doCheckerBoardColoring = from.DoCheckerBoardColoring;
            _originChunkColor = from.OriginChunkColor;
            _viewCastTarget = from._viewCastTarget;

           
        }

        internal override void Initialize()
        { 
            if(ChunkBackground==null)
            {
                throw new InitializationFailedException("ChunkViewConfig requires a background texture.");
            }
            base.Initialize();
        }



    }
}
