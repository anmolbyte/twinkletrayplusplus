using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;
using TwinkleTrayAvalonia.Views;
using TwinkleTrayAvalonia.Worker;

namespace TwinkleTrayAvalonia;

public partial class App : Application
{
    private BrightnessFlyout? _flyout;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Start Engine
        Engine.Instance.Start();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            _flyout = new BrightnessFlyout();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void TrayIcon_Clicked(object? sender, EventArgs e)
    {
        if (_flyout != null)
        {
            _flyout.LoadMonitors();
            
            // Initial show to ensure window is initialized
            _flyout.Show();

            // Position flyout near tray
            var screens = _flyout.Screens;
            if (screens.Primary != null)
            {
                var workArea = screens.Primary.WorkingArea;
                
                // Calculate position: Bottom-Right corner
                int x = workArea.Right - (int)(_flyout.Width * _flyout.RenderScaling) - 10;
                int y = workArea.Bottom - (int)(_flyout.Height * _flyout.RenderScaling) - 10;
                
                _flyout.Position = new PixelPoint(x, y);
            }

            _flyout.Activate();
        }
    }

    private void OnSettingsClick(object? sender, EventArgs e)
    {
        // Show settings window
    }

    private void OnExitClick(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }
}
