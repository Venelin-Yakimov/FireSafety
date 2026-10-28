namespace FireSafety.Model.Data.Sql;

public static class SqlObjects
{
    public const string CreateIdListType = """
        IF TYPE_ID(N'dbo.IdListType') IS NULL
            EXEC(N'CREATE TYPE dbo.IdListType AS TABLE (Id int NOT NULL PRIMARY KEY)');
        """;

    public const string AcceptOfferProcedure = """
        CREATE OR ALTER PROCEDURE dbo.usp_AcceptOffer
            @OfferId int,
            @ClientId int
        AS
        BEGIN
            SET NOCOUNT ON;
            SET XACT_ABORT ON;

            DECLARE @SafetyRequestId int;
            SELECT @SafetyRequestId = SafetyRequestId
            FROM dbo.Offers
            WHERE OfferId = @OfferId;

            IF @SafetyRequestId IS NULL
                THROW 50001, N'Офертата не е намерена.', 1;

            BEGIN TRY
                BEGIN TRANSACTION;

                DECLARE @OwnerId int;
                DECLARE @RequestStatus nvarchar(20);
                SELECT @OwnerId = ClientId, @RequestStatus = Status
                FROM dbo.SafetyRequests WITH (UPDLOCK, HOLDLOCK)
                WHERE SafetyRequestId = @SafetyRequestId;

                IF @OwnerId IS NULL OR @OwnerId <> @ClientId
                    THROW 50002, N'Заявката не принадлежи на този клиент.', 1;

                IF @RequestStatus <> N'Open'
                    THROW 50003, N'Заявката вече не е отворена.', 1;

                DECLARE @OfferStatus nvarchar(20);
                SELECT @OfferStatus = Status
                FROM dbo.Offers WITH (UPDLOCK, HOLDLOCK)
                WHERE OfferId = @OfferId AND SafetyRequestId = @SafetyRequestId;

                IF @OfferStatus IS NULL OR @OfferStatus <> N'Pending'
                    THROW 50004, N'Офертата вече не чака приемане.', 1;

                UPDATE dbo.Offers
                SET Status = N'Accepted'
                WHERE OfferId = @OfferId;

                UPDATE dbo.Offers
                SET Status = N'Rejected'
                WHERE SafetyRequestId = @SafetyRequestId
                  AND OfferId <> @OfferId
                  AND Status = N'Pending';

                UPDATE dbo.SafetyRequests
                SET Status = N'Assigned'
                WHERE SafetyRequestId = @SafetyRequestId;

                COMMIT TRANSACTION;
            END TRY
            BEGIN CATCH
                IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
                THROW;
            END CATCH;
        END;
        """;

    public const string AddReviewProcedure = """
        CREATE OR ALTER PROCEDURE dbo.usp_AddReview
            @SafetyRequestId int,
            @ClientId int,
            @Rating tinyint,
            @Comment nvarchar(1000) = NULL
        AS
        BEGIN
            SET NOCOUNT ON;
            SET XACT_ABORT ON;

            BEGIN TRY
                BEGIN TRANSACTION;

                DECLARE @OwnerId int;
                DECLARE @RequestStatus nvarchar(20);
                SELECT @OwnerId = ClientId, @RequestStatus = Status
                FROM dbo.SafetyRequests WITH (UPDLOCK, HOLDLOCK)
                WHERE SafetyRequestId = @SafetyRequestId;

                IF @OwnerId IS NULL OR @OwnerId <> @ClientId
                    THROW 50011, N'Заявката не принадлежи на този клиент.', 1;

                IF @RequestStatus <> N'Completed'
                    THROW 50012, N'Отзив може да се остави само за завършена заявка.', 1;

                IF @Rating < 1 OR @Rating > 5
                    THROW 50013, N'Оценката трябва да бъде между 1 и 5.', 1;

                IF EXISTS (
                    SELECT 1 FROM dbo.Reviews WITH (UPDLOCK, HOLDLOCK)
                    WHERE SafetyRequestId = @SafetyRequestId)
                    THROW 50014, N'За тази заявка вече има отзив.', 1;

                INSERT INTO dbo.Reviews (SafetyRequestId, Rating, Comment)
                VALUES (@SafetyRequestId, @Rating, @Comment);

                COMMIT TRANSACTION;
            END TRY
            BEGIN CATCH
                IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
                THROW;
            END CATCH;
        END;
        """;

