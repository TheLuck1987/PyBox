using Microsoft.AspNetCore.Components;
using PyBox.Shared.Enums;
using PyBox.Shared.Models.Script;
using PyBox.Shared.Services.Classes;
using PyBox.Shared.Services.Interfaces;
using PyBox.UI.Components.Modals;

namespace PyBox.UI.Components.ScriptsTable
{
    public partial class ScriptsTable
    {
        #region Ouputs
        [Parameter]
        public EventCallback<ScriptDataServiceResponse> OnError { get; set; }
        #endregion
        private List<ScriptView>? items { get; set; }
        private ScriptDefinition? newItem = null;
        private bool showNewScriptModal = false;
        private string? modalErrorClass = null;
        private string? modalErrorMessage = null;

        private ScriptView? itemOnDeleting = null;

        protected override async Task OnInitializedAsync()
        {
            var ok = await getScripts();
            if (!ok)
                return;
            await base.OnInitializedAsync();
        }

        private void onCreateRequest()
        {
            newItem = new();
            showNewScriptModal = true;
        }

        private async Task onCreateEnd(ModalResult result)
        {
            showNewScriptModal = false;
            if (result.CancelClicked)
            {
                newItem = null;
                modalErrorClass = null;
                modalErrorMessage = null;
                return;
            }
            if (result.Result == null)
            {
                modalErrorClass = "danger";
                modalErrorMessage = "Unhandled error";
            }
            else
            {
                newItem = (ScriptDefinition)result.Result;
                var data = await DataService.CheckTitle(new ScriptCheckTitleUnique() { Title = newItem!.Title!, Id = null });
                data = await checkData(data);
                if (data == null)
                    return;
                if ((bool)data.Result!)
                {
                    modalErrorClass = "warning";
                    modalErrorMessage = "This title already exists!";
                    showNewScriptModal = true;
                    return;
                }
                data = await DataService.CreateScript(newItem);
                data = await checkData(data, true);
                if (data == null)
                    return;
                if (data.ErrorLevel != WarningLevel.NO_WARNING || data.Result == null)
                {
                    modalErrorClass = data.ErrorLevel != WarningLevel.WARNING || data.Errors == null ? "danger" : "warning";
                    modalErrorMessage = data.Errors == null ? "Unhandled error" : data.Errors;
                    showNewScriptModal = true;
                    return;
                }
                else
                {
                    var item = (ScriptEdit)data.Result!;
                    UriHelper.NavigateTo($"/edit/{item.ScriptId}");
                }
            }
        }
        private async Task<bool> getScripts()
        {
            var data = await DataService.GetScripts();
            data = await checkData(data);
            if (data == null)
                return false;
            items = (List<ScriptView>?)data.Result;
            return true;
        }
        private async Task onItemStatusChange(ScriptView item)
        {
            var data = await DataService.ToggleStatus(item.ScriptId);
            data = await checkData(data);
            if (data == null)
                return;
            item.Enabled = !item.Enabled;
        }
        private void onItemEdit(int id) => UriHelper.NavigateTo($"/edit/{id}");
        private void onItemDeleting(ScriptView item) => itemOnDeleting = item;
        private void onItemDeleted(ScriptView item)
        {
            itemOnDeleting = null;
            items!.Remove(item);
        }
        private async Task<ScriptDataServiceResponse?> checkData(IScriptDataServiceResponse? data, bool internalError = false)
        {
            if (data == null || data.Result == null || data.ErrorLevel != WarningLevel.NO_WARNING)
            {
                string message = data == null || data.Errors == null ? "Unhandled exception" : data.Errors;
                WarningLevel level = data == null || data.ErrorLevel == WarningLevel.NO_WARNING ? WarningLevel.ERROR : data.ErrorLevel;
                var result = new ScriptDataServiceResponse() { Result = null, ErrorLevel = level, Errors = message };
                if (internalError && level != WarningLevel.ERROR)
                    return result;
                await OnError.InvokeAsync(result);
                return null;
            }
            return (ScriptDataServiceResponse)data;
        }
    }
}
