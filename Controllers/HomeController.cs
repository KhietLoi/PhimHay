using Microsoft.AspNetCore.Mvc;
using MovieWebsite.Data;
using MovieWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace MovieWebsite.Controllers
{
   
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(AppDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult Index()
        {
            _logger.LogInformation("Bắt đầu tải trang chủ");
            //Lấy danh sách phim từ cơ sở dữ liệu:
            var movieList = _context.Movies.ToList();
            _logger.LogInformation("Lấy danh sách phim thành công - Tổng số: {Count}", movieList.Count);
            // Lấy 3 phim đầu tiên để làm nổi bật (ví dụ: hiện banner hoặc carousel)
            var featuredMovies = movieList.Take(3).ToList();
            _logger.LogInformation("Lấy {Count} phim nổi bật thành công", featuredMovies.Count);
            //Trả về ViewModel
            return View(new HomePageViewModel
            {
                FeaturedMovies = featuredMovies,   // mới thêm
                Movies = movieList
            });
        }
    }
}
