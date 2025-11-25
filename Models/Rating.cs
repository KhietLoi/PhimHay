using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MovieWebsite.Models
{
    public class Rating
    {
        [Key]
        [StringLength(10, ErrorMessage = "Mã đánh giá không được dài quá 10 ký tự.")]
        [Required]
        /*  public string RatingId { get; set; } = string.Empty; */
        public string RatingId { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 10);

        [Range(1, 5, ErrorMessage = "Số sao đánh giá phải từ 1 đến 5.")]
        public int Stars { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime RateAt { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;

        [Required]
        public string MovieId { get; set; } = string.Empty;
        //public Movie Movie { get; set; } = null!;

        // Navigation property
        [ForeignKey("MovieId")]
        public virtual Movie Movie { get; set; }
    }
}
