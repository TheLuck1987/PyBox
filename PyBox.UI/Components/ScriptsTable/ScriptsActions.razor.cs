using Microsoft.AspNetCore.Components;
using PyBox.Shared.Models.Script;

namespace PyBox.UI.Components.ScriptsTable
{
    public partial class ScriptsActions
    {
        #region Inputs
        [Parameter]
        public ScriptView? Item { get; set; }

        [Parameter]
        public bool ShowEditButton { get; set; } = true;

        [Parameter]
        public bool ShowSmallButtons { get; set; }

        [Parameter]
        public string CssClasses { get; set; } = "";
        #endregion

        #region Ouputs
        [Parameter]
        public EventCallback<ScriptView> OnStatusChange { get; set; }
        [Parameter]
        public EventCallback<int> OnEdit { get; set; }
        [Parameter]
        public EventCallback<ScriptView> OnDelete { get; set; }
        #endregion

        private string confirmId = $"modal-{Guid.NewGuid()}";
        private string confirmMessage => Item != null ? $"Are you sure you want to delete {Item.Title}?" : "Are you sure you want to delete this item?";

        private async Task OnDeleteRequest(ScriptView item)
        {
            // await ModalService.ShowModal(confirmId);
        }

        private async Task OnDeleteConfirmed(bool abort)
        {
            // await ModalService.HideModal(confirmId);
            // if (!abort)
            //     await OnDelete.InvokeAsync(Item);
        }
    }
}
