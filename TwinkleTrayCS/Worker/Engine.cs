using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Newtonsoft.Json;
using TwinkleTrayCS.Models;
using TwinkleTrayCS.Helpers;

namespace TwinkleTrayCS.Worker
{
    public class Engine
    {
        private static Engine? _instance;
        public static Engine Instance => _instance ??= new Engine();

        public UserSettings Settings { get; private set; } = new UserSettings();
        public List<MonitorInfo> Monitors { get; private set; } = new List<MonitorInfo>();
        
        private System.Timers.Timer _updateTimer;
        private string _settingsPath;
        private bool _isIdleDimmed = false;
        private Dictionary<string, int> _preIdleBrightness = new Dictionary<string, int>();

        private Engine()
        {
            _settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TwinkleTrayCS", "settings.json");
            LoadSettings();

            _updateTimer = new System.Timers.Timer(Settings.UpdateInterval);
            _updateTimer.Elapsed += OnUpdateTimerElapsed;
            _updateTimer.AutoReset = true;
        }

        public void Start()
        {
            RefreshMonitors();
            _updateTimer.Start();
            
            HotkeyManager.Instance.HotkeyTyped += OnHotkeyTyped;
            ApplyHotkeys();
        }

        private void OnHotkeyTyped(object? sender, HotkeyConfig config)
        {
            foreach (var action in config.Actions)
            {
                if (action.Type == "offset")
                {
                    AdjustBrightness(action, action.Value);
                }
                else if (action.Type == "set")
                {
                    SetBrightnessFromAction(action, action.Value);
                }
            }
        }

        private void AdjustBrightness(HotkeyAction action, int offset)
        {
            foreach (var monitor in Monitors)
            {
                if (action.AllMonitors || action.Monitors.ContainsKey(monitor.Id))
                {
                    int newBrightness = Math.Clamp(monitor.Brightness + offset, 0, 100);
                    MonitorManager.SetBrightness(monitor, newBrightness);
                }
            }
        }

        private void SetBrightnessFromAction(HotkeyAction action, int value)
        {
            foreach (var monitor in Monitors)
            {
                if (action.AllMonitors || action.Monitors.ContainsKey(monitor.Id))
                {
                    MonitorManager.SetBrightness(monitor, value);
                }
            }
        }

        public void ApplyHotkeys()
        {
            HotkeyManager.Instance.UnregisterAll();
            foreach (var hotkey in Settings.Hotkeys)
            {
                HotkeyManager.Instance.RegisterHotkey(hotkey);
            }
        }

        private void OnUpdateTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            // Handle time-based adjustments and idle detection here
            CheckTimeAdjustments();
            CheckIdleDetection();
        }

        public void RefreshMonitors()
        {
            Monitors = MonitorManager.DiscoverMonitors();
        }

        public void SetBrightness(string monitorId, int brightness)
        {
            var monitor = Monitors.Find(m => m.Id == monitorId);
            if (monitor != null)
            {
                MonitorManager.SetBrightness(monitor, brightness);
            }
        }

        private void CheckIdleDetection()
        {
            if (!Settings.IdleDimEnabled) return;

            uint idleTimeMs = IdleMonitor.GetIdleTime();
            bool shouldBeIdle = idleTimeMs >= (uint)(Settings.IdleDimThreshold * 1000);

            if (shouldBeIdle && !_isIdleDimmed)
            {
                // Entering idle
                _isIdleDimmed = true;
                _preIdleBrightness.Clear();
                foreach (var monitor in Monitors)
                {
                    _preIdleBrightness[monitor.Id] = monitor.Brightness;
                    MonitorManager.SetBrightness(monitor, Settings.IdleDimBrightness);
                }
            }
            else if (!shouldBeIdle && _isIdleDimmed)
            {
                // Waking up
                _isIdleDimmed = false;
                foreach (var monitor in Monitors)
                {
                    if (_preIdleBrightness.TryGetValue(monitor.Id, out int oldVal))
                    {
                        MonitorManager.SetBrightness(monitor, oldVal);
                    }
                }
            }
        }

        private void CheckTimeAdjustments()
        {
            // Simple logic: find adjustment for current time
            string now = DateTime.Now.ToString("HH:mm");
            var adjustment = Settings.AdjustmentTimes.Find(a => a.Time == now);
            if (adjustment != null)
            {
                foreach (var monitor in Monitors)
                {
                    MonitorManager.SetBrightness(monitor, adjustment.Brightness);
                }
            }
        }

        public void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    string json = File.ReadAllText(_settingsPath);
                    Settings = JsonConvert.DeserializeObject<UserSettings>(json) ?? new UserSettings();
                }
            }
            catch { }
        }

        public void SaveSettings()
        {
            try
            {
                string dir = Path.GetDirectoryName(_settingsPath)!;
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                string json = JsonConvert.SerializeObject(Settings, Formatting.Indented);
                File.WriteAllText(_settingsPath, json);
            }
            catch { }
        }
    }
}
