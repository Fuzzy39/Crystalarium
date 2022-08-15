using CrystalCore.Rulesets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.View.Configs
{
    /// <summary>
    /// A skin describes all of a gridview's graphical settings when rendering a specific ruleset.
    /// </summary>
    public class Skin : IInitializable
    {
        private Ruleset ruleset; // the ruleset we render.
        private SkinSet parent; // the skinset we are part of.

        // the configs which make up most of this skin's settings.
        private ChunkViewConfig _chunkConfig;
        private BeamViewConfig _beamConfig;
        private List<AgentViewConfig> _agentConfigs;

        // Settings specific to the gridview itself belong here, such as border and background textures.

        private Texture2D _gridViewBG;
        private Color _gridViewBGColor;
    
        private bool _doAgentRendering; // whether a gridview with this skin renders agents and signals.

        // NOTE TO SELF: THIS BELONGS IN THE SKINSET!!
        private Texture2D _viewCastOverlay; // if this is non-null, any gridview with this skin must have a non-null ViewCastTarget.


        

        public ChunkViewConfig ChunkConfig
        {
            get 
            {
                return _chunkConfig; 
                
            }
            set 
            {
                if (!Initialized){ _chunkConfig = value; return; }
                throw new InvalidOperationException("Cannot access skin configs after initialization.");
            }
        }

        public BeamViewConfig BeamConfig
        {
            get
            {  
                // allowing access does risk users changing settings, but the beamconfigs will not allow this once they are initialized.
                return _beamConfig; 
                
            }
            set
            {
                if (!Initialized) { _beamConfig = value; return; }
                throw new InvalidOperationException("Cannot access skin configs after initialization.");
            }
        }
        
        public List<AgentViewConfig> AgentConfigs
        {
            get { return _agentConfigs; }
    
           
        }


        public Texture2D GridViewBG
        {
            get => _gridViewBG;
            set 
            {
                if (!Initialized) { _gridViewBG = value; return; }
                throw new InvalidOperationException("Cannot access skin configs after initialization.");
            }
        }

        public Texture2D ViewCastOverlay
        {
            get { return _viewCastOverlay; }
            set
            {
                if (!Initialized) { _viewCastOverlay = value; return; }
                throw new InvalidOperationException("Cannot access skin configs after initialization.");
            }
        }


        public bool DoAgentRendering // whether agents are rendered in this gridview
        {
            get => _doAgentRendering;
            set
            {
                if (!Initialized)
                {
                    _doAgentRendering = value; 
                    return;
                }
                throw new InvalidOperationException("Cannot access skin configs after initialization.");
            }

        }

        private bool Initialized { get; set; }

        internal Skin(Ruleset rs, SkinSet parent)
        {
            this.ruleset = rs;
            this.parent = parent;

            Initialized = false;
            _agentConfigs = new List<AgentViewConfig>();

        }

        void IInitializable.Initialize()
        {


            try
            {
                if (ChunkConfig == null)
                {
                    throw new InitializationFailedException("The Skin's ChunkConfig property was null.");
                }

                ChunkConfig.Initialize();

                if (BeamConfig == null)
                {
                    throw new InitializationFailedException("The Skin's BeamConfig property was null.");
                }

                BeamConfig.Initialize();
                // Skinby.Initalize();

                InitializeAgentConfigs();
            }
            catch (InitializationFailedException e)
            {
                throw new InitializationFailedException("The skin rendering ruleset '" + ruleset.Name + "' Failed to initialize:\n    " + e.Message);
            }
          
            Initialized = true;
        }


        void InitializeAgentConfigs()
        {
            List<AgentType> typesConfigured = new List<AgentType>();
            foreach(AgentViewConfig config in _agentConfigs)
            {
                if (typesConfigured.Contains( config.AgentType))
                {
                    throw new InitializationFailedException("There are multiple AgentViewConfigs for AgentType '" + config.AgentType.Name+"'.");
                }

                if(ruleset != config.AgentType.Ruleset)
                {
                    throw new InitializationFailedException("A config exists for an incorrect ruleset '"+ config.AgentType.Ruleset.Name + "', attempting to configure AgentType '" +
                         config.AgentType.Name+ "'.");
                }

                config.Initialize();
                
            }

            // check that all agent types have been configured.
            foreach(AgentType type in ruleset.AgentTypes)
            {
                if(!typesConfigured.Contains(type))
                {
                    throw new InitializationFailedException("Skin failed to include configs for AgentType '"+type.Name+"'.");
                }
            }
        }


    }
}
