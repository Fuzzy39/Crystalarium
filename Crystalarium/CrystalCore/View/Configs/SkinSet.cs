using CrystalCore.Rulesets;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.View.Configs
{
    internal class SkinSet : InitializableObject
    {
        private List<Skin> _skins;

        public List<Skin> Skins
        {
            get { return _skins; }
        }

        public SkinSet():base()
        {
            _skins = new List<Skin>();
        }

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
            throw new NotImplementedException();
            base.Initialize();
        }
    }
}
