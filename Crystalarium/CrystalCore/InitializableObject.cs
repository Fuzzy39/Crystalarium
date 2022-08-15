using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore
{
    /// <summary>
    /// Classes that implement this are part of the engine that requires initialization checks in order to be used.
    /// These checks are run by calling Engine.Initialize().
    /// </summary>
    public abstract class InitializableObject
    {
        public bool Initialized { get; private set; }

        internal InitializableObject()
        {
            Initialized = false;
        }

        internal virtual void Initialize()
        {
            Initialized = true;
        }
        
    }

    public class InitializationFailedException : Exception
    {
        public InitializationFailedException()
        {
        }

        public InitializationFailedException(string message)
            : base(message)
        {
        }

        public InitializationFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
