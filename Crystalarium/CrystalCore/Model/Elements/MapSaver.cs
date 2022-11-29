using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CrystalCore.Model.Elements
{
    public class MapSaver
    {

        private Map m;
        internal MapSaver(Map m)
        { 
            this .m = m;
        }


        public void Save(string path)
        {
            // throw new NotImplementedException();
            XmlWriter.Create(IS);
        }

        public void Load(string path)
        {
            throw new NotImplementedException();
        }

    }
}
