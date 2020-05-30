CREATE TABLE [dbo].[EmailTelemetry]
(
	[EmailTelemetryId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CorrelationId] nvarchar(255) NOT NULL,
    [ElapsedMilliseconds] int NOT NULL,
    [IsSuccessful] bit NOT NULL,
    [Source] nvarchar(max) NOT NULL,
    [Subject] nvarchar(max) NOT NULL,
    [To] nvarchar(255) NOT NULL
)