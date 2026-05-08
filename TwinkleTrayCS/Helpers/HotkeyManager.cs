using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TwinkleTrayCS.Models;
using static TwinkleTrayCS.Helpers.NativeMethods;

namespace TwinkleTrayCS.Helpers
{
    public class HotkeyManager : NativeWindow, IDisposable
    {
        private static HotkeyManager? _instance;
        public static HotkeyManager Instance => _instance ??= new HotkeyManager();

        private Dictionary<int, HotkeyConfig> _hotkeys = new Dictionary<int, HotkeyConfig>();
        private int _currentId = 1;

        public event EventHandler<HotkeyConfig>? HotkeyTyped;

        private HotkeyManager()
        {
            this.CreateHandle(new CreateParams());
        }

        public void RegisterHotkey(HotkeyConfig config)
        {
            uint modifiers = 0;
            Keys key = Keys.None;

            // Parse accelerator string (e.g., "Ctrl+Alt+Up")
            string[] parts = config.Accelerator.Split('+');
            foreach (var part in parts)
            {
                switch (part.Trim().ToLower())
                {
                    case "ctrl": modifiers |= MOD_CONTROL; break;
                    case "alt": modifiers |= MOD_ALT; break;
                    case "shift": modifiers |= MOD_SHIFT; break;
                    case "win": modifiers |= MOD_WIN; break;
                    default:
                        if (Enum.TryParse(part, true, out Keys k))
                        {
                            key = k;
                        }
                        break;
                }
            }

            if (key != Keys.None)
            {
                int id = _currentId++;
                if (NativeMethods.RegisterHotKey(this.Handle, id, modifiers, (uint)key))
                {
                    _hotkeys[id] = config;
                }
            }
        }

        public void UnregisterAll()
        {
            foreach (var id in _hotkeys.Keys)
            {
                NativeMethods.UnregisterHotKey(this.Handle, id);
            }
            _hotkeys.Clear();
            _currentId = 1;
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                if (_hotkeys.TryGetValue(id, out var config))
                {
                    HotkeyTyped?.Invoke(this, config);
                }
            }
            base.WndProc(ref m);
        }

        public void Dispose()
        {
            UnregisterAll();
            this.DestroyHandle();
        }
    }
}
