namespace Craftorio.Json;

[Newtonsoft.Json.JsonConverter(typeof(Color))]
public class ColorConverter : Newtonsoft.Json.JsonConverter<Color>
{
    public override Color ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
    {
        var colorPackedString = serializer.Deserialize<string>(reader)!;
        var colorPacked = uint.Parse(colorPackedString, System.Globalization.NumberStyles.HexNumber);
        var ret = new Color(colorPacked);
        return ret;
    }
    public override void WriteJson(Newtonsoft.Json.JsonWriter writer, Color value, Newtonsoft.Json.JsonSerializer serializer)
    {
        writer.WriteValue(value.R);
        writer.WriteValue(value.G);
        writer.WriteValue(value.B);
        writer.WriteValue(value.A);
    }
}