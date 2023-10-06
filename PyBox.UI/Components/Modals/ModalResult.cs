using PyBox.UI.Components.Modals.Interfaces;

namespace PyBox.UI.Components.Modals
{
    public class ModalResult : IModalResult
    {
        public bool CancelClicked { get; set; }
        public object? Result { get; set; } = null;
    }
}
