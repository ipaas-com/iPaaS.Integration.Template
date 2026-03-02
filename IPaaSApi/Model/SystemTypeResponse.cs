using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Integration.Data.IPaaSApi.Model
{
    public class SystemTypeResponse
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("dll_name")]
        public string DllName { get; set; }

        [JsonProperty("dll_namespace")]
        public string DllNamespace { get; set; }

        [JsonProperty("api_version")]
        public string ApiVersion { get; set; }

        [JsonProperty("version_major")]
        public int VersionMajor { get; set; }

        [JsonProperty("version_minor")]
        public int VersionMinor { get; set; }

        [JsonProperty("versions")]
        public List<SystemTypeVersionResponse> Versions { get; set; }
    }

    public class SystemTypeVersionResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("dll_name")]
        public string DllName { get; set; }

        [JsonProperty("dll_namespace")]
        public string DllNamespace { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("release_notes")]
        public string ReleaseNotes { get; set; }

        [JsonProperty("version_major")]
        public int VersionMajor { get; set; }

        [JsonProperty("version_minor")]
        public int VersionMinor { get; set; }

        [JsonProperty("custom_fields")]
        public Dictionary<string, string> CustomFields { get; set; }
    }
}
