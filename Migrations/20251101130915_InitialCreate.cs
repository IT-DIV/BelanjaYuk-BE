using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BelanjaYuk.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LtCategory",
                columns: table => new
                {
                    IdCategory = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateUp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserUp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LtCategory", x => x.IdCategory);
                });

            migrationBuilder.CreateTable(
                name: "LtGender",
                columns: table => new
                {
                    IdGender = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GenderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateUp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserUp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LtGender", x => x.IdGender);
                });

            migrationBuilder.CreateTable(
                name: "LtPayment",
                columns: table => new
                {
                    IdPayment = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PaymentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateUp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserUp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LtPayment", x => x.IdPayment);
                });

            migrationBuilder.CreateTable(
                name: "MsProduct",
                columns: table => new
                {
                    IdProduct = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountProduct = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    DateIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateUp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserUp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdUserSeller = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdCategory = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MsProduct", x => x.IdProduct);
                });

            migrationBuilder.CreateTable(
                name: "MsUser",
                columns: table => new
                {
                    IdUser = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateUp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserUp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdGender = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MsUser", x => x.IdUser);
                });

            migrationBuilder.CreateTable(
                name: "MsUserPassword",
                columns: table => new
                {
                    IdUserPassword = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateUp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserUp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdUser = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MsUserPassword", x => x.IdUserPassword);
                });

            migrationBuilder.CreateTable(
                name: "MsUserSeller",
                columns: table => new
                {
                    IdUserSeller = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SellerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SellerDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SellerCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateUp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserUp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdUser = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MsUserSeller", x => x.IdUserSeller);
                });

            migrationBuilder.CreateTable(
                name: "TrBuyerCart",
                columns: table => new
                {
                    IdBuyerCart = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    DateIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateUp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserUp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdProduct = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrBuyerCart", x => x.IdBuyerCart);
                });

            migrationBuilder.CreateTable(
                name: "TrBuyerTransaction",
                columns: table => new
                {
                    IdBuyerTransaction = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FinalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    RatingComment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateUp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserUp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdPayment = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrBuyerTransaction", x => x.IdBuyerTransaction);
                });

            migrationBuilder.CreateTable(
                name: "TrBuyerTransactionDetail",
                columns: table => new
                {
                    IdBuyerTransactionDetail = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    PriceProduct = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountProduct = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    RatingComment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateUp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserUp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdBuyerTransaction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdProduct = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrBuyerTransactionDetail", x => x.IdBuyerTransactionDetail);
                });

            migrationBuilder.CreateTable(
                name: "TrHomeAddress",
                columns: table => new
                {
                    IdHomeAddress = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Provinsi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KotaKabupaten = table.Column<string>(name: "Kota/Kabupaten", type: "nvarchar(max)", nullable: false),
                    Kecamatan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KodePos = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HomeAddressDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPrimaryAddress = table.Column<bool>(type: "bit", nullable: false),
                    DateIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateUp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserUp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdUser = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrHomeAddress", x => x.IdHomeAddress);
                });

            migrationBuilder.CreateTable(
                name: "TrProductImages",
                columns: table => new
                {
                    IdProductImages = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductImage = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    DateIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateUp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserUp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IdProduct = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrProductImages", x => x.IdProductImages);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LtCategory");

            migrationBuilder.DropTable(
                name: "LtGender");

            migrationBuilder.DropTable(
                name: "LtPayment");

            migrationBuilder.DropTable(
                name: "MsProduct");

            migrationBuilder.DropTable(
                name: "MsUser");

            migrationBuilder.DropTable(
                name: "MsUserPassword");

            migrationBuilder.DropTable(
                name: "MsUserSeller");

            migrationBuilder.DropTable(
                name: "TrBuyerCart");

            migrationBuilder.DropTable(
                name: "TrBuyerTransaction");

            migrationBuilder.DropTable(
                name: "TrBuyerTransactionDetail");

            migrationBuilder.DropTable(
                name: "TrHomeAddress");

            migrationBuilder.DropTable(
                name: "TrProductImages");
        }
    }
}
