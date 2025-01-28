using IPA;
using IPA.Config;
using IPA.Config.Stores;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using IPALogger = IPA.Logging.Logger;

namespace touchsaber
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }
        public static ctrlMode mode;

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(IPALogger logger)
        {
            Instance = this;
            Log = logger;
            Log.Info("touchsaber initialized.");
            Log.Info("spicy modding :)");

            // set control mode
            string filepath = Environment.CurrentDirectory + "/UserData";
            if (!File.Exists(filepath + "/touchsaber.conf")) {
                StreamWriter f = new StreamWriter(File.Create(filepath + "/touchsaber.conf"));
                f.WriteLine(mode.ToString());
                f.WriteLine("control modes: touch, keybmouse, controller, arrows");
                f.Close();
            }
            StreamReader sr = new StreamReader(filepath + "/touchsaber.conf");
            mode = Enum.Parse<ctrlMode>(sr.ReadLine());
            sr.Close();
            Console.WriteLine("Control Mode: " + mode);
        }

        #region BSIPA Config
        //Uncomment to use BSIPA's config
        /*
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }
        */
        #endregion

        [OnStart]
        public void OnApplicationStart()
        {
            Log.Debug("OnApplicationStart");
            new GameObject("touchsaberController").AddComponent<touchsaberController>();
            //Log.Info(Input.GetTouch);
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Log.Debug("OnApplicationQuit");

        }
    }
}
