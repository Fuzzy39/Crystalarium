using CrystalCore.Model.Objects;
using CrystalCore.Model.Rules;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.View.Configs
{
    public class AgentViewConfig:InitializableObject
    {


        private AgentType _type; // what type of agent is this config for?

        private Texture2D _background;  // Our background texture

        private Color _backgroundColor; // our background color.

        private Texture2D _defaultTexture;
        private Direction _textureFacing;
        private Color _color;

        private float _shrinkage;// the amount of the tile, in pixels per edge (at camera scale of 100) that is left blank when this agent is rendered.
                                  // did that make any sense? it is valid between 0 and 49. 
        private bool _doBackgroundShrinkage;  // whether the background is shrunken along with the primary texture.



        public Texture2D Background { get => _background; set => _background = value; }

        public Color BackgroundColor
        {
            get => _backgroundColor;
            set => _backgroundColor = value;
        }

        /// <summary>
        /// the direction the texture is facing.
        /// </summary>
        public Direction TextureFacing
        {
            get => _textureFacing;
            set => _textureFacing = value;
        }

        public float Shrinkage
        {
            get => _shrinkage;
            set
            {

                if (value < 0 || value > .49)
                {
                    throw new ArgumentException("The appropriate values for Background Shrinkage for agents are between 0 and .49 (inclusive). " + value + " is not valid.");
                }


                _shrinkage = value; 

            }
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

        public bool DoBackgroundShrinkage
        {
            get => _doBackgroundShrinkage;
            set => _doBackgroundShrinkage = value;
        }

        public AgentType AgentType
        {
            get => _type;
        }

        public AgentViewConfig(AgentType type) : base()
        {
            // defaults for stuff.
            Background = null;
            BackgroundColor = Color.White;
            Shrinkage = 0;
            Color = Color.White;
            DefaultTexture = null;
            DoBackgroundShrinkage = false;
            TextureFacing = Direction.up;
            _type = type;
        }

        public AgentViewConfig(AgentViewConfig from, AgentType type) : base()
        {
            Background = from.Background;
            BackgroundColor = from.BackgroundColor;
            Shrinkage = from.Shrinkage;
            Color = from.Color;
            DefaultTexture = from.DefaultTexture;
            TextureFacing = from.TextureFacing;
            _type = type;
        }


        public override void Initialize()
        {
            if (DefaultTexture == null)
            {
                throw new InitializationFailedException("AgentView missing Default Texture.");
            }

            base.Initialize();
        }
       

    }
}
