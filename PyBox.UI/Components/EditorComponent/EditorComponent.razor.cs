using Microsoft.AspNetCore.Components;
using PyBox.Shared.Enums;
using PyBox.Shared.Models.Script;
using PyBox.Shared.Services.Classes;
using PyBox.Shared.Services.Interfaces;
using PyBox.UI.Components.Modals;

namespace PyBox.UI.Components.EditorComponent
{
	public partial class EditorComponent
	{
		#region Inputs
		[Parameter]
		public ScriptEdit? Item { get; set; }
		#endregion

		#region Ouputs
		[Parameter]
		public EventCallback<ScriptDataServiceResponse> OnError { get; set; }
		#endregion

		private string parameters = "";
		private ScriptEdit? editItem = null;
		private string? scriptText = null;

		private bool showEditTitle = false;
		private string? showEditTitleTitle = null;
		private string? titleErrorClass = null;
		private string? titleErrorMessage = null;

		private bool showEditDescription = false;
		private string? showEditDescriptionTitle = null;
		private string? descriptionErrorClass = null;
		private string? descriptionErrorMessage = null;

		private bool showConfirm = false;

		protected override void OnInitialized()
		{
			scriptText = Item?.ScriptText;
			base.OnInitialized();
		}

		private void editTitle()
		{
			editItem = new() { Description = Item.Description, ScriptId = Item.ScriptId, Enabled = Item.Enabled, ScriptText = Item.ScriptText, Title = Item.Title };
			showEditTitleTitle = $"Rename {Item!.Title}";
			showEditTitle = true;
		}
		private void editDescription()
		{
			editItem = new() { Description = Item.Description, ScriptId = Item.ScriptId, Enabled = Item.Enabled, ScriptText = Item.ScriptText, Title = Item.Title };
			showEditDescriptionTitle = $"Edit {Item!.Title} description";
			showEditDescription = true;
		}
		private async void saveScript()
		{
			Item.ScriptText = scriptText;
			var data = await DataService.UpdateScript(Item!.ScriptId, Item);
			data = await CheckData(data);
			if (data == null)
			{
				editItem = null;
				return;
			}
			else if (data.ErrorLevel != WarningLevel.NO_WARNING)
			{
				await OnError.InvokeAsync((ScriptDataServiceResponse)data);
			}
		}

		private async Task goBack()
		{
			if (scriptText != Item.ScriptText)
			{
				showConfirm = true;
				return;
			}
			UriHelper.NavigateTo("/");
		}

		private async Task goBackConfirmed(ModalResult result)
		{
			showConfirm = false;
			if (!result.CancelClicked)
				UriHelper.NavigateTo("/");
		}

		private async Task scriptChanged(string? text)
		{
			scriptText = text;
		}

		private async Task OnEditTitleEnd(ModalResult result)
		{
			showEditTitle = false;
			if (result.CancelClicked)
			{
				editItem = null;
				titleErrorClass = null;
				titleErrorMessage = null;
				return;
			}
			if (result.Result == null)
			{
				titleErrorClass = "danger";
				titleErrorMessage = "Unhandled error";
			}
			else
			{
				editItem = (ScriptEdit)result.Result;
				var data = await DataService.CheckTitle(editItem!.Title!, editItem!.ScriptId);
				data = await CheckData(data);
				if (data == null)
				{
					editItem = null;
					return;
				}
				if ((bool)data.Result!)
				{
					titleErrorClass = "warning";
					titleErrorMessage = "This title already exists!";
					showEditTitle = true;
					return;
				}
				data = await DataService.UpdateScript(Item!.ScriptId, editItem);
				data = await CheckData(data);
				if (data == null)
				{
					editItem = null;
					return;
				}
				Item = editItem;
				editItem = null;
				titleErrorClass = null;
				titleErrorMessage = null;
			}
		}
		private async Task OnEditDescriptionEnd(ModalResult result)
		{
			showEditDescription = false;
			if (result.CancelClicked)
			{
				editItem = null;
				descriptionErrorClass = null;
				descriptionErrorMessage = null;
				return;
			}
			if (result.Result == null)
			{
				descriptionErrorClass = "danger";
				descriptionErrorMessage = "Unhandled error";
			}
			else
			{
				editItem = (ScriptEdit)result.Result;
				var data = await DataService.UpdateScript(Item!.ScriptId, editItem);
				data = await CheckData(data);
				if (data == null)
				{
					editItem = null;
					return;
				}
				Item = editItem;
				editItem = null;
			}
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
