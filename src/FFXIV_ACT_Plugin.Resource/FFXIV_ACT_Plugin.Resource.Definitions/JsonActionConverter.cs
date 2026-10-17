using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace FFXIV_ACT_Plugin.Resource.Definitions;

internal class JsonActionConverter : JsonConverter
{
	public override bool CanRead => true;

	public override bool CanWrite => false;

	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Dictionary<uint, Action>);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		Dictionary<uint, Action> dictionary = new Dictionary<uint, Action>();
		if (reader.TokenType == JsonToken.StartArray)
		{
			reader.Read();
		}
		while (reader.TokenType == JsonToken.StartObject)
		{
			Action action = new Action();
			if (!reader.Read())
			{
				throw new JsonParseException("Cannot read StartObject token.");
			}
			if (reader.TokenType != JsonToken.PropertyName)
			{
				throw new JsonParseException("PropertyName token not found after StartObject.");
			}
			if (uint.TryParse(reader.Value?.ToString(), NumberStyles.HexNumber, null, out var result))
			{
				action.Id = result;
				action.Name = reader.ReadAsString();
				if (!reader.Read())
				{
					throw new JsonParseException($"Unexpected token {reader.TokenType} after reading name.");
				}
				if (reader.Value?.ToString() == "damage")
				{
					if (!reader.Read())
					{
						throw new JsonParseException($"Unexpected token {reader.TokenType} after reading damage node.");
					}
					action.Damage = serializer.Deserialize<DamageEntry[]>(reader);
					if (reader.TokenType != JsonToken.EndArray || !reader.Read())
					{
						throw new JsonParseException($"Unexpected token {reader.TokenType} when reading EndArray.");
					}
				}
				if (reader.Value?.ToString() == "heal")
				{
					if (!reader.Read())
					{
						throw new JsonParseException($"Unexpected token {reader.TokenType} after reading heal node.");
					}
					action.Heal = serializer.Deserialize<HealEntry[]>(reader);
					if (reader.TokenType != JsonToken.EndArray || !reader.Read())
					{
						throw new JsonParseException($"Unexpected token {reader.TokenType} when reading EndArray.");
					}
				}
				if (reader.TokenType != JsonToken.EndObject || !reader.Read())
				{
					throw new JsonParseException($"Unexpected token {reader.TokenType} when reading EndObject.");
				}
				if (action.Damage == null)
				{
					action.Damage = Array.Empty<DamageEntry>();
				}
				if (action.Heal == null)
				{
					action.Heal = Array.Empty<HealEntry>();
				}
				dictionary.Add(action.Id, action);
				continue;
			}
			throw new JsonParseException("Cannot read action id.");
		}
		return dictionary;
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		throw new NotImplementedException();
	}
}
