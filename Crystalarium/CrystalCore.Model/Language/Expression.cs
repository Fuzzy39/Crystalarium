using CrystalCore.Util;

namespace CrystalCore.Model.Language
{
    /// <summary>
    /// Part of a condition. Can be resolved.
    /// </summary>
    public abstract class Expression : InitializableObject
    {
        internal TokenType ReturnType { get; private set; }


        internal Expression(TokenType tokenType)
        {
            ReturnType = tokenType;

        }

        internal abstract Token Resolve(object context);


        public override void Initialize()
        {
            if (Initialized)
            {
                throw new InitializationFailedException("Hit Circular Resolution. Conditions may not contain themselves.");
            }

            base.Initialize();
        }

    }
}
