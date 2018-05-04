using System;

namespace Davinci.Api.Models
{
    public class PostModel : IEquatable<PostModel>
    {
        public string _id { get; set; }
        public DbOwner owner { get; set; }
        public DbCategory category { get; set; }
        public string image { get; set; }
        public string createdAt { get; set; }
        public int likes { get; set; }
        public int dislikes { get; set; }
        public int LikeRatio
        {
            get
            {
                if (dislikes == 0)
                    return 100;
                if (likes + dislikes == 0)
                    return -1;

                return 100 * likes / (likes + dislikes);
            }
        }

        public bool Equals(PostModel other)
        {
            return this._id == other._id;
        }
    }

    public class DbOwner
    {
        public string _id { get; set; }
        public string username { get; set; }
    }

    public class DbCategory
    {
        public string _id { get; set; }
        public string name { get; set; }
    }
}