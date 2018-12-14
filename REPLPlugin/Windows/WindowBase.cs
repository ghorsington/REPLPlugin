using REPLPlugin.Util;
using UnityEngine;

namespace REPLPlugin.Windows
{
    public abstract class WindowBase
    {
        private bool isResizing;
        protected Rect windowRect;
        private Rect resizeStart;
        public int ID { get; }
        public Vector2 MinSize { get; set; }
        public string Title { get; set; }

        protected WindowBase(int id, string title)
        {
            ID = id;
            Title = title;
            MinSize = new Vector2(100f, 100f);
            windowRect = new Rect(0, 0, MinSize.x, MinSize.y);
        }

        public void Show()
        {
            var rect = GUI.Window(0, windowRect, Render, Title);
            windowRect.x = Mathf.Max(rect.x, 0);
            windowRect.y = Mathf.Max(rect.y, 0);
        }

        private void Render(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, windowRect.width, 20));
            OnGUI();
            windowRect = GUIUtil.ResizeButton(windowRect, MinSize, ref isResizing, ref resizeStart);
        }

        protected abstract void OnGUI();
    }
}
