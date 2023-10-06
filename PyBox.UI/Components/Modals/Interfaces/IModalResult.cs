namespace PyBox.UI.Components.Modals.Interfaces
{
    public interface IModalResult
    {
        public bool CancelClicked { get; set; }
        public object? Result { get; set; }
    }
}
