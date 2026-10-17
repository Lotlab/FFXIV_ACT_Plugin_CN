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
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Invalid comparison between Unknown and I4
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Invalid comparison between Unknown and I4
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Invalid comparison between Unknown and I4
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Invalid comparison between Unknown and I4
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Invalid comparison between Unknown and I4
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Invalid comparison between Unknown and I4
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Invalid comparison between Unknown and I4
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Invalid comparison between Unknown and I4
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Invalid comparison between Unknown and I4
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<uint, StatusEffect> dictionary = new Dictionary<uint, StatusEffect>();
		if ((int)reader.TokenType == 2)
		{
			reader.Read();
		}
		while ((int)reader.TokenType == 1)
		{
			StatusEffect statusEffect = new StatusEffect();
			if (!reader.Read())
			{
				throw new JsonParseException("Cannot read StartObject token.");
			}
			if ((int)reader.TokenType != 4)
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
				while ((int)reader.TokenType != 13)
				{
					if (reader.Value?.ToString() == "potency")
					{
						if (!reader.Read())
						{
							throw new JsonParseException($"Unexpected token {reader.TokenType} after reading potency node.");
						}
						statusEffect.PotencyEffects = serializer.Deserialize<Potency[]>(reader);
						if ((int)reader.TokenType != 14 || !reader.Read())
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
						if ((int)reader.TokenType != 13 || !reader.Read())
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
						if ((int)reader.TokenType != 14 || !reader.Read())
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
						if ((int)reader.TokenType != 13 || !reader.Read())
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
						if ((int)reader.TokenType != 13 || !reader.Read())
						{
							throw new JsonParseException($"Unexpected token {reader.TokenType} when reading EndArray.");
						}
					}
				}
				if ((int)reader.TokenType != 13 || !reader.Read())
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
