using CrystalCore.Model.Core;

namespace CrystalCore.Model.Physical
{
    public interface MapComponent
    {

        public event ComponentEvent OnDestroy;



        public Grid Grid { get; }

        public bool Destroyed
        {
            get;

        }

        public abstract void Destroy();





    }
}
