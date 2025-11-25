using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieWebsite.Data;
using MovieWebsite.Models;
using System.Globalization;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace MovieWebsite.Controllers
{
    public class MovieController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;
        public MovieController(AppDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult Index(int page =1)
        {
            _logger.LogInformation("Bắt đầu tải danh sách phim - Trang: {Page}", page);
            //Số lượng phim mỗi trang:
            int pageSize = 16;
            var totalMovies = _context.Movies.Count();
            var movies = _context.Movies
                .Skip((page-1)*pageSize) // Bỏ qua các bộ phim của trang trước
                .Take(pageSize) //Lấy 10 bộ phim trên trang hiện tại
                .ToList();

            //Tính tổng số trang 
            var totalPages = (int)Math.Ceiling((double)totalMovies / pageSize);
            //Tạo view
            var viewModel = new MovieListViewModel
            {
                Movies = movies,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalMovies = totalMovies
            };

            _logger.LogInformation("Tải danh sách phim thành công - Trang: {Page}, Tổng số phim: {TotalMovies}", page, totalMovies);
            return View(viewModel);
            
        }
        /*        public IActionResult Category(string name)
                {
                    var movies = _context.MovieCategories
                        .Where(mc => mc.Category.CategoryName == name)
                        .Select(mc => mc.Movie)
                        .Distinct()
                        .ToList();

                    ViewBag.CategoryName = name;
                    return View("MovieByCategory", movies);
                }
        */

        //Lọc phim
        public IActionResult Filter(string? category, string? country, string? sort, int page = 1, bool? isNew = null)
        {
            _logger.LogInformation("Bắt đầu lọc phim - Category: {Category}, Country: {Country}, Sort: {Sort}, Page: {Page}, IsNew: {IsNew}", category, country, sort, page, isNew);
            int pageSize = 8;
            var query = _context.Movies.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(m => m.MovieCategories.Any(mc => mc.Category.CategoryName == category));
                _logger.LogInformation("Áp dụng bộ lọc danh mục: {Category}", category);
            }

            if (!string.IsNullOrEmpty(country))
            {
                query = query.Where(m => m.Country == country);
                _logger.LogInformation("Áp dụng bộ lọc quốc gia: {Country}", country);
            }

            if (isNew == true)
            {
                // Ví dụ: phim phát hành trong 30 ngày gần đây
                var recentDate = DateTime.Now.AddDays(-30);
                query = query.Where(m => m.RealeaseAt >= recentDate);
                _logger.LogInformation("Áp dụng bộ lọc phim mới - Từ: {RecentDate}", recentDate);
            }

            // sort logic
            switch (sort)
            {
                case "latest": 
                    query = query.OrderByDescending(m => m.CreatedAt);
                    _logger.LogInformation("Sắp xếp phim theo thời gian tạo mới nhất");
                    break;
                case "az":
                    query = query.OrderBy(m => m.Title);
                    _logger.LogInformation("Sắp xếp phim theo tiêu đề (A-Z)");
                    break;
                case "za":
                    query = query.OrderByDescending(m => m.Title);
                    _logger.LogInformation("Sắp xếp phim theo tiêu đề (Z-A)");
                    break;
            }

            int totalMovies = query.Count();
            var movies = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            /*var viewModel = new FilterViewModel
            {
                Movies = movies,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalMovies / pageSize),
                SelectedCategory = category,
                SelectedCountry = country,
                SortOrder = sort,
                Categories = _context.Categories.Select(c => c.CategoryName).ToList(),
                Countries = _context.Movies.Select(m => m.Country).Distinct().ToList()
            };*/

            var model = new FilterViewModel
            {
                Movies = movies,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalMovies / pageSize),
                SelectedCategory = category,
                SelectedCountry = country,
                SortOrder = sort,
                Categories = _context.Categories.Select(c => c.CategoryName).ToList(),
                Countries = _context.Movies.Select(m => m.Country).Distinct().ToList(),
                IsNew = isNew //Lọc theo phim mới nhất 

            };

            _logger.LogInformation("Lọc phim thành công - Tổng số phim: {TotalMovies}, Trang: {Page}", totalMovies, page);
            return View(model);
        }

        //Hiển thị thông tin chi tiết phim:
        [Authorize]
        public IActionResult Detail(string id)
        {
            _logger.LogInformation("Bắt đầu tải chi tiết phim - MovieId: {Id}", id);
            var movie = _context.Movies
                .Include(m => m.Ratings) // 👈 thêm dòng này để lấy đánh giá
                .FirstOrDefault(m => m.MovieId == id);

            if (movie == null)
            {
                _logger.LogWarning("Không tìm thấy phim - MovieId: {Id}", id);
                return NotFound();
            }

            _logger.LogInformation("Tải chi tiết phim thành công - MovieId: {Id}", id);
            return View(movie);
        }
        //Xem phim
        /*        public IActionResult Watch(string id)
                {
                    var movie = _context.Movies.FirstOrDefault(m => m.MovieId == id);
                    if (movie == null)
                        return NotFound();

                    return View(movie); // Trả về View chứa video player
                }
        */

        [Authorize]

        public async Task<IActionResult> Watch(string id)
        {
            _logger.LogInformation("Bắt đầu xem phim - MovieId: {Id}", id);
            var movie = await _context.Movies
                .Include(m => m.Comments)
                    .ThenInclude(c => c.User)
                .Include(m => m.Ratings)
                .FirstOrDefaultAsync(m => m.MovieId == id);

            if (movie == null)
            {
                _logger.LogWarning("Không tìm thấy phim để xem - MovieId: {Id}", id);
                return NotFound();
            }
                

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                var history = await _context.WatchHistories
                    .FirstOrDefaultAsync(w => w.UserId == userId && w.MovieId == id);

                var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

                if (history == null)
                {
                    // Chưa từng xem → tạo mới
                    history = new WatchHistory
                    {
                        MovieId = id,
                        UserId = userId,
                        WatchedAt = vietnamTime
                    };
                    _context.WatchHistories.Add(history);
                    _logger.LogInformation("Thêm lịch sử xem phim - UserId: {UserId}, MovieId: {MovieId}", userId, id);
                }
                else
                {
                    // Đã xem rồi → cập nhật lại thời gian
                    history.WatchedAt = vietnamTime;
                    _context.WatchHistories.Update(history);
                    _logger.LogInformation("Cập nhật lịch sử xem phim - UserId: {UserId}, MovieId: {MovieId}", userId, id);
                }

                await _context.SaveChangesAsync();
            }
            _logger.LogInformation("Tải trang xem phim thành công - MovieId: {Id}", id);
            return View(movie);
        }

        /*       public IActionResult Watch(string id)
               {
                   var movie = _context.Movies
                       .Include(m => m.Comments)
                           .ThenInclude(c => c.User)
                       .Include(m => m.Ratings)
                       .FirstOrDefault(m => m.MovieId == id);

                   if (movie == null)
                       return NotFound();
                   var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


                   return View(movie);
               }*/
        //Thêm ở movie:
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(string movieId, string commentText)
        {
            _logger.LogInformation("Bắt đầu thêm bình luận - MovieId: {MovieId}", movieId);
            if (string.IsNullOrWhiteSpace(commentText))
            {
                _logger.LogWarning("Thêm bình luận thất bại - Nội dung bình luận trống - MovieId: {MovieId}", movieId);
                return RedirectToAction("Watch", new { id = movieId });
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var comment = new Comment
            {
                CommentText = commentText,
                MovieId = movieId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Thêm bình luận thành công - MovieId: {MovieId}, UserId: {UserId}", movieId, userId);
            return RedirectToAction("Watch", new { id = movieId });

        }


        //Thêm tiếp phân cho Rating:
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RateMovie(string movieId, int stars)
        {
            _logger.LogInformation("Bắt đầu đánh giá phim - MovieId: {MovieId}, Stars: {Stars}", movieId, stars);
            if (stars < 1 || stars > 5) {
                _logger.LogWarning("Đánh giá phim thất bại - Số sao không hợp lệ: {Stars} - MovieId: {MovieId}", stars, movieId);
                return RedirectToAction("Watch", new { id = movieId });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var existing = await _context.Ratings
                .FirstOrDefaultAsync(r => r.MovieId == movieId && r.UserId == userId);

            if (existing != null)
            {
                _logger.LogInformation("Cập nhật đánh giá phim - MovieId: {MovieId}, UserId: {UserId}, Stars: {Stars}", movieId, userId, stars);
                existing.Stars = stars;
            }
            else
            {
                var rating = new Rating
                {
                    MovieId = movieId,
                    UserId = userId,
                    Stars = stars,
                    RateAt = DateTime.UtcNow
                };
                _context.Ratings.Add(rating);
                _logger.LogInformation("Thêm đánh giá phim mới - MovieId: {MovieId}, UserId: {UserId}, Stars: {Stars}", movieId, userId, stars);
            }
            Console.WriteLine("Đã gửi đánh giá");

            await _context.SaveChangesAsync();

            _logger.LogInformation("Đánh giá phim thành công - MovieId: {MovieId}, UserId: {UserId}", movieId, userId);
            TempData["SuccessMessage_rating"] = "Đánh giá thành công!";
            return RedirectToAction("Watch", new { id = movieId });
        }


        //Thêm vào danh mục yêu thích:
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToFavorites(string movieId)
        {
            _logger.LogInformation("Bắt đầu thêm phim vào danh sách yêu thích - MovieId: {MovieId}", movieId);
            Console.WriteLine($"👉 AddToFavorites được gọi với movieId = {movieId}");
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Kiểm tra xem đã có trong yêu thích chưa
            var exists = await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.MovieId == movieId);
            Console.WriteLine("hello");
            if (!exists)
            {
                var favorite = new Favorite
                {
                    MovieId = movieId,
                    UserId = userId,
                    Created = DateTime.UtcNow
                };

                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage_favo"] = "Đã thêm vào danh sách yêu thích!";
                _logger.LogInformation("Thêm phim vào danh sách yêu thích thành công - MovieId: {MovieId}, UserId: {UserId}", movieId, userId);
            }
            else
            {
                TempData["InfoMessage"] = "Phim đã có trong danh sách yêu thích.";
                _logger.LogInformation("Phim đã có trong danh sách yêu thích - MovieId: {MovieId}, UserId: {UserId}", movieId, userId);
            }

            return RedirectToAction("Detail", "Movie", new { id = movieId });
        }
        //Hiển thị danh sách yêu thích của người dùng:
        [Authorize]
        public async Task<IActionResult> MyFavorites()
        {
            _logger.LogInformation("Bắt đầu tải danh sách yêu thích của người dùng");
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var favorites = await _context.Favorites
                .Include(f => f.Movie)
                .Where(f => f.UserId == userId)
                .ToListAsync();
            _logger.LogInformation("Tải danh sách yêu thích thành công - UserId: {UserId}, Tổng số: {Count}", userId, favorites.Count);
            return View(favorites); // Phải có View tương ứng
        }


        //Action Lịch sử xem phim:
        [Authorize]
        public async Task<IActionResult> WatchHistory()
        {
            _logger.LogInformation("Bắt đầu tải lịch sử xem phim của người dùng");
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var history = await _context.WatchHistories
                .Where(w => w.UserId == userId)
                .Include(w => w.Movie)
                .OrderByDescending(w => w.WatchedAt)
                .ToListAsync();
            _logger.LogInformation("Tải lịch sử xem phim thành công - UserId: {UserId}, Tổng số: {Count}", userId, history.Count);
            return View(history);
        }
        [HttpGet]
        public IActionResult Search(string query, int page = 1)
        {
            _logger.LogInformation("Bắt đầu tìm kiếm phim - Query: {Query}, Trang: {Page}", query, page);
            if (string.IsNullOrWhiteSpace(query))
            {
                _logger.LogWarning("Tìm kiếm thất bại - Từ khóa trống");
                TempData["ErrorMessage"] = "Vui lòng nhập từ khóa để tìm kiếm.";
                return RedirectToAction("Index"); // hoặc trả về View("Filter") rỗng
            }

            var normalizedQuery = query.ToLower();

            var movies = _context.Movies
                .Where(m =>
                    (!string.IsNullOrEmpty(m.Title) && m.Title.ToLower().Contains(normalizedQuery)) ||
                    (!string.IsNullOrEmpty(m.Description) && m.Description.ToLower().Contains(normalizedQuery)) ||
                    (!string.IsNullOrEmpty(m.Country) && m.Country.ToLower().Contains(normalizedQuery))
                )
                .OrderByDescending(m => m.Title.ToLower().Contains(normalizedQuery)) // Ưu tiên kết quả trùng
                .ThenByDescending(m => m.Description != null && m.Description.ToLower().Contains(normalizedQuery))
                .ToList();

            int pageSize = 8;
            int totalMovies = movies.Count;
            var pagedMovies = movies.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var model = new FilterViewModel
            {
                Movies = pagedMovies,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalMovies / pageSize),
                SearchQuery = query
            };
            _logger.LogInformation("Tìm kiếm phim thành công - Query: {Query}, Tổng số phim: {TotalMovies}", query, totalMovies);
            return View("Filter", model);
        }





    }
}
