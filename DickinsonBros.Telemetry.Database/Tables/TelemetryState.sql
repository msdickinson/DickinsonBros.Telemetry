CREATE TABLE [Telemetry].[TelemetryState]
(
	[TelemetryStateId] INT NOT NULL IDENTITY(1,1) CONSTRAINT TelemetryState_PK PRIMARY KEY,   
    [State] VARCHAR(50) NOT NULL,
    [DateCreated] DATETIME2 NOT NULL, 
    [DateChanged] DATETIME2 NOT NULL 
)
