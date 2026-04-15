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
    BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(HelpMode), 
                            defaultValue: Colors.Black, propertyChanged: OnTextColorPropertyChanged);
    
    private static void OnTextColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var Text = (HelpMode)bindable;
        Text.TextColor = (Color)newValue;
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
