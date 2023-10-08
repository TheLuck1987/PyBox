using Microsoft.AspNetCore.Components;
using PyBox.Shared.Models.Script;
using PyBox.Shared.Services.Classes;

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
    }
}
