

namespace CrystalCore.Model.ObjectContract
{
    public interface MapComponent
    {

        public event EventHandler OnDestroy;
        public event EventHandler OnReady;


        public Map Map { get; }

        public bool Destroyed
        {
            get;

        }

        public abstract void Destroy();

        protected void Ready();



    }
}
