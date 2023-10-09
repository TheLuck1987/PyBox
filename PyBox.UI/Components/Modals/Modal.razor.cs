using Microsoft.AspNetCore.Components;

namespace PyBox.UI.Components.Modals
{
	public partial class Modal
	{
		#region Inputs
		[Parameter]
		public string? TitleIcon { get; set; } = null;
		[Parameter]
		public string? Title { get; set; }
		[Parameter]
		public RenderFragment? ModalBody { get; set; }
		[Parameter]
		public RenderFragment? FormValidator { get; set; } = null;
		[Parameter]
		public string CancellationLable { get; set; } = "Cancel";
		[Parameter]
		public string ConfirmLable { get; set; } = "Ok";
		[Parameter]
		public string ConfirmCssColor { get; set; } = "primary";
		[Parameter]
		public bool HideCancelButton { get; set; } = false;
		[Parameter]
		public bool ModalIsForm { get; set; } = false;
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

		private async Task Close(bool cancelClicked)
		{
			await OnExit.InvokeAsync(new() { CancelClicked = cancelClicked });
		}

		private async Task CloseForm(bool cancelClicked)
		{
			if (cancelClicked)
				FormObject = null;
			await OnExit.InvokeAsync(new() { CancelClicked = cancelClicked, Result = FormObject });
		}
	}
}
