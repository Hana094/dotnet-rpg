using System.Text.Json.Serialization;
namespace dotnet_rpg.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RpgClass
    {
        Necromancer = 1, Mage = 2, Barbarian = 3
    }
}