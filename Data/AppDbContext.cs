//Tạo thư mục quản lí và kết nối cơ sở dữ liệu
using Microsoft.EntityFrameworkCore;
using MovieWebsite.Models;
namespace MovieWebsite.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MovieCategory> MovieCategories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<WatchHistory> WatchHistories { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MovieCategory>()
                .HasKey(mc => new { mc.MovieId, mc.CategoryId});

            modelBuilder.Entity<Movie>()
                .Property(m => m.Title)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Movie>()
                .Property(m => m.Country)
                .IsRequired()
                .HasMaxLength(60);

            modelBuilder.Entity<Movie>()
                .Property(m => m.VideoUrl)
                .IsRequired();

            // Configure many-to-many relationship between Movie and Category
            modelBuilder.Entity<MovieCategory>()
                .HasOne(mc => mc.Movie)
                .WithMany(m => m.MovieCategories)
                .HasForeignKey(mc => mc.MovieId);

            modelBuilder.Entity<MovieCategory>()
                .HasOne(mc => mc.Category)
                .WithMany(c => c.MovieCategories)
                .HasForeignKey(mc => mc.CategoryId);

            //Thêm dữ liệu cho bảng Movies:
            // Seed dữ liệu cho bảng Movies
            modelBuilder.Entity<Movie>().HasData(
                new Movie
                {
                    MovieId = "MV001",
                    Title = "Avengers: Endgame",
                    Description = "The Avengers assemble once again to stop Thanos from destroying the universe.",
                    Duration = "181 minutes",
                    ThumbnailUrl = "/images/phim1.png",
                    VideoUrl = "https://www.youtube-nocookie.com/embed/dQw4w9WgXcQ",
                    CreatedAt = new DateTime(2025, 05, 06, 12, 0, 0),
                    Country = "Vietnam",
                    RealeaseAt = new DateTime(2025, 05,01,11, 0, 0)
                },
            new Movie
            {
                MovieId = "MV002",
                Title = "Inception",
                Description = "A thief who steals corporate secrets through the use of dream-sharing technology.",
                Duration = "148 minutes",
                ThumbnailUrl = "/images/phim2.png",
                VideoUrl = "https://www.youtube-nocookie.com/embed/dQw4w9WgXcQ",
                CreatedAt = new DateTime(2025, 05, 06, 12, 0, 0),
                Country = "Vietnam",
                RealeaseAt = new DateTime(2025, 05, 01, 11, 0, 0)
            },
            new Movie
            {
                MovieId = "MV003",
                Title = "The Dark Knight",
                Description = "Batman faces his most personal and dangerous challenge yet: the Joker, a criminal mastermind who wants to bring Gotham to its knees.",
                Duration = "152 minutes",
                ThumbnailUrl = "/images/phim3.png",
                VideoUrl = "https://www.youtube.com/watch?v=YQ6JRqzpASM",
                CreatedAt = new DateTime(2023, 04, 01, 12, 2, 0),
                Country = "Ấn Độ",
                RealeaseAt = new DateTime(2008, 05, 01, 11, 0, 0)
            },
            new Movie
            {
                MovieId = "MV004",
                Title = "Interstellar",
                Description = "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.",
                Duration = "169 minutes",
                ThumbnailUrl = "/images/phim4.jpg",
                VideoUrl = "https://www.youtube.com/watch?v=example4",
                CreatedAt = new DateTime(2023, 04, 01, 12, 3, 0),
                Country = "Nhật Bản",
                RealeaseAt = new DateTime(2008, 05, 01, 11, 0, 0)
            },
            new Movie
            {
                MovieId = "MV005",
                Title = "Iron Man",
                Description = "After being held captive in an Afghan cave, billionaire engineer Tony Stark creates a unique weaponized suit of armor.",
                Duration = "126 minutes",
                ThumbnailUrl = "/images/phim5.jpg",
                VideoUrl = "https://www.youtube.com/watch?v=example5",
                CreatedAt = new DateTime(2023, 04, 01, 12, 4, 0),
                Country = "Nhật Bản",
                RealeaseAt = new DateTime(2008, 05, 01, 11, 0, 0)
            },
            new Movie
            {
                MovieId = "MV006",
                Title = "Captain America: Civil War",
                Description = "Political involvement in the Avengers' affairs causes a rift between Captain America and Iron Man.",
                Duration = "147 minutes",
                ThumbnailUrl = "/images/phim6.jpg",
                VideoUrl = "https://www.youtube.com/watch?v=example6",
                CreatedAt = new DateTime(2023, 04, 01, 12, 5, 0),
                Country = "Nhật Bản",
                RealeaseAt = new DateTime(2008, 05, 01, 11, 0, 0)
            },
            new Movie
            {
                MovieId = "MV007",
                Title = "Doctor Strange",
                Description = "While on a journey of physical and spiritual healing, a brilliant neurosurgeon is drawn into the world of the mystic arts.",
                Duration = "115 minutes",
                ThumbnailUrl = "/images/phim7.jpg",
                VideoUrl = "https://www.youtube.com/watch?v=example7",
                CreatedAt = new DateTime(2023, 04, 01, 12, 6, 0),
                Country = "Hoa Kì",
                RealeaseAt = new DateTime(2008, 05, 01, 11, 0, 0)
            },
            new Movie
            {
                MovieId = "MV008",
                Title = "Black Panther",
                Description = "T'Challa returns home to the African nation of Wakanda to take his rightful place as king.",
                Duration = "134 minutes",
                ThumbnailUrl = "/images/phim8.jpg",
                VideoUrl = "https://www.youtube.com/watch?v=example8",
                CreatedAt = new DateTime(2023, 04, 01, 12, 7, 0),
                Country = "Hoa Kì",
                RealeaseAt = new DateTime(2008, 05, 01, 11, 0, 0)
            },
            new Movie
            {
                MovieId = "MV009",
                Title = "Guardians of the Galaxy",
                Description = "A group of intergalactic criminals must pull together to stop a fanatical warrior with plans to purge the universe.",
                Duration = "121 minutes",
                ThumbnailUrl = "/images/phim9.jpg",
                VideoUrl = "https://www.youtube.com/watch?v=example9",
                CreatedAt = new DateTime(2023, 04, 01, 12, 8, 0),
                Country = "Hoa Kì",
                RealeaseAt = new DateTime(2008, 05, 01, 11, 0, 0)
            },
            new Movie
            {
                MovieId = "MV010",
                Title = "Thor: Ragnarok",
                Description = "Thor must escape the alien planet Sakaar in time to save Asgard from Hela and the impending Ragnarok.",
                Duration = "130 minutes",
                ThumbnailUrl = "/images/phim10.jpg",
                VideoUrl = "https://www.youtube.com/watch?v=example10",
                CreatedAt = new DateTime(2023, 04, 01, 12, 9, 0),
                Country = "Trung Quốc",
                RealeaseAt = new DateTime(2008, 05, 01, 11, 0, 0)
            },
            new Movie
            {
                MovieId = "MV011",
                Title = "Spider-Man: No Way Home",
                Description = "Peter Parker tries to fix everything after his identity is revealed, but ends up facing multiversal consequences.",
                Duration = "148 minutes",
                ThumbnailUrl = "/images/phim11.jpg",
                VideoUrl = "https://www.youtube.com/watch?v=example11",
                CreatedAt = new DateTime(2023, 04, 01, 12, 10, 0),
                Country = "Trung Quốc",
                RealeaseAt = new DateTime(2008, 05, 01, 11, 0, 0)
            },
            new Movie
            {
                MovieId = "MV012",
                Title = "The Matrix",
                Description = "A hacker learns from mysterious rebels about the true nature of his reality and his role in the war against its controllers.",
                Duration = "136 minutes",
                ThumbnailUrl = "/images/phim12.jpg",
                VideoUrl = "https://www.youtube.com/watch?v=example12",
                CreatedAt = new DateTime(2023, 04, 01, 12, 11, 0),
                Country = "Hàn Quốc",
                RealeaseAt = new DateTime(2008, 05, 01, 11, 0, 0)
            },
            new Movie
            {
                MovieId = "MV013",
                Title = "Joker",
                Description = "A mentally troubled stand-up comedian embarks on a downward spiral that leads to the creation of an iconic villain.",
                Duration = "122 minutes",
                ThumbnailUrl = "/images/phim13.jpg",
                VideoUrl = "https://www.youtube.com/watch?v=example13",
                CreatedAt = new DateTime(2023, 04, 01, 12, 12, 0),
                Country = "Hàn Quốc",
                RealeaseAt = new DateTime(2008, 05, 01, 11, 0, 0)
            },
            new Movie
            {
                MovieId = "MV014",
                Title = "Tenet",
                Description = "Armed with only one word, Tenet, a protagonist fights for the survival of the world.",
                Duration = "150 minutes",
                ThumbnailUrl = "/images/phim14.jpg",
                VideoUrl = "https://www.youtube.com/watch?v=example14",
                CreatedAt = new DateTime(2023, 04, 01, 12, 13, 0),
                Country = "Hàn Quốc",
                RealeaseAt = new DateTime(2008, 05, 01, 11, 0, 0)
            },
            new Movie
            {
                MovieId = "MV015",
                Title = "Dune",
                Description = "Paul Atreides leads nomadic tribes in a battle to control the desert planet Arrakis.",
                Duration = "155 minutes",
                ThumbnailUrl = "/images/phim15.jpg",
                VideoUrl = "https://www.youtube.com/watch?v=example15",
                CreatedAt = new DateTime(2023, 04, 01, 12, 14, 0),
                Country = "Vietnam",
                RealeaseAt = new DateTime(2008, 05, 01, 11, 0, 0)
            },
            new Movie
            {
                MovieId = "MV016",
                Title = "Avatar",
                Description = "A paraplegic Marine dispatched to the moon Pandora on a unique mission becomes torn between following orders and protecting an alien civilization.",
                Duration = "162 minutes",
                ThumbnailUrl = "/images/phim16.jpg",
                VideoUrl = "https://www.youtube.com/watch?v=example16",
                CreatedAt = new DateTime(2025, 04, 01, 12, 15, 0),
                Country = "Vietnam",
                RealeaseAt = new DateTime(2008, 05, 01, 11, 0, 0)
            },
            new Movie
            {
                MovieId = "MV017",
                Title = "Shang-Chi and the Legend of the Ten Rings",
                Description = "Shang-Chi must confront the past he thought he left behind when he is drawn into the web of the mysterious Ten Rings organization.",
                Duration = "132 minutes",
                ThumbnailUrl = "/images/phim17.jpg",
                VideoUrl = "https://www.youtube.com/watch?v=example17",
                CreatedAt = new DateTime(2025, 04, 01, 12, 16, 0),
                Country = "Vietnam",
                RealeaseAt = new DateTime(2008, 05, 01, 11, 0, 0)
            },
            new Movie
            {
                MovieId = "MV018",
                Title = "Eternals",
                Description = "The saga of the Eternals, a race of immortal beings who lived on Earth and shaped its history and civilizations.",
                Duration = "157 minutes",
                ThumbnailUrl = "/images/phim18.jpg",
                VideoUrl = "https://www.youtube.com/watch?v=example18",
                CreatedAt = new DateTime(2025, 04, 01, 12, 17, 0),
                Country = "Vietnam",
                RealeaseAt = new DateTime(2008, 05, 01, 11, 0, 0)
            },
            new Movie
            {
                MovieId = "MV019",
                Title = "The Batman",
                Description = "Batman ventures into Gotham City's underworld when a sadistic killer leaves behind a trail of cryptic clues.",
                Duration = "176 minutes",
                ThumbnailUrl = "/images/phim19.jpg",
                VideoUrl = "https://www.youtube.com/watch?v=example19",
                CreatedAt = new DateTime(2025, 04, 01, 12, 18, 0),
                Country = "Vietnam",
                RealeaseAt = new DateTime(2008, 05, 01, 11, 0, 0)
            },
            new Movie
            {
                MovieId = "MV020",
                Title = "Avengers: Infinity War",
                Description = "The Avengers and their allies must be willing to sacrifice all in an attempt to defeat the powerful Thanos.",
                Duration = "149 minutes",
                ThumbnailUrl = "/images/phim20.jpg",
                VideoUrl = "https://www.youtube.com/watch?v=example20",
                CreatedAt = new DateTime(2025, 04, 01, 12, 19, 0),
                Country = "Vietnam",
                RealeaseAt = new DateTime(2008, 05, 01, 11, 0, 0)
            },
            new Movie
            {
                MovieId = "MV021",
                Title ="Onepiece tap 419",
                Description ="Luffy",
                Duration = "23 minutes",
                ThumbnailUrl = "/images/phim21.jpg",
                VideoUrl = "https://www.youtube.com/embed/df5mticutx8",
                CreatedAt = new DateTime(2025,05,10,09,0,0,0),
                Country = "Japan",
                RealeaseAt = new DateTime(2025,05,10,10,0,0,0)
            });

