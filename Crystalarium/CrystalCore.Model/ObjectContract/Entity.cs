using CrystalCore.Model.CoreContract;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.Model.ObjectContract
{

    /// <summary>
    /// An Entity is responsible for the simulation behavior of a MapOpject.
    /// </summary>
    public interface Entity
    {
        public event EventHandler OnReady;

        public MapObject PhysicalRepresentation { get; }

        public Point Size { get; }

        public bool Destroyed { get; }

        public void Destroy();

    }
}
