using Microsoft.AspNetCore.Components;
using PyBox.Shared.Enums;
using PyBox.Shared.Models.Script;
using PyBox.Shared.Services.Classes;
using PyBox.Shared.Services.Interfaces;
using PyBox.UI.Components.Modals;

namespace PyBox.UI.Components.ScriptsTable
{
    public partial class ScriptsActions
    {
        #region Inputs
        [Parameter]
        public ScriptView? Item { get; set; }
        [Parameter]
        public bool ShowSmallButtons { get; set; }
        #endregion

        #region Ouputs
        [Parameter]
        public EventCallback<ScriptView> OnStatusChange { get; set; }
        [Parameter]
        public EventCallback<int> OnEdit { get; set; }
        [Parameter]
        public EventCallback<ScriptView> OnItemDeleting { get; set; }
        [Parameter]
        public EventCallback<ScriptView> OnDeleted { get; set; }
        [Parameter]
        public EventCallback<ScriptDataServiceResponse> OnError { get; set; }
        #endregion

        private bool sowDeleteConfirm = false;
        private string confirmMessage => Item != null ? $"Are you sure you want to delete {Item.Title}?" : "Are you sure you want to delete this item?";

        private bool disabled = false;
        private bool onDeleting = false;
        private ScriptDataServiceResponse? errorResponse;

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
                    var data = await _dataService.DeleteScript(id);
                    if (data == null || data.Result == null || data.ErrorLevel != WarningLevel.NO_WARNING)
                    {
                        string message = data == null || data.Errors == null ? "Unhandled exception" : data.Errors;
                        WarningLevel level = data == null || data.ErrorLevel == WarningLevel.NO_WARNING ? WarningLevel.ERROR : data.ErrorLevel;
                        var result = new ScriptDataServiceResponse() { Result = null, ErrorLevel = level, Errors = message };
                        await InvokeAsync(() => errorResponse = result);
                        await InvokeAsync(onError);
                    }
                    else if (!(bool)data.Result!)
                    {
                        var result = new ScriptDataServiceResponse() { Result = null, ErrorLevel = WarningLevel.ERROR, Errors = "Unhandled exception" };
                        await InvokeAsync(() => errorResponse = result);
                        await InvokeAsync(onError);
                    }
                    else
                        await InvokeAsync(new Action(itemDeleted));
                }).ConfigureAwait(false);
            }
        }
        private async Task onError()
        {
            await OnError.InvokeAsync(errorResponse);
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
