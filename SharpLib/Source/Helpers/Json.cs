using Newtonsoft.Json;

namespace SharpLib.Source.Helpers
{
    public static class Json
    {
        #region Поля

        private static readonly JsonSerializerSettings _jsonSettings;

        #endregion

        #region Конструктор

        static Json()
        {
            _jsonSettings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffzzz"
            };
        }

        #endregion

        #region Методы

        public static string Serialize(object value, Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(value, formatting, _jsonSettings);
        }

        public static T Deserialize<T>(string value)
        {
            return (T)JsonConvert.DeserializeObject(value, typeof(T), _jsonSettings);
        }

        #endregion
    }
}