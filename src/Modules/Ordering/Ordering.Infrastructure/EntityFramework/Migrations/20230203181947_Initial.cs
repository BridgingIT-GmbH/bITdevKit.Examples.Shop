namespace Modules.Ordering.Infrastructure.EntityFramework.Migrations;

using System;
using Microsoft.EntityFrameworkCore.Migrations;

public partial class Initial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "ordering");

        migrationBuilder.CreateTable(
            name: "Buyers",
            schema: "ordering",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Identity = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Buyers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Orders",
            schema: "ordering",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                BuyerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                Address_Line1 = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                Address_Line2 = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                Address_PostalCode = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                Address_City = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                Address_Country = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                OrderStatus = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Orders", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "PaymentMethods",
            schema: "ordering",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Alias = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                CardNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                SecurityNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CardHolderName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Expiration = table.Column<DateTime>(type: "datetime2", maxLength: 32, nullable: false),
                CardType = table.Column<int>(type: "int", nullable: false),
                BuyerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                table.ForeignKey(
                    name: "FK_PaymentMethods_Buyers_BuyerId",
                    column: x => x.BuyerId,
                    principalSchema: "ordering",
                    principalTable: "Buyers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "OrderItems",
            schema: "ordering",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProductName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                Units = table.Column<int>(type: "int", nullable: false),
                PictureUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OrderItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_OrderItems_Orders_OrderId",
                    column: x => x.OrderId,
                    principalSchema: "ordering",
                    principalTable: "Orders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Buyers_Identity",
            schema: "ordering",
            table: "Buyers",
            column: "Identity",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_OrderItems_OrderId",
            schema: "ordering",
            table: "OrderItems",
            column: "OrderId");

        migrationBuilder.CreateIndex(
            name: "IX_PaymentMethods_BuyerId",
            schema: "ordering",
            table: "PaymentMethods",
            column: "BuyerId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "OrderItems",
            schema: "ordering");

        migrationBuilder.DropTable(
            name: "PaymentMethods",
            schema: "ordering");

        migrationBuilder.DropTable(
            name: "Orders",
            schema: "ordering");

        migrationBuilder.DropTable(
            name: "Buyers",
            schema: "ordering");
    }
}
