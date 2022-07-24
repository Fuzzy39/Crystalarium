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
    internal class AgentView : Subview
    {
        /*
         * An agent renderer renders agents. Makes sense.
         * AgentView.Renderer and ChunkView.RendererBase should have one superclass, shouldn't they...
         * 
         */


        private Texture2D _background; // Our background texture
        private Color _BGcolor; // our background color.

        private Texture2D _defaultTexture;
        private Color _color;
        private float _shrinkage; // the amount of the tile, in pixels per edge (at camera scale of 100) that is left blank when this agent is rendered.
                                  // did that make any sense? it is valid between 0 and 49. 

        public bool DoBackgroundShrinkage;
        public bool DoBackgroundRotation;

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

                
            _background = null;
            _BGcolor = Color.White;

            _defaultTexture = null;
            _color = Color.White;
            // get our parent

            DoBackgroundRotation = false;
            DoBackgroundShrinkage = false;
          
        }

        protected override void Render(SpriteBatch sb)
        {
         
            // render the thing if we have been set to.
            if (_defaultTexture == null)
            {
                throw new InvalidOperationException("RenderConfig not supplied with required texture.");
            }
           
            // render the background.
            if(_background != null)
            {
                RenderBackground(sb);
            }

            // render the Agent.
            renderTarget.Camera.RenderTexture(sb, _defaultTexture, ShrinkBorders(), _color, ((Agent)RenderData).Facing);

            if(renderTarget.DoDebugPortRendering)
            {
                if( _background == null)
                {
                    throw new InvalidOperationException("Agent Type " + ((Agent)RenderData).Type.Name + "'s RenderConfig requires a background texture to render debug ports.");
                }
                RenderDebugPorts(sb);
            }
            
        }


        private void RenderBackground(SpriteBatch sb)
        {
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

        private void RenderDebugPorts(SpriteBatch sb)
        {
            foreach (List < Port > ports in ((Agent)RenderData).Ports)
            {
                foreach(Port port in ports)
                {

                    RectangleF bounds= new RectangleF();

                    float width = .7f; // the amount of tile across
                    float thickness = .15f; // the amount of tile through
                    switch (port.AbsoluteFacing)
                    {
                        case CompassPoint.north:
                            bounds = new RectangleF((1-width)/2f, 0, width, thickness);
                            break;
                        case CompassPoint.northeast:
                            bounds = new RectangleF( 1-thickness, 0, thickness, thickness);
                            break;
                        case CompassPoint.east:
                            bounds = new RectangleF(1-thickness, (1 - width) / 2f, thickness, width);
                            break;
                        case CompassPoint.southeast:
                            bounds = new RectangleF(1 - thickness, 1 - thickness, thickness, thickness);
                            break;
                        case CompassPoint.south:
                            bounds = new RectangleF((1 - width) / 2f, 1-thickness, width, thickness);
                            break;  
                        case CompassPoint.southwest:
                            bounds = new RectangleF(0, 1 - thickness, thickness, thickness);
                            break;
                        case CompassPoint.west:
                            bounds = new RectangleF(0, (1 - width) / 2f, thickness, width);
                            break;
                        case CompassPoint.northwest:
                            bounds = new RectangleF(0, 0, thickness, thickness);
                            break;


                    }

                    
                    bounds = new RectangleF(bounds.Location + port.Location.ToVector2(), bounds.Size);

                    Color c = new Color();
                    switch(port.Status)
                    {
                        case PortStatus.inactive:
                            c = Color.DimGray;
                            break;
                        case PortStatus.receiving:
                            c = Color.Blue;
                            break;
                        case PortStatus.transmitting:
                            c = Color.Red;
                            break;
                        case PortStatus.transceiving:
                            c = Color.Purple;
                            break;
                    }

                    renderTarget.Camera.RenderTexture(sb, _background, bounds, c);
                }
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

