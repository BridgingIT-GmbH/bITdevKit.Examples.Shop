namespace Modules.Catalog.Infrastructure.EntityFramework.Migrations;

using System;
using Microsoft.EntityFrameworkCore.Migrations;

public partial class Initial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "catalog");

        migrationBuilder.CreateTable(
            name: "Brand",
            schema: "catalog",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                Keywords = table.Column<string>(type: "nvarchar(max)", maxLength: 8192, nullable: true),
                PictureSvg = table.Column<string>(type: "nvarchar(max)", maxLength: 8192, nullable: true),
                PictureFileName = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                PictureUri = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                AuditState_CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                AuditState_CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                AuditState_CreatedDescription = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                AuditState_UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                AuditState_UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                AuditState_UpdatedDescription = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                AuditState_UpdatedReasons = table.Column<string>(type: "nvarchar(max)", maxLength: 8192, nullable: true),
                AuditState_Deactivated = table.Column<bool>(type: "bit", nullable: true),
                AuditState_DeactivatedReasons = table.Column<string>(type: "nvarchar(max)", maxLength: 8192, nullable: true),
                AuditState_DeactivatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                AuditState_DeactivatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                AuditState_DeactivatedDescription = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                AuditState_Deleted = table.Column<bool>(type: "bit", nullable: true),
                AuditState_DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                AuditState_DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                AuditState_DeletedReason = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                AuditState_DeletedDescription = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Brand", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ProductType",
            schema: "catalog",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProductType", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Product",
            schema: "catalog",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                Keywords = table.Column<string>(type: "nvarchar(max)", maxLength: 8192, nullable: true),
                PictureSvg = table.Column<string>(type: "nvarchar(max)", maxLength: 8192, nullable: true),
                PictureFileName = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                PictureUri = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                Sku = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Barcode = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                Size = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                Rating = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                TypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AuditState_CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                AuditState_CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                AuditState_CreatedDescription = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                AuditState_UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                AuditState_UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                AuditState_UpdatedDescription = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                AuditState_UpdatedReasons = table.Column<string>(type: "nvarchar(max)", maxLength: 8192, nullable: true),
                AuditState_Deactivated = table.Column<bool>(type: "bit", nullable: true),
                AuditState_DeactivatedReasons = table.Column<string>(type: "nvarchar(max)", maxLength: 8192, nullable: true),
                AuditState_DeactivatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                AuditState_DeactivatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                AuditState_DeactivatedDescription = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                AuditState_Deleted = table.Column<bool>(type: "bit", nullable: true),
                AuditState_DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                AuditState_DeletedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                AuditState_DeletedReason = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                AuditState_DeletedDescription = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Product", x => x.Id);
                table.ForeignKey(
                    name: "FK_Product_Brand_BrandId",
                    column: x => x.BrandId,
                    principalSchema: "catalog",
                    principalTable: "Brand",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Product_ProductType_TypeId",
                    column: x => x.TypeId,
                    principalSchema: "catalog",
                    principalTable: "ProductType",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_Product_BrandId",
            schema: "catalog",
            table: "Product",
            column: "BrandId");

        migrationBuilder.CreateIndex(
            name: "IX_Product_Sku",
            schema: "catalog",
            table: "Product",
            column: "Sku",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Product_TypeId",
            schema: "catalog",
            table: "Product",
            column: "TypeId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Product",
            schema: "catalog");

        migrationBuilder.DropTable(
            name: "Brand",
            schema: "catalog");

        migrationBuilder.DropTable(
            name: "ProductType",
            schema: "catalog");
    }
}
