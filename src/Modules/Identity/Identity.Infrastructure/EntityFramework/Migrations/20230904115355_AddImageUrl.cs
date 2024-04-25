using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Identity.Infrastructure.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfilePictureDataUrl",
                schema: "identity",
                table: "AspNetUsers",
                newName: "ImageUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                schema: "identity",
                table: "AspNetUsers",
                newName: "ProfilePictureDataUrl");
        }
    }
}
