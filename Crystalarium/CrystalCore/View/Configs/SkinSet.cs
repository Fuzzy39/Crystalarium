using CrystalCore.Rulesets;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.View.Configs
{
    public class SkinSet : InitializableObject
    {
        private List<Skin> _skins;

       
        
        private Texture2D _viewCastOverlay; // if this is non-null, any gridview with this skin must have a non-null ViewCastTarget.
        private Engine parent;

        public string Name { get; private set; }
        
        public List<Skin> Skins
        {
            get { return _skins; }
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


        // Constructors
        internal SkinSet(string name, Engine e) : base()
        {
            _skins = new List<Skin>();
            Name = name;
            parent = e;
        }

        // Methods
        public Skin GetSkin(Ruleset rs)
        {
            foreach(Skin skin in _skins)
            {
                if(skin.Ruleset == rs)
                {
                    return skin;
                }
               
            }

            return null;
        }

        public Skin GetSkin(string s)
        {
            foreach (Skin skin in _skins)
            {
                if (skin.Ruleset.Name == s)
                {
                    return skin;
                }
            }
            return null;
        }

        internal override void Initialize()
        {
            // viewcastoverlay: required
            try
            {
                if (_viewCastOverlay == null)
                {
                    throw new InitializationFailedException("The skinset's ViewCastOverlay property was null.");
                }

                // check that a skin exists for every ruleset.
                foreach(Ruleset rs in parent.Rulesets)
                {
                    if(GetSkin(rs)==null)
                    {
                        throw new InitializationFailedException("The skinset is missing a skin for ruleset '" + rs.Name + "'.");
                    }
                }

            }
            catch(InitializationFailedException e)
            {
               
                throw new InitializationFailedException("SkinSet '"+Name+"' failed to initialize: "+Util.Util.Indent(e.Message));
            }


            base.Initialize();
        }
    }
}
