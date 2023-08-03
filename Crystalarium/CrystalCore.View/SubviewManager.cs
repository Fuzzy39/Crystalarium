using CrystalCore.Model.Elements;
using CrystalCore.Model.Objects;
using CrystalCore.View.Core;
using CrystalCore.View.Subviews;
using CrystalCore.View.Subviews.Agents;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

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

        internal SubviewManager( GridView parent)
        {

            _parent = parent;

            parent.Map.OnMapObjectReady += OnMapObjectReady;

            Reset();

           

        }


        private void OnMapObjectReady(object o, EventArgs e)
        {
            if(o is Agent)
            {
               
                Agent a = (Agent)o;
                _agentViews.Add(new AgentView(Parent, a, Parent.CurrentSkin.GetAgentViewConfig(a.Type)));
            }

            if(o is Connection)
            {
                Connection c = (Connection)o;
                _beamViews.Add(new SignalView(Parent, c, Parent.CurrentSkin.SignalConfig));
            }

            if(o is Chunk)
            {
                Chunk c = (Chunk)o;
                _chunkViews.Add(new ChunkView(Parent, c, Parent.CurrentSkin.ChunkConfig));
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

            List<Chunk> chunks = Parent.Map.ChunksInBounds(Parent.Map.Bounds);

            foreach (Chunk ch in chunks )
            {

                OnMapObjectReady(ch, new());

               /* foreach(ChunkMember chm in ch.Children)
                {
                    OnMapObjectReady(chm, new());
                }*/
                
            }

          
      
        }


        internal void AddGhost(AgentGhost gh)
        {
            if(!_parent.AllowMultipleGhosts)
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
                foreach(AgentView av in _agentViews)
                {
                    av.DrawBackground(rend);
                }

                DrawObjects(rend, _agentViews);
                DrawObjects(rend, _beamViews);

                

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

        private void DrawGhosts( IRenderer rend)
        {
            foreach(AgentGhost gh in _ghosts)
            {
                gh.Draw(rend);
            }
        }

    }
}
