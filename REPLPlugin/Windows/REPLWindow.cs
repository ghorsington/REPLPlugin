using System.Collections.Generic;
using System.IO;
using System.Text;
using BepInEx.Logging;
using REPLPlugin.MCS;
using UnityEngine;
using Logger = BepInEx.Logger;

namespace REPLPlugin.Windows
{
    public class REPLWindow : WindowBase
    {
        private const int HISTORY_LIMIT = 50;
        private readonly ScriptEvaluator evaluator;
        private string inputField = "";
        private readonly StringWriter logger;
        private readonly StringBuilder sb = new StringBuilder();
        private Vector2 scrollPosition = Vector2.zero;
        private List<string> history = new List<string>();
        private int historyPosition = 0;

        public REPLWindow(int id) : base(id, "Unity REPL")
        {
            logger = new StringWriter(sb);
            evaluator = new ScriptEvaluator(logger);
        }

        protected override void OnGUI()
        {
            var layoutRect = new Rect(4, 20, windowRect.width - 8, windowRect.height - 40);
            GUILayout.BeginArea(layoutRect);
            {
                GUILayout.BeginVertical();
                {
                    scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                    {
                        GUILayout.TextArea(sb.ToString(), GUILayout.ExpandHeight(true));
                    }
                    GUILayout.EndScrollView();

                    GUI.SetNextControlName("replInput");
                    inputField = GUILayout.TextField(inputField);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndArea();

            CheckReplInput();
        }

        private object Evaluate(string str)
        {
            object ret = typeof(void);
            evaluator.Compile(str, out var compiled);
            compiled?.Invoke(ref ret);
            return ret;
        }

        private void FetchHistory(int move)
        {
            historyPosition += move;
            historyPosition %= history.Count;
            if (historyPosition < 0)
                historyPosition = history.Count - 1;

            inputField = history[historyPosition];
        }

        private void FetchSuggestions()
        {
            var completions = evaluator.GetCompletions(inputField, out var prefix);
            Logger.Log(LogLevel.Message, $"Available completions (with prefix {prefix}):");
            foreach (var completion in completions)
            {
                Logger.Log(LogLevel.Message, completion);
            }
        }

        private void CheckReplInput()
        {
            if (GUI.GetNameOfFocusedControl() != "replInput")
                return;

            if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
            {
                sb.AppendLine($"> {inputField}");
                var result = Evaluate(inputField);
                if (result != null && !Equals(result, typeof(void)))
                    sb.AppendLine(result.ToString());

                history.Add(inputField);
                if (history.Count > HISTORY_LIMIT)
                    history.RemoveRange(0, history.Count - HISTORY_LIMIT);
                historyPosition = 0;

                inputField = string.Empty;
            }

            if (Event.current.isKey)
            {
                if(Event.current.keyCode == KeyCode.UpArrow)
                    FetchHistory(-1);
                else if(Event.current.keyCode == KeyCode.DownArrow)
                    FetchHistory(1);
                else if(Event.current.keyCode == KeyCode.Tab)
                    FetchSuggestions();
            }
        }
    }
}