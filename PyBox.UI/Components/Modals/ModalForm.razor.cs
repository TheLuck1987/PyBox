using Microsoft.AspNetCore.Components;

namespace PyBox.UI.Components.Modals
{
    public partial class ModalForm
    {
        #region Inputs
        [Parameter]
        public string? Title { get; set; }
        [Parameter]
        public RenderFragment? FormContent { get; set; }
        [Parameter]
        public RenderFragment? FormValidator { get; set; }
        [Parameter]
        public string? ConfirmLable { get; set; }
        [Parameter]
        public string? ConfirmColor { get; set; } = "primary";
        [Parameter]
        public object? FormObject { get; set; } = null;
        [Parameter]
        public string? FormAlertClass { get; set; } = null;
        [Parameter]
        public string? FormAlertMessage { get; set; } = null;
        #endregion

        #region Outputs
        [Parameter]
        public EventCallback<ModalResult> OnExit { get; set; }
        #endregion

        private string? alertClass = null;
    }
}
