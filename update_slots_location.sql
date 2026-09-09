USE BookExchangeDb;
GO

-- Drop old check constraint on SellerAvailability
IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_SellerAvailability_TimeSlot')
    ALTER TABLE SellerAvailability DROP CONSTRAINT CK_SellerAvailability_TimeSlot;
ALTER TABLE SellerAvailability ADD CONSTRAINT CK_SellerAvailability_TimeSlot CHECK (TimeSlot BETWEEN 0 AND 6);

-- Drop old check constraint on SellerBlackoutDates
IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_SellerBlackoutDates_TimeSlot')
    ALTER TABLE SellerBlackoutDates DROP CONSTRAINT CK_SellerBlackoutDates_TimeSlot;
ALTER TABLE SellerBlackoutDates ADD CONSTRAINT CK_SellerBlackoutDates_TimeSlot CHECK (TimeSlot IS NULL OR (TimeSlot BETWEEN 0 AND 6));

-- Add Location column to Meetings if not exists
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Meetings') AND name = 'Location')
    ALTER TABLE Meetings ADD Location NVARCHAR(200) NULL;
GO
