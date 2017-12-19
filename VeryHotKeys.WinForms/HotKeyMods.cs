using System;

namespace VeryHotKeys.WinForms
{
    /// <summary>
    /// Mods to use in <see cref="RegisterHotKey(IntPtr, int, uint, uint)"/>
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
