using UnityEngine;

namespace REPLPlugin.Util
{
    public static class GUIUtil
    {
        private static GUIStyle resizeButtonStyle;
        private static readonly GUIContent gcResize = new GUIContent("//", "drag to resize");

        public static Rect ResizeButton(Rect windowRect, Vector2 minSize, ref bool isResizing, ref Rect resizeStart)
        {
            if (resizeButtonStyle == null)
            {
                resizeButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    stretchWidth = false,
                    stretchHeight = false,
                    border = new RectOffset(0, 0, 0, 0),
                    hover = {background = null},
                    onHover = {background = null}
                };
            }
                
            var r = GUILayoutUtility.GetRect(gcResize, resizeButtonStyle);
            r.x = windowRect.width - r.width;
            r.y = windowRect.height - r.height;

            var currentMousePosition =
                GUIUtility.ScreenToGUIPoint(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y));

            if (!isResizing && Input.GetMouseButton(0) && r.Contains(currentMousePosition))
            {
                isResizing = true;
                resizeStart = new Rect(currentMousePosition.x, currentMousePosition.y, windowRect.width,
                    windowRect.height);
            }
            else if (isResizing && !Input.GetMouseButton(0))
            {
                isResizing = false;
            }

            if (isResizing)
            {
                windowRect.width = Mathf.Max(resizeStart.width + (currentMousePosition.x - resizeStart.x), minSize.x);
                windowRect.height = Mathf.Max(resizeStart.height + (currentMousePosition.y - resizeStart.y), minSize.y);
                windowRect.xMax = Mathf.Min(windowRect.xMax, Screen.width);
                windowRect.yMax = Mathf.Min(windowRect.yMax, Screen.height);
            }

            GUI.Button(r, gcResize, resizeButtonStyle);
            return windowRect;
        }
    }
}
