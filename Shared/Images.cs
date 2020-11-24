using System.Collections.Generic;

namespace CatMash.Shared
{
    public class Image
    {
        public string Url { get; set; }
        public string Id { get; set; }
    }

    public class ImageContainer
    {
        public List<Image> Images { get; set; }
    }
}
