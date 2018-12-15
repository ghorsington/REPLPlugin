using REPLPlugin.Util;
using UnityEngine;

namespace REPLPlugin.Windows
{
    public abstract class WindowBase
    {
        public Rect windowRect;
        private bool isResizing;
        private Rect resizeStart;

        protected WindowBase(int id, string title)
        {
            ID = id;
            Title = title;
            MinSize = new Vector2(100f, 100f);
            windowRect = new Rect(0, 0, MinSize.x, MinSize.y);
        }

        public bool Draggable { get; set; } = true;
        public bool Hidden { get; set; } = false;
        public int ID { get; }
        public Vector2 MinSize { get; set; }
        public bool Resizable { get; set; } = true;
        public string Title { get; set; }

        public void Show()
        {
            if (Hidden)
                return;

            var rect = GUI.Window(ID, windowRect, Render, Title, GUI.skin.box);
            windowRect.x = Mathf.Max(rect.x, 0);
            windowRect.y = Mathf.Max(rect.y, 0);
            OnGUI();
        }

        protected abstract void RenderWindow();

        protected virtual void OnGUI() { }

        private void Render(int windowID)
        {
            if (Draggable)
                GUI.DragWindow(new Rect(0, 0, windowRect.width, 20));

            RenderWindow();

            if (Resizable)
                windowRect = GUIUtil.ResizeButton(windowRect, MinSize, ref isResizing, ref resizeStart);
        }
    }
}