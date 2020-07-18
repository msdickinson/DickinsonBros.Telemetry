CREATE PROCEDURE Telemetry.TelemetryTypeUpsert
( 
	@telemetryTypeId int,
	@type varchar(50),
    @dateChanged datetime2(7)
)
AS 
	SET NOCOUNT ON;

  SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
  BEGIN TRAN
 
    IF EXISTS ( SELECT * FROM Telemetry.TelemetryType WITH (UPDLOCK) WHERE TelemetryTypeId = @telemetryTypeId )
      UPDATE Telemetry.telemetryType
      SET 
         [Type] = @type,
         DateChanged = SYSUTCDATETIME()
      WHERE TelemetryTypeId = @telemetryTypeId;
 
    ELSE 
      INSERT Telemetry.TelemetryType 
	  (
        TelemetryTypeId,
        [Type],
        DateChanged,
        DateCreated
      )
      VALUES
	  (
        @telemetryTypeId,
        @type,
        @dateChanged,
        @dateChanged
      );
 
  COMMIT