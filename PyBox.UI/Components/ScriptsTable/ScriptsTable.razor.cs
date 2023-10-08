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
            var ok = await GetScripts();
            if (!ok)
                return;
            await base.OnInitializedAsync();
        }

        public void OnCreateRequest()
        {
            newItem = new();
            showNewScriptModal = true;
        }

        public async Task OnCreateEnd(ModalResult result)
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
                var data = await DataService.CheckTitle(newItem!.Title!);
                data = await CheckData(data);
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
                data = await CheckData(data);
                if (data == null)
                    return;
                var item = (ScriptEdit)data.Result!;
                //Change with redirect to edit page
                items!.Add(new ScriptView() { CreatedAt = DateTime.Now, Description = item.Description, Enabled = item.Enabled, ScriptId = item.ScriptId, ScriptText = item.ScriptText, Title = item.Title, UpdatedAt = null });

            }
        }
        private async Task<bool> GetScripts()
        {
            var data = await DataService.GetScripts();
            data = await CheckData(data);
            if (data == null)
                return false;
            items = (List<ScriptView>?)data.Result;
            return true;
        }
        private async Task OnItemStatusChange(ScriptView item)
        {
            var data = await DataService.ToggleStatus(item.ScriptId);
            data = await CheckData(data);
            if (data == null)
                return;
            item.Enabled = !item.Enabled;
        }
        public async Task OnItemEdit(int id)
        {
            UriHelper.NavigateTo($"/edit/{id}");
        }
        private void OnItemDeleting(ScriptView item)
        {
            itemOnDeleting = item;
        }
        private void OnItemDeleted(ScriptView item)
        {
            itemOnDeleting = null;
            items!.Remove(item);
        }
        private async Task<ScriptDataServiceResponse?> CheckData(IScriptDataServiceResponse? data)
        {
            if (data == null || data.Result == null || data.ErrorLevel != WarningLevel.NO_WARNING)
            {
                string message = data == null || data.Errors == null ? "Unhandled exception" : data.Errors;
                WarningLevel level = data == null || data.ErrorLevel == WarningLevel.NO_WARNING ? WarningLevel.ERROR : data.ErrorLevel;
                await OnError.InvokeAsync(new ScriptDataServiceResponse() { Result = null, ErrorLevel = level, Errors = message });
                return null;
            }
            return (ScriptDataServiceResponse)data;
        }
    }
}
