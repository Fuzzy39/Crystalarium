using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Util
{


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


    public class MapLoadException : Exception
    {
        public MapLoadException()
        {
        }

        public MapLoadException(string message)
            : base(message)
        {
        }

        public MapLoadException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

}
