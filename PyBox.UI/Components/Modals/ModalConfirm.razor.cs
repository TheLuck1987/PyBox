using Microsoft.AspNetCore.Components;

namespace PyBox.UI.Components.Modals
{
	public partial class ModalConfirm
	{
		#region Inputs
		[Parameter]
		public string? TitleIcon { get; set; } = null;
		[Parameter]
		public string? Title { get; set; }
		[Parameter]
		public string? Message { get; set; }
		[Parameter]
		public string ConfirmLable { get; set; } = "Ok";
		[Parameter]
		public string ButtonColor { get; set; } = "primary";
		#endregion

		#region Outputs
		[Parameter]
		public EventCallback<ModalResult> OnExit { get; set; }
		#endregion

		private string[] messages = Array.Empty<string>();

		protected override async Task OnInitializedAsync()
		{
			if (Message != null)
				messages = Message.Split('\n');
			await base.OnInitializedAsync();
		}
	}
}
