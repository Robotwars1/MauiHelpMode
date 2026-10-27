namespace HelpMode;

public partial class ElementHighlighter : ContentView
{
    private readonly List<GridLength> _columns = [];
    private readonly List<GridLength> _rows = [];

    private bool _hasCalcedStarValue;
    private double _starHeight;
    private double _starWidth;

    public ElementHighlighter()
    {
        InitializeComponent();
    }

    public void Setup(ColumnDefinitionCollection gridColumns, RowDefinitionCollection gridRows)
    {
        // Parse and clean _columns and _rows to only save whats important (should in theory be better for memory usage)
        for (int i = 0; i < gridColumns.Count; i++)
        {
            _columns.Add(gridColumns[i].Width);
        }
        for (int i = 0; i < gridRows.Count; i++)
        {
            _rows.Add(gridRows[i].Height);
        }
    }

    private void CalcStarValues()
    {
#if ANDROID
        if (Platform.CurrentActivity == null) { return; }
        if (Platform.CurrentActivity.ApplicationContext == null) { return; }
        if (Platform.CurrentActivity.ApplicationContext.Resources == null) { return; }
#endif

        double pixelDensity = DeviceDisplay.MainDisplayInfo.Density;

        // Get viewport size
        double viewportHeight = DeviceDisplay.MainDisplayInfo.Height / pixelDensity;
        double viewportWidth = DeviceDisplay.MainDisplayInfo.Width / pixelDensity;

        // Declare vars before OS-specifics so it can always access them
        double statusBarHeight = 0;
        double navBarHeight = 0;

#if ANDROID
        // Get height of Statusbar
        int statusID = Platform.CurrentActivity.ApplicationContext.Resources.GetIdentifier("status_bar_height", "dimen", "android");
        statusBarHeight = Platform.CurrentActivity.ApplicationContext.Resources.GetDimensionPixelSize(statusID) / pixelDensity;
        // Get height of Navbar
        int navID = Platform.CurrentActivity.ApplicationContext.Resources.GetIdentifier("navigation_bar_height", "dimen", "android");
        navBarHeight = Platform.CurrentActivity.ApplicationContext.Resources.GetDimensionPixelSize(navID) / pixelDensity;
#endif

        // Then calc size of "interactable space"
        double maxHeight = viewportHeight - statusBarHeight - navBarHeight;
        double maxWidth = viewportWidth;

        // Then find height for one star, first by getting left-over height
        double absoluteValue = 0;
        int stars = 0;
        for (int i = 0; i < _rows.Count; i++)
        {
            if (_rows[i].IsAbsolute)
            {
                absoluteValue += _rows[i].Value;
            }
            // Else, meaning if star
            else
            {
                stars += (int)_rows[i].Value;
            }
        }

        // Then divide left-over height by amount of stars
        _starHeight = (maxHeight - absoluteValue) / stars;

        // Then find width for one star, first by getting left-over width
        absoluteValue = 0;
        stars = 0;
        for (int i = 0; i < _columns.Count; i++)
        {
            if (_columns[i].IsAbsolute)
            {
                absoluteValue += _columns[i].Value;
            }
            // Else, meaning if star
            else
            {
                stars += (int)_columns[i].Value;
            }
        }

        // Then divide left-over height by amount of stars
        _starWidth = (maxWidth - absoluteValue) / stars;

        _hasCalcedStarValue = true;
    }

    public void Highlight(GridPosition positionToHighlight)
    {
        if (!_hasCalcedStarValue)
        {
            CalcStarValues();
        }

        (double[], double[]) returnVals = CalcGridProperties(positionToHighlight);

        MainGrid.ColumnDefinitions =
        [
            new ColumnDefinition { Width = returnVals.Item1[0] },
            new ColumnDefinition { Width = returnVals.Item1[1] },
            new ColumnDefinition { Width = returnVals.Item1[2] }
        ];

        MainGrid.RowDefinitions =
        [
            new RowDefinition { Height = returnVals.Item2[0] },
            new RowDefinition { Height = returnVals.Item2[1] },
            new RowDefinition { Height = returnVals.Item2[2] }
        ];
    }

    private (double[], double[]) CalcGridProperties(GridPosition position)
    {
        // Calc each Row and Column height
        double[] rowHeights = new double[3];
        double[] columnWidths = new double[3];

        // First Column and Row
        columnWidths[0] = CalcSize(0, position.Column, _columns, _starWidth);
        rowHeights[0] = CalcSize(0, position.Row, _rows, _starHeight);

        // Second Column and Row
        columnWidths[1] = CalcSize(position.Column, position.Column + position.ColumnSpan, _columns, _starWidth);
        rowHeights[1] = CalcSize(position.Row, position.Row + position.RowSpan, _rows, _starHeight);

        // Third Column and Row
        columnWidths[2] = CalcSize(position.Column + position.ColumnSpan, _columns.Count, _columns, _starWidth);
        rowHeights[2] = CalcSize(position.Row + position.RowSpan, _rows.Count, _rows, _starHeight);

        if (position.Width > 0)
        {
            double leftOverWidth = columnWidths[1] - position.Width;

            if (position.HorizontalOptions == LayoutOptions.Start)
            {
                columnWidths[2] += leftOverWidth;
            }
            else if (position.HorizontalOptions == LayoutOptions.Center)
            {
                columnWidths[0] += leftOverWidth / 2;
                columnWidths[2] += leftOverWidth / 2;
            }
            // Right-aligned
            else
            {
                columnWidths[0] += leftOverWidth;
            }

            columnWidths[1] = position.Width;
        }
        if (position.Height > 0)
        {
            double leftOverHeight = rowHeights[1] - position.Height;

            if (position.VerticalOptions == LayoutOptions.Start)
            {
                rowHeights[2] += leftOverHeight;
            }
            else if (position.VerticalOptions == LayoutOptions.Center)
            {
                rowHeights[0] += leftOverHeight / 2;
                rowHeights[2] += leftOverHeight / 2;
            }
            // Bottom-aligned
            else
            {
                rowHeights[0] += leftOverHeight;
            }

            rowHeights[1] = position.Height;
        }

        return (columnWidths, rowHeights);
    }

    private static double CalcSize(int startIndex, int endIndex, List<GridLength> list, double starValue)
    {
        double size = 0;

        for (int i = startIndex; i < endIndex; i++)
        {
            if (list[i].IsAbsolute)
            {
                size += list[i].Value;
            }
            // Else, meaning if star
            else
            {
                size += list[i].Value * starValue;
            }
        }

        return size;
    }
}
