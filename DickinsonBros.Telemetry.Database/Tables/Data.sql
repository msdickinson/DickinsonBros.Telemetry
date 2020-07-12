CREATE TABLE [Telemetry].[Data]
(
	[DataId] BIGINT NOT NULL IDENTITY(1,1) CONSTRAINT Data_PK PRIMARY KEY,
    [Name] nvarchar(255) NOT NULL, 
    [ElapsedMilliseconds] int NOT NULL,
    [TelemetryType] int NOT NULL CONSTRAINT FK_Queue_TelemetryType REFERENCES [Telemetry].[TelemetryType]([TelemetryTypeId]),
    [TelemetryState] int NOT NULL CONSTRAINT FK_Queue_TelemetryStatee REFERENCES [Telemetry].[TelemetryState]([TelemetryStateId]), 
    [DateTime] DATETIME2 NOT NULL, 
    [Source] NVARCHAR(255) NOT NULL
)