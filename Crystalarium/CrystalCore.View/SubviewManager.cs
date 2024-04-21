using CrystalCore.Model.Communication;
using CrystalCore.Model.Physical;
using CrystalCore.Model.Simulation;
using CrystalCore.View.Core;
using CrystalCore.View.Subviews;
using CrystalCore.View.Subviews.Agents;

namespace CrystalCore.View
{
    internal class SubviewManager : IRenderable
    {
        /*
         *  An integral part of a gridview, the Subview manager is responsible for updating the list of subviews and rendering them when commanded. 
         */

        // fields

        private GridView _parent; // the gridview that we help manage.

        private List<Subview> _chunkViews; // list of chunk renderers currently in existence


        private List<Subview> _agentViews; // the list of agent renderers currently in existance.

        private List<Subview> _beamViews; // the list of beam renderers currently rendering;

        private List<AgentGhost> _ghosts; // the ghosts currently in existance. usually only one.


        // properties

        internal List<Subview> ChunkViews
        {
            get => _chunkViews;
        }

        internal List<Subview> AgentViews
        {
            get => _agentViews;
        }


        internal List<Subview> SignalViews
        {
            get => _beamViews;
        }


        internal GridView Parent
        {
            get => _parent;
        }

        // constructors

        internal SubviewManager(GridView parent)
        {

            _parent = parent;

            parent.Map.OnMapComponentReady += OnMapObjectReady;

            Reset();



        }


        private void OnMapObjectReady(MapComponent o, EventArgs e)
        {
            if(o is not MapObject)
            {
                // it's a chunk!
                Chunk c = (Chunk)o;
                _chunkViews.Add(new ChunkView(Parent, c, Parent.CurrentSkin.ChunkConfig));
                return;
            }

            MapObject obj = (MapObject)o;


            if ( obj.Entity is Node)
            {

                Agent a = ((Node)(obj.Entity)).Agent;
                _agentViews.Add(new AgentView(Parent, a, Parent.CurrentSkin.AgentConfigs));
                return;
            }

            if (obj.Entity is Connection)
            {
                Connection c = (Connection)(obj.Entity);
                _beamViews.Add(new SignalView(Parent, c, Parent.CurrentSkin.SignalConfig));
                return;
            }


        }

        // methods

        internal void Reset()
        {

            // remove all curent  ghosts: they are now outdated.
            _ghosts = new List<AgentGhost>();

            // initialize our lists
            _chunkViews = new List<Subview>();
            _agentViews = new List<Subview>();
            _beamViews = new List<Subview>();

            // we now need to add all appropriate objects

            List<Chunk> chunks = Parent.Map.Grid.ChunksIntersecting(Parent.Map.Grid.Bounds);

            foreach (Chunk ch in chunks)
            {


                _chunkViews.Add(new ChunkView(Parent, ch, Parent.CurrentSkin.ChunkConfig));

                /* foreach(ChunkMember chm in ch.Children)
                 {
                     OnMapObjectReady(chm, new());
                 }*/

            }



        }


        internal void AddGhost(AgentGhost gh)
        {
            if (!_parent.AllowMultipleGhosts)
            {
                _ghosts.Clear();
            }

            _ghosts.Add(gh);

        }


        public bool Draw(IRenderer rend)
        {


            // first update the chunk list and draw chunks.
            DrawObjects(rend, _chunkViews);

            // do the same with agents.
            if (Parent.DoAgentRendering)
            {
                foreach (AgentView av in _agentViews)
                {
                    av.DrawBackground(rend);
                }


             
                DrawObjects(rend, _beamViews);
                DrawObjects(rend, _agentViews);


            }
            DrawGhosts(rend);

            return true;

        }



        private void DrawObjects(IRenderer rend, List<Subview> list)
        {
            // render them
            for (int i = 0; i < list.Count;)
            {

                Subview r = list[i];


                // repeat the previous index if this renderer was destroyed.
                if (r.Draw(rend))
                {
                    i++;
                    continue;
                }

                list.Remove(r);


            }
        }


     

        private void DrawGhosts(IRenderer rend)
        {
            foreach (AgentGhost gh in _ghosts)
            {
                gh.Draw(rend);
            }
        }

    }
}
