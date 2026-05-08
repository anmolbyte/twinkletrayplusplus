using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TwinkleTrayAvalonia.Models;

namespace TwinkleTrayAvalonia.Helpers
{
    public class HotkeyManager : IDisposable
    {
        private static HotkeyManager? _instance;
        public static HotkeyManager Instance => _instance ??= new HotkeyManager();

        private Dictionary<int, HotkeyConfig> _hotkeys = new Dictionary<int, HotkeyConfig>();
        private int _currentId = 1;
        private IntPtr _hwnd = IntPtr.Zero;

        public event EventHandler<HotkeyConfig>? HotkeyTyped;

        private HotkeyManager() { }

        public void SetWindowHandle(IntPtr hwnd)
        {
            _hwnd = hwnd;
        }

        public void RegisterHotkey(HotkeyConfig config)
        {
            if (_hwnd == IntPtr.Zero) return;

            uint modifiers = 0;
            uint key = 0;

            string[] parts = config.Accelerator.Split('+');
            foreach (var part in parts)
            {
                switch (part.Trim().ToLower())
                {
                    case "ctrl": modifiers |= NativeMethods.MOD_CONTROL; break;
                    case "alt": modifiers |= NativeMethods.MOD_ALT; break;
                    case "shift": modifiers |= NativeMethods.MOD_SHIFT; break;
                    case "win": modifiers |= NativeMethods.MOD_WIN; break;
                    default:
                        if (Enum.TryParse(part, true, out VirtualKey k))
                        {
                            key = (uint)k;
                        }
                        break;
                }
            }

            if (key != 0)
            {
                int id = _currentId++;
                if (NativeMethods.RegisterHotKey(_hwnd, id, modifiers, key))
                {
                    _hotkeys[id] = config;
                }
            }
        }

        public void UnregisterAll()
        {
            if (_hwnd == IntPtr.Zero) return;

            foreach (var id in _hotkeys.Keys)
            {
                NativeMethods.UnregisterHotKey(_hwnd, id);
            }
            _hotkeys.Clear();
            _currentId = 1;
        }

        public void ProcessMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                if (_hotkeys.TryGetValue(id, out var config))
                {
                    HotkeyTyped?.Invoke(this, config);
                    handled = true;
                }
            }
        }

        public void Dispose()
        {
            UnregisterAll();
        }
    }
}
