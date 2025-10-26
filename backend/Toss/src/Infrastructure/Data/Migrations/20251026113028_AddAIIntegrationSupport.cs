using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toss.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAIIntegrationSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestTimeoutSeconds",
                table: "AISettings");

            migrationBuilder.RenameColumn(
                name: "ApiKey",
                table: "AISettings",
                newName: "GeminiApiKey");

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Vendors",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeywords",
                table: "Vendors",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitle",
                table: "Vendors",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Products",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeywords",
                table: "Products",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitle",
                table: "Products",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "ProductCategories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeywords",
                table: "ProductCategories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitle",
                table: "ProductCategories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowMetaDescriptionGeneration",
                table: "AISettings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowMetaKeywordsGeneration",
                table: "AISettings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowMetaTitleGeneration",
                table: "AISettings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ChatGptApiKey",
                table: "AISettings",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeepSeekApiKey",
                table: "AISettings",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescriptionQuery",
                table: "AISettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaKeywordsQuery",
                table: "AISettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitleQuery",
                table: "AISettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductDescriptionQuery",
                table: "AISettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestTimeout",
                table: "AISettings",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "MetaKeywords",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "MetaTitle",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MetaKeywords",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MetaTitle",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "MetaKeywords",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "MetaTitle",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "AllowMetaDescriptionGeneration",
                table: "AISettings");

            migrationBuilder.DropColumn(
                name: "AllowMetaKeywordsGeneration",
                table: "AISettings");

            migrationBuilder.DropColumn(
                name: "AllowMetaTitleGeneration",
                table: "AISettings");

            migrationBuilder.DropColumn(
                name: "ChatGptApiKey",
                table: "AISettings");

            migrationBuilder.DropColumn(
                name: "DeepSeekApiKey",
                table: "AISettings");

            migrationBuilder.DropColumn(
                name: "MetaDescriptionQuery",
                table: "AISettings");

            migrationBuilder.DropColumn(
                name: "MetaKeywordsQuery",
                table: "AISettings");

            migrationBuilder.DropColumn(
                name: "MetaTitleQuery",
                table: "AISettings");

            migrationBuilder.DropColumn(
                name: "ProductDescriptionQuery",
                table: "AISettings");

            migrationBuilder.DropColumn(
                name: "RequestTimeout",
                table: "AISettings");

            migrationBuilder.RenameColumn(
                name: "GeminiApiKey",
                table: "AISettings",
                newName: "ApiKey");

            migrationBuilder.AddColumn<int>(
                name: "RequestTimeoutSeconds",
                table: "AISettings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
