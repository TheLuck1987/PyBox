﻿using PyBox.Shared.Models.Script;
using PyBox.Shared.Services.Interfaces;

namespace PyBox.Shared.Services.Classes
{
	public class DataService : IScriptDataService
	{
		private List<ScriptEntity> _database;

		public DataService()
		{
			_database = new List<ScriptEntity>()
			{
				new()
				{
					CreatedAt = DateTime.Parse("21/10/1987"),
					DeletedAt = null,
					Description = "Sono nato io",
					Enabled = true,
					ScriptId = 0,
					ScriptText = "",
					Title = "Andrea",
					UpdatedAt = null
				},
				new()
				{
					CreatedAt = DateTime.Parse("03/08/1994"),
					DeletedAt = null,
					Description = "È nata Valentina",
					Enabled = true,
					ScriptId = 1,
					ScriptText = "",
					Title = "Valentina",
					UpdatedAt = null
				},
				new()
				{
					CreatedAt = DateTime.Parse("06/07/2018 16:58"),
					DeletedAt = null,
					Description = "È nato Pippo",
					Enabled = true,
					ScriptId = 2,
					ScriptText = "",
					Title = "Filippo",
					UpdatedAt = null
				},
				new()
				{
					CreatedAt = DateTime.Parse("19/07/2022 03:04"),
					DeletedAt = null,
					Description = "È nato Simonello",
					Enabled = true,
					ScriptId = 3,
					ScriptText = "",
					Title = "Simone",
					UpdatedAt = null
				}
			};
		}

		public async Task<IScriptDataServiceResponse> CheckTitle(string title, int? id = null)
		{
			IEnumerable<ScriptEntity> query = _database.Where(d => d.DeletedAt == null && d.Title.ToUpper().Trim() == title.ToUpper().Trim());
			if (id != null)
				query = query.Where(q => q.ScriptId != id!);
			return new ScriptDataServiceResponse() { ErrorLevel = Enums.WarningLevel.NO_WARNING, Errors = null, Result = query.Any() };
		}
		public async Task<IScriptDataServiceResponse> GetScripts()
		{
			//return new ScriptDataServiceResponse()
			//{
			//    ErrorLevel = Enums.WarningLevel.ERROR,
			//    Errors = "Test error message",
			//    Result = null
			//};
			//return new ScriptDataServiceResponse()
			//{
			//    ErrorLevel = Enums.WarningLevel.WARNING,
			//    Errors = "Test warning message",
			//    Result = null
			//};
			return new ScriptDataServiceResponse()
			{
				ErrorLevel = Enums.WarningLevel.NO_WARNING,
				Errors = null,
				Result = _database.Select(d => d.GetView()).ToList()
			};
		}

		public async Task<IScriptDataServiceResponse> CreateScript(ScriptDefinition input)
		{
			if (_database.Where(d => d.Title == input.Title && d.DeletedAt != null).Any())
			{
				return new ScriptDataServiceResponse() { ErrorLevel = Enums.WarningLevel.WARNING, Errors = "This Title already exists", Result = null };
			}
			_database.Add(
				new ScriptEntity()
				{
					ScriptId = _database.Any() ? _database.Max(d => d.ScriptId) + 1 : 0,
					Title = input.Title!.Trim(),
					Description = input.Description,
					ScriptText = "import sys\n\nargs = sys.argv\nprint(f'Hello {args[1]}')\n",
					Enabled = true,
					CreatedAt = DateTime.Now,
					UpdatedAt = null,
					DeletedAt = null
				}
			);
			return new ScriptDataServiceResponse() { ErrorLevel = Enums.WarningLevel.NO_WARNING, Errors = null, Result = _database.Last().GetEdit() };
		}
		public async Task<IScriptDataServiceResponse> ToggleStatus(int id)
		{
			_database.Where(d => d.ScriptId == id).First().Enabled = !_database.Where(d => d.ScriptId == id).First().Enabled;
			return new ScriptDataServiceResponse() { ErrorLevel = Enums.WarningLevel.NO_WARNING, Errors = null, Result = true };
		}

		public async Task<IScriptDataServiceResponse> DeleteScript(int id)
		{
			_database.Remove(_database.Where(d => d.ScriptId == id).First());
			return new ScriptDataServiceResponse() { ErrorLevel = Enums.WarningLevel.NO_WARNING, Errors = null, Result = true };
		}

		public async Task<IScriptDataServiceResponse> GetScript(int id)
		{
			//return new ScriptDataServiceResponse() { ErrorLevel = Enums.WarningLevel.WARNING, Errors = "This method is not yet implemented", Result = null };
			return new ScriptDataServiceResponse() { ErrorLevel = Enums.WarningLevel.NO_WARNING, Errors = null, Result = _database.Where(d => d.ScriptId == id).First().GetEdit() };
		}

		public async Task<IScriptDataServiceResponse> UpdateScript(int id, ScriptEdit input)
		{
			var entity = _database.Where(d => d.ScriptId == id).First();
			_database.Where(d => d.ScriptId == id).First().Title = input.Title!.Trim();
			_database.Where(d => d.ScriptId == id).First().Description = input.Description;
			_database.Where(d => d.ScriptId == id).First().ScriptText = input.ScriptText;
			_database.Where(d => d.ScriptId == id).First().Enabled = input.Enabled;
			_database.Where(d => d.ScriptId == id).First().UpdatedAt = DateTime.Now;
			return new ScriptDataServiceResponse() { ErrorLevel = Enums.WarningLevel.NO_WARNING, Errors = null, Result = true };
		}

	}
}
