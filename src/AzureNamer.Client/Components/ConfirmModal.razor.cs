using Blazored.Modal;
using Blazored.Modal.Services;

using Microsoft.AspNetCore.Components;

namespace AzureNamer.Client.Components;

public partial class ConfirmModal
{
    [CascadingParameter]
    BlazoredModalInstance Modal { get; set; } = null!;

    [Parameter, EditorRequired]
    public required string Message { get; set; }

    [Parameter]
    public string ActionClass { get; set; } = "btn-danger";

    [Parameter]
    public string ActionName { get; set; } = "Delete";

    [Parameter]
    public string CancelName { get; set; } = "Cancel";


    private async Task Close() => await Modal.CloseAsync(ModalResult.Ok(true));

    private async Task Cancel() => await Modal.CancelAsync();
}
