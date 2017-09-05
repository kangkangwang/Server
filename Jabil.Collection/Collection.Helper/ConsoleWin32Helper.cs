//using System;
//using System.Runtime.InteropServices;
//using System.Threading;
//using System.Windows.Forms;

//namespace Collection.Base.Helper
//{
//    internal class ConsoleWin32Helper
//    {
//        static ConsoleWin32Helper()
//        {
//            //_NotifyIcon.Icon = new Icon(@"C:\Users\MR.Mei\Desktop\1.ico");
//            //_NotifyIcon.Visible = false;
//            //_NotifyIcon.Text = "tray";

//            var menu = new ContextMenu();
//            var item = new MenuItem();
//            item.Text = "右键菜单，还没有添加事件";
//            item.Index = 0;

//            menu.MenuItems.Add(item);
//            _NotifyIcon.ContextMenu = menu;

//            _NotifyIcon.MouseDoubleClick += _NotifyIcon_MouseDoubleClick;
//        }

//        private static void _NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
//        {
//            Console.WriteLine("托盘被双击.");
//        }

//        #region 禁用关闭按钮

//        [DllImport("User32.dll", EntryPoint = "FindWindow")]
//        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

//        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
//        private static extern IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);

//        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
//        private static extern IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

//        /// <summary>
//        ///     禁用关闭按钮
//        /// </summary>
//        /// <param name="consoleName">控制台名字</param>
//        public static void DisableCloseButton(string title)
//        {
//            //线程睡眠，确保closebtn中能够正常FindWindow，否则有时会Find失败。。
//            Thread.Sleep(100);

//            IntPtr windowHandle = FindWindow(null, title);
//            IntPtr closeMenu = GetSystemMenu(windowHandle, IntPtr.Zero);
//            uint SC_CLOSE = 0xF060;
//            RemoveMenu(closeMenu, SC_CLOSE, 0x0);
//        }

//        public static bool IsExistsConsole(string title)
//        {
//            IntPtr windowHandle = FindWindow(null, title);
//            if (windowHandle.Equals(IntPtr.Zero)) return false;

//            return true;
//        }

//        #endregion

//        #region 托盘图标

//        private static readonly NotifyIcon _NotifyIcon = new NotifyIcon();

//        public static void ShowNotifyIcon()
//        {
//            _NotifyIcon.Visible = true;
//            _NotifyIcon.ShowBalloonTip(3000, "", "我是托盘图标，用右键点击我试试，还可以双击看看。", ToolTipIcon.None);
//        }

//        public static void HideNotifyIcon()
//        {
//            _NotifyIcon.Visible = false;
//        }

//        #endregion
//    }
//}

