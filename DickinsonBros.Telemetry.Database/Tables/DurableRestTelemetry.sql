CREATE TABLE [dbo].[DurableRestTelemetry]
(
	[DurableRestTelemetryId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Attempt] int NOT NULL,
    [BaseUrl] nvarchar(255) NOT NULL,
    [CorrelationId] nvarchar(255) NOT NULL,
    [ElapsedMilliseconds] int NOT NULL,
    [Name] nvarchar(50) NOT NULL,
    [RequestRedacted] nvarchar(max) NOT NULL,
    [Resource] nvarchar(255) NOT NULL,
    [ResponseRedacted] nvarchar(max) NULL,
    [Source] nvarchar(50) NOT NULL,
    [StatusCode] int NOT NULL
)

