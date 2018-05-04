namespace Davinci.Api.Models
{
    public class SearchSuggestionModel : BaseApiModel
    {
        public CategoryModel[] results { get; set; }
    }
}