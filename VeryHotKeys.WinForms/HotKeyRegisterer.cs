using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VeryHotKeys.WinForms
{
    public class HotKeyRegisterer : IDisposable
    {
        private static int _idCount = 0;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// Called when the hot key is pressed. 
        /// </summary>
        public event EventHandler OnTriggerFunction;

        private readonly uint _finalKey;
        private readonly int _hotKeyUniqueId = 857502;
        private static readonly Random Random = new Random();


        private readonly IntPtr _handle = default(IntPtr);

        /// <summary>
        /// Constructor for a new hotkey
        /// </summary>
        /// <param name="window">The current WinForms window.</param>
        /// <param name="act">What to execute when the hot key is called</param>
        /// <param name="mod">The first optional key combination </param>
        /// <param name="key">The final key, bound with <paramref name="mod"/></param>
        /// <param name="disposeOnClosing">Tells whether or not the registerer should dispose when the form closes.</param>
        public HotKeyRegisterer(Form window, EventHandler act, HotKeyMods mod, uint key, bool disposeOnClosing = true)
        {
            _idCount++;
            _hotKeyUniqueId += _idCount;
            OnTriggerFunction += act;
            _finalKey = key;
            RegisterHotKey(window.Handle, _hotKeyUniqueId, (uint)mod, key);
            if (disposeOnClosing)
            {
                window.Closed += (sender, args) => Dispose();
            }
        }

        /// <summary>
        /// Constructor for a new hot key
        /// </summary>
        /// <param name="window">The current WinForms window.</param>
        /// <param name="act">What to execute when the hot key is called</param>
        /// <param name="mod">The first optional key combination </param>
        /// <param name="key">The final key using the ConsoleKey enum, for more keys, use the other constructor. Bound with <paramref name="mod"/></param>
        /// <param name="disposeOnClosing">Tells whether or not the registerer should dispose when the form closes.</param>
        public HotKeyRegisterer(Form window, EventHandler act, HotKeyMods mod, ConsoleKey key, bool disposeOnClosing = true) : this(window, act, mod, (uint)key, disposeOnClosing) { }
        /// <inheritdoc />
        public HotKeyRegisterer(Form window, Action act, HotKeyMods mod, ConsoleKey key, bool disposeOnClosing = true) : this(window, (_,__) => act(), mod, (uint)key, disposeOnClosing) { }
        /// <inheritdoc/> 
        public HotKeyRegisterer(Form window, Action act, HotKeyMods mod, uint key, bool disposeOnClosing = true) : this(window, (_, __) => act(), mod, key, disposeOnClosing) { }
        internal IntPtr WndCall(ref Message msg)
        {
            const int wmHotkey = 0x0312;
            switch (msg.Msg)
            {
                case wmHotkey:
                    if (msg.WParam.ToInt32() == _hotKeyUniqueId)
                    {
                        int vkey = ((int)msg.LParam >> 16) & 0xFFFF;
                        if (vkey == _finalKey)
                        {
                            OnTriggerFunction?.Invoke(this, EventArgs.Empty);
                        }
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        #region IDisposable Support
        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                OnTriggerFunction = null;
            }
            UnregisterHotKey(_handle, _hotKeyUniqueId);
            _disposedValue = true;
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