using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace v1Remastered.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSupportDetailsModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SupportRaisedDate",
                table: "SupportDetails",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "SupportCommentDate",
                table: "SupportConversations",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupportRaisedDate",
                table: "SupportDetails");

            migrationBuilder.DropColumn(
                name: "SupportCommentDate",
                table: "SupportConversations");
        }
    }
}
