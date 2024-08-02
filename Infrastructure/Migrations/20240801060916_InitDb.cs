using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "数据库自增Id")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "唯一标识"),
                    UserInfoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "用户信息Id"),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "父级Id"),
                    FileName = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false, comment: "文件名"),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "修改时间"),
                    FileSize_Value = table.Column<double>(type: "float", nullable: false),
                    FileSize_Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FileMimeType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true, comment: "文件的mime类型"),
                    FileOnlyTag = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true, comment: "文件的唯一标识，表示给用户看的"),
                    IsDisable = table.Column<bool>(type: "bit", nullable: false, comment: "当前文件是否被禁止被访问"),
                    IsFolder = table.Column<bool>(type: "bit", nullable: false, comment: "是否为文件夹")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "数据库自增Id，必须")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "用户唯一标识，必须"),
                    Available = table.Column<bool>(type: "bit", nullable: false, comment: "当前账户是否可用，必须"),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "更新时间，必须"),
                    Avatar = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true, comment: "用户头像，必须"),
                    BanTime_Value = table.Column<long>(type: "bigint", nullable: false),
                    BanTime_Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, comment: "用户邮箱，必须"),
                    UserName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, comment: "用户名，必须"),
                    Password = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DisableReason = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true, comment: "账户禁用原因，可选"),
                    UnLockTime = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "账户解封时间，可选")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserInfos_Email",
                table: "UserInfos",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserInfos_UserName",
                table: "UserInfos",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "UserInfos");
        }
    }
}
