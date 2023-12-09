using CrystalCore.Model.Physical;
using CrystalCore.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CrystalCore.Model.Communication
{
    internal interface Port 
    {
        public PortDescriptor Descriptor { get; }

        public CompassPoint AbsoluteFacing { get; }

        public Point Location { get; }

        
        public Connection Connection { get; set; }
        public Port ConnectedTo { get; } // for convience's sake



        /// <summary>
        /// The value being output by this port.
        /// </summary>
        public int Output { get; set; }

        /// <summary>
        /// The value being recieved by this port.
        /// </summary>
        public int Input { get; }


        /// <summary>
        /// Whether this port's Input has changed since this property was last checked.
        /// </summary>
        public bool InputUpdated { get; }


        public string? ToString()
        {
            return "Port: { Location:" + Location + " Descriptor:  "+Descriptor+ "(ABS):" + AbsoluteFacing + "}";
        }
    }


 }

       