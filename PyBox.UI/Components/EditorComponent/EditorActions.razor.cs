using Microsoft.AspNetCore.Components;
using PyBox.Shared.Models.Script;
using PyBox.Shared.Services.Interfaces;
using PyBox.UI.Components.Modals;

namespace PyBox.UI.Components.EditorComponent
{
    public partial class EditorActions
    {
        #region Inputs
        [Parameter]
        public ScriptEdit? Item { get; set; }
        [Parameter]
        public bool ShowSmallButtons { get; set; }
        #endregion

        #region Ouputs
        [Parameter]
        public EventCallback<ScriptEdit> OnStatusChange { get; set; }
        [Parameter]
        public EventCallback<ScriptEdit> OnItemDeleting { get; set; }
        [Parameter]
        public EventCallback<ScriptEdit> OnDeleted { get; set; }
        #endregion

        private bool sowDeleteConfirm = false;
        private string confirmMessage => Item != null ? $"Are you sure you want to delete {Item.Title}?" : "Are you sure you want to delete this item?";

        private bool disabled = false;
        private bool onDeleting = false;

        private async Task confirmRequestClosed(ModalResult result)
        {
            sowDeleteConfirm = false;
            if (!result.CancelClicked)
            {
                disabled = true;
                onDeleting = true;
                await OnItemDeleting.InvokeAsync(Item);
                int id = Item!.ScriptId;
                IScriptDataService _dataService = DataService;
                await Task.Run(async () =>
                {
                    await _dataService.DeleteScript(id);
                    await InvokeAsync(new Action(itemDeleted));
                }).ConfigureAwait(false);
            }
        }
        private void itemDeleted()
        {
            disabled = false;
            onDeleting = false;
            StateHasChanged();
            OnDeleted.InvokeAsync(Item);
        }
        private void OnDeleteRequest()
        {
            sowDeleteConfirm = true;
        }
    }
}
