﻿using Microsoft.AspNetCore.Components;
using PyBox.Shared.Enums;
using PyBox.Shared.Models.Script;
using PyBox.Shared.Services.Classes;
using PyBox.Shared.Services.Interfaces;
using PyBox.Shared.Services.Interops;
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
		private bool showInfoMessage = false;
		private readonly string infoMessage = "*The maximum script execution time cannot exceed 10 seconds. Otherwise you will run into a TimeOut error\n\n" +
			"*You cannot import potentially harmful modules for the system (os, sys, cmd...) Otherwise you will encounter a Security error\n\n" +
			"*The parameters will be accessible from the script by calling the \"args[<index>]\" array.\n" +
			"PS do not use the \"args\" variable for anything else unless you want to override the input parameters\n" +
			"Example:\n\n" +
			"print(f'Hello {args[0]}')";
		private bool showDisabledMessage = false;
		private readonly string disabledMessage = "Execution of this script is disabled\n\nEnable the script from the main list to be able to run it";

		private string executionResult = "";
		private string executionClass = "";
		private bool isRunning = false;

		protected override void OnInitialized()
		{
			scriptText = Item?.ScriptText;
			base.OnInitialized();
		}

		private void editTitle()
		{
			if (isRunning)
				return;
			editItem = new() { Description = Item!.Description, ScriptId = Item.ScriptId, Enabled = Item.Enabled, ScriptText = Item.ScriptText, Title = Item.Title };
			showEditTitleTitle = $"Rename {Item!.Title}";
			showEditTitle = true;
		}

		private void editDescription()
		{
			if (isRunning)
				return;
			editItem = new() { Description = Item!.Description, ScriptId = Item.ScriptId, Enabled = Item.Enabled, ScriptText = Item.ScriptText, Title = Item.Title };
			showEditDescriptionTitle = $"Edit {Item!.Title} description";
			showEditDescription = true;
		}

		private void showInfo() => showInfoMessage = !showInfoMessage;
		private void showDisabled() => showDisabledMessage = !showDisabledMessage;

		private async void saveScript(bool runIt)
		{
			executionClass = "info";
			executionResult = "Saving...";
			StateHasChanged();
			Item!.ScriptText = scriptText;
			var data = await DataService.UpdateScript(Item!.ScriptId, Item);
			data = await checkData(data);
			if (data == null)
			{
				editItem = null;
				executionResult = "";
				StateHasChanged();
				return;
			}
			else if (data.ErrorLevel != WarningLevel.NO_WARNING)
			{
				await OnError.InvokeAsync((ScriptDataServiceResponse)data);
			}
			executionResult = "";
			StateHasChanged();
			if (runIt)
				await runScript();
		}

		private async Task runScript()
		{
			isRunning = true;
			executionClass = "info";
			executionResult = "Running...";
			StateHasChanged();
			if (!Item!.Enabled)
			{
				isRunning = false;
				showDisabled();
				executionResult = "";
				StateHasChanged();
				return;
			}
			var data = await DataService.RunScript(Item.ScriptId, parameters == null ? "" : parameters);
			data = await checkData(data, true);
			if (data == null)
				return;
			if (data.Errors != null && data.Errors == "Script is disabled")
			{
				isRunning = false;
				showDisabled();
				return;
			}
			if (data.Errors != null && data.Errors == "Script is empty")
			{
				isRunning = false;
				executionClass = "warning";
				executionResult = data.Errors;
				StateHasChanged();
				return;
			}
			if (data.Errors != null && data.Errors.StartsWith("Security error"))
			{
				isRunning = false;
				executionClass = "danger";
				executionResult = data.Errors;
				StateHasChanged();
				return;
			}
			var result = (InteropsResult)data.Result!;
			if (!string.IsNullOrWhiteSpace(result.Error))
			{
				isRunning = false;
				executionClass = "danger";
				executionResult = result.Error;
				StateHasChanged();
			}
			else if (result.Result != null)
			{
				isRunning = false;
				executionClass = "light";
				executionResult = result.Result;
				StateHasChanged();
			}
		}

		private void goBack()
		{
			if (scriptText != Item!.ScriptText)
			{
				showConfirm = true;
				return;
			}
			UriHelper.NavigateTo("/");
		}

		private void goBackConfirmed(ModalResult result)
		{
			showConfirm = false;
			if (!result.CancelClicked)
				UriHelper.NavigateTo("/");
		}

		private void scriptChanged(string? text) => scriptText = text;
		private void scriptIsEquals() => scriptText = Item!.ScriptText;

		private async Task onEditTitleEnd(ModalResult result)
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
				var data = await DataService.CheckTitle(new ScriptCheckTitleUnique() { Title = editItem!.Title!, Id = editItem!.ScriptId });
				data = await checkData(data);
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
				data = await checkData(data);
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
		private async Task onEditDescriptionEnd(ModalResult result)
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
				data = await checkData(data);
				if (data == null)
				{
					editItem = null;
					return;
				}
				Item = editItem;
				editItem = null;
			}
		}
		private async Task<ScriptDataServiceResponse?> checkData(IScriptDataServiceResponse? data, bool internalError = false)
		{
			if (data == null || data.Result == null || data.ErrorLevel != WarningLevel.NO_WARNING)
			{
				string message = data == null || data.Errors == null ? "Unhandled exception" : data.Errors;
				WarningLevel level = data == null || data.ErrorLevel == WarningLevel.NO_WARNING ? WarningLevel.ERROR : data.ErrorLevel;
				var result = new ScriptDataServiceResponse() { Result = null, ErrorLevel = level, Errors = message };
				if (internalError && level != WarningLevel.ERROR)
					return result;
				await OnError.InvokeAsync(result);
				return null;
			}
			return (ScriptDataServiceResponse)data;
		}

	}
}
