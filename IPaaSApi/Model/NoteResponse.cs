using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Integration.Data.IPaaSApi.Model
{
    /// <summary>
    /// Represents a note/comment nested within a transaction response.
    /// </summary>
    public class NoteResponse : BaseResponse
    {
        #region Properties
        [JsonProperty("sequence_number", Order = 10)]
        public int SequenceNumber { get; set; }

        [JsonProperty("type", Order = 15)]
        public string Type { get; set; }

        [JsonProperty("text", Order = 20)]
        public string Text { get; set; }

        [JsonProperty("note_created_date_time", Order = 25)]
        public DateTime? NoteCreatedDateTime { get; set; }

        [JsonProperty("is_public", Order = 30)]
        public bool IsPublic { get; set; }
        #endregion

        #region Method(s)
        public override object GetPrimaryId()
        {
            return Convert.ToString(Id);
        }
        #endregion
    }
}
