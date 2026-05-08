using System;
using System.Collections.Generic;

namespace TwinkleTrayCS.Models
{
    public class UserSettings
    {
        public string SettingsVer { get; set; } = "v1.0.0";
        public bool UserClosedIntro { get; set; } = false;
        public string Theme { get; set; } = "default";
        public int UpdateInterval { get; set; } = 500;
        public bool OpenAtLogin { get; set; } = true;
        public bool BrightnessAtStartup { get; set; } = true;
        
        public List<HotkeyConfig> Hotkeys { get; set; } = new List<HotkeyConfig>();
        public int HotkeyPercent { get; set; } = 10;
        
        public List<AdjustmentTime> AdjustmentTimes { get; set; } = new List<AdjustmentTime>();
        public bool AdjustmentTimeIndividualDisplays { get; set; } = false;
        
        public Dictionary<string, string> Names { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, bool> HideDisplays { get; set; } = new Dictionary<string, bool>();
        
        public bool ScrollShortcut { get; set; } = true;
        public int ScrollShortcutAmount { get; set; } = 2;

        public bool IdleDimEnabled { get; set; } = false;
        public int IdleDimThreshold { get; set; } = 300; // seconds
        public int IdleDimBrightness { get; set; } = 10;
        
        public bool UseAcrylic { get; set; } = false;
        public string Language { get; set; } = "system";
        
        public string Uuid { get; set; } = Guid.NewGuid().ToString();

        public Dictionary<string, List<CalibrationPoint>> Remaps { get; set; } = new Dictionary<string, List<CalibrationPoint>>();
    }

    public class CalibrationPoint
    {
        public int Input { get; set; }
        public int Output { get; set; }
    }

    public class HotkeyConfig
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Accelerator { get; set; } = "";
        public List<HotkeyAction> Actions { get; set; } = new List<HotkeyAction>();
    }

    public class HotkeyAction
    {
        public string Type { get; set; } = "offset"; // offset, set, off
        public int Value { get; set; } = 0;
        public bool AllMonitors { get; set; } = false;
        public Dictionary<string, bool> Monitors { get; set; } = new Dictionary<string, bool>();
    }

    public class AdjustmentTime
    {
        public string Time { get; set; } = "00:00";
        public int Brightness { get; set; } = 50;
        public bool UseSunCalc { get; set; } = false;
        public string SunCalc { get; set; } = "sunrise";
    }
}