            //Thêm dữ liệu mẫu cho thể loại phim:

            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = "CT1", CategoryName = "Âm Nhạc"},
                new Category { CategoryId = "CT2", CategoryName = "Chiến Tranh"},
                new Category { CategoryId = "CT3", CategoryName = "Cổ Trang"},
                new Category { CategoryId= "CT4", CategoryName = "Gia Đình"},
                new Category { CategoryId = "CT5", CategoryName = "Hài Hước"},
                new Category { CategoryId = "CT6", CategoryName = "Hành Động"},
                new Category { CategoryId = "CT7", CategoryName = "Khoa Học"},
                new Category { CategoryId = "CT8", CategoryName = "Kinh Dị"},
                new Category { CategoryId = "CT9", CategoryName = "Lãng Mạn"},
                new Category { CategoryId = "CT10", CategoryName = "Phim Lẻ"},
                new Category { CategoryId = "CT11", CategoryName = "TV Shows"}
            );
            //Xác định thể loại với từng bộ phim:
            modelBuilder.Entity<MovieCategory>().HasData(
                new MovieCategory { MovieId = "MV001", CategoryId = "CT6" }, // Hành Động
                new MovieCategory { MovieId = "MV001", CategoryId = "CT10" }, // Phim Lẻ

                new MovieCategory { MovieId = "MV002", CategoryId = "CT7" }, // Khoa Học

                new MovieCategory { MovieId = "MV003", CategoryId = "CT6" }, // Hành Động
                new MovieCategory { MovieId = "MV003", CategoryId = "CT8" }, // Kinh Dị

                new MovieCategory { MovieId = "MV004", CategoryId = "CT7" }, // Khoa Học

                new MovieCategory { MovieId = "MV005", CategoryId = "CT6" },

                new MovieCategory { MovieId = "MV006", CategoryId = "CT2" }, // Chiến Tranh
                new MovieCategory { MovieId = "MV006", CategoryId = "CT6" },

                new MovieCategory { MovieId = "MV007", CategoryId = "CT7" },

                new MovieCategory { MovieId = "MV008", CategoryId = "CT4" }, // Gia Đình

                new MovieCategory { MovieId = "MV009", CategoryId = "CT11" }, // TV Shows

                new MovieCategory { MovieId = "MV010", CategoryId = "CT6" },

                new MovieCategory { MovieId = "MV011", CategoryId = "CT10" },

                new MovieCategory { MovieId = "MV012", CategoryId = "CT7" },

                new MovieCategory { MovieId = "MV013", CategoryId = "CT8" },

                new MovieCategory { MovieId = "MV014", CategoryId = "CT7" },

                new MovieCategory { MovieId = "MV015", CategoryId = "CT7" },
                new MovieCategory { MovieId = "MV015", CategoryId = "CT5" }, // Hài Hước

                new MovieCategory { MovieId = "MV016", CategoryId = "CT7" },

                new MovieCategory { MovieId = "MV017", CategoryId = "CT6" },
                new MovieCategory { MovieId = "MV017", CategoryId = "CT10" },

                new MovieCategory { MovieId = "MV018", CategoryId = "CT7" },

                new MovieCategory { MovieId = "MV019", CategoryId = "CT8" },

                new MovieCategory { MovieId = "MV020", CategoryId = "CT6" }

            );

            //
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = "U001",
                    UserName = "admin",
                    Email = "admin@example.com",
                    PasswordHash = "$2a$12$4UBBXElUopV4./fG2gNQJeVZnbRTI7dqpTUOW0ORZjp0eY35AAreW",
                    Role = "admin",
                    CreatedAt = new DateTime(2023, 04, 01, 10, 0, 0),
                    IsVerified = true,
                    VerificationToken = null,
                    TokenExpiryTime = null
                }
               
            );


            base.OnModelCreating(modelBuilder);
        }

       

    }
}
