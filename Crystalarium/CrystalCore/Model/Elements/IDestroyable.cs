using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Elements
{
    internal interface IDestroyable
    {

        public event EventHandler OnDestroy;

        public bool Destroyed
        {
            get;
         
        }

        public abstract void Destroy();
       
    }
}
