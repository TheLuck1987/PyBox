using Microsoft.AspNetCore.Mvc;
using PyBox.PyRunner;
using PyBox.Shared.Services.Classes;
using PyBox.Shared.Services.Interops;

namespace PyBox.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ConfigurationsController : ControllerBase
    {
        private readonly ILogger<ScriptsController> _logger;

        public ConfigurationsController(ILogger<ScriptsController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<InteropsResult>> Check()
        {
            try
            {
                var result = await DependencyInstaller.Check();
                if (result == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Unable to check dependencies");
                if (!string.IsNullOrEmpty(result.Error) || result.NotInstalled)
                    return BadRequest(result.Error);
                if (string.IsNullOrEmpty(result.Result))
                    return StatusCode(StatusCodes.Status500InternalServerError, "Unable to check dependencies");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR FROM: ScriptsController/GetScripts() => {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error has occurred");
            }
        }

        [HttpGet("install")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ScriptDataServiceResponse>> Install()
        {
            try
            {
                var result = await DependencyInstaller.InstallDependencyAsync();
                if (result == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error has occurred");
                if (!string.IsNullOrEmpty(result.Error) || result.NotInstalled)
                    return BadRequest(result.Error);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR FROM: ScriptsController/GetScripts() => {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error has occurred");
            }
        }
    }
}
