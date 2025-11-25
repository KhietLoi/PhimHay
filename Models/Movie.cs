using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MovieWebsite.Models
{
    public class Movie
    {
        [Key]
        [StringLength(10, ErrorMessage = "Mã phim không được dài quá 10 ký tự.")]
        [Required]
        public string MovieId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tiêu đề là bắt buộc.")]
        [StringLength(300, ErrorMessage = "Tiêu đề không được dài quá 300 ký tự.")]
        public required string Title { get; set; }

        [StringLength(3000, ErrorMessage = "Mô tả không được dài quá 3000 ký tự.")]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "Thời lượng không được dài quá 50 ký tự.")]
        public string? Duration { get; set; } // Thời lượng phim

        //[Url(ErrorMessage = "Đường dẫn ảnh không hợp lệ.")]
        public string? ThumbnailUrl { get; set; } //Ảnh đại diện phim 

        [Url(ErrorMessage = "Đường dẫn video không hợp lệ.")]
        public string VideoUrl { get; set; } = string.Empty ;

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [StringLength(100, ErrorMessage = "Tên quốc gia không được dài quá 100 ký tự.")]
        public required string Country {  get; set; }

        //Thời điểm phim được sản xuất:
        public DateTime RealeaseAt { get; set; } // Đây là thời điẻm phim được upload

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<WatchHistory> WatchHistories { get; set; } = new List<WatchHistory>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        //[NotMapped]
        //public ICollection<MovieCategory> MoviesCategories { get; set; } = new List<MovieCategory>();
        //[NotMapped]
        public ICollection<MovieCategory> MovieCategories { get; set; } = new List<MovieCategory>();

    }
}
