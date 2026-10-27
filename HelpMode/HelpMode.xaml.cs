namespace HelpMode;

public partial class HelpMode : ContentView
{
    private int _helpmodeIndex;
    private string[]? _tips;
    private List<GridPosition>? _gridPositions;
    private ElementHighlighter? _highlighter;

    #region TextColor Property

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty TextColorProperty =
    BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(HelpMode), defaultValue: Colors.Black, propertyChanged: OnTextColorPropertyChanged);

    private static void OnTextColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        HelpMode control = (HelpMode)bindable;
        control.HelpmodeTipLabel.TextColor = (Color)newValue;
        control.CloseLabel.TextColor = (Color)newValue;
    }

    #endregion

    #region BackgroundColor Property

    public new Color BackgroundColor
    {
        get => (Color)GetValue(BackgroundColorProperty);
        set => SetValue(BackgroundColorProperty, value);
    }

    public static new readonly BindableProperty BackgroundColorProperty =
    BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(HelpMode), defaultValue: Colors.White, propertyChanged: OnBackgroundColorPropertyChanged);

    private static void OnBackgroundColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        HelpMode control = (HelpMode)bindable;
        control.ControlBorder.Background = (Color)newValue;
        control.ControlBorder.Stroke = (Color)newValue;
    }

    #endregion

    public HelpMode()
    {
        InitializeComponent();
    }

    #region Setup (With Overloads)

    public void Setup(string[] tipArray)
    {
        _tips = tipArray;
    }

    public void Setup(string[] tipArray, ElementHighlighter elementHighlighter, List<GridPosition> gridPositions)
    {
        _tips = tipArray;
        _gridPositions = gridPositions;
        _highlighter = elementHighlighter;
    }

    #endregion

    public void Toggle()
    {
        IsVisible = !IsVisible;

        _helpmodeIndex = 0;

        UpdateView();
    }

    public void NextTip()
    {
        _helpmodeIndex++;

        UpdateView();
    }

    private void UpdateView()
    {
        if (_tips == null)
        {
            throw new ArgumentNullException(nameof(_tips), "Tips is null. Remember to setup HelpMode before using it");
        }

        // If all _tips have been read
        if (_helpmodeIndex == _tips.Length)
        {
            _helpmodeIndex = 0;
            IsVisible = false;

            _highlighter?.IsVisible = false;
        }
        else
        {
            HelpmodeTipLabel.Text = _tips[_helpmodeIndex];
            IsVisible = true;

            if (_highlighter == null) { return; }

            if (_gridPositions == null)
            {
                throw new ArgumentNullException(nameof(_gridPositions), "GridPositions is null. Consider providing a List of GridPositions or removing the ElementHighlighter from setup");
            }

            _highlighter.IsVisible = true;
            _highlighter.Highlight(_gridPositions[_helpmodeIndex]);
        }
    }
}
