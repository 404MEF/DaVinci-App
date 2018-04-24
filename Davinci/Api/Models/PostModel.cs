using System;

namespace Davinci.Api.Models
{
    public class PostModel
    {
        public DbImage image { get; set; }
        public string owner { get; set; }
        public string category { get; set; }
        public string createdAt { get; set; }
        public int likes { get; set; }
        public int dislikes { get; set; }
        public int LikeRatio
        {
            get
            {
                if (dislikes == 0)
                    return 100;

                return 100 * likes / (likes + dislikes);
            }
        }
    }

    public class DbImage
    {
        public string type { get; set; }
        public byte[] data { get; set; }
    }
}