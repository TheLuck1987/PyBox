using Microsoft.AspNetCore.Components;
using PyBox.Shared.Enums;
using PyBox.Shared.Models.Script;
using PyBox.UI.Components.Modals;

namespace PyBox.UI.Components.ScriptsTable
{
    public partial class ScriptsTable
    {
        #region Inputs
        [Parameter]
        public List<ScriptView>? Items { get; set; }
        #endregion

        private ScriptDefinition? newItem = null;
        private bool showNewScriptModal = false;
        private string? modalErrorClass = null;
        private string? modalErrorMessage = null;

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
                if (data.Result == null || ((bool)data.Result))
                {
                    modalErrorClass = "warning";
                    modalErrorMessage = "This title already exists!";
                    showNewScriptModal = true;
                    return;
                }
                data = await DataService.CreateScript(newItem);
                if (data.ErrorLevel != WarningLevel.NO_WARNING || data.Result == null)
                {
                    modalErrorClass = data.ErrorLevel == WarningLevel.WARNING ? "warning" : "danger";
                    modalErrorMessage = data.Errors == null ? "Unhandled error" : data.Errors;
                    showNewScriptModal = true;
                    return;
                }
                else
                {
                    var item = (ScriptEdit)data.Result;
                    //Change with redirect to edit page
                    Items.Add(new ScriptView() { CreatedAt = DateTime.Now, Description = item.Description, Enabled = item.Enabled, ScriptId = item.ScriptId, ScriptText = item.ScriptText, Title = item.Title, UpdatedAt = null });
                }
            }

        }
    }
}
