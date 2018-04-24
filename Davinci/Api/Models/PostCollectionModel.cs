using System.Collections.Generic;

namespace Davinci.Api.Models
{
    public class PostCollectionModel : BaseApiModel
    {
        public List<PostModel> posts { get; set; }
    }
}