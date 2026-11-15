using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MathLock.Models;
using MathLock.Services;

namespace MathLock.Views;

public class StarItem : INotifyPropertyChanged
{
    private string _starGlyph = "☆";
    private Brush _starColor = new SolidColorBrush(Color.FromRgb(71, 85, 105)); // Gray #475569

    public string StarGlyph
    {
        get => _starGlyph;
        set { _starGlyph = value; OnPropertyChanged(nameof(StarGlyph)); }
    }

    public Brush StarColor
    {
        get => _starColor;
        set { _starColor = value; OnPropertyChanged(nameof(StarColor)); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public partial class LockWindow : Window
{
    private readonly AppSettings _settings;
    private readonly QuestionGenerator _generator;
    private readonly KeyboardBlocker _keyboardBlocker;
    private readonly List<BlockerWindow> _secondaryBlockers = new();

    private MathQuestion? _currentQuestion;
    private int _solvedCount = 0;
    private readonly int _targetCount;
    private readonly ObservableCollection<StarItem> _stars = new();

    // Anti-calculator timer
    private readonly DispatcherTimer _questionTimer;
    private double _remainingSeconds;
    private readonly double _totalSecondsPerQuestion;

    // Secret corner tap tracker (5 taps within 3 seconds)
    private readonly List<DateTime> _cornerTapTimes = new();

    private readonly bool _closeOnlyOnUnlock;

    public LockWindow(AppSettings settings, bool closeOnlyOnUnlock = false)
    {
        InitializeComponent();
        _settings = settings;
        _closeOnlyOnUnlock = closeOnlyOnUnlock;
        _targetCount = settings.RequiredQuestions;
        _totalSecondsPerQuestion = settings.SecondsPerQuestion;
        _generator = new QuestionGenerator();
        _keyboardBlocker = new KeyboardBlocker();

        // Setup Star display
        for (int i = 0; i < _targetCount; i++)
        {
            _stars.Add(new StarItem());
        }
        StarsItemsControl.ItemsSource = _stars;

        // Setup timer
        _questionTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        _questionTimer.Tick += QuestionTimer_Tick;

        Loaded += LockWindow_Loaded;
        Closing += LockWindow_Closing;
        Deactivated += LockWindow_Deactivated;
    }

    private void LockWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Block Windows keys, Alt+Tab, Alt+F4
        _keyboardBlocker.Start();

        // Cover secondary monitors
        CoverAllMonitors();

        // Topmost and active
        Topmost = true;
        Activate();
        Focus();

        StartNextQuestion();
    }

    private void CoverAllMonitors()
    {
        var monitors = DisplayService.GetAllMonitors();
        var primary = monitors.FirstOrDefault(m => m.IsPrimary) ?? monitors.FirstOrDefault();

        if (primary != null)
        {
            Left = primary.Left;
            Top = primary.Top;
            Width = primary.Width;
            Height = primary.Height;
            WindowState = WindowState.Normal;
            WindowState = WindowState.Maximized;
        }

        foreach (var monitor in monitors)
        {
            if (primary != null && monitor.DeviceName == primary.DeviceName)
                continue;

            var blocker = new BlockerWindow
            {
                Left = monitor.Left,
                Top = monitor.Top,
                Width = monitor.Width,
                Height = monitor.Height,
                WindowState = WindowState.Maximized
            };
            blocker.Show();
            _secondaryBlockers.Add(blocker);
        }
    }

    private void LockWindow_Deactivated(object? sender, EventArgs e)
    {
        // Force refocus if user clicks another app
        if (Visibility == Visibility.Visible)
        {
            Topmost = true;
            Activate();
        }
    }

    private void LockWindow_Closing(object? sender, CancelEventArgs e)
    {
        // Prevent closing unless math solved or parent unlocked
        if (_solvedCount < _targetCount)
        {
            e.Cancel = true;
        }
        else
        {
            CleanupAndExit();
        }
    }

    private void CleanupAndExit()
    {
        _questionTimer.Stop();
        _keyboardBlocker.Stop();

        foreach (var b in _secondaryBlockers)
        {
            try { b.Close(); } catch { }
        }
        _secondaryBlockers.Clear();
    }

    private void StartNextQuestion()
    {
        _currentQuestion = _generator.NextQuestion(_settings.Difficulty);

        TxtPromptTitle.Text = _currentQuestion.PromptTitle;
        TxtExpression.Text = _currentQuestion.ExpressionText;

        if (!string.IsNullOrEmpty(_currentQuestion.VisualHint))
        {
            TxtVisualHint.Text = _currentQuestion.VisualHint;
            TxtVisualHint.Visibility = Visibility.Visible;
        }
        else
        {
            TxtVisualHint.Visibility = Visibility.Collapsed;
        }

        TxtProgressDesc.Text = $"Question {_solvedCount + 1} of {_targetCount}: Solve it quick!";
        TxtFeedback.Text = "";

        // Populate Choice Buttons
        ChoicesGrid.Children.Clear();
        foreach (var choice in _currentQuestion.Choices)
        {
            var btn = new Button
            {
                Content = choice,
                Style = (Style)FindResource("ChoiceButtonStyle")
            };
            btn.Click += ChoiceButton_Click;
            ChoicesGrid.Children.Add(btn);
        }

        // Reset and start countdown timer
        _remainingSeconds = _totalSecondsPerQuestion;
        TimerProgressBar.Value = 100;
        TimerProgressBar.Foreground = new SolidColorBrush(Color.FromRgb(245, 158, 11)); // Amber
        _questionTimer.Start();
    }

    private void QuestionTimer_Tick(object? sender, EventArgs e)
    {
        _remainingSeconds -= 0.1;
        double pct = Math.Max(0, (_remainingSeconds / _totalSecondsPerQuestion) * 100);
        TimerProgressBar.Value = pct;

        if (pct < 30)
        {
            TimerProgressBar.Foreground = new SolidColorBrush(Color.FromRgb(239, 68, 68)); // Red
        }

        if (_remainingSeconds <= 0)
        {
            _questionTimer.Stop();
            SoundService.PlayIncorrect();
            TxtFeedback.Text = "⏳ Time's up! Let's try another one!";
            TxtFeedback.Foreground = new SolidColorBrush(Color.FromRgb(239, 68, 68));

            // Wait 1 second and generate a new question (doesn't advance progress)
            var restartTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            restartTimer.Tick += (_, _) =>
            {
                restartTimer.Stop();
                StartNextQuestion();
            };
            restartTimer.Start();
        }
    }

    private void ChoiceButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Content is string answer)
        {
            HandleAnswerSubmission(answer, btn);
        }
    }

