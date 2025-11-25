# 🎬 Movie Website – ASP.NET Core MVC (.NET 8)

Website xem phim trực tuyến được phát triển bằng **ASP.NET Core MVC (.NET 8)** kết hợp **Entity Framework Core**, **SQL Server**, và **Bootstrap 5**.  
Hệ thống bao gồm giao diện người dùng (User) và trang quản trị (Admin), cho phép xem phim, tìm kiếm, đánh giá, bình luận, quản lý nội dung và phân quyền người dùng.

---

## 🚀 Tính năng chính

### 👥 Người dùng (User)
- Đăng ký / đăng nhập / đăng xuất  
- Xác thực email (Email Verification)  
- Quên mật khẩu + đặt lại mật khẩu  
- Xem danh sách phim  
- Tìm kiếm phim theo:
  - Tên phim  
  - Thể loại  
  - Quốc gia  
- Xem chi tiết phim  
- Xem video phim trực tuyến  
- Bình luận phim  
- Đánh giá (1–5 sao)  
- Thêm phim vào danh sách yêu thích  
- Lịch sử xem phim  
- Chỉnh sửa thông tin cá nhân  

---

### 🛠 Quản trị viên (Admin)
- Quản lý phim (thêm, sửa, xóa)
- Quản lý thể loại phim
- Quản lý quốc gia phim
- Quản lý người dùng
- Quản lý bình luận
- Quản lý đánh giá
- Dashboard tổng quan
- Phân quyền User / Admin

---

## 🧱 Công nghệ sử dụng

### 🔹 Backend
- **ASP.NET Core MVC (.NET 8)**
- **Entity Framework Core** (Code First)
- **LINQ**
- Authentication & Authorization (ASP.NET Identity)
- Logging với **NLog** 

### 🔹 Frontend
- **HTML, CSS**
- **Bootstrap 5**
- **Razor Views**

### 🔹 Database
- **SQL Server**

---

## ⚙️ Cách chạy dự án (FULL)

### 1️⃣ Clone dự án
git clone https://github.com/KhietLoi/PhimHay.git

### 2️⃣ Mở solution
Mở file:
MovieWebsite.sln
bằng Visual Studio 2022.

### 3️⃣ Tạo database (nếu chưa có)
CREATE DATABASE PhimHayDB;

### 4️⃣ Sửa connection string trong appsettings.json trong đó YOUR_DATABASE_SERVER là tên SQL Server trên máy bạn
"DefaultConnection": "Server=YOUR_DATABASE_SERVER;Database=PhimHayDB;Trusted_Connection=True;TrustServerCertificate=True;"

### 5️⃣ Cập nhật database
Update-Database

### 6️⃣ Chạy dự án
Nhấn F5 trong Visual Studio.

Truy cập:
https://localhost:7xxx  (port tự sinh)

### 🔑 Tài khoản Admin mặc định
Email: admin@example.com
Mật khẩu: Admin@123


