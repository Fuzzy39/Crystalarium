namespace CrystalCore.Util
{
    /// <summary>
    /// Classes that implement this are part of the engine that requires initialization checks in order to be used.
    /// These checks are run by calling Engine.Initialize().
    /// </summary>
    public abstract class InitializableObject
    {
        public bool Initialized { get; private set; }

        public InitializableObject()
        {
            Initialized = false;
        }

        public virtual void Initialize()
        {
            Initialized = true;
        }

    }

}
