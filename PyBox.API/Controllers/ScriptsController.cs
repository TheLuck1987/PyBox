﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PyBox.API.Data;
using PyBox.PyRunner;
using PyBox.Shared.Models.Script;
using PyBox.Shared.Services.Interops;
using System.Text;

namespace PyBox.API.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class ScriptsController : ControllerBase
	{
		private readonly DataContext _context;
		private readonly ILogger<ScriptsController> _logger;

		public ScriptsController(DataContext context, ILogger<ScriptsController> logger)
		{
			_context = context;
			_logger = logger;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<IEnumerable<ScriptView>>> GetScripts()
		{
			try
			{
				if (_context.Scripts == null)
					return StatusCode(StatusCodes.Status500InternalServerError, "Unable to connect to database");
				return await _context.Scripts.Where(e => e.DeletedAt == null).Select(e =>
					new ScriptView()
					{
						ScriptId = e.ScriptId,
						Title = e.Title,
						Description = e.Description,
						ScriptText = e.ScriptText,
						Enabled = e.Enabled,
						CreatedAt = e.CreatedAt,
						UpdatedAt = e.UpdatedAt
					}
				).ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError($"ERROR FROM: ScriptsController/GetScripts() => {ex}");
				return StatusCode(StatusCodes.Status500InternalServerError, "An error has occurred");
			}
		}

		[HttpPost("check_titles")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<bool>> CheckTitle(ScriptCheckTitleUnique input)
		{
			try
			{
				if (_context.Scripts == null)
					return StatusCode(StatusCodes.Status500InternalServerError, "Unable to connect to database");
				if (input == null || input.Title == null)
					return BadRequest("Script Title is required");
				IQueryable<ScriptEntity> query = _context.Scripts.Where(d => d.DeletedAt == null && d.Title!.ToUpper().Trim() == input.Title.ToUpper().Trim());
				if (input.Id != null)
					query = query = query.Where(q => q.ScriptId != input.Id!);
				return await query.AnyAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError($"ERROR FROM: ScriptsController/GetScripts() => {ex}");
				return StatusCode(StatusCodes.Status500InternalServerError, "An error has occurred");
			}
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<ScriptEdit>> PostScript(ScriptDefinition input)
		{
			if (_context.Scripts == null)
				return StatusCode(StatusCodes.Status500InternalServerError, "Unable to connect to database");
			try
			{
				if (await tilteExists(input.Title!))
					return BadRequest("This title already exists!");
				var entity = new ScriptEntity()
				{
					Title = input.Title,
					Description = input.Description,
					ScriptText = "#Remember: the parameters will be accessible from the script by calling the \"args[<index>]\" array.\n\n" +
						"print(f'Hello {args[0]}')\n",
					CreatedAt = DateTime.Now,
					Enabled = true,
					UpdatedAt = null,
					DeletedAt = null
				};
				_context.Scripts.Add(entity);
				await _context.SaveChangesAsync();
				return entity.GetEdit();
			}
			catch (Exception ex)
			{
				_logger.LogError($"ERROR FROM: ScriptsController/PostScript() WITH PARAMETER input = {JsonConvert.SerializeObject(input)} => {ex}");
				return StatusCode(StatusCodes.Status500InternalServerError, "An error has occurred");
			}
		}

		[HttpGet("toggle_status/{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<bool>> ToggleStatus(int id)
		{
			if (_context.Scripts == null)
				return StatusCode(StatusCodes.Status500InternalServerError, "Unable to connect to database");
			if (!scriptEntityExists(id))
				return NotFound($"No script found with ID {id}");
			var entity = await _context.Scripts.FindAsync(id);
			if (entity == null)
				return NotFound($"No script found with ID {id}");
			entity.Enabled = !entity.Enabled;
			_context.Entry(entity).State = EntityState.Modified;
			try
			{
				await _context.SaveChangesAsync();
				return Ok(true);
			}
			catch (DbUpdateConcurrencyException ex)
			{
				_logger.LogError($"ERROR FROM: ScriptsController/ToggleStatus() WITH PARAMETERS id = {id} => {ex}");
				return StatusCode(StatusCodes.Status500InternalServerError, "An error has occurred");
			}
		}

		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<ScriptEdit>> GetScript(int id)
		{
			try
			{
				if (_context.Scripts == null)
					return StatusCode(StatusCodes.Status500InternalServerError, "Unable to connect to database");
				if (!scriptEntityExists(id))
					return NotFound($"No script found with ID {id}");
				var output = await _context.Scripts.Where(e => e.ScriptId == id && e.DeletedAt == null).Select(e =>
					new ScriptEdit()
					{
						ScriptId = id,
						Title = e.Title,
						Description = e.Description,
						ScriptText = e.ScriptText,
						Enabled = e.Enabled
					}
				).SingleOrDefaultAsync();
				if (output == null)
					return NotFound($"No script found with ID {id}");
				return output;
			}
			catch (Exception ex)
			{
				_logger.LogError($"ERROR FROM: ScriptsController/GetScript() WITH PARAMETER id = {id} => {ex}");
				return StatusCode(StatusCodes.Status500InternalServerError, "An error has occurred");
			}
		}

		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<ScriptEdit>> PutScrip(int id, ScriptEdit input)
		{
			if (id != input.ScriptId)
				return BadRequest("The IDs do not match");
			if (_context.Scripts == null)
				return StatusCode(StatusCodes.Status500InternalServerError, "Unable to connect to database");
			if (!scriptEntityExists(id))
				return NotFound($"No script found with ID {id}");
			var entity = await _context.Scripts.FindAsync(id);
			if (await tilteExists(input.Title!, input.ScriptId))
				return BadRequest("This title already exists!");
			if (entity == null)
				return NotFound($"No script found with ID {id}");
			entity.Title = input.Title;
			entity.Description = input.Description;
			entity.ScriptText = input.ScriptText;
			entity.UpdatedAt = DateTime.Now;
			entity.Enabled = input.Enabled;
			_context.Entry(entity).State = EntityState.Modified;
			try
			{
				await _context.SaveChangesAsync();
				return await GetScript(entity.ScriptId);
			}
			catch (DbUpdateConcurrencyException ex)
			{
				_logger.LogError($"ERROR FROM: ScriptsController/PutScript() WITH PARAMETERS id = {id}, input = {JsonConvert.SerializeObject(input)} => {ex}");
				return StatusCode(StatusCodes.Status500InternalServerError, "An error has occurred");
			}
		}

		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<bool>> DeleteScript(int id)
		{
			if (_context.Scripts == null)
				return StatusCode(StatusCodes.Status500InternalServerError, "Unable to connect to database");
			var entity = await _context.Scripts.FindAsync(id);
			if (entity == null)
				return NotFound($"No script found with ID {id}");
			entity.DeletedAt = DateTime.Now;
			_context.Entry(entity).State = EntityState.Modified;
			try
			{
				await _context.SaveChangesAsync();
				return Ok(true);
			}
			catch (Exception ex)
			{
				_logger.LogError($"ERROR FROM: ScriptsController/DeleteScript() WITH PARAMETER id = {id} => {ex}");
				return StatusCode(StatusCodes.Status500InternalServerError, "An error has occurred");
			}
		}

		[HttpPost("run/{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<InteropsResult>> RunScript(int id, [FromBody] string parameters = "")
		{
			try
			{
				if (_context.Scripts == null)
					return StatusCode(StatusCodes.Status500InternalServerError, "Unable to connect to database");
				var script = await _context.Scripts.FindAsync(id);
				if (script == null || script.DeletedAt != null)
					return NotFound($"No script found with ID {id}");
				if (!script.Enabled)
					return BadRequest("Script is disabled");
				string[] dangerous = new string[] { "import sys", "import os", "import cmd" };
				if (string.IsNullOrEmpty(script.ScriptText))
					return BadRequest("Script is empty");
				foreach (string d in dangerous)
					if (script.ScriptText.ToLower().Contains(d))
						return BadRequest("Security error. You cannot import module \"os\" or \"sys\" or \"cmd\"");
				string text = script.ScriptText;
				bool haveParameters = false;
				if (!string.IsNullOrWhiteSpace(parameters))
				{
					haveParameters = true;
					string[] ps = parameters.Split(' ');
					for (int i = 0; i < ps.Length; i++)
						ps[i] = $"\"{ps[i]}\"";
					text = "args = [" + string.Join(", ", ps) + "]\n" + text;
				}
				byte[] data = Encoding.UTF8.GetBytes(text);
				InteropsResult result;
				try
				{
					using Runner runner = new(data, haveParameters);
					await runner.Run();
					result = new InteropsResult()
					{
						Result = runner.Result.Replace("<scriptName>", script.Title + ".py"),
						Error = runner.Errors.Replace("<scriptName>", script.Title + ".py")
					};
				}
				catch (Exception ex)
				{
					result = new()
					{
						Result = string.Empty,
						Error = ex.Message
					};
				}
				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger.LogError($"ERROR FROM: ScriptsController/RunScript() WITH PARAMETERS id = {id}, parameters = {parameters} => {ex}");
				return StatusCode(StatusCodes.Status500InternalServerError, "An error has occurred");
			}
		}
		private async Task<bool> tilteExists(string title, int? id = null)
		{
			IQueryable<ScriptEntity>? query = _context.Scripts.Where(s => s.Title == title && s.DeletedAt == null);
			if (id != null)
				query = query.Where(e => e.ScriptId != (int)id);
			return await query.AnyAsync();
		}
		private bool scriptEntityExists(int id)
		{
			return (_context.Scripts?.Any(e => e.ScriptId == id)).GetValueOrDefault();
		}
	}
}
