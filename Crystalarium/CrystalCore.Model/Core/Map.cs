using CrystalCore.Model.Physical;
using CrystalCore.Model.Rules;
using CrystalCore.Model.Simulation;
using CrystalCore.Util;
//using CrystalCore.Model.Rules;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.Core
{
    /// <summary>
    /// A Map represents, well, a map.
    /// It's grid represents the physical objects on the map.
    /// </summary>
    public interface Map
    {

        public Grid Grid { get; }
        public Ruleset Ruleset { get; set; }

        public int AgentCount { get; }


        // these are events that can be subscribed to by, say, the gridview.
        public event ComponentEvent? OnMapComponentDestroyed;
        public event ComponentEvent? OnMapComponentReady;
        public event EventHandler? OnReset;


        public void Reset();
        public void Reset(Rectangle minimumBounds);

        public Agent CreateAgent(AgentType at, Point location, Direction facing);

        public bool IsValidPosition(AgentType at, Point location, Direction facing);

        public bool IsValidPosition(Rectangle bounds);

        public List<Agent> AgentsWithin(Rectangle bounds);

        public Agent getAgentAtPos(Point coords);




        public void Step();

        internal void OnComponentDestroyed(MapComponent component, EventArgs e);

        internal void OnComponentReady(MapComponent mapObj, EventArgs e);

    }

    public delegate void ComponentEvent(MapComponent mc, EventArgs e);

    public delegate void MapObjectEvent(MapObject obj, EventArgs e);


}