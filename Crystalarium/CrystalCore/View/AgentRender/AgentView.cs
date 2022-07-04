using CrystalCore.Sim;
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
    internal class AgentView : Subview
    {
        /*
         * An agent renderer renders agents. Makes sense.
         * AgentView.Renderer and ChunkView.RendererBase should have one superclass, shouldn't they...
         * 
         */


        private Texture2D _background; // Our background texture
        private Color _BGcolor; // our background color.
        private float _BGShrinkage; // the amount of the tile, in pixels per edge (at camera scale of 100) that is left blank when this agent is rendered.
                                  // did that make any sense? it is valid between 0 and 49. 



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

        public float BGShrinkage
        {

            get => _BGShrinkage;
            set
            {

                if (value < 0 || value >.49)
                {
                    throw new ArgumentException("The appropriate values for Background Shrinkage for agents are between 0 and .49 (inclusive). "+value+" is not valid.");
                }

              
                _BGShrinkage = value;

            }

        }


        internal AgentView(SubviewManager m, Agent a, List<Subview> others) : base(m, a, others)
        {

                
            _background = null;
            _BGcolor = Color.White;
            // get our parent


          
        }

        protected override void Render(SpriteBatch sb)
        {
         
            // render the thing if we have been set to.
            if (_background == null)
            {
                throw new InvalidOperationException("RenderConfig not supplied with required texture.");
            }
           
            renderTarget.Camera.RenderTexture(sb, _background, GetBGBorders(), _BGcolor, ((Agent)RenderData).Facing);
            
        }


        // get the borders of the background in tiles. fair enough.
        private RectangleF GetBGBorders()
        {
           

            RectangleF toReturn = new RectangleF(RenderData.Bounds);

            // Perform shrinkage.
            toReturn.Width -= _BGShrinkage * 2;
            toReturn.Height -= _BGShrinkage * 2;
            toReturn.X += _BGShrinkage;
            toReturn.Y += _BGShrinkage;
            
     
            return toReturn;

        }

    



    }
}

