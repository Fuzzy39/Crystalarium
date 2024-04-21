using CrystalCore.Model.Core;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.Physical.Default
{
    internal class DefaultComponentFactory : ComponentFactory
    {
        private Map _map;

        public Map Map => _map;

        public DefaultComponentFactory(Map map)
        {
            _map = map;
        }

        public Chunk CreateChunk(Point chunkCoords)
        {
            Chunk toReturn = new DefaultChunk(_map, chunkCoords);
            toReturn.OnDestroy += _map.OnComponentDestroyed;
            return toReturn;
        }

        public MapObject CreateObject(Point position, Entity entity)
        {
            if (!IsValidPosition(position, entity))
            {
                throw new ArgumentException("Bounds: " + new Rectangle(position, entity.Size) + " is invalid for " + entity.ToString());
            }

            MapObject toReturn = new DefaultMapObject(_map.Grid, position, entity);
            toReturn.OnDestroy += _map.OnComponentDestroyed;
            return toReturn;

        }

        public bool IsValidPosition(Point position, Entity entity)
        {
            return IsValidPosition(new(position, entity.Size), entity.HasCollision);
        }

        public bool IsValidPosition(Rectangle bounds, bool hasCollision)
        {
            if (!_map.Grid.Bounds.Contains(bounds))
            {
                // the position suggested is outside of bounds.
                return false;
            }

            if (!hasCollision)
            {
                return true;
            }

            // test for collision.

            // if any intersecting object has collision, return false.
            return !_map.Grid.ObjectsIntersecting(bounds).Any(obj => obj.Entity.HasCollision);
            // LINQ is neat, but also witchcraft.

        }

    }
}
