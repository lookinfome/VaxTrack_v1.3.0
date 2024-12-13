using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace v1Remastered.Migrations
{
    /// <inheritdoc />
    public partial class AddSupportDetailsModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupportConversations",
                columns: table => new
                {
                    SupportCommentId = table.Column<string>(type: "TEXT", nullable: false),
                    SupportId = table.Column<string>(type: "TEXT", nullable: false),
                    SupportComment = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportConversations", x => x.SupportCommentId);
                });

            migrationBuilder.CreateTable(
                name: "SupportDetails",
                columns: table => new
                {
                    SupportId = table.Column<string>(type: "TEXT", nullable: false),
                    SupportStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    SupportTitle = table.Column<string>(type: "TEXT", nullable: false),
                    SupportDescription = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportDetails", x => x.SupportId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupportConversations");

            migrationBuilder.DropTable(
                name: "SupportDetails");
        }
    }
}
