IF OBJECT_ID(N'dbo.Players', N'U') IS NULL
BEGIN
    THROW 50001, 'The dbo.Players table must exist before adding CountryCode.', 1;
END;

IF COL_LENGTH(N'dbo.Players', N'CountryCode') IS NULL
BEGIN
    ALTER TABLE dbo.Players ADD CountryCode varchar(2) NULL;
END;
