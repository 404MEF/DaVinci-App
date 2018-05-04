using System.Collections.Generic;

namespace Davinci.Api.Models
{
    public class FeedModel : BaseApiModel
    {
        public FeedDataModel data { get; set; }
    }

    public class FeedDataModel
    {
        public List<CategoryModel> follows;
    }
}