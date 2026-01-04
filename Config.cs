namespace MESFileProcessor
{
    public class Config
    {
        public string MesUrl { get; set; }
        public string StationName { get; set; }
        public string LineName { get; set; }

        public Config()
        {
            MesUrl = string.Empty;
            StationName = string.Empty;
            LineName = string.Empty;
        }
    }
}
