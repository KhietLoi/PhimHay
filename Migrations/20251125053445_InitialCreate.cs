using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MovieWebsite.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    MovieId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    Duration = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    RealeaseAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.MovieId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerificationToken = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResetPasswordToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResetTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "MovieCategories",
                columns: table => new
                {
                    MovieId = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    CategoryId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieCategories", x => new { x.MovieId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_MovieCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieCategories_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    CommentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CommentText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MovieId = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_Comments_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    FavoriteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MovieId = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.FavoriteId);
                    table.ForeignKey(
                        name: "FK_Favorites_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favorites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    RatingId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Stars = table.Column<int>(type: "int", nullable: false),
                    RateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MovieId = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.RatingId);
                    table.ForeignKey(
                        name: "FK_Ratings_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ratings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WatchHistories",
                columns: table => new
                {
                    WatchId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WatchedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MovieId = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchHistories", x => x.WatchId);
                    table.ForeignKey(
                        name: "FK_WatchHistories_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WatchHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName" },
                values: new object[,]
                {
                    { "CT1", "Âm Nhạc" },
                    { "CT10", "Phim Lẻ" },
                    { "CT11", "TV Shows" },
                    { "CT2", "Chiến Tranh" },
                    { "CT3", "Cổ Trang" },
                    { "CT4", "Gia Đình" },
                    { "CT5", "Hài Hước" },
                    { "CT6", "Hành Động" },
                    { "CT7", "Khoa Học" },
                    { "CT8", "Kinh Dị" },
                    { "CT9", "Lãng Mạn" }
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "MovieId", "Country", "CreatedAt", "Description", "Duration", "RealeaseAt", "ThumbnailUrl", "Title", "VideoUrl" },
                values: new object[,]
                {
                    { "MV001", "Vietnam", new DateTime(2025, 5, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), "The Avengers assemble once again to stop Thanos from destroying the universe.", "181 minutes", new DateTime(2025, 5, 1, 11, 0, 0, 0, DateTimeKind.Unspecified), "/images/phim1.png", "Avengers: Endgame", "https://www.youtube-nocookie.com/embed/dQw4w9WgXcQ" },
                    { "MV002", "Vietnam", new DateTime(2025, 5, 6, 12, 0, 0, 0, DateTimeKind.Unspecified), "A thief who steals corporate secrets through the use of dream-sharing technology.", "148 minutes", new DateTime(2025, 5, 1, 11, 0, 0, 0, DateTimeKind.Unspecified), "/images/phim2.png", "Inception", "https://www.youtube-nocookie.com/embed/dQw4w9WgXcQ" },
                    { "MV003", "Ấn Độ", new DateTime(2023, 4, 1, 12, 2, 0, 0, DateTimeKind.Unspecified), "Batman faces his most personal and dangerous challenge yet: the Joker, a criminal mastermind who wants to bring Gotham to its knees.", "152 minutes", new DateTime(2008, 5, 1, 11, 0, 0, 0, DateTimeKind.Unspecified), "/images/phim3.png", "The Dark Knight", "https://www.youtube.com/watch?v=YQ6JRqzpASM" },
                    { "MV004", "Nhật Bản", new DateTime(2023, 4, 1, 12, 3, 0, 0, DateTimeKind.Unspecified), "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.", "169 minutes", new DateTime(2008, 5, 1, 11, 0, 0, 0, DateTimeKind.Unspecified), "/images/phim4.jpg", "Interstellar", "https://www.youtube.com/watch?v=example4" },
                    { "MV005", "Nhật Bản", new DateTime(2023, 4, 1, 12, 4, 0, 0, DateTimeKind.Unspecified), "After being held captive in an Afghan cave, billionaire engineer Tony Stark creates a unique weaponized suit of armor.", "126 minutes", new DateTime(2008, 5, 1, 11, 0, 0, 0, DateTimeKind.Unspecified), "/images/phim5.jpg", "Iron Man", "https://www.youtube.com/watch?v=example5" },
                    { "MV006", "Nhật Bản", new DateTime(2023, 4, 1, 12, 5, 0, 0, DateTimeKind.Unspecified), "Political involvement in the Avengers' affairs causes a rift between Captain America and Iron Man.", "147 minutes", new DateTime(2008, 5, 1, 11, 0, 0, 0, DateTimeKind.Unspecified), "/images/phim6.jpg", "Captain America: Civil War", "https://www.youtube.com/watch?v=example6" },
                    { "MV007", "Hoa Kì", new DateTime(2023, 4, 1, 12, 6, 0, 0, DateTimeKind.Unspecified), "While on a journey of physical and spiritual healing, a brilliant neurosurgeon is drawn into the world of the mystic arts.", "115 minutes", new DateTime(2008, 5, 1, 11, 0, 0, 0, DateTimeKind.Unspecified), "/images/phim7.jpg", "Doctor Strange", "https://www.youtube.com/watch?v=example7" },
                    { "MV008", "Hoa Kì", new DateTime(2023, 4, 1, 12, 7, 0, 0, DateTimeKind.Unspecified), "T'Challa returns home to the African nation of Wakanda to take his rightful place as king.", "134 minutes", new DateTime(2008, 5, 1, 11, 0, 0, 0, DateTimeKind.Unspecified), "/images/phim8.jpg", "Black Panther", "https://www.youtube.com/watch?v=example8" },
                    { "MV009", "Hoa Kì", new DateTime(2023, 4, 1, 12, 8, 0, 0, DateTimeKind.Unspecified), "A group of intergalactic criminals must pull together to stop a fanatical warrior with plans to purge the universe.", "121 minutes", new DateTime(2008, 5, 1, 11, 0, 0, 0, DateTimeKind.Unspecified), "/images/phim9.jpg", "Guardians of the Galaxy", "https://www.youtube.com/watch?v=example9" },
                    { "MV010", "Trung Quốc", new DateTime(2023, 4, 1, 12, 9, 0, 0, DateTimeKind.Unspecified), "Thor must escape the alien planet Sakaar in time to save Asgard from Hela and the impending Ragnarok.", "130 minutes", new DateTime(2008, 5, 1, 11, 0, 0, 0, DateTimeKind.Unspecified), "/images/phim10.jpg", "Thor: Ragnarok", "https://www.youtube.com/watch?v=example10" },
                    { "MV011", "Trung Quốc", new DateTime(2023, 4, 1, 12, 10, 0, 0, DateTimeKind.Unspecified), "Peter Parker tries to fix everything after his identity is revealed, but ends up facing multiversal consequences.", "148 minutes", new DateTime(2008, 5, 1, 11, 0, 0, 0, DateTimeKind.Unspecified), "/images/phim11.jpg", "Spider-Man: No Way Home", "https://www.youtube.com/watch?v=example11" },
                    { "MV012", "Hàn Quốc", new DateTime(2023, 4, 1, 12, 11, 0, 0, DateTimeKind.Unspecified), "A hacker learns from mysterious rebels about the true nature of his reality and his role in the war against its controllers.", "136 minutes", new DateTime(2008, 5, 1, 11, 0, 0, 0, DateTimeKind.Unspecified), "/images/phim12.jpg", "The Matrix", "https://www.youtube.com/watch?v=example12" },
                    { "MV013", "Hàn Quốc", new DateTime(2023, 4, 1, 12, 12, 0, 0, DateTimeKind.Unspecified), "A mentally troubled stand-up comedian embarks on a downward spiral that leads to the creation of an iconic villain.", "122 minutes", new DateTime(2008, 5, 1, 11, 0, 0, 0, DateTimeKind.Unspecified), "/images/phim13.jpg", "Joker", "https://www.youtube.com/watch?v=example13" },
                    { "MV014", "Hàn Quốc", new DateTime(2023, 4, 1, 12, 13, 0, 0, DateTimeKind.Unspecified), "Armed with only one word, Tenet, a protagonist fights for the survival of the world.", "150 minutes", new DateTime(2008, 5, 1, 11, 0, 0, 0, DateTimeKind.Unspecified), "/images/phim14.jpg", "Tenet", "https://www.youtube.com/watch?v=example14" },
                    { "MV015", "Vietnam", new DateTime(2023, 4, 1, 12, 14, 0, 0, DateTimeKind.Unspecified), "Paul Atreides leads nomadic tribes in a battle to control the desert planet Arrakis.", "155 minutes", new DateTime(2008, 5, 1, 11, 0, 0, 0, DateTimeKind.Unspecified), "/images/phim15.jpg", "Dune", "https://www.youtube.com/watch?v=example15" },
                    { "MV016", "Vietnam", new DateTime(2025, 4, 1, 12, 15, 0, 0, DateTimeKind.Unspecified), "A paraplegic Marine dispatched to the moon Pandora on a unique mission becomes torn between following orders and protecting an alien civilization.", "162 minutes", new DateTime(2008, 5, 1, 11, 0, 0, 0, DateTimeKind.Unspecified), "/images/phim16.jpg", "Avatar", "https://www.youtube.com/watch?v=example16" },
                    { "MV017", "Vietnam", new DateTime(2025, 4, 1, 12, 16, 0, 0, DateTimeKind.Unspecified), "Shang-Chi must confront the past he thought he left behind when he is drawn into the web of the mysterious Ten Rings organization.", "132 minutes", new DateTime(2008, 5, 1, 11, 0, 0, 0, DateTimeKind.Unspecified), "/images/phim17.jpg", "Shang-Chi and the Legend of the Ten Rings", "https://www.youtube.com/watch?v=example17" },
                    { "MV018", "Vietnam", new DateTime(2025, 4, 1, 12, 17, 0, 0, DateTimeKind.Unspecified), "The saga of the Eternals, a race of immortal beings who lived on Earth and shaped its history and civilizations.", "157 minutes", new DateTime(2008, 5, 1, 11, 0, 0, 0, DateTimeKind.Unspecified), "/images/phim18.jpg", "Eternals", "https://www.youtube.com/watch?v=example18" },
                    { "MV019", "Vietnam", new DateTime(2025, 4, 1, 12, 18, 0, 0, DateTimeKind.Unspecified), "Batman ventures into Gotham City's underworld when a sadistic killer leaves behind a trail of cryptic clues.", "176 minutes", new DateTime(2008, 5, 1, 11, 0, 0, 0, DateTimeKind.Unspecified), "/images/phim19.jpg", "The Batman", "https://www.youtube.com/watch?v=example19" },
                    { "MV020", "Vietnam", new DateTime(2025, 4, 1, 12, 19, 0, 0, DateTimeKind.Unspecified), "The Avengers and their allies must be willing to sacrifice all in an attempt to defeat the powerful Thanos.", "149 minutes", new DateTime(2008, 5, 1, 11, 0, 0, 0, DateTimeKind.Unspecified), "/images/phim20.jpg", "Avengers: Infinity War", "https://www.youtube.com/watch?v=example20" },
                    { "MV021", "Japan", new DateTime(2025, 5, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), "Luffy", "23 minutes", new DateTime(2025, 5, 10, 10, 0, 0, 0, DateTimeKind.Unspecified), "/images/phim21.jpg", "Onepiece tap 419", "https://www.youtube.com/embed/df5mticutx8" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "AvatarUrl", "CreatedAt", "Email", "IsVerified", "PasswordHash", "ResetPasswordToken", "ResetTokenExpiryTime", "Role", "TokenExpiryTime", "UserName", "VerificationToken" },
                values: new object[] { "U001", "/images/avt_macdinh.jpg", new DateTime(2023, 4, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), "admin@example.com", true, "$2a$12$4UBBXElUopV4./fG2gNQJeVZnbRTI7dqpTUOW0ORZjp0eY35AAreW", null, null, "admin", null, "admin", null });

            migrationBuilder.InsertData(
                table: "MovieCategories",
                columns: new[] { "CategoryId", "MovieId" },
                values: new object[,]
                {
                    { "CT10", "MV001" },
                    { "CT6", "MV001" },
                    { "CT7", "MV002" },
                    { "CT6", "MV003" },
                    { "CT8", "MV003" },
                    { "CT7", "MV004" },
                    { "CT6", "MV005" },
                    { "CT2", "MV006" },
                    { "CT6", "MV006" },
                    { "CT7", "MV007" },
                    { "CT4", "MV008" },
                    { "CT11", "MV009" },
                    { "CT6", "MV010" },
                    { "CT10", "MV011" },
                    { "CT7", "MV012" },
                    { "CT8", "MV013" },
                    { "CT7", "MV014" },
                    { "CT5", "MV015" },
                    { "CT7", "MV015" },
                    { "CT7", "MV016" },
                    { "CT10", "MV017" },
                    { "CT6", "MV017" },
                    { "CT7", "MV018" },
                    { "CT8", "MV019" },
                    { "CT6", "MV020" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_MovieId",
                table: "Comments",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_MovieId",
                table: "Favorites",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId",
                table: "Favorites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieCategories_CategoryId",
                table: "MovieCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_MovieId",
                table: "Ratings",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_UserId",
                table: "Ratings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchHistories_MovieId",
                table: "WatchHistories",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchHistories_UserId",
                table: "WatchHistories",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "MovieCategories");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "WatchHistories");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
