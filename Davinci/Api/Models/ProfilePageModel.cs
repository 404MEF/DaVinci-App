using System.Collections.Generic;

namespace Davinci.Api.Models
{
    public class ProfilePageModel : BaseApiModel
    {
        public List<PostModel> likedPosts { get; set; }
        public List<PostModel> userPosts { get; set; }
    }
}