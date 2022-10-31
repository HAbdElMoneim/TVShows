using System.ComponentModel.DataAnnotations;

namespace TVShowsUpdateWorkerJob.Configurations
{
    public class ScheduleConfigurations
    {
        public ScheduleConfigurations()
        {
            Schedule =  String.Empty;
        }
        public string Schedule { get; set; }
        public int OnErrorDelay { get; set; }
        public bool Enabled { get; set; }

    }
}