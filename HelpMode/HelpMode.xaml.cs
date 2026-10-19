namespace HelpMode;

public partial class HelpMode : ContentView
{
	private int HelpmodeIndex = 0;
	private string[]? Tips = null;
    private List<GridPosition>? GridPositions = null;
    private ElementHighlighter? Highlighter = null;

    #region TextColor Property

    public Color TextColor
    {
        get
        {
            return (Color)GetValue(TextColorProperty);
        }
        set
        {
            SetValue(TextColorProperty, value);
        }
    }

    public static readonly BindableProperty TextColorProperty =
    BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(HelpMode), defaultValue: Colors.Black, propertyChanged: OnTextColorPropertyChanged);
    
    private static void OnTextColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var Control = (HelpMode)bindable;
        Control.HelpmodeTipLabel.TextColor = (Color)newValue;
        Control.CloseLabel.TextColor = (Color)newValue;
    }

    #endregion

    #region BackgroundColor Property

    public new Color BackgroundColor
    {
        get
        {
            return (Color)GetValue(BackgroundColorProperty);
        }
        set
        {
            SetValue(BackgroundColorProperty, value);
        }
    }

    public static readonly new BindableProperty BackgroundColorProperty =
    BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(HelpMode), defaultValue: Colors.White, propertyChanged: OnBackgroundColorPropertyChanged);

    private static void OnBackgroundColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var Control = (HelpMode)bindable;
        Control.ControlBorder.Background = (Color)newValue;
        Control.ControlBorder.Stroke = (Color)newValue;
    }

    #endregion

    public HelpMode()
	{
		InitializeComponent();
	}

    #region Setup (With Overloads)

    public void Setup(string[] TipArray)
	{
		Tips = TipArray;
	}

    public void Setup(string[] TipArray, ElementHighlighter ElementHighlighter, List<GridPosition> gridPositions)
    {
        Tips = TipArray;
        GridPositions = gridPositions;
        Highlighter = ElementHighlighter;
    }

    #endregion

    public void Toggle()
	{
		IsVisible = !IsVisible;

        HelpmodeIndex = 0;

		UpdateView();
    }

	public void NextTip()
	{
		HelpmodeIndex++;

		UpdateView();
	}

	private void UpdateView()
	{
        if (Tips == null)
        {
            throw new ArgumentNullException(nameof(Tips), "Tips is null. Remember to setup HelpMode before using it");
        }
        
        // If all tips have been read
        if (HelpmodeIndex == Tips.Length)
        {
            HelpmodeIndex = 0;
            IsVisible = false;

            if (Highlighter != null)
            {
                Highlighter.IsVisible = false;
            }
        }
        else
        {
            HelpmodeTipLabel.Text = Tips[HelpmodeIndex];
            IsVisible = true;

            if (Highlighter == null) { return; }

            if (GridPositions == null)
            {
                throw new ArgumentNullException(nameof(GridPositions), "GridPositions is null. Consider providing a List of GridPositions or removing the ElementHighlighter from setup");
            }

            Highlighter.IsVisible = true;
            Highlighter.Highlight(GridPositions[HelpmodeIndex]);
        }
    }
}
