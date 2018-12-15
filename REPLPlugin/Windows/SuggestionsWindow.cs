using UnityEngine;

namespace REPLPlugin.Windows
{
    public class SuggestionsWindow : WindowBase
    {
        public delegate void AcceptSuggestionDelegate(string suggestion);

        private readonly GUIStyle completionsListingStyle;

        private Vector2 scrollPosition = Vector2.zero;

        private string[] suggestions;

        public SuggestionsWindow(int id) : base(id, "Suggestions")
        {
            Hidden = true;
            Draggable = false;
            Resizable = false;

            completionsListingStyle = new GUIStyle(GUI.skin.button)
            {
                    border = new RectOffset(0, 0, 0, 0),
                    margin = new RectOffset(0, 0, 0, 0),
                    padding = new RectOffset(0, 0, 0, 0),
                    hover = {background = Texture2D.whiteTexture, textColor = Color.black},
                    normal = {background = null},
                    focused = {background = Texture2D.whiteTexture, textColor = Color.black},
                    active = {background = Texture2D.whiteTexture, textColor = Color.black}
            };
        }

        public string Prefix { get; set; }

        public string[] Suggestions
        {
            get => suggestions;
            set
            {
                suggestions = value;
                Hidden = suggestions == null || suggestions.Length == 0;
            }
        }

        public event AcceptSuggestionDelegate SuggestionAccept;

        protected override void RenderWindow()
        {
            var layoutRect = new Rect(0, 20, windowRect.width, windowRect.height - 20);
            GUILayout.BeginArea(layoutRect);
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                {
                    GUILayout.BeginVertical();
                    {
                        foreach (string suggestion in suggestions)
                        {
                            if (!GUILayout.Button($"{Prefix}{suggestion}", completionsListingStyle, GUILayout.ExpandWidth(true)))
                                continue;
                            SuggestionAccept?.Invoke(suggestion);
                            break;
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndArea();
        }
    }
}