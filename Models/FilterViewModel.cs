namespace MovieWebsite.Models
{
    public class FilterViewModel
    {
        public List<Movie> Movies { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        // Các tiêu chí lọc
        public string? SelectedCategory { get; set; }
        public string? SelectedCountry { get; set; }
        public string? SortOrder { get; set; }

        // Để render dropdown
        public List<string> Categories { get; set; }
        public List<string> Countries { get; set; }
        //Thêm vào đây mới:
        public string? SearchQuery { get; set; }

        public bool ? IsNew { get; set; }
    }
}
