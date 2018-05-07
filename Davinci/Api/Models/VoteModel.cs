namespace Davinci.Api.Models
{
    public class VoteModel : BaseApiModel
    {
        public int vote { get; set; }
        public int likes { get; set; }
        public int dislikes { get; set; }

    }
}