CREATE TABLE [Telemetry].[TelemetryType]
(
	[TelemetryTypeId] INT NOT NULL IDENTITY(1,1) CONSTRAINT TelemetryType_PK PRIMARY KEY,   
    [Type] VARCHAR(50) NOT NULL,
    [DateCreated] DATETIME2 NOT NULL, 
    [DateChanged] DATETIME2 NOT NULL 
)
