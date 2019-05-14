using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FutureConcepts.Media.CommonControls
{
    /// <summary>
    /// Provides DragDrop functionality for non STA WinForms applications. Only works within a single process.
    /// </summary>
    /// <remarks>
    /// kdixon 05/11/2009
    /// </remarks>
    public static class DragDropManager
    {
        private static Dictionary<Control, EventHandler<DataDroppedEventArgs>> callbacks = new Dictionary<Control, EventHandler<DataDroppedEventArgs>>();

        /// <summary>
        /// Loads cursors from the resource file
        /// </summary>
        static DragDropManager()
        {
            try
            {
                CursorDragInProgress = Win32.CreateCursor(Properties.Resources.drag_cancel, 0, 0);
            }
            catch
            {
                CursorDragInProgress = Cursors.Help;
            }

            try
            {
                CursorInvalidTarget = Win32.CreateCursor(Properties.Resources.drag_cancel, 0, 0);
            }
            catch
            {
                CursorInvalidTarget = Cursors.No;
            }

            try
            {
                CursorTarget = Win32.CreateCursor(Properties.Resources.drag_play, 0, 0);
            }
            catch
            {
                CursorTarget = Cursors.Hand;
            }
        }

        /// <summary>
        /// the registered form
        /// </summary>
        private static Form owner;

        /// <summary>
        /// Cursor to use while dragging is in progress
        /// </summary>
        public static Cursor CursorDragInProgress { get; set; }
        /// <summary>
        /// Cursor to use while dragging over an invalid target
        /// </summary>
        public static Cursor CursorInvalidTarget { get; set; }
        /// <summary>
        /// Cursor to use while dragging over a valid target
        /// </summary>
        public static Cursor CursorTarget { get; set; }

        private static object _data;
        /// <summary>
        /// Set this property to begin a drag.
        /// Get this property to get associated data.
        /// </summary>
        public static object Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                if (owner != null)
                {
                    if (value != null)
                    {
                        owner.Cursor = CursorDragInProgress;
                    }
                    else
                    {
                        owner.Cursor = Cursors.Default;
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if there is data being dragged
        /// </summary>
        public static bool IsDragging
        {
            get
            {
                return (_data != null);
            }
        }

        #region Registration

        /// <summary>
        /// Register a Form for drag and drop
        /// </summary>
        /// <param name="form">form to register</param>
        public static void Register(Form form)
        {
            if (owner != null)
            {
                throw new InvalidOperationException("Only one form may be registered at a time!");
            }

            owner = form;
            owner.MouseLeave += new EventHandler(owner_MouseLeave);
            RegisterChildren(form.Controls);
        }

        /// <summary>
        /// Recursively registers all controls in a ControlCollection
        /// </summary>
        /// <param name="controls">control collection to register</param>
        private static void RegisterChildren(Control.ControlCollection controls)
        {
            if (controls == null)
            {
                return;
            }

            foreach (Control c in controls)
            {
                RegisterChild(c);
                RegisterChildren(c.Controls);
            }
        }

        /// <summary>
        /// Registers a single child control.
        /// </summary>
        /// <param name="child">child control to register</param>
        private static void RegisterChild(Control child)
        {
            if (child.Cursor == Cursors.Default)
            {
                child.MouseMove += new MouseEventHandler(child_MouseMove);
            }

            child.MouseUp += new MouseEventHandler(child_MouseUp);
        }

        /// <summary>
        /// Cleans up
        /// </summary>
        public static void UnRegister()
        {
            if (owner == null)
            {
                throw new InvalidOperationException("Manager is not regsitered!");
            }

            callbacks.Clear();

            owner.MouseLeave -= new EventHandler(owner_MouseLeave);
            UnRegisterChildren(owner.Controls);
            owner = null;
        }

        /// <summary>
        /// Recursively removes all event handlers from children
        /// </summary>
        /// <param name="controls">ControlCollection to clean up</param>
        private static void UnRegisterChildren(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                c.MouseUp -= new MouseEventHandler(child_MouseUp);
                c.MouseUp -= new MouseEventHandler(target_MouseUp);
                c.MouseMove -= new MouseEventHandler(child_MouseMove);
                c.MouseMove -= new MouseEventHandler(target_MouseMove);
                if (c.Controls != null)
                {
                    UnRegisterChildren(c.Controls);
                }
            }
        }

        /// <summary>
        /// Register a non-target control. Use only if the control did not exist on the owner form when you called Register(Form)
        /// </summary>
        /// <param name="nonTarget">a late-created or late-added control</param>
        public static void RegisterNonTarget(Control nonTarget)
        {
            if (nonTarget == null)
            {
                throw new ArgumentNullException("Control cannot be null!");
            }
            RegisterChild(nonTarget);
            RegisterChildren(nonTarget.Controls);
        }

        /// <summary>
        /// Register a control as a valid drop target. Drop events will be raised through the DataDropped event.
        /// </summary>
        /// <param name="target">Control to register.</param>
        public static void RegisterTarget(Control target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("Target control cannot be null!");
            }

            target.MouseMove += new MouseEventHandler(target_MouseMove);
            target.MouseUp += new MouseEventHandler(target_MouseUp);
            target.MouseUp -= new MouseEventHandler(child_MouseUp);
            target.MouseMove -= new MouseEventHandler(child_MouseMove);
        }

        /// <summary>
        /// Register a control as a valid drop target, providing a specific callback for this control.
        /// The DataDropped event will still be raised.
        /// Notes if the control is already registered:
        /// 1. If it was registered with this method, you may call this method again to change the callback for this control.
        /// 2. If it was registered with the single argument method, you should call UnRegisterTarget first.
        /// </summary>
        /// <param name="target">Control to register</param>
        /// <param name="callback">specific callback for this control</param>
        public static void RegisterTarget(Control target, EventHandler<DataDroppedEventArgs> callback)
        {
            if (callbacks.ContainsKey(target))
            {
                callbacks[target] = callback;
                return;
            }
            else
            {
                RegisterTarget(target);
                callbacks.Add(target, callback);
            }
        }

        /// <summary>
        /// Causes a control to no longer be a target control.
        /// DO NOT CALL ON A NON-TARGET.
        /// </summary>
        /// <param name="target">previously registered target control</param>
        public static void UnRegisterTarget(Control target)
        {
            callbacks.Remove(target);

            target.MouseMove -= new MouseEventHandler(target_MouseMove);
            target.MouseUp -= new MouseEventHandler(target_MouseUp);

            RegisterChild(target);
        }

        #endregion

        /// <summary>
        /// Call to prematurely cancel a drag operation
        /// </summary>
        public static void FinishDrag()
        {
            Data = null;

            if (DraggingFinished != null)
            {
                DraggingFinished(null, new EventArgs());
            }
        }

        /// <summary>
        /// This event is raised when any drag operation has completed, whether the data was dropped or thrown away.
        /// </summary>
        public static event EventHandler DraggingFinished;

        /// <summary>
        /// This event is raised when data is dropped successfully.
        /// </summary>
        public static event EventHandler<DataDroppedEventArgs> DataDropped;

        #region Dragging Logic

        /// <summary>
        /// Executes when the mouse is released over a target control
        /// </summary>
        /// <param name="sender">control that is the target</param>
        /// <param name="e">event args</param>
        static void target_MouseUp(object sender, MouseEventArgs e)
        {
            if (IsDragging)
            {
                Control sctrl = sender as Control;

                DataDroppedEventArgs args = new DataDroppedEventArgs(sctrl, DragDropManager.Data);

                if (sctrl != null)
                {
                    if (callbacks.ContainsKey(sctrl))
                    {
                        callbacks[sctrl](sender, args);
                    }
                }

                if (DataDropped != null)
                {
                    DataDropped(sender, args);
                }

                FinishDrag();
            }
        }
        
        /// <summary>
        /// Action to take when the mouse is moving over a target.
        /// </summary>
        /// <param name="sender">target control</param>
        /// <param name="e">moust event args</param>
        static void target_MouseMove(object sender, MouseEventArgs e)
        {
            Control c = sender as Control;
            if (c == null)
            {
                return;
            }
            if (IsDragging)
            {
                c.Cursor = CursorTarget;
            }
            else
            {
                c.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// If the cursor leaves the owner form, we will drop the data on the floor.
        /// </summary>
        static void owner_MouseLeave(object sender, EventArgs e)
        {
            Point p = owner.PointToClient(Cursor.Position);

            if ((p.X > owner.ClientRectangle.Right) ||
                (p.X < owner.ClientRectangle.Left) ||
                (p.Y < owner.ClientRectangle.Top) ||
                (p.Y > owner.ClientRectangle.Bottom))
            {
                FinishDrag();
            }
        }

        /// <summary>
        /// Clear the data on mouse up
        /// </summary>
        static void owner_MouseUp(object sender, MouseEventArgs e)
        {
            FinishDrag();
        }

        /// <summary>
        /// Action to take on a non-target child control
        /// </summary>
        /// <param name="sender">non-target child control</param>
        /// <param name="e">mouse event args</param>
        static void child_MouseMove(object sender, MouseEventArgs e)
        {
            Control child = sender as Control;
            if (sender is Control)
            {
                if (IsDragging)
                {
                    ((Control)sender).Cursor = CursorInvalidTarget;
                }
                else
                {
                    ((Control)sender).Cursor = Cursors.Default;
                }
            }
        }

        /// <summary>
        /// Drop on a child control -- data goes on the floor
        /// </summary>
        static void child_MouseUp(object sender, MouseEventArgs e)
        {
            FinishDrag();
        }

        #endregion

        /// <summary>
        /// Provides Win32 interop for actually creating a cursor properly
        /// </summary>
        private static class Win32
        {
            private struct IconInfo
            {
                public bool fIcon;
                public int xHotspot;
                public int yHotspot;
                public IntPtr hbmMask;
                public IntPtr hbmColor;
            }

            [DllImport("user32.dll")]
            private static extern IntPtr CreateIconIndirect(ref IconInfo icon);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

            /// <summary>
            /// Calls the apprpriate win32 methods to create a cursor
            /// </summary>
            /// <param name="bmp">bitmap to use for the image</param>
            /// <param name="xHotSpot">x coord for hot spot</param>
            /// <param name="yHotSpot">y coord for hot spot</param>
            /// <returns>a ready-to-use cursor</returns>
            public static Cursor CreateCursor(Bitmap bmp, int xHotSpot, int yHotSpot)
            {
                IconInfo tmp = new IconInfo();
                GetIconInfo(bmp.GetHicon(), ref tmp);
                tmp.xHotspot = xHotSpot;
                tmp.yHotspot = yHotSpot;
                tmp.fIcon = false;
                return new Cursor(CreateIconIndirect(ref tmp));
            }
        }
    }

    /// <summary>
    /// A class to pass the data that has been dropped to a callback
    /// </summary>
    public class DataDroppedEventArgs : EventArgs
    {
        internal DataDroppedEventArgs(Control target, object data)
            : base()
        {
            this.Target = target;
            this.Data = data;
        }

        /// <summary>
        /// Target control the data was dropped on.
        /// </summary>
        public Control Target { get; internal set; }

        /// <summary>
        /// The data associated with the data-drop event
        /// </summary>
        public object Data { get; internal set; }
    }
}
