using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieWebsite.Data;
using MovieWebsite.Models;
using Microsoft.Extensions.Logging;

namespace MovieWebsite.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(AppDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Bắt đầu tải dữ liệu tổng quan cho trang admin");

            ViewBag.TotalMovies = await _context.Movies.CountAsync();
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalComments = await _context.Comments.CountAsync();
            ViewBag.TotalRatings = await _context.Ratings.CountAsync();

            _logger.LogInformation("Tải dữ liệu tổng quan thành công");
            return View();
        }

        // MovieAdminViewModel to hold the data for the Movies management page
        public class MovieAdminViewModel
        {
            public String Id { get; set; }
            public string Title { get; set; }
            public double Rating { get; set; }
            public string Categories { get; set; }
            public int Views { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        // UserAdminViewModel to hold the data for the Users management page
        public class UserAdminViewModel
        {
            public string Id { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string AvatarUrl { get; set; }
            public int CommentCount { get; set; }
            public int RatingCount { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        //public class CommentAdminViewModel
        //{
        //    public string CommentId { get; set; }
        //    public string CommentText { get; set; }
        //    public DateTime CreatedAt { get; set; }
        //    public string UserId { get; set; }
        //    public string UserName { get; set; }
        //    public string MovieId { get; set; }
        //    public string MovieTitle { get; set; }
        //}


        // GET: /Admin/Movies
        public async Task<IActionResult> Movies(string searchTerm, string sortOrder)
        {
            _logger.LogInformation("Bắt đầu tải danh sách phim - SearchTerm: {SearchTerm}, SortOrder: {SortOrder}", searchTerm, sortOrder);
            var moviesQuery = _context.Movies
                .Include(m => m.MovieCategories).ThenInclude(mc => mc.Category)
                .Select(m => new MovieAdminViewModel
                {
                    Id = m.MovieId,
                    Title = m.Title,
                    CreatedAt = m.CreatedAt,
                    Categories = string.Join(", ", m.MovieCategories.Select(mc => mc.Category.CategoryName)),
                    Views = _context.WatchHistories.Count(w => w.MovieId == m.MovieId),
                    Rating = _context.Ratings.Where(r => r.MovieId == m.MovieId).Any()
                        ? _context.Ratings.Where(r => r.MovieId == m.MovieId).Average(r => (double?)r.Stars) ?? 0
                        : 0
                });

            // FILTER (Search)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                moviesQuery = moviesQuery.Where(m => m.Title.Contains(searchTerm));
                _logger.LogInformation("Áp dụng bộ lọc tìm kiếm cho danh sách phim: {SearchTerm}", searchTerm);
            }

            // SORT
            switch (sortOrder)
            {
                case "views":
                    moviesQuery = moviesQuery.OrderByDescending(m => m.Views);
                    _logger.LogInformation("Sắp xếp danh sách phim theo lượt xem");
                    break;
                case "rating":
                    moviesQuery = moviesQuery.OrderByDescending(m => m.Rating);
                    _logger.LogInformation("Sắp xếp danh sách phim theo đánh giá");
                    break;
                case "title":
                    moviesQuery = moviesQuery.OrderBy(m => m.Title);
                    _logger.LogInformation("Sắp xếp danh sách phim theo tiêu đề");
                    break;
                case "latest":
                default:
                    moviesQuery = moviesQuery.OrderByDescending(m => m.CreatedAt);
                    _logger.LogInformation("Sắp xếp danh sách phim theo thời gian tạo mới nhất");
                    break;
            }

            var movies = await moviesQuery.ToListAsync();

            ViewData["TotalMovies"] = movies.Count;
            ViewData["SearchTerm"] = searchTerm;
            ViewData["SortOrder"] = sortOrder;

            _logger.LogInformation("Tải danh sách phim thành công - Tổng số: {TotalMovies}", movies.Count);
            return View(movies);
        }

        // GET: /Admin/AddMovie
        [HttpGet]
        public async Task<IActionResult> AddMovie()
        {
            _logger.LogInformation("Bắt đầu tải trang thêm phim mới");

            // Load all available categories for selection
            ViewBag.Categories = await _context.Categories.ToListAsync();
            _logger.LogInformation("Tải danh sách danh mục thành công cho trang thêm phim");
            return View();
        }

        // POST: /Admin/AddMovie
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMovie(Movie model, string[] SelectedCategories, IFormFile thumbnailFile)
        {
            _logger.LogInformation("Bắt đầu xử lý thêm phim mới - Title: {Title}", model.Title);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Thêm phim thất bại - Dữ liệu không hợp lệ cho phim: {Title}", model.Title);
                ViewBag.Categories = await _context.Categories.ToListAsync();
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"{key}: {error.ErrorMessage}");
                    }
                }
                return View(model);
            }

            // Handle thumbnail file upload if provided
            if (thumbnailFile != null && thumbnailFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(thumbnailFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "thumbnails", fileName);

                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await thumbnailFile.CopyToAsync(stream);
                }
                // Set the thumbnail URL to the uploaded file path
                model.ThumbnailUrl = "/uploads/thumbnails/" + fileName;
                _logger.LogInformation("Tải lên thumbnail thành công cho phim: {Title}, FilePath: {FilePath}", model.Title, filePath);
            }

            // Find the highest movie ID number after "MV" prefix
            string highestMovieId = await _context.Movies
                .Select(m => m.MovieId)
                .Where(id => id.StartsWith("MV"))
                .OrderByDescending(id => id)
                .FirstOrDefaultAsync() ?? "MV000";

            // Extract the numeric part, parse it to int, and increment
            int currentHighestNumber = 0;
            if (highestMovieId.Length >= 5 && int.TryParse(highestMovieId.Substring(2), out int parsedNumber))
            {
                currentHighestNumber = parsedNumber;
            }

            // Generate the next ID
            string nextId = $"MV{(currentHighestNumber + 1).ToString("D3")}"; // MV001, MV002...
            model.MovieId = nextId;
            model.CreatedAt = DateTime.Now;

            // First, save the movie
            _context.Movies.Add(model);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Thêm phim thành công - MovieId: {MovieId}", model.MovieId);
            // Then, add category relationships if any categories were selected
            if (SelectedCategories != null && SelectedCategories.Length > 0)
            {
                foreach (var categoryId in SelectedCategories)
                {
                    var movieCategory = new MovieCategory
                    {
                        MovieId = model.MovieId,
                        CategoryId = categoryId
                    };
                    _context.MovieCategories.Add(movieCategory);
                }
                await _context.SaveChangesAsync();
                _logger.LogInformation("Cập nhật danh mục thành công cho phim: {MovieId}", model.MovieId);
            }

            TempData["SuccessMessage"] = "Movie added successfully!";
            return RedirectToAction("Movies");
        }

        // GET: /Admin/EditMovie/{id}
        [HttpGet]
        public async Task<IActionResult> EditMovie(string id)
        {
            _logger.LogInformation("Bắt đầu tải trang chỉnh sửa phim - MovieId: {Id}", id);
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                _logger.LogWarning("Không tìm thấy phim để chỉnh sửa - MovieId: {Id}", id);
                return NotFound();
            }

            // Get all categories
            ViewBag.Categories = await _context.Categories.ToListAsync();

            // Get currently selected categories for this movie
            var selectedCategoryIds = await _context.MovieCategories
                .Where(mc => mc.MovieId == id)
                .Select(mc => mc.CategoryId)
                .ToListAsync();

            ViewBag.SelectedCategories = selectedCategoryIds;
            _logger.LogInformation("Tải trang chỉnh sửa phim thành công - MovieId: {Id}", id);
            return View(movie);
        }

        // POST: /Admin/EditMovie/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMovie(String id, Movie model, string[] SelectedCategories, IFormFile thumbnailFile, string thumbnailOption, string thumbnailUrlInput)
        {
            _logger.LogInformation("Bắt đầu chỉnh sửa phim - MovieId: {Id}, Title: {Title}", id, model.Title);
            if (id != model.MovieId)
            {
                _logger.LogWarning("Chỉnh sửa phim thất bại - ID không khớp - MovieId: {Id}", id);
                return BadRequest();
            }

            if (!ModelState.IsValid && thumbnailOption != "keep")
            {
                _logger.LogWarning("Chỉnh sửa phim thất bại - Dữ liệu không hợp lệ - MovieId: {Id}", id);
                ViewBag.Categories = await _context.Categories.ToListAsync();
                var selectedCategoryIds = await _context.MovieCategories
                    .Where(mc => mc.MovieId == id)
                    .Select(mc => mc.CategoryId)
                    .ToListAsync();
                ViewBag.SelectedCategories = selectedCategoryIds;
                return View(model);
            }

            try
            {
                var movie = await _context.Movies.FindAsync(id);
                if (movie == null)
                {
                    _logger.LogWarning("Chỉnh sửa phim thất bại - Không tìm thấy phim - MovieId: {Id}", id);
                    return NotFound();
                }

                // Handle thumbnail based on the selected option
                switch (thumbnailOption)
                {
                    case "upload":
                        // Handle thumbnail file upload if provided
                        if (thumbnailFile != null && thumbnailFile.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(thumbnailFile.FileName);
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "thumbnails", fileName);

                            // Ensure directory exists
                            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await thumbnailFile.CopyToAsync(stream);
                            }

                            // Set the thumbnail URL to the uploaded file path
                            model.ThumbnailUrl = "/uploads/thumbnails/" + fileName;
                            _logger.LogInformation("Tải lên thumbnail mới thành công - MovieId: {Id}, FilePath: {FilePath}", id, filePath);
                        }
                        break;

                    case "url":
                        // Use the URL provided in the input field
                        if (!string.IsNullOrWhiteSpace(thumbnailUrlInput))
                        {
                            model.ThumbnailUrl = thumbnailUrlInput;
                            _logger.LogInformation("Cập nhật thumbnail bằng URL thành công - MovieId: {Id}, URL: {URL}", id, thumbnailUrlInput);
                        }
                        break;

                    case "keep":
                        if (!string.IsNullOrWhiteSpace(movie.ThumbnailUrl))
                        {
                            _logger.LogInformation("URL trong");
                        }
                        else
                        {
                            model.ThumbnailUrl = movie.ThumbnailUrl;
                        }
                        break;
                    default:
                        // Keep the existing thumbnail URL
                        break;
                }

                // Update movie information
                movie.Title = model.Title;
                movie.Description = model.Description;
                movie.Duration = model.Duration;
                movie.ThumbnailUrl = model.ThumbnailUrl;
                movie.VideoUrl = model.VideoUrl;
                movie.Country = model.Country;
                movie.RealeaseAt = model.RealeaseAt;

                _context.Update(movie);

                // Update movie categories
                // First, remove all existing category relationships
                var existingCategories = await _context.MovieCategories
                    .Where(mc => mc.MovieId == id)
                    .ToListAsync();

                _context.MovieCategories.RemoveRange(existingCategories);

                // Then add the newly selected categories
                if (SelectedCategories != null && SelectedCategories.Length > 0)
                {
                    foreach (var categoryId in SelectedCategories)
                    {
                        var movieCategory = new MovieCategory
                        {
                            MovieId = movie.MovieId,
                            CategoryId = categoryId
                        };
                        _context.MovieCategories.Add(movieCategory);
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Chỉnh sửa phim thành công - MovieId: {Id}", id);
                TempData["SuccessMessage"] = "Movie updated successfully!";
                return RedirectToAction("Movies");
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error updating movie: {ex.Message}");
                ModelState.AddModelError("", "Something went wrong.");
                _logger.LogError(ex, "Lỗi khi chỉnh sửa phim - MovieId: {Id}", id);

                ViewBag.Categories = await _context.Categories.ToListAsync();
                var selectedCategoryIds = await _context.MovieCategories
                    .Where(mc => mc.MovieId == id)
                    .Select(mc => mc.CategoryId)
                    .ToListAsync();
                ViewBag.SelectedCategories = selectedCategoryIds;

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMovie(string id)
        {
            _logger.LogInformation("Bắt đầu xóa phim - MovieId: {Id}", id);
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Xóa phim thất bại - ID không hợp lệ");
                return BadRequest();
            }
                

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                _logger.LogWarning("Xóa phim thất bại - Không tìm thấy phim với ID: {Id}", id);
                return NotFound();
            }
                

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Xóa phim thành công - MovieId: {Id}", id);
            TempData["SuccessMessage"] = "Movie deleted successfully!";
            return RedirectToAction("Movies");
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Users(string searchTerm, string sortOrder)
        {
            _logger.LogInformation("Bắt đầu tải danh sách người dùng - SearchTerm: {SearchTerm}, SortOrder: {SortOrder}", searchTerm, sortOrder);

            var usersQuery = _context.Users
                .Select(u => new UserAdminViewModel
                {
                    Id = u.UserId,
                    Username = u.UserName,
                    Email = u.Email,
                    AvatarUrl = u.AvatarUrl ?? "/images/default-profile.png",
                    CommentCount = _context.Comments.Count(c => c.UserId == u.UserId),
                    RatingCount = _context.Ratings.Count(r => r.UserId == u.UserId),
                    CreatedAt = u.CreatedAt
                });

            // FILTER (Search)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                usersQuery = usersQuery.Where(u =>
                    u.Username.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm));
                _logger.LogInformation("Áp dụng bộ lọc tìm kiếm cho danh sách người dùng: {SearchTerm}", searchTerm);
            }

            // SORT
            switch (sortOrder)
            {
                case "alphabetical":
                    usersQuery = usersQuery.OrderBy(u => u.Username);
                    _logger.LogInformation("Sắp xếp danh sách người dùng theo thứ tự bảng chữ cái");
                    break;
                case "newest":
                default:
                    usersQuery = usersQuery.OrderByDescending(u => u.CreatedAt);
                    _logger.LogInformation("Sắp xếp danh sách người dùng theo thời gian tạo mới nhất");
                    break;
            }

            var users = await usersQuery.ToListAsync();

            ViewData["TotalUsers"] = users.Count;
            ViewData["SearchTerm"] = searchTerm;
            ViewData["SortOrder"] = sortOrder;

            _logger.LogInformation("Tải danh sách người dùng thành công - Tổng số: {TotalUsers}", users.Count);
            return View(users);
        }

        // GET: /Admin/EditUser/{id}
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            _logger.LogInformation("Bắt đầu tải trang chỉnh sửa người dùng - UserId: {Id}", id);
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Không tìm thấy người dùng để chỉnh sửa - UserId: {Id}", id);
                return NotFound();
            }
            // Count related data for this user
            ViewBag.CommentCount = await _context.Comments.CountAsync(c => c.UserId == id);
            ViewBag.RatingCount = await _context.Ratings.CountAsync(r => r.UserId == id);
            _logger.LogInformation("Tải trang chỉnh sửa người dùng thành công - UserId: {Id}", id);
            return View(user);
        }

        // POST: /Admin/EditUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, [Bind("UserId,UserName,Email,AvatarUrl")] User model)
        {
            _logger.LogInformation("Bắt đầu chỉnh sửa người dùng - UserId: {Id}, Username: {Username}", id, model.UserName);
            // Always load statistics regardless of validation outcome
            ViewBag.CommentCount = await _context.Comments.CountAsync(c => c.UserId == id);
            ViewBag.RatingCount = await _context.Ratings.CountAsync(r => r.UserId == id);

            if (id != model.UserId)
            {
                _logger.LogWarning("Chỉnh sửa người dùng thất bại - ID không khớp - UserId: {Id}", id);
                TempData["ErrorMessage"] = "User ID mismatch detected.";
                return BadRequest();
            }

            // Load the original user
            var userToUpdate = await _context.Users.FindAsync(id);
            if (userToUpdate == null)
            {
                _logger.LogWarning("Chỉnh sửa người dùng thất bại - Không tìm thấy user - UserId: {Id}", id);
                TempData["ErrorMessage"] = "User not found.";
                return NotFound();
            }

            // Preserve the creation date
            model.CreatedAt = userToUpdate.CreatedAt;

            // Important: Remove ModelState validation errors for password-related fields
            // since we're not updating them in this form
            ModelState.Remove("Password");
            ModelState.Remove("PasswordHash");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Chỉnh sửa người dùng thất bại - Dữ liệu không hợp lệ - UserId: {Id}", id);
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"- {state.Key}: {string.Join(", ", state.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                }

                TempData["ErrorMessage"] = "Please fix the validation errors.";
                return View(model);
            }

            try
            {
                // Update only the editable fields
                userToUpdate.UserName = model.UserName;
                userToUpdate.Email = model.Email;
                userToUpdate.AvatarUrl = string.IsNullOrEmpty(model.AvatarUrl) ? null : model.AvatarUrl;

                // Save changes
                await _context.SaveChangesAsync();

                _logger.LogInformation("Chỉnh sửa người dùng thành công - UserId: {Id}", id);
                TempData["SuccessMessage"] = "User updated successfully!";
                return RedirectToAction("Users");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Lỗi xung đột khi chỉnh sửa người dùng - UserId: {Id}", id);
                ModelState.AddModelError("", "The record has been modified by another user. Please try again.");
                return View(model);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Lỗi cơ sở dữ liệu khi chỉnh sửa người dùng - UserId: {Id}", id);
                // Check for common database issues like unique constraint violations
                if (ex.InnerException?.Message.Contains("duplicate") == true ||
                    ex.InnerException?.Message.Contains("unique") == true)
                {
                    ModelState.AddModelError("", "A user with this username or email already exists.");
                }
                else
                {
                    ModelState.AddModelError("", "Database error occurred while saving changes.");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi chỉnh sửa người dùng - UserId: {Id}", id);
                ModelState.AddModelError("", "An unexpected error occurred while updating the user.");
                return View(model);
            }
        }


        // POST: /Admin/DeleteUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            _logger.LogInformation("Bắt đầu xóa người dùng - UserId: {Id}", id);
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Xóa người dùng thất bại - ID không hợp lệ");
                return BadRequest();
            }
                
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Xóa người dùng thất bại - Không tìm thấy user với ID: {Id}", id);
                return NotFound();
            }
                

            // Optional: Consider if you want to delete related data or just disable the user
            // You may want to add a "IsActive" flag instead of permanent deletion

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Xóa người dùng thành công - UserId: {Id}", id);
            TempData["SuccessMessage"] = "User deleted successfully!";
            return RedirectToAction("Users");
        }

        //// GET: /Admin/AddUser
        //[HttpGet]
        //public IActionResult AddUser()
        //{
        //    return View();
        //}

        //// POST: /Admin/AddUser
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddUser(User model, string password)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // Generate a user ID (similar to movie ID pattern)
        //    int totalUsers = await _context.Users.CountAsync();
        //    string nextId = $"USR{(totalUsers + 1).ToString("D3")}"; // USR001, USR002...

        //    model.UserId = nextId;
        //    model.CreatedAt = DateTime.Now;

        //    // TODO: Hash the password before storing
        //    // model.PasswordHash = HashPassword(password);

        //    _context.Users.Add(model);
        //    await _context.SaveChangesAsync();

        //    TempData["SuccessMessage"] = "User added successfully!";
        //    return RedirectToAction("Users");
        //}


        // Add these methods to your existing AdminController.cs file

        [HttpGet]
        public IActionResult Comments(string sortOrder = "newest", string searchTerm = "")
        {
            _logger.LogInformation("Bắt đầu tải danh sách bình luận - SortOrder: {SortOrder}, SearchTerm: {SearchTerm}", sortOrder, searchTerm);
            ViewData["SortOrder"] = sortOrder;
            ViewData["SearchTerm"] = searchTerm;

            var comments = from c in _context.Comments
                           join u in _context.Users on c.UserId equals u.UserId
                           join m in _context.Movies on c.MovieId equals m.MovieId
                           select new CommentAdminViewModel
                           {
                               CommentId = c.CommentId,
                               CommentText = c.CommentText,
                               CreatedAt = c.CreatedAt,
                               UserId = c.UserId,
                               UserName = u.UserName,
                               MovieId = c.MovieId,
                               MovieTitle = m.Title
                           };

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                comments = comments.Where(c => c.UserName.Contains(searchTerm) ||
                                               c.MovieTitle.Contains(searchTerm) ||
                                               c.CommentText.Contains(searchTerm));
                _logger.LogInformation("Áp dụng bộ lọc tìm kiếm cho danh sách bình luận: {SearchTerm}", searchTerm);
            }

            // Apply sorting
            switch (sortOrder)
            {
                case "oldest":
                    comments = comments.OrderBy(c => c.CreatedAt);
                    _logger.LogInformation("Sắp xếp danh sách bình luận theo thời gian cũ nhất");
                    break;
                case "movie":
                    comments = comments.OrderBy(c => c.MovieTitle);
                    _logger.LogInformation("Sắp xếp danh sách bình luận theo tiêu đề phim");
                    break;
                case "user":
                    comments = comments.OrderBy(c => c.UserName);
                    _logger.LogInformation("Sắp xếp danh sách bình luận theo tên người dùng");
                    break;
                default: // "newest"
                    _logger.LogInformation("Sắp xếp danh sách bình luận theo thời gian mới nhất");
                    comments = comments.OrderByDescending(c => c.CreatedAt);
                    break;
            }
            var commentList = comments.ToList();
            _logger.LogInformation("Tải danh sách bình luận thành công - Tổng số: {Count}", commentList.Count);
            return View(commentList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(string id)
        {
            _logger.LogInformation("Bắt đầu xóa bình luận - CommentId: {Id}", id);
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Xóa bình luận thất bại - ID không hợp lệ");
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                _logger.LogWarning("Xóa bình luận thất bại - Không tìm thấy bình luận với ID: {Id}", id);
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Xóa bình luận thành công - CommentId: {Id}", id);
            TempData["SuccessMessage"] = "Comment deleted successfully.";
            return RedirectToAction(nameof(Comments));
        }

        // Add these methods to your existing AdminController.cs file

        [HttpGet]
        public IActionResult Ratings(string sortOrder = "newest", string searchTerm = "")
        {
            _logger.LogInformation("Bắt đầu tải danh sách đánh giá - SortOrder: {SortOrder}, SearchTerm: {SearchTerm}", sortOrder, searchTerm);

            ViewData["SortOrder"] = sortOrder;
            ViewData["SearchTerm"] = searchTerm;

            var ratings = from r in _context.Ratings
                          join u in _context.Users on r.UserId equals u.UserId
                          join m in _context.Movies on r.MovieId equals m.MovieId
                          select new RatingAdminViewModel
                          {
                              RatingId = r.RatingId,
                              Stars = r.Stars,
                              RateAt = r.RateAt,
                              UserId = r.UserId,
                              UserName = u.UserName,
                              MovieId = r.MovieId,
                              MovieTitle = m.Title
                          };

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ratings = ratings.Where(r => r.UserName.Contains(searchTerm) ||
                                            r.MovieTitle.Contains(searchTerm));
                _logger.LogInformation("Áp dụng bộ lọc tìm kiếm cho danh sách đánh giá: {SearchTerm}", searchTerm);
            }

            // Apply sorting
            switch (sortOrder)
            {
                case "oldest":
                    ratings = ratings.OrderBy(r => r.RateAt);
                    _logger.LogInformation("Sắp xếp danh sách đánh giá theo thời gian cũ nhất");
                    break;
                case "movie":
                    ratings = ratings.OrderBy(r => r.MovieTitle);
                    _logger.LogInformation("Sắp xếp danh sách đánh giá theo tiêu đề phim");
                    break;
                case "user":
                    ratings = ratings.OrderBy(r => r.UserName);
                    _logger.LogInformation("Sắp xếp danh sách đánh giá theo tên người dùng");
                    break;
                case "rating":
                    ratings = ratings.OrderByDescending(r => r.Stars);
                    _logger.LogInformation("Sắp xếp danh sách đánh giá theo số sao");
                    break;
                default: // "newest"
                    ratings = ratings.OrderByDescending(r => r.RateAt);
                    _logger.LogInformation("Sắp xếp danh sách đánh giá theo thời gian mới nhất");
                    break;
            }
            var ratingList = ratings.ToList();
            _logger.LogInformation("Tải danh sách đánh giá thành công - Tổng số: {Count}", ratingList.Count);
            return View(ratingList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRating(string id)
        {
            _logger.LogInformation("Bắt đầu xóa đánh giá - RatingId: {Id}", id);
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Xóa đánh giá thất bại - ID không hợp lệ");
                return NotFound();
            }

            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
            {
                _logger.LogWarning("Xóa đánh giá thất bại - Không tìm thấy đánh giá với ID: {Id}", id);
                return NotFound();
            }

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Xóa đánh giá thành công - RatingId: {Id}", id);
            TempData["SuccessMessage"] = "Rating deleted successfully.";
            return RedirectToAction(nameof(Ratings));
        }
    }
}