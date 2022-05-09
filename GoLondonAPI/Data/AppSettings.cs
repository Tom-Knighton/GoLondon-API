using System;
namespace GoLondonAPI.Data
{
    public class AppSettings
    {
        public string MapboxAPIKey { get; set; }
        public string TflAPIKey { get; set; }
        public string TflAPPKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
    }
}

