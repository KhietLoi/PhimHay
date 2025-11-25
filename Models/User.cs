using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieWebsite.Models
{
    public class User
    {
        [Key]
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên người dùng là bắt buộc.")]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string Email {  get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [StringLength(300, ErrorMessage = "Mật khẩu không được dài quá 300 ký tự.")]
        public required string PasswordHash { get; set; }

        //[Url(ErrorMessage = "Đường dẫn ảnh không hợp lệ")]
        [StringLength(500)]
        public string AvatarUrl { get; set; } = "/images/avt_macdinh.jpg";

        [StringLength(20)]
        public string Role { get; set; } = "User";

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
        //Thêm thuộc tính:
        
        //đã xác minh tài khoản chưa:
        public bool IsVerified {  get; set; }
        //Token xác minh:
        [StringLength(200)]
        public string? VerificationToken {  get; set; }
        public DateTime? TokenExpiryTime { get; set; }

        //Thêm các trường cho đổi mật khẩu:
        public string? ResetPasswordToken {  get; set; }
        public DateTime? ResetTokenExpiryTime { get; set; }




        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<WatchHistory> WatchHistories { get; set; } = new List<WatchHistory>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
