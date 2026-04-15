namespace HelpMode
{
    public class GridPosition
    {
        // By default Span is only 1 and sets both Column and Row to 0,0 (Top left of grid)
        public int Column = 0;
        public int ColumnSpan = 1;

        public int Row = 0;
        public int RowSpan = 1;

        // -1 means fill all the available space
        public double Width = -1;
        public double Height = -1;

        public LayoutOptions HorizontalOptions = LayoutOptions.Start;
        public LayoutOptions VerticalOptions = LayoutOptions.Start;
    }
}
