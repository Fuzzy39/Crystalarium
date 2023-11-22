using CrystalCore.Model.CoreContract;
using CrystalCore.Model.ObjectContract;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.Model.DefaultObjects
{
    internal class DefaultMapObject : MapObject, MapComponent
    {
        private Rectangle _bounds;
        private Entity _entity;
        private Grid _grid;

        private bool _destroyed;

        public event ComponentEvent OnDestroy;


        DefaultMapObject(Point position, Entity entity, Map map)
        {
            _destroyed = false;
            _grid = map.Grid;
            _entity = entity;
            _bounds = new(position, Entity.Size);

            // register with chunks.
            List<Chunk> chunks = Grid.ChunksIntersecting(Bounds);
            foreach (Chunk ch in chunks)
            {
                ch.RegisterObject(this);
            }

            OnDestroy += map.OnComponentDestroyed;

        }

        public Rectangle Bounds => _bounds;

        public Entity Entity => _entity;

        public Chunk Parent => _grid.ChunkAtCoords(Bounds.Location);

        public Grid Grid => _grid;

        public bool Destroyed => _destroyed;

      
        public void Destroy()
        {
            _destroyed = true;
            OnDestroy.Invoke(this, new());  

            _bounds = new();
            if(!_entity.Destroyed)
            {
                Entity.Destroy();
            }

            _entity = null;
            _grid = null;

           
        }

       
    }
}
