using System.Windows;
using System.Windows.Controls;
using MathLock.Models;

namespace MathLock.Views;

public partial class ParentPinDialog : Window
{
    private readonly AppSettings _settings;
    public bool IsUnlocked { get; private set; } = false;

    public ParentPinDialog(AppSettings settings)
    {
        InitializeComponent();
        _settings = settings;
        Loaded += (_, _) => TxtPin.Focus();
    }

    private void NumBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Content is string digit)
        {
            if (TxtPin.Password.Length < 6)
            {
                TxtPin.Password += digit;
            }
        }
    }

    private void ClearBtn_Click(object sender, RoutedEventArgs e)
    {
        TxtPin.Password = "";
        TxtError.Text = "";
    }

    private void BackBtn_Click(object sender, RoutedEventArgs e)
    {
        if (TxtPin.Password.Length > 0)
        {
            TxtPin.Password = TxtPin.Password[..^1];
        }
    }

    private void TxtPin_PasswordChanged(object sender, RoutedEventArgs e)
    {
        TxtError.Text = "";
    }

    private void BtnUnlock_Click(object sender, RoutedEventArgs e)
    {
        ValidateAndUnlock();
    }

    private void ValidateAndUnlock()
    {
        string entered = TxtPin.Password.Trim();
        if (_settings.VerifyPin(entered))
        {
            IsUnlocked = true;
            DialogResult = true;
            Close();
        }
        else
        {
            TxtError.Text = "Incorrect PIN. Try again.";
            TxtPin.SelectAll();
            TxtPin.Focus();
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
