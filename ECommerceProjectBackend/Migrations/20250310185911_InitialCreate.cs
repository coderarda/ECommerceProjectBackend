using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceProjectBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    userId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    userName = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    userEmail = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    userBirthDate = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    userGender = table.Column<sbyte>(type: "tinyint", nullable: false),
                    userPassword = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.userId);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    commentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    commentTitle = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    commentText = table.Column<string>(type: "text", nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    User_userId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.commentId);
                    table.ForeignKey(
                        name: "fk_Comment_User1",
                        column: x => x.User_userId,
                        principalTable: "User",
                        principalColumn: "userId");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "PaymentInfo",
                columns: table => new
                {
                    paymentInfoId = table.Column<int>(type: "int", nullable: false),
                    paymentInfoCreditCardNumber = table.Column<int>(type: "int", nullable: true),
                    paymentInfoCVV = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    paymentInfoExpirationDate = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    paymentInfoCreditCardUserName = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    paymentInfoTitle = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    User_userId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.paymentInfoId);
                    table.ForeignKey(
                        name: "fk_PaymentInfo_User1",
                        column: x => x.User_userId,
                        principalTable: "User",
                        principalColumn: "userId");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "Seller",
                columns: table => new
                {
                    sellerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    sellerTitle = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    sellerRating = table.Column<float>(type: "float", nullable: true),
                    User_userId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.sellerId);
                    table.ForeignKey(
                        name: "fk_Seller_User1",
                        column: x => x.User_userId,
                        principalTable: "User",
                        principalColumn: "userId");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    productId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    productTitle = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    productPrice = table.Column<int>(type: "int", nullable: false),
                    ProductCategory = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    ProductImage = table.Column<byte[]>(type: "longblob", nullable: false),
                    productDescription = table.Column<string>(type: "text", nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Comment_commentId = table.Column<int>(type: "int", nullable: false),
                    Seller_sellerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.productId);
                    table.ForeignKey(
                        name: "fk_Product_Comment1",
                        column: x => x.Comment_commentId,
                        principalTable: "Comment",
                        principalColumn: "commentId");
                    table.ForeignKey(
                        name: "fk_Product_Seller1",
                        column: x => x.Seller_sellerId,
                        principalTable: "Seller",
                        principalColumn: "sellerId");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateIndex(
                name: "commentId_UNIQUE",
                table: "Comment",
                column: "commentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_Comment_User1_idx",
                table: "Comment",
                column: "User_userId");

            migrationBuilder.CreateIndex(
                name: "fk_PaymentInfo_User1_idx",
                table: "PaymentInfo",
                column: "User_userId");

            migrationBuilder.CreateIndex(
                name: "fk_Product_Comment1_idx",
                table: "Products",
                column: "Comment_commentId");

            migrationBuilder.CreateIndex(
                name: "fk_Product_Seller1_idx",
                table: "Products",
                column: "Seller_sellerId");

            migrationBuilder.CreateIndex(
                name: "productId_UNIQUE",
                table: "Products",
                column: "productId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "fk_Seller_User1_idx",
                table: "Seller",
                column: "User_userId");

            migrationBuilder.CreateIndex(
                name: "userEmail_UNIQUE",
                table: "User",
                column: "userEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "userId_UNIQUE",
                table: "User",
                column: "userId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentInfo");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Seller");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
