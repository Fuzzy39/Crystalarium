using CrystalCore.Model.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrystalCore.Model.Rulesets.Conditions
{
    internal abstract class Resolvable : InitializableObject
    {

        internal abstract Token Resolve(Agent a);
    }
}
