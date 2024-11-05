using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Calendar.Migrations
{
    /// <inheritdoc />
    public partial class CalendarSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    AdminId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.AdminId);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    EventDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    AdminApproval = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    RecuringDays = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Attendance",
                columns: table => new
                {
                    AttendanceId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AttendanceDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendance", x => x.AttendanceId);
                    table.ForeignKey(
                        name: "FK_Attendance_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Event_Attendance",
                columns: table => new
                {
                    Event_AttendanceId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Rating = table.Column<int>(type: "INTEGER", nullable: false),
                    Feedback = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event_Attendance", x => x.Event_AttendanceId);
                    table.ForeignKey(
                        name: "FK_Event_Attendance_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Event_Attendance_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Admin",
                columns: new[] { "AdminId", "Email", "Password", "UserName" },
                values: new object[,]
                {
                    { 1, "admin1@example.com", "^�H��(qQ��o��)'s`=\rj���*�rB�", "admin1" },
                    { 2, "admin2@example.com", "\\N@6��G��Ae=j_��a%0�QU��\\", "admin2" },
                    { 3, "admin3@example.com", "�j\\��f������x�s+2��D�o���", "admin3" },
                    { 4, "admin4@example.com", "�].��g��Պ��t��?��^�T��`aǳ", "admin4" },
                    { 5, "admin5@example.com", "E�=���:�-����gd����bF��80]�", "admin5" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admin_UserName",
                table: "Admin",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_UserId",
                table: "Attendance",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Attendance_EventId",
                table: "Event_Attendance",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Attendance_UserId",
                table: "Event_Attendance",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "Attendance");

            migrationBuilder.DropTable(
                name: "Event_Attendance");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
