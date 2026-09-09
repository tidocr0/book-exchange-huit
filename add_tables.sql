USE BookExchangeDb;
GO

CREATE TABLE SellerAvailability (
    AvailabilityId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    DayOfWeek TINYINT NOT NULL,
    TimeSlot TINYINT NOT NULL,
    CONSTRAINT FK_SellerAvailability_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    CONSTRAINT CK_SellerAvailability_DayOfWeek CHECK (DayOfWeek BETWEEN 0 AND 6),
    CONSTRAINT CK_SellerAvailability_TimeSlot CHECK (TimeSlot BETWEEN 0 AND 3),
    CONSTRAINT UQ_SellerAvailability UNIQUE (UserId, DayOfWeek, TimeSlot)
);
GO

CREATE TABLE SellerBlackoutDates (
    BlackoutId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    BlackoutDate DATE NOT NULL,
    TimeSlot TINYINT NULL,
    CONSTRAINT FK_SellerBlackoutDates_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    CONSTRAINT CK_SellerBlackoutDates_TimeSlot CHECK (TimeSlot IS NULL OR (TimeSlot BETWEEN 0 AND 3)),
    CONSTRAINT UQ_SellerBlackoutDates UNIQUE (UserId, BlackoutDate, TimeSlot)
);
GO
