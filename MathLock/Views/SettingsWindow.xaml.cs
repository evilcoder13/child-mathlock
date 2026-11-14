using System.Diagnostics;
using System.IO;
using System.Windows;
using MathLock.Models;
using MathLock.Services;

namespace MathLock.Views;

public partial class SettingsWindow : Window
{
    private readonly AppSettings _settings;

    public SettingsWindow(AppSettings settings)
    {
        InitializeComponent();
        _settings = settings;

        LoadCurrentSettings();
    }

    private void LoadCurrentSettings()
    {
        CmbDifficulty.SelectedIndex = (int)_settings.Difficulty;
        SliderQuestions.Value = _settings.RequiredQuestions;
        SliderTimer.Value = _settings.SecondsPerQuestion;
        ChkSound.IsChecked = _settings.PlaySounds;
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        _settings.Difficulty = (DifficultyLevel)CmbDifficulty.SelectedIndex;
        _settings.RequiredQuestions = (int)SliderQuestions.Value;
        _settings.SecondsPerQuestion = (int)SliderTimer.Value;
        _settings.PlaySounds = ChkSound.IsChecked ?? true;

        if (!string.IsNullOrEmpty(TxtNewPin.Password))
        {
            if (TxtNewPin.Password != TxtConfirmPin.Password)
            {
                System.Windows.MessageBox.Show("PIN confirmation does not match!", "PIN Mismatch", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (TxtNewPin.Password.Length < 4)
            {
                System.Windows.MessageBox.Show("PIN must be at least 4 digits.", "PIN too short", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _settings.ParentPinSha256 = AppSettings.ComputeSha256(TxtNewPin.Password);
            TxtNewPin.Password = "";
            TxtConfirmPin.Password = "";
        }

        SettingsService.Save(_settings);
        System.Windows.MessageBox.Show("Settings saved successfully!", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void BtnTestLock_Click(object sender, RoutedEventArgs e)
    {
        // Save first
        BtnSave_Click(sender, e);

        // Open LockWindow
        var lockWin = new LockWindow(_settings);
        lockWin.Show();
        Close();
    }

    private void BtnRegisterTask_Click(object sender, RoutedEventArgs e)
    {
        string? exePath = Process.GetCurrentProcess().MainModule?.FileName;
        if (string.IsNullOrEmpty(exePath) || !File.Exists(exePath))
        {
            TxtTaskStatus.Text = "Could not locate executable path.";
            return;
        }

        bool ok = TaskSchedulerService.RegisterTriggers(exePath);
        TxtTaskStatus.Text = ok ? "Auto-lock triggers successfully registered!" : "Failed to register triggers. Please run as Administrator.";
    }

    private void BtnUnregisterTask_Click(object sender, RoutedEventArgs e)
    {
        bool ok = TaskSchedulerService.UnregisterTriggers();
        TxtTaskStatus.Text = ok ? "Auto-lock triggers removed." : "Failed to remove triggers.";
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
