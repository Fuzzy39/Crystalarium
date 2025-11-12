using CrystalCore.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace CrystalCore.Input
{
	public class Controller
	{

		private MouseState _prevMouseState;

		// actual control related things.
		private InputHandler ih;

		//keybinds
		private List<Keybind> _keybinds;
		private List<Control> _controls;

		// context system
		private string _context;




		public List<Keybind> Keybinds
		{
			get => new List<Keybind>(_keybinds);
		}


		public int DeltaScroll
		{
			get => Mouse.GetState().ScrollWheelValue - _prevMouseState.ScrollWheelValue;
		}



		public string Context
		{
			get => _context;
			set => _context = value;
		}


		public Controller()
		{
			ih = new InputHandler();


			_keybinds = new List<Keybind>();
			_controls = new List<Control>();

			_context = "";

		}


		/// <summary>
		/// Loads keybinds from a file, typically Settings/Controls.xml
		/// Controls the game wants bound need to be added to the controller prior to this method being called.
		/// </summary>
		internal void LoadKeyboindsFromFile(string path, bool ClearPreviousKeybinds)
		{
			if (ClearPreviousKeybinds)
			{
				_keybinds.Clear();
			}

			try
			{

				using (XmlHelper xml = new XmlHelper(path, writing: false))
				{
					xml.Reader.Read();

					xml.Reader.ReadStartElement("Controls");

					while (xml.Reader.NodeType != XmlNodeType.EndElement)
					{
						ParseNextKeybind(xml);
					}

				}
			}
			catch (XmlException e)
			{

				throw new XmlException("Crystalarium Could not find its keybind file, or there was an error in parsing it.\n" + e.Message);
			}

		}

		private void ParseNextKeybind(XmlHelper xml)
		{

			string positionOfKeybind = xml.FormattedReaderPosition;

			// find the control we will be binding to.
			string controlName = xml.Reader.Name;
			Control con = GetAction(controlName);
			if (con == null) { throw new XmlException("Invalid element at " + positionOfKeybind + "."); }

			// attempt to read every button in a keybind.
			string[] buttonStrings = xml.Reader.ReadElementContentAsString().Split(',');
		

			// convert the button strings into buttons.
			Button[] buttons = new Button[buttonStrings.Length];
			for (int i = 0; i<buttonStrings.Length; i++ )
			{

				string buttonString = buttonStrings[i].Trim();
				if (!Enum.TryParse(buttonString, out buttons[i]))
				{
					throw new XmlException(
						"Invalid button '" + buttonString + "' in keybind '" + controlName + "' starting at " + positionOfKeybind + "."
					);
				}
			}

			// setup the keybind.
			con.Bind(buttons);

		}



		public List<Keybind> ConflictingKeybinds()
		{
			List<Keybind> toReturn = new List<Keybind>();

			foreach (Keybind kb in Keybinds)
			{
				if (kb.HasConflicts)
				{
					toReturn.Add(kb);
				}
			}

			return toReturn;

		}



		public Control CreateControl(string name, Keystate ks)
		{

			foreach (Control a in _controls)
			{
				if (a.Name == name)
				{
					throw new InvalidOperationException();
				}
			}

			Control c = new Control(this, name, ks);
			_controls.Add(c);
			return c;

		}


		public Control GetAction(string name)
		{
			foreach (Control a in _controls)
			{
				if (a.Name == name)
				{
					return a;
				}
			}

			return null;
		}

		public Keybind BindControl(Control c, params Button[] buttons)
		{
			Keybind k = new Keybind(this, c, buttons);

			_keybinds.Add(k);

			foreach (Keybind kb in Keybinds)
			{


				kb.UpdateSupersets();
			}

			return k;
		}

		internal void RemoveKeybind(Keybind k)
		{
			_keybinds.Remove(k);

			foreach (Keybind kb in Keybinds)
			{

				kb.UpdateSupersets();
			}
		}



		internal void Update()
		{


			// run keybindss
			foreach (Keybind k in _keybinds)
			{
				k.Update(ih);
			}



			// Update the input handler.
			ih.Update();
			_prevMouseState = Mouse.GetState();


		}





	}
}
