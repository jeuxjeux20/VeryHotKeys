using System;

namespace VeryHotKeys.WPF
{
    /// <summary>
    /// Mods to use in <see cref="HotKeyRegisterer.HotKeyRegisterer(System.Windows.Window, Action, WPF.HotKeyMods, ConsoleKey)"/>
    /// </summary>

    [Flags]
    public enum HotKeyMods : uint
    {
        None = 0x0000, // (none)
        Alt = 0x0001, //ALT
        Control = 0x0002, //CTRL
        Shift = 0x0004, //SHIFT
        Windows = 0x0008 //WINDOWS KEY
    }
}
