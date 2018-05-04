using System.Collections.Generic;

namespace Davinci.Api.Models
{
    public class CategoryModel
    {
        public string _id { get; set; }
        public string name { get; set; }
        public int imagecount { get; set; }
        public List<PostModel> posts { get; set; }
    }
}