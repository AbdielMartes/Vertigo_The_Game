SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @playersId int = OBJECT_ID(N'dbo.Players', N'U');
    IF @playersId IS NULL
        THROW 50001, 'dbo.Players does not exist in the selected database.', 1;

    IF COL_LENGTH(N'dbo.Players', N'Username') = 40 AND COL_LENGTH(N'dbo.Players', N'Email') = 140
    BEGIN
        COMMIT TRANSACTION;
        RETURN;
    END;

    IF ISNULL(COL_LENGTH(N'dbo.Players', N'Username'), 0) <> 64
        OR ISNULL(COL_LENGTH(N'dbo.Players', N'Email'), 0) <> 240
        THROW 50002, 'Expected the original Players column lengths (32 and 120).', 1;

    IF EXISTS (
        SELECT 1
        FROM dbo.Players WITH (TABLOCKX, HOLDLOCK)
        WHERE DATALENGTH(Username) > 40 OR DATALENGTH(Email) > 140
    )
        THROW 50003, 'Existing accounts exceed the new lengths. Resolve them before rerunning this script.', 1;

    DECLARE @usernameConstraint sysname;
    DECLARE @emailConstraint sysname;

    SELECT
        @usernameConstraint = MAX(CASE WHEN columns.name = N'Username' THEN constraints.name END),
        @emailConstraint = MAX(CASE WHEN columns.name = N'Email' THEN constraints.name END)
    FROM sys.key_constraints AS constraints
    INNER JOIN sys.index_columns AS keys
        ON keys.object_id = constraints.parent_object_id AND keys.index_id = constraints.unique_index_id
    INNER JOIN sys.columns AS columns
        ON columns.object_id = keys.object_id AND columns.column_id = keys.column_id
    WHERE constraints.parent_object_id = @playersId
        AND constraints.type = 'UQ'
        AND keys.key_ordinal = 1
        AND columns.name IN (N'Username', N'Email')
        AND NOT EXISTS (
            SELECT 1
            FROM sys.index_columns AS remainingKeys
            WHERE remainingKeys.object_id = keys.object_id AND remainingKeys.index_id = keys.index_id
                AND remainingKeys.key_ordinal > 1
        );

    IF @usernameConstraint IS NULL OR @emailConstraint IS NULL OR @usernameConstraint = @emailConstraint
        THROW 50004, 'Expected separate unique constraints for Players.Username and Players.Email.', 1;

    DECLARE @usernameIndexExists bit = 0;
    IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = @playersId AND name = N'IX_Players_Username')
    BEGIN
        SET @usernameIndexExists = 1;
        DROP INDEX IX_Players_Username ON dbo.Players;
    END;

    DECLARE @sql nvarchar(max) = N'ALTER TABLE dbo.Players DROP CONSTRAINT '
        + QUOTENAME(@usernameConstraint) + N', ' + QUOTENAME(@emailConstraint);
    EXEC sys.sp_executesql @sql;

    ALTER TABLE dbo.Players ALTER COLUMN Username nvarchar(20) NOT NULL;
    ALTER TABLE dbo.Players ALTER COLUMN Email nvarchar(70) NOT NULL;

    SET @sql = N'ALTER TABLE dbo.Players ADD CONSTRAINT '
        + QUOTENAME(@usernameConstraint) + N' UNIQUE (Username), CONSTRAINT '
        + QUOTENAME(@emailConstraint) + N' UNIQUE (Email)';
    EXEC sys.sp_executesql @sql;

    IF @usernameIndexExists = 1
        CREATE INDEX IX_Players_Username ON dbo.Players (Username);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
