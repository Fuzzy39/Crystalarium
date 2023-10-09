using CrystalCore.Model.Rules;
using CrystalCore.Util;
using Microsoft.Xna.Framework.Graphics;

namespace CrystalCore.View.Configs
{
    public class SkinSet : InitializableObject
    {
        private List<Skin> _skins;

        private Texture2D _viewCastOverlay; // if this is non-null, any gridview with this skin must have a non-null ViewCastTarget.


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
        public SkinSet(string name) : base()
        {
            _skins = new List<Skin>();
            Name = name;

        }

        // Methods
        public Skin GetSkin(Ruleset rs)
        {
            foreach (Skin skin in _skins)
            {
                if (skin.Ruleset == rs)
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

        public override void Initialize()
        {
            // viewcastoverlay: required
            try
            {
                if (_viewCastOverlay == null)
                {
                    throw new InitializationFailedException("The skinset's ViewCastOverlay property was null.");
                }




                foreach (Skin skin in _skins)
                {
                    skin.Initialize();
                }

            }
            catch (InitializationFailedException e)
            {

                throw new InitializationFailedException("SkinSet '" + Name + "' failed to initialize: " + MiscUtil.Indent(e.Message));
            }


            base.Initialize();
        }
    }
}
