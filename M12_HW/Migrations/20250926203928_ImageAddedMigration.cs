using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace M12_HW.Migrations
{
    /// <inheritdoc />
    public partial class ImageAddedMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ImageFile",
                table: "Products",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageType",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFile",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ImageType",
                table: "Products");
        }
    }
}
