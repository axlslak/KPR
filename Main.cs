using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Pathfinding;
using SharpNav;
using SharpNav.Pathfinding;
using System.Threading;

/*
    "new Vector3(141.3196f, 14.01f, 242.172f);",
    "new Vector3(444.7452f, 25.21f, 129.8088f);",
    "new Vector3(355.9326f, 25.21f, 271.7541f);",
    "new Vector3(367.9113f, 25.21f, 347.9453f);"
*/


namespace KPR
{

    public class Main : AOPluginEntry
    {
        public const int IPANDE_PF = 4389;
        public static string PLUGIN_DIR;

        public static Vector3 IPANDE_LEFT = new Vector3(141.3196f, 14.01f, 242.172f);
        public static Vector3 IPANDE_RIGHT = new Vector3(444.7452f, 25.21f, 129.8088f);
        public static Vector3 IPANDE_MID1 = new Vector3(355.9326f, 25.21f, 271.7541f);
        public static Vector3 IPANDE_MID2 = new Vector3(367.9113f, 25.21f, 347.9453f);
        public static Vector3 IPANDE_WAIT = new Vector3(244.2435f, 35.38172f, 91.04551f);
        public static Vector3 IPANDE_EXIT = new Vector3(220.4058f, 29.025f, 120.4852f);

        private bool _running = false;
        private bool _exiting = false;
        private Vector3 destination;
        private bool found_left = false, found_right = false, found_mid1 = false, found_mid2 = false;
        private string left = null, right = null, mid1 = null, mid2 = null;

        public override void Run(string pluginDir)
        {
            Game.OnUpdate += OnUpdate;
            Chat.WriteLine("/kpr - starts runner");

            PLUGIN_DIR = pluginDir;

            Chat.RegisterCommand("kpr", (string command, string[] param, ChatWindow chatWindow) =>
            {
                start();
            });

        }
        private void OnUpdate(object s, float deltaTime)
        {

            if (_running == false)
                return;

            if (Game.IsZoning)
            {
                reset();
                return;
            }

            foreach (Dynel dynel in DynelManager.AllDynels)
            {
                if (((dynel.Name == "Aries") || (dynel.Name == "Leo") || (dynel.Name == "Virgo")) && (found_left == false))
                {
                    left = dynel.Name;
                    found_left = true;
                    Chat.WriteLine("Left is: " +  left);
                }

                if (((dynel.Name == "Aquarius") || (dynel.Name == "Cancer") || (dynel.Name == "Gemini")) && (found_right == false))
                {
                    right = dynel.Name;
                    found_right = true;
                    Chat.WriteLine("Right is: " + right);
                }

                if (((dynel.Name == "Libra") || (dynel.Name == "Pisces") || (dynel.Name == "Taurus")) && (found_mid1 == false)) 
                {
                    mid1 = dynel.Name;
                    found_mid1 = true;
                    Chat.WriteLine("Mid1 is: " + mid1);
                }

                if (((dynel.Name == "Capricorn") || (dynel.Name == "Sagittarius") || (dynel.Name == "Scorpio")) && (found_mid2 == false))
                {
                    mid2 = dynel.Name;
                    found_mid2 = true;
                    Chat.WriteLine("Mid2 is: " + mid2);
                }

                if (found_left == false)
                {
                    destination = IPANDE_LEFT;
                    SMovementController.SetNavDestination(destination);
                }
                else if (found_right == false)
                {
                    destination = IPANDE_RIGHT;
                    SMovementController.SetNavDestination(destination);
                }
                else if (found_mid1 == false)
                {
                    destination = IPANDE_MID1;
                    SMovementController.SetNavDestination(destination);
                }
                else if (found_mid2 == false)
                {
                    destination = IPANDE_MID2;
                    SMovementController.SetNavDestination(destination);
                }
                else if ((right == "Aquarius") && (mid1 == "Libra") && (mid2 == "Sagittarius"))
                {
                    destination = IPANDE_WAIT;
                    SMovementController.SetNavDestination(destination);
                }
                else
                {
                    destination = IPANDE_EXIT;
                    SMovementController.SetNavDestination(destination);
                    _exiting = true;
                }

                if (DynelManager.LocalPlayer.HealthPercent < 35)
                {
                    SMovementController.SetNavDestination(DynelManager.LocalPlayer.Position);
                }
            }
        }

        public void start()
        {
            if (Playfield.ModelIdentity.Instance != IPANDE_PF)
            {
                Chat.WriteLine("You must be in iPande to start this bot!");
                return;
            }

            string navPath = $"{PLUGIN_DIR}\\{IPANDE_PF}.nav";

            if (!SNavMeshSerializer.LoadFromFile(navPath, out NavMesh navMesh))
            {
                Chat.WriteLine($"Could not find navmesh file at given path! {navPath}");
                return;
            }

            SMovementController.Set();
            SMovementController.LoadNavmesh(navMesh);
            _running = true;
        }

        private void reset()
        {
            _running = false;
            found_left = false;
            found_right = false;
            found_mid1 = false;
            found_mid2 = false;
        }
    }
}