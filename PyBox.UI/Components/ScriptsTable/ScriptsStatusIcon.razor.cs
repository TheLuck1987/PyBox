using Microsoft.AspNetCore.Components;
using PyBox.Shared.Models.Script;

namespace PyBox.UI.Components.ScriptsTable
{
    public partial class ScriptsStatusIcon
    {
        #region Inputs
        [Parameter]
        public ScriptView? Item { get; set; }
        [Parameter]
        public bool Deleting { get; set; } = false;
        #endregion
    }
}
