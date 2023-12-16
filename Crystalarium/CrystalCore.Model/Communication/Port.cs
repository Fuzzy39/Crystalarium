using CrystalCore.Util;
using Microsoft.Xna.Framework;

namespace CrystalCore.Model.Communication
{
    internal interface Port
    {
        public PortDescriptor Descriptor { get; }

        public CompassPoint AbsoluteFacing { get; }

        public Point Location { get; }



        public Connection Connection { get; set; }
        public Port ConnectedTo { get; } // for convience's sake


        public event EventHandler? OnDestroy;
        public bool Destroyed { get; }



        public event EventHandler? OnInputUpdated;

        /// <summary>
        /// The value being output by this port.
        /// </summary>
        public int Output { get; set; }

        /// <summary>
        /// The value being recieved by this port.
        /// </summary>
        public int Input { get; }





        public string? ToString()
        {
            return "Port: { Location:" + Location + " Descriptor:  " + Descriptor + "(ABS):" + AbsoluteFacing + "}";
        }

        public void Destroy();
    }



}

