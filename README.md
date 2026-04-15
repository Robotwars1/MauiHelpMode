# MauiHelpMode
MauiHelpMode is a .Net Maui package that adds an ElementHighlighter and the HelpMode itself. The ElementHighlighter is designed to be integrated into the HelpMode but can also be used on its own.

## Supported platforms
| Platform | Supported          |
| -------- |:------------------:|
| Windows  | :white_check_mark: |
| Android  | :white_check_mark: |
| iOS      | :x:                |
| macOS    | :x:                |
| Tizen    | :x:                |

## ElementHighlighter


## HelpMode


## GridPosition
To assign which part of the screen to highlight the program uses a custom GridPosition class similar to how you assign grid positioning in Maui.

```c#
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
```
