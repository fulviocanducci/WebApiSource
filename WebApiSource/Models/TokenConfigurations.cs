using Microsoft.Extensions.Configuration;

namespace WebApiSource
{
    public class TokenConfigurations
    {
        public TokenConfigurations(IConfiguration configuration)
        {
            IConfigurationSection section = configuration.GetSection("TokenConfigurations");
            Audience = section.GetValue<string>("Audience");
            Issuer = section.GetValue<string>("Issuer");
            Seconds = section.GetValue<int>("Seconds");
        }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int Seconds { get; set; }
    }
}
