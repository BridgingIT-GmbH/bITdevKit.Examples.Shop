namespace Modules.Catalog.Presentation.Web;

public class UploadRequestModel
{
    public string FileName { get; set; }

    public string Extension { get; set; }

    public string Type { get; set; }

    public byte[] Data { get; set; }
}
