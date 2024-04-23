using CrystalCore;
using CrystalCore.Input;
using CrystalCore.Model.Core;
using CrystalCore.Model.Rules;
using CrystalCore.Model.Simulation;
using CrystalCore.Util;
using CrystalCore.View.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Crystalarium.Main
{
    internal class Actions
    {

        /*
         * The actions class is (probably) temporary. It's purpose is to set up user interaction with crystalarium.
         * This includes defining controller actions (code for, for example, placing an agent or panning the camera).
         * And, at least as it stands now, it also defines the keybinds for these actions, though it would be nice to maybe
         * Make those editable, as some sort of file the user can edit, or eventually through UI maybe.
         * 
         * I feel like this class won't exactly be good practice.
         */

        private Controller c; // the controller we bind everything to

        private CrystalGame game; // The game.

        private Clipboard clipboard;

        internal Direction Rotation { get; private set; } // the direction that any agents placed down will be facing.



        internal AgentType CurrentType { get; private set; } // the agent type selected to place.





        // used when panning
        private Point panOrigin = new Point();
        private Vector2 panPos = new Vector2();


        internal Actions(Controller c, CrystalGame game)
        {
            // it is assumed that actions is setup after configuration.

            this.c = c;
            this.game = game;
            clipboard = new Clipboard();

            Rotation = Direction.up;

            SetupController();

            CurrentType = game.CurrentRuleset.AgentTypes[0];

            game.Engine.ReportKeybindConflicts();



        }

        internal void OnMapReset(object sender, EventArgs e)
        {

            game.CurrentRuleset = game.Map.Ruleset;
            CurrentType = game.CurrentRuleset.AgentTypes[0];
        }






        private void SetupController()
        {
            c.Context = "play";

            SetupViewInteraction();

            SetupMapInteraction();

            SetupSimInteraction();

            SetupAgentSelection();

            SetupMenuInteraction();

            ReadBindings();

        }

        private void ReadBindings()
        {

            try
            {

                using (XmlHelper xml = new XmlHelper(Path.Combine("Settings", "Controls.xml"), writing: false))
                {
                    xml.Reader.Read();

                    xml.Reader.ReadStartElement("Controls");

                    // xml.Reader.ReadStartElement();

                    while (xml.Reader.NodeType != XmlNodeType.EndElement)
                    {

                        //xml.Reader.Read();
                        string s = xml.Reader.Name;
                        Control con = c.GetAction(s);

                        if (con == null) { throw new XmlException("invalid Element at " + xml.FormattedReaderPosition + "."); }


                        s = xml.Reader.ReadElementContentAsString();

                        string[] strings = s.Split(',');
                        Button[] buttons = new Button[strings.Length];

                        int index = 0;
                        foreach (string i in strings)
                        {
                            string j = i.Trim();
                            if (!Enum.TryParse(j, out buttons[index]))
                            {
                                throw new XmlException("invalid button '" + j + "' at " + xml.FormattedReaderPosition + ".");
                            }

                            index++;
                        }

                        con.Bind(buttons);


                    }

                }
            }
            catch (XmlException e)
            {


                throw new XmlException("Crystalarium Could not find its keybind file, or there was an error in parsing it.\n" + e.Message);
            }
        }


        private void SetupViewInteraction()
        {


            // camera controls
            c.CreateControl("CamUp", Keystate.Down)
                .AddAction("play", () => MoveCamera(Direction.up));



            c.CreateControl("CamDown", Keystate.Down)
                .AddAction("play", () => MoveCamera(Direction.down));


            c.CreateControl("CamLeft", Keystate.Down)
                .AddAction("play", () => MoveCamera(Direction.left));


            c.CreateControl("CamRight", Keystate.Down)
                .AddAction("play", () => MoveCamera(Direction.right));




            // panning

            c.CreateControl("StartPan", Keystate.OnPress)
                .AddAction("play", () =>
                {
                    Point pixelCoords = game.view.LocalizeCoords(Mouse.GetState().Position);


                    panOrigin = pixelCoords;
                    panPos = game.view.Camera.Position;


                });




            c.CreateControl("Pan", Keystate.Down)
                .AddAction("play", () =>
                {
                    // the position, in pixels relative to the primary camera, where the mouse is.
                    Point pixelCoords = game.view.LocalizeCoords(Mouse.GetState().Position);

                    // the position, in tiles, where the mouse is.
                    Vector2 mousePos = game.view.Camera.PixelToTileCoords(pixelCoords);

                    // the position, in tiles, where the pan started.
                    Vector2 originPos = game.view.Camera.PixelToTileCoords(panOrigin);


                    Vector2 pos = panPos + (originPos - mousePos);


                    if (!game.Map.Grid.Bounds.Contains(pos))
                    {
                        originPos = originPos - DistanceFrom(pos, game.Map.Grid.Bounds);
                        panOrigin = game.view.Camera.TileToPixelCoords(originPos);

                        // redo calculation
                        pos = panPos + (originPos - mousePos);
                    }


                    game.view.Camera.Position = pos;

                });


            c.CreateControl("ToggleDebugView", Keystate.OnPress)
                .AddAction("play", () => game.view.DoDebugRendering = !game.view.DoDebugRendering);


        }


        private void SetupMapInteraction()
        {


            c.CreateControl("PlaceAgent", Keystate.Down)
                .AddAction("play", () =>
                {

                    Point clickCoords = GetMousePos();
                    Rectangle boundsToCheck = new(clickCoords, CurrentType.GetSize(Rotation));

                    // grow grid
                    game.Map.Grid.ExpandToFit(boundsToCheck);

                    // destroy agents intersecting bounds

                    List<Agent> toRemove = game.Map.AgentsWithin(boundsToCheck);


                    if (toRemove.Count > 1)
                    {
                        return;

                    }

                    if (toRemove.Count == 1)
                    {
                        toRemove[0].Destroy();
                    }

                    // create agent
                    if (game.Map.IsValidPosition( CurrentType, clickCoords, Rotation))
                    {
                  
                        game.Map.CreateAgent(CurrentType, clickCoords, Rotation);
                    }



                });


            c.CreateControl("RemoveAgent", Keystate.Down)
                .AddAction("play", () =>
                {
                    Point clickCoords = GetMousePos();
                    Agent toRemove = null;

                    // remove all agents on this tile (there should only be one once things are working properly)
                    while (true)
                    {

                        toRemove = game.Map.getAgentAtPos(clickCoords);
                        if (toRemove == null)
                        {
                            break;
                        }

                        toRemove.Destroy();
                    }

                });



            c.CreateControl("RotateAgent", Keystate.OnPress)
                .AddAction("play", () =>
                {
                    Point clickCoords = GetMousePos();

                    Agent a = game.Map.getAgentAtPos(clickCoords);
                    if (a == null)
                    {
                        Rotation = Rotation.Rotate(RotationalDirection.cw);
                        return;
                    }


                    a.Rotate(RotationalDirection.cw);
                    Rotation = a.Node.Facing;


                });


            c.CreateControl("Copy", Keystate.OnPress)
                .AddAction("play", () =>
                {
                    if (!Copy("Copy").IsEmpty)
                    {
                        Console.WriteLine("Copied!");
                    }

                });



            c.CreateControl("Cut", Keystate.OnPress)
                .AddAction("play", () =>
                {
                    Rectangle sel = Copy("Cut");

                    if (sel.IsEmpty)
                    {
                        return;
                    }

                    List<Agent> agents = game.Map.AgentsWithin(sel);
                    foreach (Agent agent in agents)
                    {
                        agent.Destroy();
                    }

                    Console.WriteLine(agents.Count + " agents cut!");
                });


            c.CreateControl("Paste", Keystate.OnPress)
               .AddAction("play", () =>
               {
                   Point clickCoords = GetMousePos();
                   clipboard.Paste(game.Map, clickCoords);

               });



        }



        private void SetupSimInteraction()
        {
            c.CreateControl("ToggleSim", Keystate.OnPress)
                .AddAction("play", () =>
                {
                    game.Engine.Sim.Paused = !game.Engine.Sim.Paused;
                });


            c.CreateControl("SimStep", Keystate.OnPress)
                .AddAction("play", () =>
                {
                    // no need to step if unpaused.
                    if (game.Engine.Sim.Paused) { game.Engine.Sim.Step(); }
                });

            c.CreateControl("IncreaseSimSpeed", Keystate.OnPress)
                .AddAction("play", () =>
                {

                    SimulationManager sim = game.Engine.Sim;
                    if (sim.TargetStepsPS < 100)
                    {
                        sim.TargetStepsPS += 10;
                    }

                });

            c.CreateControl("DecreaseSimSpeed", Keystate.OnPress)
                .AddAction("play", () =>
                {

                    // no need to step if unpaused.
                    SimulationManager sim = game.Engine.Sim;
                    if (sim.TargetStepsPS > 10)
                    {
                        sim.TargetStepsPS -= 10;
                    }

                });
        }

        private void SetupAgentSelection()
        {
            c.CreateControl("Pipette", Keystate.OnPress)
               .AddAction("play", () =>
                {
                    Point clickCoords = GetMousePos();

                    Agent a = game.Map.getAgentAtPos(clickCoords);
                    if (a == null)
                    {
                        return;
                    }


                    CurrentType = a.Type;
                    Rotation = a.Node.Facing;

                });


            c.CreateControl("NextAgent", Keystate.OnPress)
                .AddAction("play", () =>
                {
                    List<AgentType> types = game.CurrentRuleset.AgentTypes;

                    int i = types.IndexOf(CurrentType);

                    i++;
                    if (i >= types.Count)
                    {
                        i = 0;
                    }

                    CurrentType = types[i];

                });

            c.CreateControl("PrevAgent", Keystate.OnPress)
                .AddAction("play", () =>
                {
                    List<AgentType> types = game.CurrentRuleset.AgentTypes;
                    int i = types.IndexOf(CurrentType);

                    i--;
                    if (i < 0)
                    {
                        i = types.Count - 1;
                    }

                    CurrentType = types[i];

                });






        }


        private void SetupMenuInteraction()
        {





            c.CreateControl("OpenRulesetMenu", Keystate.OnPress)
               .AddAction("play", () =>
                {

                    c.Context = "menu";
                    game.UI.currentMenu = game.UI.RulesetMenu;
                })
               .AddAction("menu", () =>
                {
                    c.Context = "play";
                    game.UI.currentMenu = null;
                });

            c.CreateControl("Close", Keystate.OnPress)
                .AddAction("play", () =>
                {
                    game.Exit();
                })
                .AddAction("menu", () =>
                {
                    c.Context = "play";
                    game.UI.currentMenu = null;
                });



            c.CreateControl("Save", Keystate.OnPress)
                .AddAction("", () =>
                {
                    game.UI.currentMenu = game.UI.SaveMenu;
                    c.Context = "menu";

                });

            c.CreateControl("Load", Keystate.OnPress)
                 .AddAction("", () =>
                 {
                     game.UI.currentMenu = game.UI.LoadMenu;
                     c.Context = "menu";

                 });


            c.CreateControl("Instructions", Keystate.OnPress)
                 .AddAction("play", () =>
                 {
                     game.UI.currentMenu = game.UI.InstructionsMenu;
                     c.Context = "menu";

                 })
                 .AddAction("menu", () =>
                 {
                     if (game.UI.currentMenu == game.UI.InstructionsMenu)
                     {
                         c.Context = "play";
                         game.UI.currentMenu = null;
                     }

                     game.UI.currentMenu = game.UI.InstructionsMenu;


                 });



            c.CreateControl("MenuAction1", Keystate.OnPress)
                .AddAction("play", () => SwitchAgent(0))
                .AddAction("menu", () => MenuAction(0))
                .Bind(Button.D1);

            c.CreateControl("MenuAction2", Keystate.OnPress)
                .AddAction("play", () => SwitchAgent(1))
                .AddAction("menu", () => MenuAction(1))
                .Bind(Button.D2);

            c.CreateControl("MenuAction3", Keystate.OnPress)
                .AddAction("play", () => SwitchAgent(2))
                .AddAction("menu", () => MenuAction(2))
                .Bind(Button.D3);

            c.CreateControl("MenuAction4", Keystate.OnPress)
                .AddAction("play", () => SwitchAgent(3))
                .AddAction("menu", () => MenuAction(3))
                .Bind(Button.D4);

            c.CreateControl("MenuAction5", Keystate.OnPress)
                .AddAction("play", () => SwitchAgent(4))
                .AddAction("menu", () => MenuAction(4))
                .Bind(Button.D5);

            c.CreateControl("MenuAction6", Keystate.OnPress)
                .AddAction("play", () => SwitchAgent(5))
                .AddAction("menu", () => MenuAction(5))
                .Bind(Button.D6);

            c.CreateControl("MenuAction7", Keystate.OnPress)
                .AddAction("play", () => SwitchAgent(6))
                .AddAction("menu", () => MenuAction(6))
                .Bind(Button.D7);

            c.CreateControl("MenuAction8", Keystate.OnPress)
                .AddAction("play", () => SwitchAgent(7))
                .AddAction("menu", () => MenuAction(7))
                .Bind(Button.D8);

            c.CreateControl("MenuAction9", Keystate.OnPress)
                .AddAction("play", () => SwitchAgent(8))
                .AddAction("menu", () => MenuAction(8))
                .Bind(Button.D9);


            c.CreateControl("Crash", Keystate.OnPress)
                .AddAction("", () => throw new Exception("An Exception was thrown intentionally by the user."));


        }

        private void MoveCamera(Direction d)
        {

            float camSpeed = 1.2f; // I guess this is acceleration
            game.view.Camera.AddVelocity(camSpeed, d);
        }

        private Rectangle Copy(string name)
        {

            Point p = GetMousePos();
            int x1 = p.X;
            int x2;
            int y1 = p.Y;
            int y2;

            Console.WriteLine(name + " command begun. Any invalid input will abort the operation.\nTop left corner of selection: " + p);

            try
            {

                Console.WriteLine("Bottom right of selection x coordinate?");
                x2 = int.Parse(Console.ReadLine());

                if (x2 <= x1)
                {
                    Console.WriteLine("Invalid X coord.");
                    throw new FormatException();
                }

                Console.WriteLine("Bottom right of selection y coordinate?");
                y2 = int.Parse(Console.ReadLine());

                if (y2 <= y1)
                {
                    Console.WriteLine("Invalid Y coord.");
                    throw new FormatException();
                }

            }
            catch (FormatException)
            {
                Console.WriteLine("operation aborted.");
                return new Rectangle();
            }

            Rectangle selection = new(x1, y1, x2 - x1, y2 - y1);
            clipboard.Copy(game.Map, selection);
            selection.Size += new Point(1);
            return selection;
        }



        private void MenuAction(int i)
        {
            if (game.UI.currentMenu == game.UI.RulesetMenu)
            {
                SwitchRuleset(i);
                return;
            }

            string path = Path.Combine("Saves", (i + 1) + ".xml");


            if (game.UI.currentMenu == game.UI.SaveMenu)
            {
                game.Engine.saveManager.Save(path, game.Map);
                c.Context = "play";
                game.UI.currentMenu = null;
                return;
            }

            // must be loading.
            if (!File.Exists(path))
            {
                return;
            }

            try
            {
                if (!game.Engine.saveManager.GetRulesetName(path).Equals("?"))
                {
                    game.Engine.saveManager.Load(path, game.Map);
                }

            }
            catch (MapLoadException e)
            {
                throw new MapLoadException("Crystalrium couldn't load the specified save file.\nReason: " + e.Message + "\n" + e.StackTrace);
            }

            c.Context = "play";
            game.UI.currentMenu = null;
        }

        private void SwitchRuleset(int i)
        {
            List<Ruleset> rulesets = game.Engine.Rulesets;
            if (rulesets.Count > i)
            {

                game.CurrentRuleset = rulesets[i];
                game.Map.Ruleset = game.CurrentRuleset;


                CurrentType = game.CurrentRuleset.AgentTypes[0];
                c.Context = "play";
            }
        }

        private void SwitchAgent(int i)
        {
            if (i < game.CurrentRuleset.AgentTypes.Count)
            {
                CurrentType = game.CurrentRuleset.AgentTypes[i];
            }
        }

        // returns the position of the mouse in tilespace relative to the maingame.view.
        internal Point GetMousePos()
        {
            // crude patch. Fix, someday?

            Vector2 virtualPixelCoords =
                //Mouse.GetState().Position.ToVector2(); // for basic renderer
                ((ScaledRenderer)game.Engine.Renderer).ToVirtualResolution(Mouse.GetState().Position.ToVector2());

            Point pixelCoords = game.view.LocalizeCoords(virtualPixelCoords.ToPoint());


            Vector2 clickCoords = game.view.Camera.PixelToTileCoords(pixelCoords);

            clickCoords.Floor();

            return clickCoords.ToPoint();

        }

        private Vector2 DistanceFrom(Vector2 vect, Rectangle rect)
        {
            Vector2 toReturn = new Vector2(0);
            if (vect.Y < rect.Top)
            {
                toReturn.Y = vect.Y - rect.Top;
            }

            if (vect.Y > rect.Bottom)
            {
                toReturn.Y = vect.Y - rect.Bottom;
            }

            if (vect.X < rect.Left)
            {
                toReturn.X = vect.X - rect.Left;
            }

            if (vect.X > rect.Right)
            {
                toReturn.X = vect.X - rect.Right;
            }

            return toReturn;

        }

    }
}
