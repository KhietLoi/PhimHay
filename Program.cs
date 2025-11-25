using MovieWebsite.Data;
using Microsoft.EntityFrameworkCore;
using MovieWebsite.Service;
using NLog.Web;
using NLog;

//Thêm
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MovieWebsite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            // Clear default logging providers and add NLog
            builder.Logging.ClearProviders();
            builder.Host.UseNLog(); // Use NLog as the logging provider

            // Đăng kí DbContext
            builder.Services.AddDbContext<AppDbContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            //Đăng ký CustomerService vào Dependency Injection
            builder.Services.AddScoped<CustomerService>();
            builder.Services.AddScoped<EmailService>();

            /* // Đăng ký dịch vụ Session
             builder.Services.AddSession();*/


            //Đăng kí JWT ở đây:
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                //  Lấy token từ cookie
                var token = context.Request.Cookies["jwt"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                // Nếu chưa xác thực, chuyển hướng về Login
                context.Response.Redirect("/Account/Login");
                context.HandleResponse(); // chặn phản hồi mặc định 401
                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "MovieWebsite",
            ValidAudience = "MovieWebsite",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my_super_secret_key_1234567890!!"))
        };
    });


            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Cấu hình request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            //Thêm mới:
            // Ngăn trình duyệt lưu cache sau khi đăng xuất
            app.Use(async (context, next) =>
            {
                context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
                context.Response.Headers["Pragma"] = "no-cache";
                context.Response.Headers["Expires"] = "0";
                await next();
            });

            /* // Bắt buộc phải có nếu dùng Session
             app.UseSession();*/

            app.UseAuthentication(); // thêm dòng này cho JWT

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
