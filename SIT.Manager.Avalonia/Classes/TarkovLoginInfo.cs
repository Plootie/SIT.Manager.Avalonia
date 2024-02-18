using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.Classes
{
    [Serializable]
    public class TarkovLoginInfo
    {
        private const string EDITION = "Edge Of Darkness";
        [JsonPropertyName("username")]
        public string Username { get; init; } = string.Empty;
        [JsonPropertyName("email")]
        public string Email => Username;
        [JsonPropertyName("edition")]
        public string Edition => EDITION;
        [JsonPropertyName("password")]
        public string Password { get; init; } = string.Empty;
        [JsonPropertyName("backendUrl")]
        public string BackendUrl { get; init; } = string.Empty;
    }
}
