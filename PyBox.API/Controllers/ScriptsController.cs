﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PyBox.API.Data;
using PyBox.PyRunner;
using PyBox.Shared.Models.Interops;
using PyBox.Shared.Models.Script;
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

		[HttpGet("check_titles")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<bool>> CheckTitle(string title, int? id = null)
		{
			try
			{
				if (_context.Scripts == null)
					return NotFound();
				IQueryable<ScriptEntity> query = _context.Scripts.Where(d => d.DeletedAt == null && d.Title.ToUpper().Trim() == title.ToUpper().Trim());
				if (id != null)
					query = query = query.Where(q => q.ScriptId != id!);
				return await query.AnyAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError($"ERROR FROM: ScriptsController/GetScripts() => {ex}");
				return StatusCode(StatusCodes.Status500InternalServerError, "An error has occurred");
			}
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<IEnumerable<ScriptView>>> GetScripts()
		{
			try
			{
				if (_context.Scripts == null)
					return NotFound();
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

		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<ScriptEdit>> GetScript(int id)
		{
			try
			{
				if (_context.Scripts == null)
					return NotFound();
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
					return NotFound();
				return output;
			}
			catch (Exception ex)
			{
				_logger.LogError($"ERROR FROM: ScriptsController/GetScript() WITH PARAMETER id = {id} => {ex}");
				return StatusCode(StatusCodes.Status500InternalServerError, "An error has occurred");
			}
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<int>> PostScript(ScriptDefinition input)
		{
			if (_context.Scripts == null)
				return NotFound();
			try
			{
				if (await TilteExists(input.Title))
					return BadRequest("This title already exists!");
				var entity = new ScriptEntity()
				{
					Title = input.Title,
					Description = input.Description,
					ScriptText = "#Remember that if you want to pass parameters to the script, you need to import the \"sys\" library (using sys).\n" +
						"#This way you will be able to access the parameters passed to the script via the \"sys.argv[<index>]\" variable\n\n#PS Remember that the first " +
						"parameter passed to the script will be at index \"1\" (sys.argv[1]) and not at index \"0\"\n\nimport sys\n\nprint(f'Hello {sys.argv[1]}')\n",
					CreatedAt = DateTime.Now,
					Enabled = true,
					UpdatedAt = null,
					DeletedAt = null
				};
				_context.Scripts.Add(entity);
				await _context.SaveChangesAsync();
				return entity.ScriptId;
			}
			catch (Exception ex)
			{
				_logger.LogError($"ERROR FROM: ScriptsController/PostScript() WITH PARAMETER input = {JsonConvert.SerializeObject(input)} => {ex}");
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
				return NotFound();
			var entity = await _context.Scripts.Where(e => e.ScriptId == id).SingleOrDefaultAsync();
			if (entity == null)
				return NotFound();
			if (await TilteExists(input.Title, input.ScriptId))
				return BadRequest("This title already exists!");
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
				if (!ScriptEntityExists(id))
				{
					return NotFound();
				}
				else
				{
					_logger.LogError($"ERROR FROM: ScriptsController/PutScript() WITH PARAMETERS id = {id}, input = {JsonConvert.SerializeObject(input)} => {ex}");
					return StatusCode(StatusCodes.Status500InternalServerError, "An error has occurred");
				}
			}
		}

		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<IEnumerable<ScriptView>>> DeleteScript(int id)
		{
			if (_context.Scripts == null)
				return NotFound();
			var entity = await _context.Scripts.FindAsync(id);
			if (entity == null)
				return NotFound();
			entity.DeletedAt = DateTime.Now;
			_context.Entry(entity).State = EntityState.Modified;
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError($"ERROR FROM: ScriptsController/DeleteScript() WITH PARAMETER id = {id} => {ex}");
				return StatusCode(StatusCodes.Status500InternalServerError, "An error has occurred");
			}
			return NoContent();
		}

		[HttpGet("run")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<InteropsResult>> RunScript(int id, string parameters = "")
		{
			try
			{
				if (_context.Scripts == null)
					return NotFound();
				var script = await _context.Scripts.Where(e => e.ScriptId == id).SingleOrDefaultAsync();
				if (script == null || script.DeletedAt != null)
					return NotFound();
				if (!script.Enabled)
					return BadRequest("Script is disabled");
				byte[] data = Encoding.UTF8.GetBytes(script.ScriptText);
				if (data.Length == 0)
					return BadRequest("Script is empty");
				InteropsResult result;
				try
				{
					using (Runner runner = new Runner(data, parameters))
					{
						await runner.Run();
						result = new InteropsResult()
						{
							Result = runner.Result,
							Error = runner.Errors
						};
					}
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
		private async Task<bool> TilteExists(string title, int? id = null)
		{
			IQueryable<ScriptEntity>? query = _context.Scripts.Where(s => s.Title == title && s.DeletedAt == null);
			if (id != null)
				query = query.Where(e => e.ScriptId != (int)id);
			return await query.CountAsync() > 0;
		}
		private bool ScriptEntityExists(int id)
		{
			return (_context.Scripts?.Any(e => e.ScriptId == id)).GetValueOrDefault();
		}
	}
}