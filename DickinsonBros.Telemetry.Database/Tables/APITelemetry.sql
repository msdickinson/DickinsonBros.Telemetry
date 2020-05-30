CREATE TABLE [dbo].[APITelemetry]
(
	[APITelemetryId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CorrelationId] nvarchar(255) NOT NULL,
    [ElapsedMilliseconds] int NOT NULL,
    [RequestRedacted] nvarchar(max) NOT NULL,
    [ResponseRedacted] nvarchar(max) NULL,
    [Source] nvarchar(50) NOT NULL,
    [StatusCode] int NOT NULL,
    [Url] nvarchar(max) NOT NULL,
)