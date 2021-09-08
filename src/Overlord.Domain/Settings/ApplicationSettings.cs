using System;
using System.IO;
using System.Text.Json;

namespace Overlord.Domain.Settings
{
    public class ApplicationSettings
    {
        public int MinTriggerIntervalSecs { get; set; }
        public double PositivePercentThresh { get; set; }

        public void LoadFromJson(string filename)
        {
            if (!File.Exists(filename))
            {
                MinTriggerIntervalSecs = 30;
                PositivePercentThresh = 0.7;
            }

            string json = File.ReadAllText(filename);
            ApplicationSettings settings = JsonSerializer.Deserialize<ApplicationSettings>(json)
                                           ?? throw new ArgumentNullException("application settings corrupted.");

            this.MinTriggerIntervalSecs = settings.MinTriggerIntervalSecs;
            this.PositivePercentThresh = settings.PositivePercentThresh;
        }
    }
}
