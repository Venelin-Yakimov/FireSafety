using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FireSafety.Api.Model.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FireServiceTypes",
                columns: table => new
                {
                    FireServiceTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FireServiceTypes", x => x.FireServiceTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.CheckConstraint("CK_Users_Role", "[Role] IN (N'Client', N'FireSpecialist', N'Admin')");
                });

            migrationBuilder.CreateTable(
                name: "SafetyRequests",
                columns: table => new
                {
                    SafetyRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    FireServiceTypeId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false),
                    ExactAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Open"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SafetyRequests", x => x.SafetyRequestId);
                    table.CheckConstraint("CK_SafetyRequests_Status", "[Status] IN (N'Open', N'Assigned', N'Completed', N'Cancelled')");
                    table.ForeignKey(
                        name: "FK_SafetyRequests_FireServiceTypes_FireServiceTypeId",
                        column: x => x.FireServiceTypeId,
                        principalTable: "FireServiceTypes",
                        principalColumn: "FireServiceTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SafetyRequests_Users_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SpecialistFireServiceTypes",
                columns: table => new
                {
                    SpecialistId = table.Column<int>(type: "int", nullable: false),
                    FireServiceTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialistFireServiceTypes", x => new { x.SpecialistId, x.FireServiceTypeId });
                    table.ForeignKey(
                        name: "FK_SpecialistFireServiceTypes_FireServiceTypes_FireServiceTypeId",
                        column: x => x.FireServiceTypeId,
                        principalTable: "FireServiceTypes",
                        principalColumn: "FireServiceTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SpecialistFireServiceTypes_Users_SpecialistId",
                        column: x => x.SpecialistId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Offers",
                columns: table => new
                {
                    OfferId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SafetyRequestId = table.Column<int>(type: "int", nullable: false),
                    FireSpecialistId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offers", x => x.OfferId);
                    table.CheckConstraint("CK_Offers_Price", "[Price] > 0");
                    table.CheckConstraint("CK_Offers_Status", "[Status] IN (N'Pending', N'Accepted', N'Rejected')");
                    table.ForeignKey(
                        name: "FK_Offers_SafetyRequests_SafetyRequestId",
                        column: x => x.SafetyRequestId,
                        principalTable: "SafetyRequests",
                        principalColumn: "SafetyRequestId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Offers_Users_FireSpecialistId",
                        column: x => x.FireSpecialistId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SafetyRequestId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<byte>(type: "tinyint", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewId);
                    table.CheckConstraint("CK_Reviews_Rating", "[Rating] BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_Reviews_SafetyRequests_SafetyRequestId",
                        column: x => x.SafetyRequestId,
                        principalTable: "SafetyRequests",
                        principalColumn: "SafetyRequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "FireServiceTypes",
                columns: new[] { "FireServiceTypeId", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, true, "Проверка и презареждане на пожарогасители" },
                    { 2, true, "Монтаж на пожарогасители" },
                    { 3, true, "Пожароизвестителни системи" },
                    { 4, true, "Пожарогасителни системи" },
                    { 5, true, "Пожарна безопасност и оценка на риска" },
                    { 6, true, "Инструктаж и евакуационни планове" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FireServiceTypes_Name",
                table: "FireServiceTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Offers_FireSpecialistId",
                table: "Offers",
                column: "FireSpecialistId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_SafetyRequestId_FireSpecialistId",
                table: "Offers",
                columns: new[] { "SafetyRequestId", "FireSpecialistId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Offers_OneAccepted",
                table: "Offers",
                column: "SafetyRequestId",
                unique: true,
                filter: "[Status] = N'Accepted'");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_SafetyRequestId",
                table: "Reviews",
                column: "SafetyRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SafetyRequests_ClientId",
                table: "SafetyRequests",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_SafetyRequests_FireServiceTypeId_Status",
                table: "SafetyRequests",
                columns: new[] { "FireServiceTypeId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_SpecialistFireServiceTypes_FireServiceTypeId",
                table: "SpecialistFireServiceTypes",
                column: "FireServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Offers");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "SpecialistFireServiceTypes");

            migrationBuilder.DropTable(
                name: "SafetyRequests");

            migrationBuilder.DropTable(
                name: "FireServiceTypes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
