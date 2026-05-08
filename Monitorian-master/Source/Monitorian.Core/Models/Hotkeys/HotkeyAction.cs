using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Monitorian.Core.Models.Hotkeys;

/// <summary>
/// The type of brightness/contrast action to perform when a hotkey fires.
/// </summary>
[DataContract]
public enum HotkeyActionType
{
	/// <summary>Adjust brightness by a relative offset (positive or negative).</summary>
	[EnumMember] BrightnessOffset,

	/// <summary>Set brightness to an absolute value (0–100).</summary>
	[EnumMember] BrightnessSet,
}

/// <summary>
/// Represents one user-configured global hotkey binding.
/// </summary>
[DataContract]
public class HotkeyAction : INotifyPropertyChanged
{
	/// <summary>
	/// Human-readable display string, e.g. "Ctrl+Alt+Right".
	/// </summary>
	[DataMember]
	public string DisplayText
	{
		get => _displayText;
		set { _displayText = value; OnPropertyChanged(nameof(DisplayText)); }
	}
	private string _displayText = string.Empty;

	/// <summary>
	/// Win32 modifier flags (MOD_ALT=1, MOD_CONTROL=2, MOD_SHIFT=4, MOD_WIN=8).
	/// </summary>
	[DataMember]
	public uint Modifiers
	{
		get => _modifiers;
		set { _modifiers = value; OnPropertyChanged(nameof(Modifiers)); }
	}
	private uint _modifiers;

	/// <summary>
	/// Win32 virtual key code.
	/// </summary>
	[DataMember]
	public uint VirtualKey
	{
		get => _virtualKey;
		set { _virtualKey = value; OnPropertyChanged(nameof(VirtualKey)); }
	}
	private uint _virtualKey;

	/// <summary>
	/// DeviceInstanceId of the target monitor, or "*" to affect all monitors.
	/// </summary>
	[DataMember]
	public string TargetMonitorId
	{
		get => _targetMonitorId;
		set { _targetMonitorId = value; OnPropertyChanged(nameof(TargetMonitorId)); }
	}
	private string _targetMonitorId = "*";

	/// <summary>
	/// What to do when this hotkey fires.
	/// </summary>
	[DataMember]
	public HotkeyActionType ActionType
	{
		get => _actionType;
		set
		{
			if (_actionType != value)
			{
				_actionType = value;
				OnPropertyChanged(nameof(ActionType));
				Value = _value;
			}
		}
	}
	private HotkeyActionType _actionType = HotkeyActionType.BrightnessOffset;

	/// <summary>
	/// For BrightnessOffset: the delta (e.g. +5 or -10).
	/// For BrightnessSet: the absolute target value (0–100).
	/// </summary>
	[DataMember]
	public int Value
	{
		get => _value;
		set
		{
			var newValue = value;
			if (ActionType == HotkeyActionType.BrightnessSet)
			{
				if (newValue < 0) newValue = 0;
				if (newValue > 100) newValue = 100;
			}
			else
			{
				if (newValue == 0) newValue = 5;
			}
			if (_value != newValue)
			{
				_value = newValue;
				OnPropertyChanged(nameof(Value));
			}
		}
	}
	private int _value = 5;

	/// <summary>
	/// Internal ID assigned at registration time, used to match WM_HOTKEY wParam.
	/// Not persisted.
	/// </summary>
	public int RegisteredId { get; set; } = -1;

	public event PropertyChangedEventHandler PropertyChanged;

	protected virtual void OnPropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
