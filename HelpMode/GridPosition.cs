namespace HelpMode;

public class GridPosition
{
    // By default Span is only 1 and sets both Column and Row to 0,0 (Top left of grid)
    public int Column { get; set; }
    public int ColumnSpan { get; set; } = 1;

    public int Row { get; set; }
    public int RowSpan { get; set; } = 1;

    // -1 means fill all the available space
    public double Width { get; set; } = -1;
    public double Height { get; set; } = -1;

    public LayoutOptions HorizontalOptions { get; set; } = LayoutOptions.Start;
    public LayoutOptions VerticalOptions { get; set; } = LayoutOptions.Start;
}
