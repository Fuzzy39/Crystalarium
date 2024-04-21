
using CrystalCore.Model.Communication;
using CrystalCore.Model.Communication.Default;
using CrystalCore.Model.Physical;
using CrystalCore.Model.Physical.Default;
using CrystalCore.Model.Rules;
using CrystalCore.Model.Simulation;
using CrystalCore.Model.Simulation.Default;
using CrystalCore.Util;
//using CrystalCore.Model.Rules;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.Core.Default
{
    /// <summary>
    /// DefaultMap represents a map made of 
    /// Also an agent factory? sorry.
    /// </summary>
    public class DefaultMap : Map
    {



        private DefaultGrid _grid;



        private Ruleset _ruleset; // the ruleset this grid is following.
        private EntityFactory _entityFactory;

        private List<Agent> _agents;


        public event EventHandler? OnReset;
        public event ComponentEvent? OnMapComponentReady;
        public event ComponentEvent? OnMapComponentDestroyed;


        public Grid Grid
        {
            get => _grid;
        }

        public Ruleset Ruleset
        {
            get => _ruleset;
            set
            {
                _ruleset = value;

                Reset();

            }
        }

        public int AgentCount
        {
            get => _agents.Count; 
        }

        public DefaultMap(Ruleset r)
        {
            if (r == null)
            {
                throw new ArgumentNullException("Null Ruleset not viable.");
            }
            _ruleset = r;

            // this seems rather hard coded... Shouldn't matter this update though.
            _grid = new DefaultGrid(new DefaultComponentFactory(this));
            _entityFactory = new DefaultEntityFactory(_grid.ComponentFactory);

            _agents = new();

            OnReset?.Invoke(this, new EventArgs());
        }


        public void Reset()
        {
            Reset(new Rectangle(0, 0, 0, 0));
        }

        public void Reset(Rectangle minimumBounds)
        {

           
            for(int i = 0; i < _agents.Count;)
            {
                _agents[i].Destroy();
            }
            _agents.Clear();

            // reseting our grid should remove all references to any remaining mapObjects
            _grid.Destroy();
            _grid = new DefaultGrid(new DefaultComponentFactory(this));
            _grid.ExpandToFit(minimumBounds);

          

            OnReset?.Invoke(this, new EventArgs());


        }

        // These handle the events of MapComponents.



        void Map.OnComponentDestroyed(MapComponent component, EventArgs e)
        {
            if(component is MapObject && ((MapObject)component).Entity is Node)
            {
              
                _agents.Remove(((Node)(((MapObject)component).Entity)).Agent);
            }


            component.OnDestroy -= ((Map)this).OnComponentDestroyed;
            OnMapComponentDestroyed?.Invoke(component, e);
        }

        void Map.OnComponentReady(MapComponent component, EventArgs e)
        {
            OnMapComponentReady?.Invoke(component, e);
        }

        public Agent CreateAgent(AgentType at, Point location, Direction facing)
        {
            if (_ruleset != at.Ruleset)
            {
               throw new InvalidOperationException("Cannot add " + at.Name + " type agent of ruleset " + at.Ruleset.Name + " to grid of ruleset " + _ruleset.Name + ".");
            }

            if(!IsValidPosition(at, location, facing))
            {
                throw new InvalidOperationException("Invalid Position '"+ new Rectangle(location, at.GetSize(facing)) + "'of agent type '"+at.Name+"'."); // should probably add details. lazy.
            }

            Node node = _entityFactory.CreateNode( new Rectangle(location, at.GetSize(facing)), facing, _ruleset.DiagonalSignalsAllowed);
            Agent a =  new DefaultAgent(this, node, at);
            _agents.Add(a);
            return a;
        }

        public bool IsValidPosition(AgentType at, Point location, Direction facing)
        {
            return Grid.ComponentFactory.IsValidPosition(new Rectangle(location, at.GetSize(facing)), true);
        }


        public bool IsValidPosition(Rectangle bounds)
        {
            return Grid.ComponentFactory.IsValidPosition(bounds, true);
        }

        public void Step()
        {
            for (int i = 0; i < _agents.Count;)
            {
                Agent a = _agents[i];
                if (a == null)
                {
                    _agents.RemoveAt(i);
                    continue;
                }

                i++;

            }

            foreach (Agent a in _agents) { a.PrepareSimulationStep(); }
            foreach (Agent a in _agents) { a.DoSimulationStep(); }
        }



        public List<Agent> AgentsWithin(Rectangle bounds)
        {
            List<MapObject> possibleList = _grid.ObjectsIntersecting(bounds);

            List<Agent> toReturn = new ();

            foreach (MapObject obj in possibleList)
            {
                if (obj.Entity is Node)
                {
                    toReturn.Add(((Node)(obj.Entity)).Agent);
                }
            }

            return toReturn;

        }



        public  Agent getAgentAtPos(Point coords)
        {
            List<Agent> agents = AgentsWithin(new(coords, new(1, 1)));

            if (agents.Count == 0) return null;

            return agents[0];
        }



    }
}
