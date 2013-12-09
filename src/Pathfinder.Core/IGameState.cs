using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Pathfinder.Core.Authentication;
using Pathfinder.Core.Text;

namespace Pathfinder.Core
{
	public interface IGameState
	{
		string Get(string key);
		void Set(string key, string value);
		void Read(string data);

		Action<string> TextLog { get; set; }
		Action<RoundtimeTag> Roundtime { get; set; }
	}

	public class SimpleGameState : IGameState
	{
		private readonly IGameParser _parser;
		private readonly List<string> _filters = new List<string>();
		private readonly SimpleDictionary _components = new SimpleDictionary();

		public Action<string> TextLog { get; set; }
		public Action<RoundtimeTag> Roundtime { get; set; }

		public SimpleGameState(IGameParser parser)
		{
			_parser = parser;
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

			if (!dontShowPrompt)
				ShowPrompt();
		}

		public void ApplyTags(IEnumerable<Tag> tags)
		{
			tags = tags.ToList();

			tags.OfType<ComponentTag>().Apply(c => {
				if(c.IsExp) {
					const string Rank_Regex = "(\\d+)\\s(\\d+)%\\s(\\w+)";
					_components.Set(c.Id + ".Ranks", Regex.Replace(c.Value, Rank_Regex, "$1.$2"));
					var mindState = Regex.Replace(c.Value, Rank_Regex, "$3");
					var learningRate = LearningRate.For(mindState);
					_components.Set(c.Id + ".LearningRate", learningRate.Id.ToString());
					_components.Set(c.Id + ".LearningRateName", learningRate.Description);
				} else {
					_components.Set(c.Id, c.Value);
				}
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
				_components.Set(ComponentKeys.RightHandId + "id", t.Id);
				_components.Set(ComponentKeys.RightHandNoun + "noun", t.Noun);
			});

			tags.OfType<SpellTag>().Apply(t => {
				_components.Set(ComponentKeys.Spell, t.Spell);
			});

			tags.OfType<PromptTag>().Apply(t => {
				_components.Set(ComponentKeys.Prompt, t.Prompt);
				_components.Set(ComponentKeys.GameTime, t.GameTime);
			});

			tags.OfType<RoundtimeTag>().Apply(t => {
				var diff = t.RoundTime - DateTime.Now;
				_components.Set(ComponentKeys.Roundtime, diff.Seconds.ToString());
				if(Roundtime != null)
				{
					Roundtime(t);
				}
			});
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
