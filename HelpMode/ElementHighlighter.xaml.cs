namespace HelpMode;

public partial class ElementHighlighter : ContentView
{
    private List<GridLength> Columns = [];
    private List<GridLength> Rows = [];

    private bool HasCalcedStarValue = false;
    private double StarHeight = 0;
    private double StarWidth = 0;

    public ElementHighlighter()
	{
		InitializeComponent();
    }

    public void Setup(ColumnDefinitionCollection GridColumns, RowDefinitionCollection GridRows)
    {
        // Parse and clean Columns and Rows to only save whats important (should in theory be better for memory usage)
        for (int i = 0; i < GridColumns.Count; i++)
        {
            Columns.Add(GridColumns[i].Width);
        }
        for (int i = 0;i < GridRows.Count; i++)
        {
            Rows.Add(GridRows[i].Height);
        }
    }

    private void CalcStarValues()
    {
        double PixelDensity = DeviceDisplay.MainDisplayInfo.Density;

        // Get viewport size
        double ViewportHeight = DeviceDisplay.MainDisplayInfo.Height / PixelDensity;
        double ViewportWidth = DeviceDisplay.MainDisplayInfo.Width / PixelDensity;

        // Declare vars before OS-specifics so it can always access them
        double Statusbarheight = 0;
        double Navbarheight = 0;

#if ANDROID
        // Get height of Statusbar
        int Statusid = Platform.CurrentActivity.ApplicationContext.Resources.GetIdentifier("status_bar_height", "dimen", "android");
        Statusbarheight = Platform.CurrentActivity.ApplicationContext.Resources.GetDimensionPixelSize(Statusid) / PixelDensity;
        // Get height of Navbar
        int Navid = Platform.CurrentActivity.ApplicationContext.Resources.GetIdentifier("navigation_bar_height", "dimen", "android");
        Navbarheight = Platform.CurrentActivity.ApplicationContext.Resources.GetDimensionPixelSize(Navid) / PixelDensity;
#endif

        // Then calc size of "interactable space"
        double MaxHeight = ViewportHeight - Statusbarheight - Navbarheight;
        double MaxWidth = ViewportWidth;

        // Then find height for one star, first by getting left-over height
        double AbsoluteValue = 0;
        int Stars = 0;
        for (int i = 0; i < Rows.Count; i++)
        {
            if (Rows[i].IsAbsolute)
            {
                AbsoluteValue += Rows[i].Value;
            }
            // Else, meaning if star
            else
            {
                Stars += (int)Rows[i].Value;
            }
        }

        // Then divide left-over height by amount of stars
        StarHeight = (MaxHeight - AbsoluteValue) / Stars;

        // Then find width for one star, first by getting left-over width
        AbsoluteValue = 0;
        Stars = 0;
        for (int i = 0; i < Columns.Count; i++)
        {
            if (Columns[i].IsAbsolute)
            {
                AbsoluteValue += Columns[i].Value;
            }
            // Else, meaning if star
            else
            {
                Stars += (int)Columns[i].Value;
            }
        }

        // Then divide left-over height by amount of stars
        StarWidth = (MaxWidth - AbsoluteValue) / Stars;
    }

    public void Highlight(GridPosition PositionToHighlight)
    {
        if (!HasCalcedStarValue)
        {
            CalcStarValues();
            HasCalcedStarValue = true;
        }

        var ReturnVals = CalcGridProperties(PositionToHighlight);

        MainGrid.ColumnDefinitions =
        [
            new ColumnDefinition { Width = ReturnVals.Item1[0] },
            new ColumnDefinition { Width = ReturnVals.Item1[1] },
            new ColumnDefinition { Width = ReturnVals.Item1[2] }
        ];

        MainGrid.RowDefinitions =
        [
            new RowDefinition { Height = ReturnVals.Item2[0] },
            new RowDefinition { Height = ReturnVals.Item2[1] },
            new RowDefinition { Height = ReturnVals.Item2[2] }
        ];
    }

    private (double[], double[]) CalcGridProperties(GridPosition Position)
    {
        // Calc each Row and Colum height
        double[] RowHeights = new double[3];
        double[] ColumnWidths = new double[3];

        // First Column and Row
        ColumnWidths[0] = CalcSize(0, Position.Column, Columns, StarWidth);
        RowHeights[0] = CalcSize(0, Position.Row, Rows, StarHeight);

        // Second Column and Row
        ColumnWidths[1] = CalcSize(Position.Column, Position.Column + Position.ColumnSpan, Columns, StarWidth);
        RowHeights[1] = CalcSize(Position.Row, Position.Row + Position.RowSpan, Rows, StarHeight);

        // Third Column and Row
        ColumnWidths[2] = CalcSize(Position.Column + Position.ColumnSpan, Columns.Count, Columns, StarWidth);
        RowHeights[2] = CalcSize(Position.Row + Position.RowSpan, Rows.Count, Rows, StarHeight);

        if (Position.Width > 0)
        {
            double LeftOverWidth = ColumnWidths[1] - Position.Width;

            if (Position.HorizontalOptions == HorizontalAlignment.LeftAligned)
            {
                ColumnWidths[2] += LeftOverWidth;
            }
            else if (Position.HorizontalOptions == HorizontalAlignment.CenterAligned)
            {
                ColumnWidths[0] += LeftOverWidth / 2;
                ColumnWidths[2] += LeftOverWidth / 2;
            }
            // Right-aligned
            else
            {
                ColumnWidths[0] += LeftOverWidth;
            }

            ColumnWidths[1] = Position.Width;
        }
        if (Position.Height > 0)
        {
            double LeftOverHeight = RowHeights[1] - Position.Height;

            if (Position.VerticalOptions == VerticalAlignment.TopAligned)
            {
                RowHeights[2] += LeftOverHeight;
            }
            else if (Position.VerticalOptions == VerticalAlignment.CenterAligned)
            {
                RowHeights[0] += LeftOverHeight / 2;
                RowHeights[2] += LeftOverHeight / 2;
            }
            // Bottom-aligned
            else
            {
                RowHeights[0] += LeftOverHeight;
            }

            RowHeights[1] = Position.Height;
        }

        return (ColumnWidths, RowHeights);
    }

    private static double CalcSize(int StartIndex,  int EndIndex, List<GridLength> List, double StarValue)
    {
        double Size = 0;

        for (int i = StartIndex; i < EndIndex; i++)
        {
            if (List[i].IsAbsolute)
            {
                Size += List[i].Value;
            }
            // Else, meaning if star
            else
            {
                Size += List[i].Value * StarValue;
            }
        }

        return Size;
    }
}
