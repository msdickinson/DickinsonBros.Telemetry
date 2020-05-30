CREATE TABLE [dbo].[QueueTelemetry]
(
	[QueueTelemetryId] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [CorrelationId] nvarchar(255) NOT NULL,
    [ElapsedMilliseconds] int NOT NULL,
    [IsSuccessful] bit NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    [QueueId] int NOT NULL,
    [Source] nvarchar(50) NOT NULL,
    [State] int NOT NULL,
)
