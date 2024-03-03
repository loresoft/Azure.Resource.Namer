using Blazored.Modal.Services;

using AzureNamer.Client.Extensions;
using AzureNamer.Client.Services;
using AzureNamer.Client.Stores;
using AzureNamer.Shared.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace AzureNamer.Client.Pages.Organizations.Components;

public partial class OrganizationContainer : IDisposable
{
    [Parameter]
    public int Id { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }


    [CascadingParameter]
    public IModalService Modal { get; set; }


    [Inject]
    public NotificationService NotificationService { get; set; }

    [Inject]
    public OrganizationStore OrganizationStore { get; set; }

    [Inject]
    public NavigationManager Navigation { get; set; }

    [Inject]
    public UserStore UserStore { get; set; }


    public OrganizationUpdateModel Organization => OrganizationStore?.Model;


    private EditContext EditContext { get; set; }

    protected bool IsOwner() => true; //Organization?.Members?.Any(m => m.Id == UserStore?.Model?.Id && m.IsOwner) == true;


    protected override async Task OnInitializedAsync()
    {
        OrganizationStore.OnChange += HandleModelChange;

        try
        {
            await OrganizationStore.Load(Id);
            if (OrganizationStore.Model == null)
                Navigation.NavigateTo("/organizations");

            EditContext = new EditContext(OrganizationStore.Model);
            EditContext.OnFieldChanged += HandleFormChange;
        }
        catch (Exception ex)
        {
            NotificationService.ShowError(ex);
        }
    }

    protected virtual async Task HandleSave()
    {
        try
        {
            await OrganizationStore.Save();

            var model = OrganizationStore.Model;

            NotificationService.ShowSuccess($"Organization '{model.Name}' saved successfully");
        }
        catch (Exception ex)
        {
            NotificationService.ShowError(ex);
        }
    }

    protected virtual async Task HandleDelete()
    {
        try
        {
            var name = OrganizationStore.Model.Name;

            if (!await Modal.ConfirmDelete($"Are you sure you want to delete organization '{name}'?"))
                return;


            await OrganizationStore.Delete(Id);
            NotificationService.ShowSuccess($"Organization '{name}' deleted successfully");
            Navigation.NavigateTo("/organizations");
        }
        catch (Exception ex)
        {
            NotificationService.ShowError(ex);
        }
    }

    private void HandleModelChange()
    {
        InvokeAsync(() => StateHasChanged());
    }

    private void HandleFormChange(object sender, FieldChangedEventArgs args)
    {
        OrganizationStore.NotifyStateChanged();
    }

    public void Dispose()
    {
        OrganizationStore.OnChange -= HandleModelChange;
        EditContext.OnFieldChanged -= HandleFormChange;
    }

}
