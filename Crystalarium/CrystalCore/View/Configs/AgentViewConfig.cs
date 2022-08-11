using CrystalCore.Model.Objects;
using CrystalCore.View.Subviews;
using CrystalCore.View.Subviews.Agents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.View.Configs
{
    public class AgentViewConfig
    {

        // See the AgentView class for descriptions of these fields.

        private Texture2D _agentBackground;
        private Color _backgroundColor;
        private bool _doBackgroundRotation;

        private Texture2D _defaultTexture;
        private Color _color;

        private float _shrinkage;
        private bool _doBackgroundShrinkage;



        public Texture2D AgentBackground { get => _agentBackground; set => _agentBackground = value; }

        public Color BackgroundColor
        {
            get => _backgroundColor;
            set => _backgroundColor = value;
        }

        public float Shrinkage
        {
            get => _shrinkage;
            set => _shrinkage = value;
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

        public bool DoBackgroundRotation
        {
            get => _doBackgroundRotation;
            set => _doBackgroundRotation = value;
        }

        public AgentViewConfig()
        {
            // defaults for stuff.
            AgentBackground = null;
            BackgroundColor = Color.White;
            Shrinkage = 0;
            Color = Color.White;
            DefaultTexture = null;
            DoBackgroundShrinkage = false;
            DoBackgroundRotation = false;
        }

        public AgentViewConfig(AgentViewConfig from)
        {
            AgentBackground = from.AgentBackground;
            BackgroundColor = from.BackgroundColor;
            Shrinkage = from.Shrinkage;
            Color = from.Color;
            DefaultTexture = from.DefaultTexture;
            DoBackgroundShrinkage = from.DoBackgroundShrinkage;
            DoBackgroundRotation = from.DoBackgroundRotation;

        }

        internal AgentView CreateRenderer(GridView v, Agent a, List<Subview> others)
        {
            AgentView toReturn = new AgentView(v, a, others)
            {
                Background = AgentBackground,
                BackgroundColor = BackgroundColor,
                Shrinkage = Shrinkage,
                DefaultTexture = DefaultTexture,
                Color = Color,
                DoBackgroundShrinkage = DoBackgroundShrinkage,
                DoBackgroundRotation = DoBackgroundRotation
            };
            return toReturn;
        }

    }
}
