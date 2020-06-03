
BEGIN TRANSACTION InsertSeedData;
	DECLARE @CurrentDateTime AS datetime2(7) = SYSUTCDATETIME()

	-- TelemetryState
	SET IDENTITY_INSERT [Telemetry].[TelemetryState] ON
	
	EXEC [Telemetry].[TelemetryStateUpsert]
			@telemetryStateId = 0,
			@state = 'Successful',
			@dateChanged = @CurrentDateTime

	EXEC [Telemetry].[TelemetryStateUpsert]
			@telemetryStateId = 1,
			@state = 'BadRequest',
			@dateChanged = @CurrentDateTime

	EXEC [Telemetry].[TelemetryStateUpsert]
			@telemetryStateId = 2,
			@state = 'Failed',
			@dateChanged = @CurrentDateTime

	Delete 
	From [Telemetry].[TelemetryState]
	Where telemetryStateId Not In (0,1,2)

	SET IDENTITY_INSERT [Telemetry].[TelemetryState]  OFF


	-- TelemetryType
	SET IDENTITY_INSERT [Telemetry].[TelemetryType] ON
	
	EXEC [Telemetry].[TelemetryTypeUpsert]
			@telemetryTypeId = 0,
			@type = 'API',
			@dateChanged = @CurrentDateTime

	EXEC [Telemetry].[TelemetryTypeUpsert]
			@telemetryTypeId = 1,
			@type = 'Rest',
			@dateChanged = @CurrentDateTime

	EXEC [Telemetry].[TelemetryTypeUpsert]
			@telemetryTypeId = 2,
			@type = 'SQL',
			@dateChanged = @CurrentDateTime

	EXEC [Telemetry].[TelemetryTypeUpsert]
			@telemetryTypeId = 3,
			@type = 'Email',
			@dateChanged = @CurrentDateTime

	Delete 
	From [Telemetry].[TelemetryType]
	Where telemetryTypeId Not In (0,1,2,3)


COMMIT;

--