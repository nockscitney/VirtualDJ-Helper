using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//  Added
using System.Globalization;
//  Newtonsoft
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace NickScotney.Internal.VDJ.LogicLibrary.Objects
{
    public class LibraryItem
    {
        [JsonProperty("Song")]
        public Song Song { get; set; }
    }

    public partial class Song
    {
        [JsonProperty("@FilePath")]
        public string FilePath { get; set; }

        public string FolderPath
        {
            get
            {
                return FilePath.Substring(0, FilePath.LastIndexOf('\\'));
            }
        }

        [JsonProperty("@FileSize")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long FileSize { get; set; }

        [JsonProperty("Tags")]
        public Tags Tags { get; set; }

        [JsonProperty("Infos")]
        public Infos Infos { get; set; }

        [JsonProperty("Scan")]
        public Scan Scan { get; set; }

        [JsonProperty("Comment")]
        public string Comment { get; set; }

        //[JsonProperty("Poi")]
        //public Poi[] Poi { get; set; }
    }

    public partial class Infos
    {
        [JsonProperty("@SongLength")]
        public string SongLength { get; set; }

        public string TrackLength
        {
            get
            {
                if (!String.IsNullOrEmpty(SongLength))
                {
                    TimeSpan ts = TimeSpan.FromSeconds(Double.Parse(SongLength));
                    return String.Format("{0:D2}:{1:D2}:{2:D2}", ts.Hours, ts.Minutes, ts.Seconds);
                }
                return "N/A";
            }
        }
        
        [JsonProperty("@FirstSeen")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long FirstSeen { get; set; }

        [JsonProperty("@Bitrate")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Bitrate { get; set; }

        [JsonProperty("@Cover")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Cover { get; set; }

        [JsonProperty("@Color")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Color { get; set; }
    }

    public partial class Poi
    {
        [JsonProperty("@Pos")]
        public string Pos { get; set; }

        [JsonProperty("@Type")]
        public string Type { get; set; }

        [JsonProperty("@Point", NullValueHandling = NullValueHandling.Ignore)]
        public string Point { get; set; }
    }

    public partial class Scan
    {
        [JsonProperty("@Version")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Version { get; set; }

        [JsonProperty("@Bpm")]
        public string Bpm { get; set; }

        [JsonProperty("@AltBpm")]
        public string AltBpm { get; set; }

        [JsonProperty("@Volume")]
        public string Volume { get; set; }

        [JsonProperty("@Key")]
        public string Key { get; set; }

        [JsonProperty("@Flag")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Flag { get; set; }
    }

    public partial class Tags
    {
        [JsonProperty("@Author")]
        public string Author { get; set; }
        [JsonProperty("@Title")]
        public string Title { get; set; }
        [JsonProperty("@Remix")]
        public string Remix { get; set; }

        [JsonProperty("@Flag")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Flag { get; set; }
    }

    public static class Serialize
    {
        public static string ToJson(this LibraryItem self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }

    public class SingleOrArrayConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objecType)
        {
            return (objecType == typeof(List<T>));
        }

        public override object ReadJson(JsonReader reader, Type objecType, object existingValue,
            JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Array)
            {
                return token.ToObject<List<T>>();
            }
            return new List<T> { token.ToObject<T>() };
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
