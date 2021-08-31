using Overlord.Core.Entities.Geometric;
using System.Text.Json.Serialization;

namespace Overlord.Core.Entities.Road
{
    public class LeaveLine : NormalizedLine
    {
        public string Name { get; set; }

        [JsonIgnore]
        public EnterLine EnterLine { get; set; }
    }
}
