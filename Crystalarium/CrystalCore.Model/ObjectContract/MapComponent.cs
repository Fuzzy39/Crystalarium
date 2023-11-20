

using CrystalCore.Model.CoreContract;

namespace CrystalCore.Model.ObjectContract
{
    public interface MapComponent
    {

        public event EventHandler OnDestroy;
      


        public Grid Grid { get; }

        public bool Destroyed
        {
            get;

        }

        public abstract void Destroy();

     



    }
}
