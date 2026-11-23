namespace MESFileProcessor
{
    public class Config
    {
        // 保留旧配置字段，确保升级后仍能读取已有 config.json。
        public string MesUrl { get; set; }
        public string StationName { get; set; }
        public string LineName { get; set; }
        public string StationId { get; set; }
        public string NcCode { get; set; }
        public string ComponentNames { get; set; }
        public string WorkStation { get; set; }
        public string OperatorId { get; set; }

        public Config()
        {
            MesUrl = string.Empty;
            StationName = string.Empty;
            LineName = string.Empty;
            StationId = string.Empty;
            NcCode = string.Empty;
            ComponentNames = string.Empty;
            WorkStation = string.Empty;
            OperatorId = string.Empty;
        }
    }
}
