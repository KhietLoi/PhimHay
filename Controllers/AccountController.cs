using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieWebsite.Data;
using MovieWebsite.Models;
using MovieWebsite.Service;

//Thêm thư viện JWT vào:
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
//Gửi mail
using MailKit.Net.Smtp;
using MimeKit;

//Ghi log
using Microsoft.Extensions.Logging;
using NLog;

namespace MovieWebsite.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CustomerService _customerService;
        private readonly EmailService _emailService;
        private readonly ILogger<AccountController> _logger;


        public AccountController(AppDbContext context, CustomerService customerService, EmailService emailService, ILogger<AccountController> logger)
        {
            _context = context;
            _customerService = customerService;
            _emailService = emailService;
            _logger = logger;
        }


        //Sinh token:
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my_super_secret_key_1234567890!!"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "MovieWebsite",
                audience: "MovieWebsite",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        //GET: Login
        public IActionResult Login()
        {
            return View(new LoginViewModel());  // Khởi tạo model và truyền vào view
        }
        //GET:Verification Account:
        public IActionResult Verification()
        {
            return View();
        }


        //GET: Register
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }


        //Thêm mới trung gian:
        public IActionResult AfterLogin()
        {
            // Tiêu thụ TempData tại đây (đọc để nó mất đi)
            ViewBag.Success = TempData["SuccessMessage"];
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            _logger.LogInformation("Đăng nhập thử với email: {Email}", model.Email);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Mô hình đăng nhập không hợp lệ cho email: {Email}", model.Email);
                return View(model);
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                _logger.LogWarning("Đăng nhập thất bại cho email: {Email} - Email hoặc mật khẩu không đúng", model.Email);
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                return View(model);
            }
            //Kiểm tra tài khoản đã xác minh chưa:
            if (!user.IsVerified)
            {
                _logger.LogWarning("Đăng nhập thất bại cho email: {Email} - Tài khoản chưa xác minh", model.Email);
                ModelState.AddModelError("", "Tài khoản của bạn chưa được xác minh. Vui lòng kiểm tra email để xác minh tài khoản");
            }

            //  Tạo JWT
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("Avatar", user.AvatarUrl ?? "/images/avt_macdinh.jpg")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my_super_secret_key_1234567890!!"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "MovieWebsite",
                audience: "MovieWebsite",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Lưu JWT vào Cookie
            Response.Cookies.Append("jwt", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddHours(1),
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            _logger.LogInformation("Đăng nhập thành công cho user: {Email}, Role: {Role}", user.Email, user.Role);

            if (user.Role.ToLower() == "admin")
                return RedirectToAction("Index", "Admin");

            /*  return RedirectToAction("Index", "Home");*/
          
            return RedirectToAction("AfterLogin");

        }

        //Sửa ở đây
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            _logger.LogInformation("Đăng ký với email: {Email}", model.Email);
            if (model.Password.Length < 8)
            {
                ModelState.AddModelError("Password", "Mật khẩu phải có ít nhất 8 ký tự.");
                _logger.LogWarning("Đăng ký thất bại cho email: {Email} - Mật khẩu phải có ít nhất 8 ký tự.", model.Email);
            }

            if (!model.Password.Any(char.IsUpper) ||
                !model.Password.Any(char.IsDigit) ||
                !model.Password.Any(c => "!@#$%^&*()".Contains(c)))
            {
                ModelState.AddModelError("Password", "Mật khẩu phải có chữ hoa, số và ký tự đặc biệt.");
                _logger.LogWarning("Đăng ký thất bại cho email: {Email} - Mật khẩu phải có chữ hoa, số và ký tự đặc biệt.", model.Email);
            }
                

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Xác nhận mật khẩu không khớp.");
                _logger.LogWarning("Đăng ký thất bại cho email: {Email} - Xác nhận mật khẩu không khớp.", model.Email);
            }
                

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
            {
                if (!existingUser.IsVerified)
                {
                    // Nếu token hết hạn → tạo mới
                    if (existingUser.TokenExpiryTime < DateTime.UtcNow)
                    {
                        existingUser.VerificationToken = Guid.NewGuid().ToString();
                        existingUser.TokenExpiryTime = DateTime.UtcNow.AddHours(24);
                        await _context.SaveChangesAsync();
                    }

                    await _emailService.SendVerificationEmail(existingUser.Email, existingUser.VerificationToken);
                    _logger.LogInformation("Gửi lại email xác minh cho tài khoản: {Email}", existingUser.Email);
                    TempData["Title"] = "Gửi lại xác minh";
                    TempData["Message"] = "Email đã được đăng ký nhưng chưa xác minh. Đã gửi lại email xác nhận. Vui lòng kiểm tra email.";
                    return RedirectToAction("Verification", "Account");
                }

                ModelState.AddModelError("Email", "Email đã được sử dụng.");
                _logger.LogWarning("Đăng ký thất bại cho email: {Email} - Email đã tồn tại", model.Email);
            }


            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Mô hình đăng ký không hợp lệ cho email: {Email}", model.Email);
                return View(model);
            }

            var customerCode = _customerService.GenerateCustomerCode();
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            var verifyToken = Guid.NewGuid().ToString();

            var user = new User
            {
                UserId = customerCode,
                UserName = model.UserName,
                Email = model.Email,
                PasswordHash = passwordHash,
                Role = "User",
                CreatedAt = DateTime.Now,
                IsVerified = false,
                VerificationToken = verifyToken,
                TokenExpiryTime = DateTime.UtcNow.AddHours(24)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _emailService.SendVerificationEmail(user.Email, verifyToken);
            _logger.LogInformation("Đăng ký thành công cho user: {Email}, Đã gửi email xác minh", user.Email);

            TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng kiểm tra email để xác minh tài khoản";
            return RedirectToAction("Verification", "Account");
        }


        [HttpGet]
        /*public async Task<IActionResult> Verify(string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Thiếu mã xác minh.");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.VerificationToken == token && u.TokenExpiryTime > DateTime.UtcNow);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Liên kết xác minh không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("Login", "Account");
            }

            user.IsVerified = true;
            user.VerificationToken = null;
            user.TokenExpiryTime = null;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tài khoản của bạn đã được xác minh thành công.";
            return RedirectToAction("Verification", "Account");
        }
*/
        [HttpGet]
        public async Task<IActionResult> Verify(string token)
        {
            _logger.LogInformation("Bắt đầu xác minh tài khoản với token: {Token}", token);

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Xác minh thất bại - Token trống");
                TempData["Title"] = "Xác minh thất bại";
                TempData["Message"] = "Thiếu mã xác minh.";
                return RedirectToAction("Verification");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);

            if (user == null)
            {
                _logger.LogWarning("Xác minh thất bại - Token không hợp lệ: {Token}", token);
                TempData["Title"] = "Xác minh thất bại";
                TempData["Message"] = "Liên kết xác minh không hợp lệ.";
                return RedirectToAction("Verification");
            }

            if (user.IsVerified)
            {
                _logger.LogInformation("Tài khoản đã được xác minh trước đó cho email: {Email}", user.Email);
                TempData["Title"] = "Đã xác minh";
                TempData["Message"] = "Tài khoản của bạn đã được xác minh trước đó. Vui lòng đăng nhập.";
                return RedirectToAction("Verification");
            }

            if (user.TokenExpiryTime < DateTime.UtcNow)
            {
                _logger.LogWarning("Xác minh thất bại - Token đã hết hạn cho email: {Email}, Token: {Token}", user.Email, token);
                TempData["Title"] = "Liên kết hết hạn";
                TempData["Message"] = "Liên kết xác minh đã hết hạn. Vui lòng đăng ký lại hoặc yêu cầu gửi lại email.";
                return RedirectToAction("Verification");
            }

            user.IsVerified = true;
            user.VerificationToken = null;
            user.TokenExpiryTime = null;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Xác minh tài khoản thành công cho email: {Email}", user.Email);
            TempData["Title"] = "Xác minh thành công";
            TempData["Message"] = "Tài khoản của bạn đã được xác minh thành công. Bây giờ bạn có thể đăng nhập.";
            return RedirectToAction("Verification");
        }




        public IActionResult Logout()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("Người dùng đăng xuất: {Email}", userEmail);
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Login");
        }


        //Thiết kế một giao diện hiển thị thông tin nguời dùng, trong đó có avt, thông tin họ tên email...
        //Và người dùng có thể chỉnh sửa và cập nhật thông tin của chính mình: // tí làm
        [Authorize] // bảo vệ route

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            /* var userId = HttpContext.Session.GetString("UserId");*/
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // Thêm phương thức cập nhật thônng tin
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string UserName)
        {
            /* var userId = HttpContext.Session.GetString("UserId");*/
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Bắt đầu cập nhật hồ sơ cho user ID: {UserId}", userId);

            if (string.IsNullOrEmpty(userId)) //Nếu ko có id trả về trạng thái login 
            {
                _logger.LogWarning("Cập nhật hồ sơ thất bại - User ID không hợp lệ");
                return RedirectToAction("Login");
            }    

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("Cập nhật hồ sơ thất bại - Không tìm thấy user với ID: {UserId}", userId);
                return RedirectToAction("Login");
            }
                
            user.UserName = UserName;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cập nhật hồ sơ thành công cho user ID: {UserId}, UserName mới: {UserName}", userId, UserName);
            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!"; 
            return RedirectToAction("Profile");
        }

        //Thêm phướng thức upload avatar
        [HttpPost]
        public async Task<IActionResult> UpdateAvatar(IFormFile avatar)
        {
            /* var userId = HttpContext.Session.GetString("UserId");*/
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Bắt đầu cập nhật avatar cho user ID: {UserId}", userId);

            if (avatar != null && avatar.Length > 0)
            {
                _logger.LogWarning("Cập nhật avatar thất bại - Không có tệp ảnh được tải lên cho user ID: {UserId}", userId);
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatar.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatar.CopyToAsync(stream);
                    _logger.LogInformation("Tệp avatar đã được tải lên thành công: {FilePath}", filePath);
                }

                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.AvatarUrl = "/uploads/" + fileName;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Cập nhật avatar thành công cho user ID: {UserId}, FileName: {FileName}", userId, fileName);
                }
                else
                {
                    _logger.LogWarning("Cập nhật avatar thất bại - Không tìm thấy user với ID: {UserId}", userId);
                }
            }
            return RedirectToAction("Profile");
        }
        //thêm ở đây
        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("Thử đổi mật khẩu cho user: {Email}", userEmail);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Mô hình đổi mật khẩu không hợp lệ cho user: {Email}", userEmail);
                return View(model);
            }
            // Kiểm tra độ mạnh của mật khẩu mới
            if (model.NewPassword.Length < 8)
                ModelState.AddModelError("NewPassword", "Mật khẩu phải có ít nhất 8 ký tự.");

            if (!model.NewPassword.Any(char.IsUpper))
                ModelState.AddModelError("NewPassword", "Mật khẩu phải chứa ít nhất một chữ cái in hoa.");

            if (!model.NewPassword.Any(char.IsDigit))
                ModelState.AddModelError("NewPassword", "Mật khẩu phải chứa ít nhất một chữ số.");

            if (!model.NewPassword.Any(c => "!@#$%^&*()".Contains(c)))
                ModelState.AddModelError("NewPassword", "Mật khẩu phải chứa ít nhất một ký tự đặc biệt.");

            // Nếu sau các kiểm tra có lỗi → return lại view
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Đổi mật khẩu thất bại cho user: {Email} - Yêu cầu mật khẩu không đạt", userEmail);
                return View(model);
            }
                
            // Xác thực mật khẩu cũ
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _context.Users.FindAsync(userId);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.OldPassword, user.PasswordHash))
            {
                ModelState.AddModelError("OldPassword", "Mật khẩu cũ không đúng.");
                _logger.LogWarning("Đổi mật khẩu thất bại cho user: {Email} - Mật khẩu cũ không đúng", userEmail);
                return View(model);
            }

            // Cập nhật mật khẩu mới
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Đổi mật khẩu thành công cho user: {Email}", userEmail);
            TempData["SuccessMessage"] = "Đổi mật khẩu thành công.";
            return RedirectToAction("Profile");
        }


        //Giao diện nhập Email:
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }
        //Xử lý gửi mail reset:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            _logger.LogInformation("Bắt đầu xử lý yêu cầu khôi phục mật khẩu cho email: {Email}", model.Email);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Yêu cầu khôi phục mật khẩu thất bại - Dữ liệu không hợp lệ cho email: {Email}", model.Email);
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                _logger.LogWarning("Yêu cầu khôi phục mật khẩu thất bại - Email không tồn tại: {Email}", model.Email);
                TempData["ErrorMessage"] = "Tài khoản không tồn tại.";
                return View(model);
            }

            var token = Guid.NewGuid().ToString();
            user.ResetPasswordToken = token;
            user.ResetTokenExpiryTime = DateTime.UtcNow.AddHours(1);

            await _context.SaveChangesAsync();

            var resetLink = $"http://localhost:5029/Account/ResetPassword?token={token}";
            await _emailService.SendResetPasswordEmail(user.Email, resetLink);

            _logger.LogInformation("Đã gửi email khôi phục mật khẩu thành công cho email: {Email}, Token: {Token}", user.Email, token);
            TempData["SuccessMessage"] = "Đã gửi liên kết khôi phục mật khẩu đến email của bạn.";
            return RedirectToAction("Login");
        }
        //Giao diện đổi mật khẩu

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            return View(new ResetPasswordViewModel { Token = token });
        }

        //Đổi mật khẩu:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            _logger.LogInformation("Bắt đầu xử lý yêu cầu đặt lại mật khẩu với token: {Token}", model.Token);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Đặt lại mật khẩu thất bại - Dữ liệu không hợp lệ với token: {Token}", model.Token);
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.ResetPasswordToken == model.Token &&
                u.ResetTokenExpiryTime > DateTime.UtcNow);

            if (user == null)
            {
                _logger.LogWarning("Đặt lại mật khẩu thất bại - Token không hợp lệ hoặc đã hết hạn: {Token}", model.Token);
                TempData["ErrorMessage"] = "Token không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("ForgotPassword");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            user.ResetPasswordToken = null;
            user.ResetTokenExpiryTime = null;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Đặt lại mật khẩu thành công cho email: {Email}", user.Email);
            TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Login");
        }
    }
}
