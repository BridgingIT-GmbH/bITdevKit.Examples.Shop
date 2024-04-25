namespace BridgingIT.DevKit.Application.Collaboration;

public class ExcelInterchangeServiceOptions
{
    public string Author { get; set; }

    public string SheetName { get; set; } = "Sheet1";

    public int CellFontSize { get; set; } = 11;

    public string CellFontName { get; set; } = "Calibri";
}