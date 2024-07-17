using Microsoft.EntityFrameworkCore.Migrations;

namespace MohiuddinCoreMasterDetailCrud.Migrations
{
    public partial class InsertUpdateStudentProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TYPE ParamModuleType AS TABLE
                (
                    ModuleName NVARCHAR(100),
                    Duration INT
                );

                GO

                CREATE OR ALTER PROCEDURE dbo.InsertStudentSP
                    @StudentName NVARCHAR(50),
                    @Dob DATETIME,
                    @Mobile NVARCHAR(14),
                    @ImageUrl NVARCHAR(MAX),
                    @IsEnroll BIT,
                    @CourseId INT,
                    @Modules ParamModuleType READONLY
                AS
                BEGIN
                    SET NOCOUNT ON;

                    BEGIN TRY
                        DECLARE @LocalModules TABLE
                        (
                            ModuleName NVARCHAR(100),
                            Duration INT,
                            StudentId INT
                        );

                        DECLARE @StudentId INT;

                        INSERT INTO dbo.Students (StudentName, Dob, Mobile, ImageUrl, IsEnroll, CourseId)
                        VALUES (@StudentName, @Dob, @Mobile, @ImageUrl, @IsEnroll, @CourseId);

                        SET @StudentId = SCOPE_IDENTITY();

                        INSERT INTO @LocalModules (ModuleName, Duration, StudentId)
                        SELECT ModuleName, Duration, @StudentId
                        FROM @Modules;

                        INSERT INTO dbo.Modules (ModuleName, Duration, StudentId)
                        SELECT ModuleName, Duration, @StudentId
                        FROM @LocalModules;
                    END TRY
                    BEGIN CATCH
                        -- Handle exceptions
                        THROW;
                    END CATCH
                END
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE dbo.UpdateStudentSP
                    @StudentId INT,
                    @StudentName NVARCHAR(50),
                    @Dob DATETIME,
                    @Mobile NVARCHAR(14),
                    @ImageUrl NVARCHAR(MAX),
                    @IsEnroll BIT,
                    @CourseId INT,
                    @Modules ParamModuleType READONLY
                AS
                BEGIN
                    SET NOCOUNT ON;

                    BEGIN TRY
                        -- Update the student record
                        UPDATE dbo.Students
                        SET StudentName = @StudentName,
                            Dob = @Dob,
                            Mobile = @Mobile,
                            ImageUrl = @ImageUrl,
                            IsEnroll = @IsEnroll,
                            CourseId = @CourseId
                        WHERE StudentId = @StudentId;

                        -- Remove old modules
                        DELETE FROM dbo.Modules WHERE StudentId = @StudentId;

                        -- Insert new modules
                        INSERT INTO dbo.Modules (ModuleName, Duration, StudentId)
                        SELECT ModuleName, Duration, @StudentId
                        FROM @Modules;
                    END TRY
                    BEGIN CATCH
                        -- Handle exceptions
                        THROW;
                    END CATCH
                END

            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.InsertStudentSP");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.UpdateStudentSP");
            migrationBuilder.Sql("DROP TYPE IF EXISTS ParamModuleType");
        }
    }
}
