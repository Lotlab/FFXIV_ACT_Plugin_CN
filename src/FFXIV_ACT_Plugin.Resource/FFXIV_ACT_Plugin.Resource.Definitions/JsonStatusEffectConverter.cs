using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace FFXIV_ACT_Plugin.Resource.Definitions;

internal class JsonStatusEffectConverter : JsonConverter
{
	public override bool CanRead => true;

	public override bool CanWrite => false;

	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Dictionary<uint, StatusEffect>);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		Dictionary<uint, StatusEffect> dictionary = new Dictionary<uint, StatusEffect>();
		if (reader.TokenType == JsonToken.StartArray)
		{
			reader.Read();
		}
		while (reader.TokenType == JsonToken.StartObject)
		{
			StatusEffect statusEffect = new StatusEffect();
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
				statusEffect.Id = result;
				statusEffect.Name = reader.ReadAsString();
				if (!reader.Read())
				{
					throw new JsonParseException($"Unexpected token {reader.TokenType} after reading name.");
				}
				while (reader.TokenType != JsonToken.EndObject)
				{
					if (reader.Value?.ToString() == "potency")
					{
						if (!reader.Read())
						{
							throw new JsonParseException($"Unexpected token {reader.TokenType} after reading potency node.");
						}
						statusEffect.PotencyEffects = serializer.Deserialize<Potency[]>(reader);
						if (reader.TokenType != JsonToken.EndArray || !reader.Read())
						{
							throw new JsonParseException($"Unexpected token {reader.TokenType} when reading EndArray.");
						}
					}
					else if (reader.Value?.ToString() == "timeproc")
					{
						if (!reader.Read())
						{
							throw new JsonParseException($"Unexpected token {reader.TokenType} after reading timeproc node.");
						}
						statusEffect.TimeProc = serializer.Deserialize<TimeProc>(reader);
						if (reader.TokenType != JsonToken.EndObject || !reader.Read())
						{
							throw new JsonParseException($"Unexpected token {reader.TokenType} when reading EndArray.");
						}
					}
					else if (reader.Value?.ToString() == "multiplier")
					{
						if (!reader.Read())
						{
							throw new JsonParseException($"Unexpected token {reader.TokenType} after reading multiplier node.");
						}
						statusEffect.Multipliers = serializer.Deserialize<Multiplier[]>(reader);
						if (reader.TokenType != JsonToken.EndArray || !reader.Read())
						{
							throw new JsonParseException($"Unexpected token {reader.TokenType} when reading EndArray.");
						}
					}
					else if (reader.Value?.ToString() == "reactiveproc")
					{
						if (!reader.Read())
						{
							throw new JsonParseException($"Unexpected token {reader.TokenType} after reading reactiveproc node.");
						}
						statusEffect.ReactiveProc = serializer.Deserialize<ReactiveProc>(reader);
						if (reader.TokenType != JsonToken.EndObject || !reader.Read())
						{
							throw new JsonParseException($"Unexpected token {reader.TokenType} when reading EndArray.");
						}
					}
					else if (reader.Value?.ToString() == "damageshield")
					{
						if (!reader.Read())
						{
							throw new JsonParseException($"Unexpected token {reader.TokenType} after reading damageshield node.");
						}
						statusEffect.DamageShield = serializer.Deserialize<DamageShield>(reader);
						if (reader.TokenType != JsonToken.EndObject || !reader.Read())
						{
							throw new JsonParseException($"Unexpected token {reader.TokenType} when reading EndArray.");
						}
					}
				}
				if (reader.TokenType != JsonToken.EndObject || !reader.Read())
				{
					throw new JsonParseException($"Unexpected token {reader.TokenType} when reading EndObject.");
				}
				if (statusEffect.PotencyEffects == null)
				{
					statusEffect.PotencyEffects = Array.Empty<Potency>();
				}
				if (statusEffect.Multipliers == null)
				{
					statusEffect.Multipliers = Array.Empty<Multiplier>();
				}
				dictionary.Add(statusEffect.Id, statusEffect);
				continue;
			}
			throw new JsonParseException("Cannot read status id.");
		}
		return dictionary;
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		throw new NotImplementedException();
	}
}
