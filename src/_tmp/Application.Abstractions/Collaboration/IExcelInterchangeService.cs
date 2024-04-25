namespace BridgingIT.DevKit.Application.Collaboration;

using System.Data;
using System.IO;
using BridgingIT.DevKit.Common;

public interface IExcelInterchangeService
{
    Task<string> ExportAsync<TData>(IEnumerable<TData> data,
        Dictionary<string, Func<TData, object>> mappers,
        ExcelInterchangeServiceOptions options = null);

    Task<IResult<IEnumerable<TEntity>>> ImportAsync<TEntity>(Stream data,
        Dictionary<string, Func<DataRow, TEntity, object>> mappers,
        ExcelInterchangeServiceOptions options);
}