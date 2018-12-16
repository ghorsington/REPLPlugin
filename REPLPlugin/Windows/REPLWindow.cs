using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BepInEx;
using REPLPlugin.MCS;
using UnityEngine;

namespace REPLPlugin.Windows
{
    public class REPLWindow : WindowBase
    {
        private const int HISTORY_LIMIT = 50;
        private const int MARGIN_BOTTOM = 40;
        private const int MARGIN_X = 4;
        private const float MIN_HEIGHT = 400f;
        private const float MIN_WIDTH = 400f;

        private const int SUGGESTIONS_WIDTH = 200;
        private readonly ScriptEvaluator evaluator;
        private readonly List<string> history = new List<string>();
        private int historyPosition;
        private string inputField = "";
        private string prevInputField = "";
        private readonly StringBuilder sb = new StringBuilder();
        private Vector2 scrollPosition = Vector2.zero;

        private readonly SuggestionsWindow suggestionsWindow;

        public REPLWindow(int id) : base(id, "Unity REPL")
        {
            sb.AppendLine("Welcome to C# REPL! Enter \"help\" to get a list of common methods.");
            evaluator = new ScriptEvaluator(new StringWriter(sb)) {InteractiveBaseClass = typeof(REPL)};
            suggestionsWindow = new SuggestionsWindow(1010);
            suggestionsWindow.SuggestionAccept += AcceptSuggestion;
            MinSize = new Vector2(MIN_WIDTH, MIN_HEIGHT);
            windowRect.x = SUGGESTIONS_WIDTH;
            windowRect.width = MIN_WIDTH;
            windowRect.height = MIN_HEIGHT;
        }

        protected override void RenderWindow()
        {
            var layoutRect = new Rect(MARGIN_X, 20, windowRect.width - MARGIN_X * 2, windowRect.height - MARGIN_BOTTOM);
            GUILayout.BeginArea(layoutRect);
            {
                GUILayout.BeginVertical();
                {
                    if (GUILayout.Button("Clear"))
                        sb.Length = 0;
                    scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                    {
                        GUILayout.TextArea(sb.ToString(), GUILayout.ExpandHeight(true));
                    }
                    GUILayout.EndScrollView();

                    GUI.SetNextControlName("replInput");
                    prevInputField = inputField;
                    inputField = GUILayout.TextField(inputField);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndArea();

            CheckReplInput();
        }

        protected override void OnGUI()
        {
            suggestionsWindow.windowRect = new Rect(windowRect.x - SUGGESTIONS_WIDTH, windowRect.y, SUGGESTIONS_WIDTH, windowRect.height);
            suggestionsWindow.Show();
        }

        private void AcceptSuggestion(string suggestion)
        {
            inputField += suggestion;
            suggestionsWindow.Suggestions = null;
        }

        private object Evaluate(string str)
        {
            object ret = VoidType.Value;
            evaluator.Compile(str, out var compiled);
            try
            {
                compiled?.Invoke(ref ret);
            }
            catch (Exception e)
            {
                sb.AppendLine(e.ToString());
            }

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
            try
            {
                suggestionsWindow.Suggestions = evaluator.GetCompletions(inputField, out string prefix);
                suggestionsWindow.Prefix = prefix;
            }
            catch (Exception)
            {
                suggestionsWindow.Suggestions = null;
                suggestionsWindow.Prefix = null;
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
                if (result != null && !Equals(result, VoidType.Value))
                    sb.AppendLine(result.ToString());

                scrollPosition.y = float.MaxValue;

                history.Add(inputField);
                if (history.Count > HISTORY_LIMIT)
                    history.RemoveRange(0, history.Count - HISTORY_LIMIT);
                historyPosition = 0;

                inputField = string.Empty;
                suggestionsWindow.Suggestions = null;
                suggestionsWindow.Prefix = null;
                Event.current.Use();
            }

            if (Event.current.isKey)
            {
                if (Event.current.keyCode == KeyCode.UpArrow)
                {
                    FetchHistory(-1);
                    Event.current.Use();
                    suggestionsWindow.Suggestions = null;
                    suggestionsWindow.Prefix = null;
                }
                else if (Event.current.keyCode == KeyCode.DownArrow)
                {
                    FetchHistory(1);
                    Event.current.Use();
                    suggestionsWindow.Suggestions = null;
                    suggestionsWindow.Prefix = null;
                }
            }

            if (inputField != prevInputField)
            {
                if (inputField.Contains("typeof("))
                {
                    suggestionsWindow.Suggestions = null;
                    suggestionsWindow.Prefix = null;
                }
                else
                {
                    FetchSuggestions();
                }
            }
        }

        private class VoidType
        {
            public static readonly VoidType Value = new VoidType();
            private VoidType() { }
        }
    }
}