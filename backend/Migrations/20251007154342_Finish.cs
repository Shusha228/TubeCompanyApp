using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class Finish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Inn = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    TelegramUserId = table.Column<long>(type: "bigint", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Inn = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AdminNotified = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PriceUpdates",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    StockId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    PriceT = table.Column<decimal>(type: "numeric", nullable: true),
                    PriceT1 = table.Column<decimal>(type: "numeric", nullable: true),
                    PriceT2 = table.Column<decimal>(type: "numeric", nullable: true),
                    PriceM = table.Column<decimal>(type: "numeric", nullable: true),
                    PriceM1 = table.Column<decimal>(type: "numeric", nullable: true),
                    PriceM2 = table.Column<decimal>(type: "numeric", nullable: true),
                    PriceLimitT1 = table.Column<decimal>(type: "numeric", nullable: true),
                    PriceLimitT2 = table.Column<decimal>(type: "numeric", nullable: true),
                    PriceLimitM1 = table.Column<decimal>(type: "numeric", nullable: true),
                    PriceLimitM2 = table.Column<decimal>(type: "numeric", nullable: true),
                    NDS = table.Column<decimal>(type: "numeric", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsProcessed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceUpdates", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ProductTypes",
                columns: table => new
                {
                    IDType = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IDParentType = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    OriginalGuid = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypes", x => x.IDType);
                });

            migrationBuilder.CreateTable(
                name: "RemnantUpdates",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    StockId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    InStockT = table.Column<decimal>(type: "numeric", nullable: true),
                    InStockM = table.Column<decimal>(type: "numeric", nullable: true),
                    SoonArriveT = table.Column<decimal>(type: "numeric", nullable: true),
                    SoonArriveM = table.Column<decimal>(type: "numeric", nullable: true),
                    ReservedT = table.Column<decimal>(type: "numeric", nullable: true),
                    ReservedM = table.Column<decimal>(type: "numeric", nullable: true),
                    AvgTubeLength = table.Column<decimal>(type: "numeric", nullable: true),
                    AvgTubeWeight = table.Column<decimal>(type: "numeric", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsProcessed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemnantUpdates", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    IDStock = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StockName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Schedule = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FIASId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OwnerInn = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    OwnerKpp = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    OwnerFullName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    OwnerShortName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RailwayStation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ConsigneeCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.IDStock);
                });

            migrationBuilder.CreateTable(
                name: "StockUpdates",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StockId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    City = table.Column<string>(type: "text", nullable: true),
                    StockName = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Schedule = table.Column<string>(type: "text", nullable: true),
                    FIASId = table.Column<string>(type: "text", nullable: true),
                    OwnerInn = table.Column<string>(type: "text", nullable: true),
                    OwnerKpp = table.Column<string>(type: "text", nullable: true),
                    OwnerFullName = table.Column<string>(type: "text", nullable: true),
                    OwnerShortName = table.Column<string>(type: "text", nullable: true),
                    RailwayStation = table.Column<string>(type: "text", nullable: true),
                    ConsigneeCode = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsProcessed = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockUpdates", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TelegramUsers",
                columns: table => new
                {
                    TelegramUserId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Inn = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramUsers", x => x.TelegramUserId);
                });

            migrationBuilder.CreateTable(
                name: "UpdateLogs",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<string>(type: "text", nullable: false),
                    Operation = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpdateLogs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "OrderCartItem",
                columns: table => new
                {
                    OrderId = table.Column<string>(type: "character varying(8)", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    IsInMeters = table.Column<bool>(type: "boolean", nullable: false),
                    FinalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderCartItem", x => new { x.OrderId, x.Id });
                    table.ForeignKey(
                        name: "FK_OrderCartItem_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Nomenclatures",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IDCat = table.Column<string>(type: "text", nullable: false),
                    IDType = table.Column<int>(type: "integer", nullable: false),
                    IDTypeNew = table.Column<string>(type: "text", nullable: false),
                    ProductionType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IDFunctionType = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Gost = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FormOfLength = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Manufacturer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SteelGrade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Diameter = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    ProfileSize2 = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    PipeWallThickness = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Koef = table.Column<decimal>(type: "numeric(10,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nomenclatures", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Nomenclatures_ProductTypes_IDType",
                        column: x => x.IDType,
                        principalTable: "ProductTypes",
                        principalColumn: "IDType",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    IsInMeters = table.Column<bool>(type: "boolean", nullable: false),
                    StockId = table.Column<string>(type: "text", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    FinalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Warehouse = table.Column<string>(type: "text", nullable: false),
                    ProductID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => new { x.UserId, x.ProductId, x.StockId, x.IsInMeters });
                    table.ForeignKey(
                        name: "FK_CartItems_Nomenclatures_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Nomenclatures",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_CartItems_Nomenclatures_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Nomenclatures",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false),
                    IDStock = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    PriceT = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PriceLimitT1 = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    PriceT1 = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    PriceLimitT2 = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    PriceT2 = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    PriceM = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PriceLimitM1 = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    PriceM1 = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    PriceLimitM2 = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    PriceM2 = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    NDS = table.Column<decimal>(type: "numeric(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => new { x.ID, x.IDStock });
                    table.ForeignKey(
                        name: "FK_Prices_Nomenclatures_ID",
                        column: x => x.ID,
                        principalTable: "Nomenclatures",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prices_Stocks_IDStock",
                        column: x => x.IDStock,
                        principalTable: "Stocks",
                        principalColumn: "IDStock",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Remnants",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false),
                    IDStock = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    InStockT = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    InStockM = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SoonArriveT = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    SoonArriveM = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    ReservedT = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    ReservedM = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    AvgTubeLength = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    AvgTubeWeight = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    NomenclatureID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Remnants", x => new { x.ID, x.IDStock });
                    table.ForeignKey(
                        name: "FK_Remnants_Nomenclatures_NomenclatureID",
                        column: x => x.NomenclatureID,
                        principalTable: "Nomenclatures",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Remnants_Stocks_IDStock",
                        column: x => x.IDStock,
                        principalTable: "Stocks",
                        principalColumn: "IDStock",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_ProductId",
                table: "CartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_UserId",
                table: "CartItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductID",
                table: "CartItems",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInfo_UserId",
                table: "CustomerInfos",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Nomenclature_IDType",
                table: "Nomenclatures",
                column: "IDType");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CreatedAt",
                table: "Orders",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Order_TelegramUserId",
                table: "Orders",
                column: "TelegramUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Price_IDStock",
                table: "Prices",
                column: "IDStock");

            migrationBuilder.CreateIndex(
                name: "IX_PriceUpdates_IsProcessed",
                table: "PriceUpdates",
                column: "IsProcessed");

            migrationBuilder.CreateIndex(
                name: "IX_PriceUpdates_ProductId_StockId",
                table: "PriceUpdates",
                columns: new[] { "ProductId", "StockId" });

            migrationBuilder.CreateIndex(
                name: "IX_Remnant_IDStock",
                table: "Remnants",
                column: "IDStock");

            migrationBuilder.CreateIndex(
                name: "IX_Remnants_NomenclatureID",
                table: "Remnants",
                column: "NomenclatureID");

            migrationBuilder.CreateIndex(
                name: "IX_RemnantUpdates_IsProcessed",
                table: "RemnantUpdates",
                column: "IsProcessed");

            migrationBuilder.CreateIndex(
                name: "IX_RemnantUpdates_ProductId_StockId",
                table: "RemnantUpdates",
                columns: new[] { "ProductId", "StockId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockUpdates_IsProcessed",
                table: "StockUpdates",
                column: "IsProcessed");

            migrationBuilder.CreateIndex(
                name: "IX_StockUpdates_StockId",
                table: "StockUpdates",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramUser_Email",
                table: "TelegramUsers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramUser_Inn",
                table: "TelegramUsers",
                column: "Inn");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramUser_Phone",
                table: "TelegramUsers",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_UpdateLogs_EntityType",
                table: "UpdateLogs",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_UpdateLogs_Timestamp",
                table: "UpdateLogs",
                column: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "CustomerInfos");

            migrationBuilder.DropTable(
                name: "OrderCartItem");

            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "PriceUpdates");

            migrationBuilder.DropTable(
                name: "Remnants");

            migrationBuilder.DropTable(
                name: "RemnantUpdates");

            migrationBuilder.DropTable(
                name: "StockUpdates");

            migrationBuilder.DropTable(
                name: "TelegramUsers");

            migrationBuilder.DropTable(
                name: "UpdateLogs");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Nomenclatures");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropTable(
                name: "ProductTypes");
        }
    }
}
