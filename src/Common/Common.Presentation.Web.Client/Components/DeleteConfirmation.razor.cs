namespace Common.Presentation.Web.Client.Components;

using Microsoft.AspNetCore.Components;
using MudBlazor;

public partial class DeleteConfirmation
{
    [CascadingParameter]
    public MudDialogInstance MudDialog { get; set; }

    [Parameter]
    public string ContentText { get; set; }

    private void Submit()
    {
        this.MudDialog.Close(DialogResult.Ok(true));
    }

    private void Cancel() => this.MudDialog.Cancel();
}
