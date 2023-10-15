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
        public MapObject PhysicalRepresentation { get; }

    }
}
