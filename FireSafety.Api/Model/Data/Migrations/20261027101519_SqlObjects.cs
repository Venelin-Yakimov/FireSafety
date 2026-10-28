using SqlObjectDefinitions = FireSafety.Model.Data.Sql.SqlObjects;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FireSafety.Api.Model.Data.Migrations
{
    public partial class SqlObjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(SqlObjectDefinitions.CreateIdListType);
            migrationBuilder.Sql(SqlObjectDefinitions.AcceptOfferProcedure);
            migrationBuilder.Sql(SqlObjectDefinitions.AddReviewProcedure);
            migrationBuilder.Sql(SqlObjectDefinitions.SetFireSpecialistServiceTypesProcedure);
            migrationBuilder.Sql(SqlObjectDefinitions.OpenSafetyRequestsView);
            migrationBuilder.Sql(SqlObjectDefinitions.FireSpecialistProfilesView);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS dbo.vw_FireSpecialistProfiles;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS dbo.vw_OpenSafetyRequests;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.usp_SetFireSpecialistServiceTypes;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.usp_AddReview;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.usp_AcceptOffer;");
            migrationBuilder.Sql("IF TYPE_ID(N'dbo.IdListType') IS NOT NULL EXEC(N'DROP TYPE dbo.IdListType');");
        }
    }
}
