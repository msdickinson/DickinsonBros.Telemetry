CREATE PROCEDURE[Telemetry].[TelemetryQuery]
(
   @nameContains varchar(200), --NameContains
   @startDateTimeUTC datetime2(7),
   @endDateTimeUTC datetime2(7),
   @telemetryTypes TelemetryTypeType READONLY, --Types
   @telemetryStates TelemetryStateType READONLY, --States
   @sourceContains varchar(200) --SourceContains
)
AS 
	SET NOCOUNT ON;

select * 
from Telemetry.[Data]
where 
	Telemetry.[Data].[DateTime] <=  @endDateTimeUTC AND
	Telemetry.[Data].[DateTime] >=  @startDateTimeUTC AND
	Telemetry.[Data].TelemetryType IN (select TelemetryTypeId from @telemetryTypes) AND
	Telemetry.[Data].TelemetryState IN (select TelemetryStateId from @telemetryStates) AND
	Telemetry.[Data].[Name]  LIKE '%' + @nameContains + '%'AND
	Telemetry.[Data].[Source]  LIKE '%' + @sourceContains + '%'

RETURN 0