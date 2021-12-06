using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Overlord.Domain.Settings
{
    public class ApplicationSettings
    {
        public double PositivePercentThresh { get; set; }

        public List<PipelineSettings> PipelineSettings { get; set; }

        public void LoadFromJson(string filename)
        {
            if (!File.Exists(filename))
            {
                PositivePercentThresh = 0.7;
            }

            string json = File.ReadAllText(filename);
            ApplicationSettings settings = JsonSerializer.Deserialize<ApplicationSettings>(json)
                                           ?? throw new ArgumentNullException("application settings corrupted.");

            this.PositivePercentThresh = settings.PositivePercentThresh;
            this.PipelineSettings = settings.PipelineSettings;
        }
    }
}
