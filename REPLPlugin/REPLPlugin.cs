using BepInEx;
using REPLPlugin.Windows;
using UnityEngine;

namespace REPLPlugin
{
    [BepInPlugin("horse.coder.unity.repl", "Unity REPL", "1.0")]
    public class REPLPlugin : BaseUnityPlugin
    {
        private readonly ConfigWrapper<KeyCode> enableKey = new ConfigWrapper<KeyCode>("EnableKey", KeyCode.Insert);
        private REPLWindow replWindow;
        private bool showGui;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            if (Input.GetKeyDown(enableKey.Value))
                showGui = !showGui;
        }

        private void OnGUI()
        {
            if (!showGui)
                return;

            if (replWindow == null)
                replWindow = new REPLWindow(0);
            replWindow.Show();
        }
    }
}