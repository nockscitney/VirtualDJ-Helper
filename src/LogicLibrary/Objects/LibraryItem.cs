using System;
using System.Collections.Generic;
//  Added
using System.Collections;
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
        public DateTime FirstSeen2 { get { return DateTimeOffset.FromUnixTimeSeconds(FirstSeen).DateTime; } }

        [JsonProperty("@LastPlay")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long LastPlay { get; set; }

        public DateTime LastPlay2 { get { return DateTimeOffset.FromUnixTimeSeconds(LastPlay).DateTime; } }

        [JsonProperty("@PlayCount")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long PlayCount { get; set; }

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
        Hashtable mikHash;

        [JsonProperty("@Version")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Version { get; set; }

        [JsonProperty("@Bpm")]
        public string Bpm { get; set; }
        public int CalculatedBPM { get => (int)Math.Round(60 / Decimal.Parse(Bpm), 2); }

        [JsonProperty("@AltBpm")]
        public string AltBpm { get; set; }

        [JsonProperty("@Volume")]
        public string Volume { get; set; }

        [JsonProperty("@Key")]
        public string Key { get; set; }

        public string MIKKey
        {
            get
            {
                return mikHash[Key].ToString();
            }
        }

        [JsonProperty("@Flag")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Flag { get; set; }

        public Scan()
            : base()
        {
            mikHash = new Hashtable();

            mikHash.Add("Abm", "01A");
            mikHash.Add("G#m", "01A");
            mikHash.Add("B", "01B");
            mikHash.Add("Ebm", "02A");
            mikHash.Add("F#", "02B");
            mikHash.Add("Bbm", "03A");
            mikHash.Add("A#m", "03A");
            mikHash.Add("Db", "03B");
            mikHash.Add("C#", "03B");
            mikHash.Add("Fm", "04A");
            mikHash.Add("G#", "04B");
            mikHash.Add("Ab", "04B");
            mikHash.Add("Cm", "05A");
            mikHash.Add("Eb", "05B");
            mikHash.Add("Gm", "06A");
            mikHash.Add("Bb", "06B");
            mikHash.Add("A#", "06B");
            mikHash.Add("Dm", "07A");
            mikHash.Add("F", "07B");
            mikHash.Add("Am", "08A");
            mikHash.Add("C", "08B");
            mikHash.Add("Em", "09A");
            mikHash.Add("G", "09B");
            mikHash.Add("Bm", "10A");
            mikHash.Add("D", "10B");
            mikHash.Add("F#m", "11A");
            mikHash.Add("A", "11B");
            mikHash.Add("C#m", "12A");
            mikHash.Add("E", "12B");
        }
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
