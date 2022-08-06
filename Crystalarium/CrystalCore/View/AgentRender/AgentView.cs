using CrystalCore.Model.Communication;
using CrystalCore.Model.Objects;
using CrystalCore.Util;
using CrystalCore.View.ChunkRender;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using ChunkRenderer = CrystalCore.View.ChunkRender.ChunkView;

namespace CrystalCore.View.AgentRender
{
    /// <summary>
    ///  An AgentView is a Subview that renders an agent and its ports.
    /// </summary>
    internal class AgentView : Subview
    {
    


        private Texture2D _background; // Our background texture
        private Color _BGcolor; // our background color.

        private Texture2D _defaultTexture;
        private Color _color;


        private float _shrinkage; // the amount of the tile, in pixels per edge (at camera scale of 100) that is left blank when this agent is rendered.
                                  // did that make any sense? it is valid between 0 and 49. 
        public bool DoBackgroundShrinkage; // whether the background is shrunken along with the primary texture.
        public bool DoBackgroundRotation; // whether the backroung is rotated along with the primary texture.


        private List<DebugPort> _ports;
        public Texture2D Background
        {
            get => _background;
            set => _background = value;
        }

        public Color BackgroundColor
        {
            get => _BGcolor;
            set => _BGcolor = value;
        }

        public Texture2D DefaultTexture
        {
            get => _defaultTexture;
            set => _defaultTexture = value;
        }

        public Color Color
        {
            get => _color;
            set => _color = value;  
        }

        public float Shrinkage
        {

            get => _shrinkage;
            set
            {

                if (value < 0 || value >.49)
                {
                    throw new ArgumentException("The appropriate values for Background Shrinkage for agents are between 0 and .49 (inclusive). "+value+" is not valid.");
                }

              
                _shrinkage = value;

            }

        }


        internal AgentView(GridView v, Agent a, List<Subview> others) : base(v, a, others)
        {
            // defaults need to be set, but there is no need here, since our template does the job.
            _ports = null;
        }


       

        protected override void Render(SpriteBatch sb)
        {
         
            // render the thing if we have been set to.
            if (_defaultTexture == null)
            {
                throw new InvalidOperationException("RenderConfig not supplied with required texture.");
            }
           
            // render the background.
            

            // render the Agent.
            renderTarget.Camera.RenderTexture(sb, _defaultTexture, ShrinkBorders(), _color, ((Agent)RenderData).Facing);

            if(renderTarget.DoDebugPortRendering)
            {
                if (_ports == null)
                {
                    DebugPortSetup();
                }

                foreach (DebugPort dp in _ports)
                {
                    dp.Draw(sb);

                }
            }
            
        }


        internal void RenderBackground(SpriteBatch sb)
        {
            if (_background == null)
            {
                return;
            }

            RectangleF bounds = new RectangleF(RenderData.Bounds);
            Direction facing = Direction.up;

            if(DoBackgroundShrinkage)
            {
                bounds = ShrinkBorders();
            }

            if(DoBackgroundRotation)
            {
                facing = ((Agent)RenderData).Facing;
            }


            renderTarget.Camera.RenderTexture(sb, _background, bounds, _BGcolor, facing);
        }
        private void DebugPortSetup()
        {
            _ports = new List<DebugPort>();

            if (_background == null)
            {
                throw new InvalidOperationException("Agent Type " + ((Agent)RenderData).Type.Name + "'s RenderConfig requires a background texture to render debug ports.");
            }

            foreach (Port p in ((Agent)RenderData).PortList)
            {
                _ports.Add(new DebugPort(_background, p, this, renderTarget));
            }

        }
    

        // get the borders of the background in tiles. fair enough.
        private RectangleF ShrinkBorders()
        {
           

            RectangleF toReturn = new RectangleF(RenderData.Bounds);

            // Perform shrinkage.
            return toReturn.Inflate(-_shrinkage, -_shrinkage);

        }

    



    }
}

