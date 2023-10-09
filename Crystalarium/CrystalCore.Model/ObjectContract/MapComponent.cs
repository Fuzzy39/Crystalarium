namespace CrystalCore.Model.ObjectContract
{
    internal interface MapComponent
    {

        public event EventHandler OnDestroy;
        public event EventHandler OnReady;

        public bool Destroyed
        {
            get;

        }

        public abstract void Destroy();

        protected void Ready();



    }
}
