namespace DotnetAPI.Models
{
    public class Configuration
    {
        public int Id { get; set; }
        public Guid ApiKey {get; set;}
        public string KeyIdentifier { get; set; }  // Chiave della configurazione
        public string ConfigData { get; set; }  // Valore della configurazione, di qualsiasi tipo

        Configuration()
        {
            if (KeyIdentifier == null)
            {
                KeyIdentifier = "";
            }
            if (ConfigData == null)
            {
                ConfigData = "";
            }
        }
    }
}