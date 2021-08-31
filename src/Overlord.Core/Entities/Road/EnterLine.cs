using Overlord.Core.Entities.Geometric;
using System.Text.Json.Serialization;

namespace Overlord.Core.Entities.Road
{
    public class EnterLine : NormalizedLine
    {
        public string Name { get; set; }

        [JsonIgnore]
        public LeaveLine LeaveLine { get; set; }
    }
}
