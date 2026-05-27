using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePatientFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Patients",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "BloodType",
                table: "Patients",
                newName: "MedicalRecordNumber");

            migrationBuilder.RenameColumn(
                name: "Allergies",
                table: "Patients",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Patients",
                newName: "FullName");

            migrationBuilder.AddColumn<string>(
                name: "Diagnosis",
                table: "Patients",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Diagnosis",
                table: "Patients");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Patients",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "MedicalRecordNumber",
                table: "Patients",
                newName: "BloodType");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "Patients",
                newName: "Allergies");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Patients",
                newName: "Address");
        }
    }
}
