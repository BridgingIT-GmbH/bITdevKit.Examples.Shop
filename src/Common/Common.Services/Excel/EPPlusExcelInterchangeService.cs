namespace Common.Services;

using System.Data;
using System.Drawing;
using System.IO;
using BridgingIT.DevKit.Application.Collaboration;
using BridgingIT.DevKit.Common;
using OfficeOpenXml;
using OfficeOpenXml.Style;

public class EPPlusExcelInterchangeService : IExcelInterchangeService // TODO: replace implementation https://github.com/blazorhero/CleanArchitecture/issues/330
                                                                      //       https://github.com/neozhu/RazorPageCleanArchitecture/blob/main/src/Infrastructure/Services/ExcelService.cs
{
    public async Task<string> ExportAsync<TData>(
        IEnumerable<TData> data,
        Dictionary<string, Func<TData, object>> mappers,
        ExcelInterchangeServiceOptions options = null)
    {
        options ??= new ExcelInterchangeServiceOptions();
        options.SheetName ??= "Sheet1";
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var p = new ExcelPackage();
        p.Workbook.Properties.Author = options.Author;
        p.Workbook.Worksheets.Add(options.SheetName);
        var ws = p.Workbook.Worksheets[0];
        //ws.Name = sheetName;
        ws.Cells.Style.Font.Size = options.CellFontSize;
        ws.Cells.Style.Font.Name = options.CellFontName;

        var colIndex = 1;
        var rowIndex = 1;
        var headers = mappers.Keys.Select(x => x).ToList();
        foreach (var header in headers)
        {
            var cell = ws.Cells[rowIndex, colIndex];
            var fill = cell.Style.Fill;
            fill.PatternType = ExcelFillStyle.Solid;
            fill.BackgroundColor.SetColor(Color.LightBlue);

            var border = cell.Style.Border;
            border.Bottom.Style =
                border.Top.Style =
                    border.Left.Style =
                        border.Right.Style = ExcelBorderStyle.Thin;

            cell.Value = header;

            colIndex++;
        }

        var dataList = data.ToList();
        foreach (var item in dataList)
        {
            colIndex = 1;
            rowIndex++;

            var result = headers.Select(header => mappers[header](item));
            foreach (var value in result)
            {
                ws.Cells[rowIndex, colIndex++].Value = value;
            }
        }

        using (var autoFilterCells = ws.Cells[1, 1, dataList.Count + 1, headers.Count])
        {
            autoFilterCells.AutoFilter = true;
            autoFilterCells.AutoFitColumns();
        }

        return Convert.ToBase64String(await p.GetAsByteArrayAsync());
    }

    public async Task<IResult<IEnumerable<TEntity>>> ImportAsync<TEntity>(
        Stream stream,
        Dictionary<string, Func<DataRow, TEntity, object>> mappers,
        ExcelInterchangeServiceOptions options)
    {
        options ??= new ExcelInterchangeServiceOptions();
        options.SheetName ??= "Sheet1";
        var result = new List<TEntity>();
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var p = new ExcelPackage();
        stream.Position = 0;
        await p.LoadAsync(stream);
        var ws = p.Workbook.Worksheets[options.SheetName];
        if (ws == null)
        {
            return Result<IEnumerable<TEntity>>.Failure(string.Format("Sheet with name {0} does not exist!", options.SheetName));
        }

        var dt = new DataTable();
        const bool titlesInFirstRow = true;
        foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
        {
            dt.Columns.Add(titlesInFirstRow ? firstRowCell.Text : $"Column {firstRowCell.Start.Column}");
        }

        const int startRow = titlesInFirstRow ? 2 : 1;
        var headers = mappers.Keys.Select(x => x).ToList();
        var errors = new List<string>();

        foreach (var header in headers)
        {
            if (!dt.Columns.Contains(header))
            {
                errors.Add(string.Format("Header '{0}' does not exist in table!", header));
            }
        }

        if (errors.Any())
        {
            return Result<IEnumerable<TEntity>>.Failure(errors);
        }

        for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
        {
            try
            {
                var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                var row = dt.Rows.Add();
                var item = (TEntity)Activator.CreateInstance(typeof(TEntity));
                foreach (var cell in wsRow)
                {
                    row[cell.Start.Column - 1] = cell.Text;
                }

                headers.ForEach(x => mappers[x](row, item));
                result.Add(item);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<TEntity>>.Failure(ex.Message);
            }
        }

        return Result<IEnumerable<TEntity>>.Success(result, "Import Success");
    }
}