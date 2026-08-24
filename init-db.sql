CREATE DATABASE BookExchangeDb;
GO
USE BookExchangeDb;
GO

CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(20) NULL,
    ZaloContact NVARCHAR(50) NULL,
    StudentIdImageUrl NVARCHAR(500) NULL,
    IsVerified BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT CK_Users_FullName CHECK (LEN(LTRIM(RTRIM(FullName))) > 0)
);
GO

CREATE TABLE Faculties (
    FacultyId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);
GO

CREATE TABLE Subjects (
    SubjectId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    FacultyId INT NOT NULL,
    CONSTRAINT FK_Subjects_Faculties FOREIGN KEY (FacultyId) REFERENCES Faculties(FacultyId),
    CONSTRAINT UQ_Subjects_Name_Faculty UNIQUE (Name, FacultyId)
);
GO

CREATE TABLE Listings (
    ListingId INT IDENTITY(1,1) PRIMARY KEY,
    SellerId INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000) NULL,
    Price DECIMAL(10,0) NOT NULL,
    Condition TINYINT NOT NULL,
    SubjectId INT NOT NULL,
    Status TINYINT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Listings_Users FOREIGN KEY (SellerId) REFERENCES Users(UserId),
    CONSTRAINT FK_Listings_Subjects FOREIGN KEY (SubjectId) REFERENCES Subjects(SubjectId),
    CONSTRAINT CK_Listings_Title CHECK (LEN(LTRIM(RTRIM(Title))) > 0),
    CONSTRAINT CK_Listings_Price CHECK (Price >= 0),
    CONSTRAINT CK_Listings_Condition CHECK (Condition IN (0,1,2)),
    CONSTRAINT CK_Listings_Status CHECK (Status IN (0,1,2))
);
GO

CREATE TABLE ListingImages (
    ImageId INT IDENTITY(1,1) PRIMARY KEY,
    ListingId INT NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL,
    SortOrder TINYINT NOT NULL DEFAULT 0,
    CONSTRAINT FK_ListingImages_Listings FOREIGN KEY (ListingId) REFERENCES Listings(ListingId) ON DELETE CASCADE
);
GO

CREATE TABLE Meetings (
    MeetingId INT IDENTITY(1,1) PRIMARY KEY,
    ListingId INT NOT NULL,
    BuyerId INT NOT NULL,
    ProposedTime DATETIME NOT NULL,
    Status TINYINT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Meetings_Listings FOREIGN KEY (ListingId) REFERENCES Listings(ListingId),
    CONSTRAINT FK_Meetings_Buyer FOREIGN KEY (BuyerId) REFERENCES Users(UserId),
    CONSTRAINT CK_Meetings_Status CHECK (Status IN (0,1,2,3))
);
GO

CREATE TABLE Reviews (
    ReviewId INT IDENTITY(1,1) PRIMARY KEY,
    MeetingId INT NOT NULL,
    ReviewerId INT NOT NULL,
    RevieweeId INT NOT NULL,
    Rating TINYINT NOT NULL,
    Comment NVARCHAR(1000) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Reviews_Meetings FOREIGN KEY (MeetingId) REFERENCES Meetings(MeetingId),
    CONSTRAINT FK_Reviews_Reviewer FOREIGN KEY (ReviewerId) REFERENCES Users(UserId),
    CONSTRAINT FK_Reviews_Reviewee FOREIGN KEY (RevieweeId) REFERENCES Users(UserId),
    CONSTRAINT CK_Reviews_Rating CHECK (Rating BETWEEN 1 AND 5),
    CONSTRAINT CK_Reviews_NotSelf CHECK (ReviewerId <> RevieweeId),
    CONSTRAINT UQ_Reviews_OnePerMeetingPerReviewer UNIQUE (MeetingId, ReviewerId)
);
GO

CREATE TABLE Reports (
    ReportId INT IDENTITY(1,1) PRIMARY KEY,
    ReporterId INT NOT NULL,
    ListingId INT NULL,
    ReportedUserId INT NULL,
    Reason NVARCHAR(500) NOT NULL,
    Status TINYINT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Reports_Reporter FOREIGN KEY (ReporterId) REFERENCES Users(UserId),
    CONSTRAINT FK_Reports_Listings FOREIGN KEY (ListingId) REFERENCES Listings(ListingId),
    CONSTRAINT FK_Reports_ReportedUser FOREIGN KEY (ReportedUserId) REFERENCES Users(UserId),
    CONSTRAINT CK_Reports_Status CHECK (Status IN (0,1)),
    CONSTRAINT CK_Reports_HasTarget CHECK (ListingId IS NOT NULL OR ReportedUserId IS NOT NULL)
);
GO

INSERT INTO Faculties (Name) VALUES
(N'Công nghệ thông tin'), (N'Kế toán - Kiểm toán'), (N'Quản trị kinh doanh'),
(N'Công nghệ thực phẩm'), (N'Cơ khí'), (N'Ngoại ngữ'), (N'Khác');
GO
