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
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Invalid comparison between Unknown and I4
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Invalid comparison between Unknown and I4
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Invalid comparison between Unknown and I4
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Invalid comparison between Unknown and I4
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Invalid comparison between Unknown and I4
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<uint, Action> dictionary = new Dictionary<uint, Action>();
		if ((int)reader.TokenType == 2)
		{
			reader.Read();
		}
		while ((int)reader.TokenType == 1)
		{
			Action action = new Action();
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
					if ((int)reader.TokenType != 14 || !reader.Read())
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
					if ((int)reader.TokenType != 14 || !reader.Read())
					{
						throw new JsonParseException($"Unexpected token {reader.TokenType} when reading EndArray.");
					}
				}
				if ((int)reader.TokenType != 13 || !reader.Read())
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
