using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VeryHotKeys.WinForms
{
    /// <summary>
    /// A form that supports global hot keys.
    /// </summary>
    public class GlobalHotKeyForm : Form
    {
        protected IReadOnlyList<HotKeyRegisterer> HotKeyRegisterers => _hotKeyRegisterers.AsReadOnly();
        private readonly List<HotKeyRegisterer> _hotKeyRegisterers = new List<HotKeyRegisterer>();
        protected HotKeyRegisterer AddHotKeyRegisterer(HotKeyRegisterer reg)
        {
            _hotKeyRegisterers.Add(reg);
            return reg;
        }
        protected HotKeyRegisterer AddHotKeyRegisterer(EventHandler act, HotKeyMods mod, ConsoleKey key) =>
            AddHotKeyRegisterer(new HotKeyRegisterer(this, act, mod, key));

        protected HotKeyRegisterer AddHotKeyRegisterer(Action act, HotKeyMods mod, ConsoleKey key) =>
            AddHotKeyRegisterer(new HotKeyRegisterer(this, act, mod, key));


        protected HotKeyRegisterer AddHotKeyRegisterer(EventHandler act, HotKeyMods mod, uint key) =>
            AddHotKeyRegisterer(new HotKeyRegisterer(this, act, mod, key));

        protected HotKeyRegisterer AddHotKeyRegisterer(Action act, HotKeyMods mod, uint key) =>
            AddHotKeyRegisterer(new HotKeyRegisterer(this, act, mod, key));

        protected bool RemoveHotKeyRegisterer(HotKeyRegisterer reg)
        {
            var succeeded = _hotKeyRegisterers.Remove(reg);
            if (succeeded)
            {
                reg.Dispose();
            }

            return succeeded;
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            foreach (var item in _hotKeyRegisterers)
            {
                item.WndCall(ref m);
            }
        }
    }
}
