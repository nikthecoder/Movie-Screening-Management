using Microsoft.EntityFrameworkCore.Migrations;

namespace Inlämning1.Migrations
{
    public partial class Third : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Screening_Movie_MovieID",
                table: "Screening");

            migrationBuilder.AlterColumn<int>(
                name: "MovieID",
                table: "Screening",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Screening_Movie_MovieID",
                table: "Screening",
                column: "MovieID",
                principalTable: "Movie",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Screening_Movie_MovieID",
                table: "Screening");

            migrationBuilder.AlterColumn<int>(
                name: "MovieID",
                table: "Screening",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Screening_Movie_MovieID",
                table: "Screening",
                column: "MovieID",
                principalTable: "Movie",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
