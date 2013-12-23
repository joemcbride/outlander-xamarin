using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Pathfinder.Core.Authentication;
using Pathfinder.Core.Text;

namespace Pathfinder.Core
{
	public delegate void TextLogHandler(string text);

	public interface IGameState
	{
		string Get(string key);
		void Set(string key, string value);
		void Read(string data);
		void Echo(string text);
		ISimpleDictionary<string, string> GlobalVars();

		event TextLogHandler TextLog;

		Action<IEnumerable<Tag>> Tags { get; set; }
		Action<SkillExp> Exp { get; set; }
	}

	public class SimpleGameState : IGameState
	{
		private readonly IGameParser _parser;
		private readonly List<string> _filters = new List<string>();
		private readonly SimpleDictionary<string, string> _components = new SimpleDictionary<string, string>();
		private readonly IRoundtimeHandler _roundtimeHandler;

		public event TextLogHandler TextLog = delegate { };

		public Action<IEnumerable<Tag>> Tags { get; set; }
		public Action<SkillExp> Exp { get; set; }

		public SimpleGameState(IGameParser parser, IRoundtimeHandler roundtimeHandler)
		{
			_parser = parser;
			_roundtimeHandler = roundtimeHandler;
			_components.Set(ComponentKeys.Prompt, ">");
		}

		public string Get(string key)
		{
			return _components.Get(key);
		}

		public void Set(string key, string value)
		{
			_components.Set(key, value);
		}

		public void Read(string data)
		{
			var result = _parser.Parse(Chunk.For(data));
			ApplyTags(result.Tags);
			RenderData(result);
		}

		public void Echo(string text)
		{
			Log(text);
		}

		public ISimpleDictionary<string, string> GlobalVars()
		{
			return new SimpleDictionary<string, string>(_components.Values());
		}

		public void RenderData(ReadResult result)
		{
			if (result.Chunk == null || string.IsNullOrWhiteSpace(result.Chunk.Text) || string.IsNullOrWhiteSpace(result.Chunk.Text.Trim()))
				return;

			var trimmed = result.Chunk.Text.Trim();
			if (trimmed.StartsWith("Obvious paths") || trimmed.StartsWith("Obvious exits"))
				trimmed = "\n" + trimmed;

			Log(trimmed);

			var dontShowPrompt = false;
			var roomName = result.Tags.OfType<RoomNameTag>().FirstOrDefault();

			if (roomName != null && trimmed.Contains(roomName.Name) && !(trimmed.Contains("Obvious paths") | trimmed.Contains("Obvious exits"))) {
				dontShowPrompt = true;
			}

			if(trimmed.Contains("<output class=\"mono\"/>") && !trimmed.Contains("EXP HELP"))
				dontShowPrompt = true;

			if (!dontShowPrompt)
				ShowPrompt();
		}

		public void ApplyTags(IEnumerable<Tag> tags)
		{
			tags = tags.ToList();

			tags.OfType<ComponentTag>().Apply(c => {
				if(c.IsExp) {
					var skill = SkillExp.For(c);
					var ranksId = c.Id + ".Ranks";
					// don't set ranks to empty if a value already exists
					if(!_components.HasKey(ranksId) || !string.IsNullOrWhiteSpace(skill.Ranks))
					{
						_components.Set(ranksId, skill.Ranks);
					}
					_components.Set(c.Id + ".LearningRate", skill.LearningRate.Id.ToString());
					_components.Set(c.Id + ".LearningRateName", skill.LearningRate.Description);
					if(Exp != null)
					{
						Exp(skill);
					}
				} else {
					_components.Set(c.Id, c.Value);
				}
			});

			tags.OfType<RoomNameTag>().Apply(t => {
				_components.Set(ComponentKeys.RoomName, t.Name);
			});

			tags.OfType<VitalsTag>().Apply(t => {
				_components.Set(t.Name, t.Value.ToString());
			});

			tags.OfType<IndicatorTag>().Apply(t => {
				_components.Set(t.Id, t.Value);
			});

			tags.OfType<LeftHandTag>().Apply(t => {
				_components.Set(ComponentKeys.LeftHand, t.Name);
				_components.Set(ComponentKeys.LeftHandId, t.Id);
				_components.Set(ComponentKeys.LeftHandNoun, t.Noun);
			});

			tags.OfType<RightHandTag>().Apply(t => {
				_components.Set(ComponentKeys.RightHand, t.Name);
				_components.Set(ComponentKeys.RightHandId, t.Id);
				_components.Set(ComponentKeys.RightHandNoun, t.Noun);
			});

			tags.OfType<SpellTag>().Apply(t => {
				_components.Set(ComponentKeys.Spell, t.Spell);
			});

			tags.OfType<PromptTag>().Apply(t => {
				_components.Set(ComponentKeys.Prompt, t.Prompt);
				_components.Set(ComponentKeys.GameTime, t.GameTime);
				_components.Set(ComponentKeys.GameTimeUpdate, DateTime.UtcNow.DateTimeToUnixTimestamp().ToString());
			});

			tags.OfType<RoundtimeTag>().Apply(t => {
				var gameTime = _components.Get(ComponentKeys.GameTime).UnixTimeStampToDateTime();
				var gameTimeLastUpdate = _components.Get(ComponentKeys.GameTimeUpdate).UnixTimeStampToDateTime();
				var diff = t.RoundTime - gameTime;
				var realDiff = diff.Subtract(DateTime.Now - gameTimeLastUpdate);
				_roundtimeHandler.Set(realDiff.Seconds);
			});

			tags.OfType<AppTag>().Apply(t => {
				_components.Set(ComponentKeys.CharacterName, t.Character);
				_components.Set(ComponentKeys.Game, t.Game);
			});

			var tagsEv = Tags;
			if(tagsEv != null && tags != null){
				tagsEv(tags);
			}
		}

		private void ShowPrompt()
		{
			Log("\n" + _components.Get(ComponentKeys.Prompt) + "\n");
		}

		private void Log(string data)
		{
			var textLog = TextLog;
			if(textLog != null)
			{
				textLog(Filter(data));
			}
		}

		private string Filter(string data)
		{
			if (string.IsNullOrEmpty(data))
				return data;

			data = Regex.Replace(data, "[\r\n]{3,}", "\n\n");

			_filters.Apply(x => data = Regex.Replace(data, x, string.Empty));

			return data;
		}
	}
}
