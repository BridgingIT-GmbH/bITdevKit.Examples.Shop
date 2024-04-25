using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Shopping.Infrastructure.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "shopping");

            migrationBuilder.CreateTable(
                name: "__Storage_Documents",
                schema: "shopping",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    PartitionKey = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    RowKey = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Properties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK___Storage_Documents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                schema: "shopping",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Identity = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_Carts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                schema: "shopping",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                schema: "shopping",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => new { x.CartId, x.Id });
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalSchema: "shopping",
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantCarts",
                schema: "shopping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantCarts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantCarts_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "shopping",
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX___Storage_Documents_PartitionKey",
                schema: "shopping",
                table: "__Storage_Documents",
                column: "PartitionKey");

            migrationBuilder.CreateIndex(
                name: "IX___Storage_Documents_RowKey",
                schema: "shopping",
                table: "__Storage_Documents",
                column: "RowKey");

            migrationBuilder.CreateIndex(
                name: "IX___Storage_Documents_Type",
                schema: "shopping",
                table: "__Storage_Documents",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_Identity",
                schema: "shopping",
                table: "Carts",
                column: "Identity",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantCarts_TenantId",
                schema: "shopping",
                table: "TenantCarts",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "__Storage_Documents",
                schema: "shopping");

            migrationBuilder.DropTable(
                name: "CartItems",
                schema: "shopping");

            migrationBuilder.DropTable(
                name: "TenantCarts",
                schema: "shopping");

            migrationBuilder.DropTable(
                name: "Carts",
                schema: "shopping");

            migrationBuilder.DropTable(
                name: "Tenants",
                schema: "shopping");
        }
    }
}
