namespace Common.Presentation.Web.Client.Components;

using BridgingIT.DevKit.Common;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

public partial class ImportExcelModal
{
    private FluentValidationValidator modelValidator;
    private IBrowserFile file;
    private bool uploading = false;
    private string dragEnterStyle;

    [CascadingParameter]
    public MudDialogInstance MudDialog { get; set; }

    [Parameter]
    public UploadRequest UploadRequest { get; set; } = new();

    [Parameter]
    public string ModelName { get; set; }

    [Parameter]
    public Func<UploadRequest, Task<Result<int>>> OnSaved { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await this.LoadDataAsync();
    }

    private async Task SaveAsync()
    {
        if (this.OnSaved != null)
        {
            this.uploading = true;
            var result = await this.OnSaved.Invoke(this.UploadRequest);
            if (result.IsSuccess)
            {
                this.snackbar.Add(result.Messages?.FirstOrDefault(), Severity.Success);
                this.MudDialog.Close();
            }
            else
            {
                foreach (var message in result.Messages.SafeNull())
                {
                    this.snackbar.Add(message, Severity.Error);
                }
            }

            this.uploading = false;
        }
        else
        {
            this.MudDialog.Close();
        }

        await Task.CompletedTask;
    }

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        this.file = e.File;
        if (this.file != null)
        {
            var buffer = new byte[this.file.Size];
            var extension = Path.GetExtension(this.file.Name);
            await this.file.OpenReadStream(this.file.Size).ReadAsync(buffer);
            this.UploadRequest = new UploadRequest
            {
                Data = buffer,
                FileName = this.file.Name,
                //UploadType = Application.Enums.UploadType.Document,
                Extension = extension
            };
        }
    }

    private async Task LoadDataAsync()
    {
        await Task.CompletedTask;
    }

    private void Cancel() => this.MudDialog.Cancel();
}

public class UploadRequest
{
    public string FileName { get; set; }

    public string Extension { get; set; }

    public string Type { get; set; }

    public byte[] Data { get; set; }
}