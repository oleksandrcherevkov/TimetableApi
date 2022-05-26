using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimetableApi.Converters.DateOnly
{
    public class DateOnlyJsonConverter : JsonConverter<System.DateOnly>
    {
        private const string Format = "yyyy-MM-dd";

        public override System.DateOnly ReadJson(JsonReader reader,
            Type objectType,
            System.DateOnly existingValue,
            bool hasExistingValue,
            JsonSerializer serializer) =>
            System.DateOnly.ParseExact((string)reader.Value, Format, CultureInfo.InvariantCulture);

        public override void WriteJson(JsonWriter writer, System.DateOnly value, JsonSerializer serializer) =>
            writer.WriteValue(value.ToString(Format, CultureInfo.InvariantCulture));

        public static System.DateOnly Parse(string date) =>
            System.DateOnly.ParseExact(date, Format, CultureInfo.InvariantCulture);
    }
}
