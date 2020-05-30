CREATE TABLE [dbo].[SQLTelemetry]
(
	[SQLTelemetryId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CorrelationId] nvarchar(255) NOT NULL,
    [Database] nvarchar(255) NOT NULL,
    [ElapsedMilliseconds] int NOT NULL,
    [IsSuccessful] bit NOT NULL,
    [Query] nvarchar(max) NOT NULL,
    [ResponseRedacted] nvarchar(max) NULL,
    [RequestRedacted] nvarchar(max) NOT NULL,
    [Source] nvarchar(50) NOT NULL
)