    private void TxtDirectAnswer_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            HandleAnswerSubmission(TxtDirectAnswer.Text.Trim(), null);
        }
    }

    private void HandleAnswerSubmission(string submittedAnswer, Button? clickedBtn)
    {
        if (_currentQuestion == null) return;
        _questionTimer.Stop();

        bool isCorrect = string.Equals(submittedAnswer, _currentQuestion.TargetAnswer, StringComparison.OrdinalIgnoreCase);

        if (isCorrect)
        {
            SoundService.PlayCorrect();
            TxtFeedback.Text = "🎉 Correct! Super job! ⭐";
            TxtFeedback.Foreground = new SolidColorBrush(Color.FromRgb(16, 185, 129)); // Green

            if (clickedBtn != null)
            {
                clickedBtn.Background = new SolidColorBrush(Color.FromRgb(16, 185, 129));
            }

            // Fill a star
            if (_solvedCount < _stars.Count)
            {
                _stars[_solvedCount].StarGlyph = "★";
                _stars[_solvedCount].StarColor = new SolidColorBrush(Color.FromRgb(250, 204, 21)); // Gold #FACC15
            }

            _solvedCount++;

            if (_solvedCount >= _targetCount)
            {
                CompleteUnlock();
            }
            else
            {
                // Advance to next after a brief celebratory pause
                var delayTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(700) };
                delayTimer.Tick += (_, _) =>
                {
                    delayTimer.Stop();
                    StartNextQuestion();
                };
                delayTimer.Start();
            }
        }
        else
        {
            SoundService.PlayIncorrect();
            TxtFeedback.Text = "❌ Oops, try this one!";
            TxtFeedback.Foreground = new SolidColorBrush(Color.FromRgb(239, 68, 68)); // Red

            if (clickedBtn != null)
            {
                clickedBtn.Background = new SolidColorBrush(Color.FromRgb(239, 68, 68));
            }

            var delayTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(900) };
            delayTimer.Tick += (_, _) =>
            {
                delayTimer.Stop();
                StartNextQuestion();
            };
            delayTimer.Start();
        }
    }

    private void CompleteUnlock()
    {
        SoundService.PlayVictory();
        TxtPromptTitle.Text = "🏆 ALL DONE!";
        TxtExpression.Text = "Unlocked! Have fun!";
        TxtFeedback.Text = "🌟 Outstanding job! Unlocking desktop...";
        TxtFeedback.Foreground = new SolidColorBrush(Color.FromRgb(56, 189, 248));
        ChoicesGrid.Children.Clear();

        var exitTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.5) };
        exitTimer.Tick += (_, _) =>
        {
            exitTimer.Stop();
            CleanupAndExit();
            if (_closeOnlyOnUnlock)
            {
                Close();
            }
            else
            {
                Application.Current.Shutdown();
            }
        };
        exitTimer.Start();
    }

    // Special Parent Unlock: Tap top-left corner 5 times within 3 seconds
    private void HiddenCornerBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var now = DateTime.UtcNow;
        _cornerTapTimes.Add(now);

        // Remove taps older than 3 seconds
        _cornerTapTimes.RemoveAll(t => (now - t).TotalSeconds > 3);

        if (_cornerTapTimes.Count >= 5)
        {
            _cornerTapTimes.Clear();
            _questionTimer.Stop();

            var dialog = new ParentPinDialog(_settings) { Owner = this };
            bool? res = dialog.ShowDialog();

            if (res == true && dialog.IsUnlocked)
            {
                // Parent override successful
                _solvedCount = _targetCount; // Allow window closing
                CleanupAndExit();
                if (_closeOnlyOnUnlock)
                {
                    Close();
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
            else
            {
                // Resumed math challenge
                _questionTimer.Start();
            }
        }
    }
}
