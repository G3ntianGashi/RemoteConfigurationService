using System.Text.Json.Nodes;

namespace DotnetAPI.Dtos
{
    public class ConfigurationDto
    {
        public string KeyIdentifier { get; set; }  // Chiave della configurazione
        public JsonObject ConfigData { get; set; }  // Valore della configurazione, di qualsiasi tipo

        public ConfigurationDto()
        {
            if (KeyIdentifier == null)
            {
                KeyIdentifier = "";
            }
            if (ConfigData == null)
            {
                ConfigData = new JsonObject();
            }
        }
    }
}