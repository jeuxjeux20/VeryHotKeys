using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace VeryHotKeys.Wpf
{
    public class HotKeyRegisterer : IDisposable
    {
        private static int _idCount;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// Called when the hot key is pressed. 
        /// </summary>
        public event EventHandler OnTriggerFunction;

        private HwndSource _source;
        private uint _finalKey;
        private readonly int _hotId = 9000;
        // public Action OnTriggerFunction { get; private set; }
        private IntPtr _handle;

        /// <summary>
        /// Constructor for a new hotkey
        /// </summary>
        /// <param name="window">The current WPF window.</param>
        /// <param name="act">What to execute when the hot key is called</param>
        /// <param name="mod">The first optional key combination </param>
        /// <param name="key">The final key, binded with <paramref name="mod"/></param>
        public HotKeyRegisterer(Window window, EventHandler act, HotKeyMods mod, uint key)
        {
            _idCount++;
            _hotId += _idCount;
            OnTriggerFunction += act;
            if (!window.IsLoaded)
            {
                window.Loaded += (_, __) => Initialize(window, mod, key);
                return;
            }
            Initialize(window, mod, key);
        }

        private void Initialize(Window window, HotKeyMods mod, uint key)
        {
            var interopHelp = new WindowInteropHelper(window);
            _handle = interopHelp.Handle;
            _source = HwndSource.FromHwnd(_handle);
            _source.AddHook(HwndHook);
            _finalKey = key;
            RegisterHotKey(_handle, _hotId, (uint) mod, key);
            window.Closed += (_, __) => Dispose();
        }

        /// <summary>
        /// Constructor for a new hotkey
        /// </summary>
        /// <param name="window">The current WPF window.</param>
        /// <param name="act">What to execute when the hot key is called</param>
        /// <param name="mod">The first optional key combination </param>
        /// <param name="key">The final key using the ConsoleKey enum, for more keys, use the other constructor. Bound with <paramref name="mod"/></param>
        public HotKeyRegisterer(Window window, EventHandler act, HotKeyMods mod, ConsoleKey key) : this(window, act, mod, (uint)key) { }
        /// <inheritdoc />
        public HotKeyRegisterer(Window window, Action act, HotKeyMods mod, ConsoleKey key) : this(window, (_,__) => act(), mod, (uint)key) { }
        /// <inheritdoc />
        public HotKeyRegisterer(Window window, Action act, HotKeyMods mod, uint key) : this(window, (_,__) => act(), mod, key) { }
        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int wmHotkey = 0x0312;
            switch (msg)
            {
                case wmHotkey:
                    if (wParam.ToInt32() == _hotId)
                    {
                        int vkey = (((int)lParam >> 16) & 0xFFFF);
                        if (vkey == _finalKey)
                        {
                            OnTriggerFunction?.Invoke(this, EventArgs.Empty);
                        }
                        handled = true;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        #region IDisposable Support
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    OnTriggerFunction = null;
                }

                _source.RemoveHook(HwndHook);
                UnregisterHotKey(_handle, _hotId);
                _disposedValue = true;
            }
        }

        ~HotKeyRegisterer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
