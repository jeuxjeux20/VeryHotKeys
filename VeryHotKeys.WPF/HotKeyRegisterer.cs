using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace VeryHotKeys.WPF
{
    class HotKeyRegisterer : IDisposable
    {
        private static int idCount = 0;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// Called when the hot key is pressed. 
        /// </summary>
        public event Action OnTriggerFunction;

        private HwndSource source;
        public readonly uint finalKey;
        private readonly int hotID = 9000;
        // public Action OnTriggerFunction { get; private set; }
        IntPtr handle;

        /// <summary>
        /// Constructor for a new hotkey
        /// </summary>
        /// <param name="window">The current WPF window. Use <see cref="HotKeyRegisterer.HotKeyRegisterer(IntPtr, Action, HotKeyMods, key)"/> for WinForms or other.</param>
        /// <param name="act">What to execute when the hot key is called</param>
        /// <param name="mod">The first optional key combination </param>
        /// <param name="key">The final key, binded with <paramref name="mod"/></param>
        public HotKeyRegisterer(System.Windows.Window window, Action act, HotKeyMods mod, uint key)
        {
            idCount++;
            hotID += idCount;
            OnTriggerFunction += act;
            var interopHelp = new WindowInteropHelper(window);
            handle = interopHelp.Handle;
            source = HwndSource.FromHwnd(handle);
            source.AddHook(HwndHook);
            finalKey = key;
            RegisterHotKey(handle, hotID, (uint)mod, key);
        }
        /// <summary>
        /// Constructor for a new hotkey
        /// </summary>
        /// <param name="window">The current WPF window. Use <see cref="HotKeyRegisterer.HotKeyRegisterer(IntPtr, Action, HotKeyMods, ConsoleKey)"/> for WinForms or other.</param>
        /// <param name="act">What to execute when the hot key is called</param>
        /// <param name="mod">The first optional key combination </param>
        /// <param name="key">The final key using the ConsoleKey enum, for more keys, use the other constructor. Binded with <paramref name="mod"/></param>
        public HotKeyRegisterer(System.Windows.Window window, Action act, HotKeyMods mod, ConsoleKey key) : this(window, act, mod, (uint)key) { }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    if (wParam.ToInt32() == hotID)
                    {
                        int vkey = (((int)lParam >> 16) & 0xFFFF);
                        if (vkey == finalKey)
                        {
                            OnTriggerFunction();
                        }
                        handled = true;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        #region IDisposable Support
        private bool disposedValue = false; // Pour détecter les appels redondants

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OnTriggerFunction = null;
                }

                source.RemoveHook(HwndHook);
                UnregisterHotKey(handle, hotID);
                disposedValue = true;
            }
        }

        ~HotKeyRegisterer()
        {
            // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
            Dispose(false);
        }

        public void Dispose()
        {
            // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
