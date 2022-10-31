using System.Text.Json;

namespace BDO_Project.BDO
{
    /// <summary>
    /// Beer item.
    /// </summary>
    public class ItemBeer
    {
        internal JsonElement webjson;

        public string Name { get; private set; }
        public string TagLine { get; private set; }
        public string Description { get; private set; }
        public string ImageURL { get; private set; }

        public uint Id { get; private set; }


        internal ResultInfo loadJson()
        {
            Id = webjson.GetProperty("id").GetUInt32();
            Name = webjson.GetProperty("name").GetString();
            TagLine = webjson.GetProperty("tagline").GetString();
            Description = webjson.GetProperty("description").GetString();
            ImageURL = webjson.GetProperty("image_url").GetString();
            return ResultInfo.Ok();
        }

        internal string ToHtml()
        {
            return $"<h3>{Name}</h3><div>{TagLine}</div><p><img style=\"max-height: 125px;display: block;\" src=\"{ImageURL}\"/>{Description}</p>";
        }
    }
}