    public const string SetFireSpecialistServiceTypesProcedure = """
        CREATE OR ALTER PROCEDURE dbo.usp_SetFireSpecialistServiceTypes
            @SpecialistId int,
            @FireServiceTypeIds dbo.IdListType READONLY
        AS
        BEGIN
            SET NOCOUNT ON;
            SET XACT_ABORT ON;

            BEGIN TRY
                BEGIN TRANSACTION;

                DECLARE @Role nvarchar(30);
                SELECT @Role = Role
                FROM dbo.Users WITH (UPDLOCK, HOLDLOCK)
                WHERE UserId = @SpecialistId;

                IF @Role IS NULL OR @Role <> N'FireSpecialist'
                    THROW 50021, N'Потребителят не е специалист по пожарна безопасност.', 1;

                IF EXISTS (
                    SELECT 1
                    FROM @FireServiceTypeIds requested
                    LEFT JOIN dbo.FireServiceTypes serviceType
                      ON serviceType.FireServiceTypeId = requested.Id AND serviceType.IsActive = 1
                    WHERE serviceType.FireServiceTypeId IS NULL)
                    THROW 50022, N'Списъкът съдържа липсващ или неактивен вид услуга.', 1;

                DELETE FROM dbo.SpecialistFireServiceTypes
                WHERE SpecialistId = @SpecialistId;

                INSERT INTO dbo.SpecialistFireServiceTypes (SpecialistId, FireServiceTypeId)
                SELECT @SpecialistId, Id FROM @FireServiceTypeIds;

                COMMIT TRANSACTION;
            END TRY
            BEGIN CATCH
                IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
                THROW;
            END CATCH;
        END;
        """;

    public const string OpenSafetyRequestsView = """
        CREATE OR ALTER VIEW dbo.vw_OpenSafetyRequests
        AS
        SELECT
            request.SafetyRequestId,
            request.Title,
            request.Description,
            request.FireServiceTypeId,
            serviceType.Name AS FireServiceTypeName,
            request.CreatedAt
        FROM dbo.SafetyRequests AS request
        INNER JOIN dbo.FireServiceTypes AS serviceType
            ON serviceType.FireServiceTypeId = request.FireServiceTypeId
        WHERE request.Status = N'Open'
          AND serviceType.IsActive = 1;
        """;

    public const string FireSpecialistProfilesView = """
        CREATE OR ALTER VIEW dbo.vw_FireSpecialistProfiles
        AS
        WITH SpecialistRatings AS
        (
            SELECT
                offer.FireSpecialistId,
                AVG(CONVERT(decimal(5, 2), review.Rating)) AS AverageRating,
                COUNT_BIG(*) AS ReviewCount
            FROM dbo.Offers AS offer
            INNER JOIN dbo.Reviews AS review
                ON review.SafetyRequestId = offer.SafetyRequestId
            WHERE offer.Status = N'Accepted'
            GROUP BY offer.FireSpecialistId
        )
        SELECT
            specialist.UserId,
            specialist.FullName,
            serviceType.FireServiceTypeId,
            serviceType.Name AS FireServiceTypeName,
            COALESCE(rating.AverageRating, CONVERT(decimal(5, 2), 0)) AS AverageRating,
            COALESCE(rating.ReviewCount, CONVERT(bigint, 0)) AS ReviewCount
        FROM dbo.Users AS specialist
        INNER JOIN dbo.SpecialistFireServiceTypes AS specialistType
            ON specialistType.SpecialistId = specialist.UserId
        INNER JOIN dbo.FireServiceTypes AS serviceType
            ON serviceType.FireServiceTypeId = specialistType.FireServiceTypeId
        LEFT JOIN SpecialistRatings AS rating
            ON rating.FireSpecialistId = specialist.UserId
        WHERE specialist.Role = N'FireSpecialist'
          AND serviceType.IsActive = 1;
        """;
}
