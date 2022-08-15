using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore
{
    /// <summary>
    /// Classes that implement this are part of the engine that requires initialization checks in order to be used.
    /// These checks are run by calling Engine.Initialize().
    /// </summary>
    internal interface IInitializable
    {
        bool Initialized { get; set; }

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
