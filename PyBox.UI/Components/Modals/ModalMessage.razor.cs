using Microsoft.AspNetCore.Components;

namespace PyBox.UI.Components.Modals
{
    public partial class ModalMessage
    {
        #region Inputs
        [Parameter]
        public string? Title { get; set; }
        [Parameter]
        public string? Message { get; set; }
        #endregion

        #region Outputs
        [Parameter]
        public EventCallback<ModalResult> OnExit { get; set; }
        #endregion

        private string[] messages = new string[0];

        protected override async Task OnInitializedAsync()
        {
            if (Message != null)
                messages = Message.Split('\n');
            await base.OnInitializedAsync();
        }
    }
}
