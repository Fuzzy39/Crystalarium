using CrystalCore.Model.Core;
using CrystalCore.Model.Physical;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.Model.Physical.Default
{
    internal class DefaultMapObject : MapObject, MapComponent
    {
        private Rectangle _bounds;
        private Entity _entity;
        private Grid _grid;

        private bool _destroyed;

        public event ComponentEvent OnDestroy;


        public DefaultMapObject(Grid grid, Point position, Entity entity)
        {
            _destroyed = false;
            _grid = grid;
            _entity = entity;
            _bounds = new(position, Entity.Size);

            // register with chunks.
            List<Chunk> chunks = Grid.ChunksIntersecting(Bounds);
            foreach (Chunk ch in chunks)
            {
                ch.RegisterObject(this);
            }

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
            _entity = null;
            _grid = null;

        }


    }
}
