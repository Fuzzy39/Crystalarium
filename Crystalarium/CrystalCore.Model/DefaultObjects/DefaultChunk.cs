using CrystalCore.Model.CoreContract;
using CrystalCore.Model.ObjectContract;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.Model.DefaultObjects
{
    internal class DefaultChunk : Chunk
    {
        private bool _destroyed;
        private Point _chunkCoords;
        private Grid _grid;
        
        // TODO: Subscribe to object's within onDestroy...
        // QUESTION: DO we add within or do within add us???

        private List<MapObject> _children;
        private List<MapObject> _objectsIntersecting;

        // Properties

        public Point ChunkCoords
        {
            get => _chunkCoords;
        }

        public List<MapObject> ObjectsWithin => throw new NotImplementedException();

        public Grid Grid
        {
            get { return _grid; }
        }


        public bool Destroyed
        {
            get => _destroyed;
        }


        public event EventHandler OnDestroy;
     

        public DefaultChunk( Map map, Point chunkCoords)
        {
            _grid = map.Grid;
            _chunkCoords = chunkCoords;
            _children = new List<MapObject>();
            _objectsIntersecting = new List<MapObject>();

            
            // wrong. We shouldn't subscribe to everything.
            // we should be more specific
            map.OnMapComponentDestroyed += OnWithinDestroyed;
           // OnDestroy += map.OnComponentDestroyed;
        }



        public void Destroy()
        {
            _destroyed = true;
            // just to make sure stuff crashes if they use us.
            _chunkCoords = new();
            _grid = null;

            // how was this not a thing until M6?
            while (_objectsIntersecting.Count > 0)
            {
                // TODO: Make sure this actually works...
                _objectsIntersecting[0].Destroy();
            }



            OnDestroy.Invoke(this, new());
        }


        internal void OnWithinDestroyed(MapComponent comp, EventArgs e)
        {
            if(comp is not MapObject)
            {
                return;
            }

            MapObject obj = (MapObject)comp;   
            
            // that's uglier than I feel like it should be, but I guess it discourages default interface implementations.
            if(! obj.Bounds.Intersects( ((Chunk)this).Bounds) )
            {
                return;
            }

            _objectsIntersecting.Remove(obj);
            _children.Remove(obj); // children may or may not have obj in it. I assume that's fine.


        }

    }
}
