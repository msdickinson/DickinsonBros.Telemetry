CREATE PROCEDURE Telemetry.TelemetryStateUpsert
( 
	@telemetryStateId int,
	@state varchar(50),
    @dateChanged datetime2(7)
)
AS 
  SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
  BEGIN TRAN
 
    IF EXISTS ( SELECT * FROM Telemetry.TelemetryState WITH (UPDLOCK) WHERE TelemetryStateId = @telemetryStateId )
      UPDATE Telemetry.telemetryState
      SET 
         [State] = @state,
         DateChanged = @dateChanged
      WHERE TelemetryStateId = @telemetryStateId;
 
    ELSE 
      INSERT Telemetry.TelemetryState 
	  (
        TelemetryStateId,
        [State],
        DateChanged,
        DateCreated
      )
      VALUES
	  (
        @telemetryStateId,
        @state,
        @dateChanged,
        @dateChanged
      );
 
  COMMIT