USE BookExchangeDb;
GO

-- 1. Insert 3 Users
INSERT INTO Users (FullName, Email, PasswordHash, Phone, IsVerified, CreatedAt)
VALUES 
(N'Nguyễn Văn A', 'sellerA@huit.edu.vn', '$2a$11$qkb8FdA5uxCEBd4Jx1IDmuoTQQ8cfz0LSetghAp5x.EjNFP6Wc9eu', '0901111111', 1, GETDATE()),
(N'Trần Thị B', 'sellerB@huit.edu.vn', '$2a$11$qkb8FdA5uxCEBd4Jx1IDmuoTQQ8cfz0LSetghAp5x.EjNFP6Wc9eu', '0902222222', 1, GETDATE()),
(N'Lê Văn C', 'sellerC@huit.edu.vn', '$2a$11$qkb8FdA5uxCEBd4Jx1IDmuoTQQ8cfz0LSetghAp5x.EjNFP6Wc9eu', '0903333333', 1, GETDATE());
GO

-- Get UserIds
DECLARE @UserA INT = (SELECT UserId FROM Users WHERE Email = 'sellerA@huit.edu.vn');
DECLARE @UserB INT = (SELECT UserId FROM Users WHERE Email = 'sellerB@huit.edu.vn');
DECLARE @UserC INT = (SELECT UserId FROM Users WHERE Email = 'sellerC@huit.edu.vn');

-- 2. Insert 12 Listings
-- 15: Lập trình căn bản
-- 16: Cấu trúc dữ liệu và giải thuật
-- 17: Cơ sở dữ liệu
-- 18: Mạng máy tính
-- 19: Kế toán tài chính
-- 20: Nguyên lý kế toán
-- 22: Marketing căn bản
-- 25: Công nghệ thực phẩm đại cương
-- 26: Hoá học đại cương
-- 27: Nguyên lý kinh tế

INSERT INTO Listings (SellerId, SubjectId, Title, Description, Price, Condition, Status, CreatedAt)
VALUES 
(@UserA, 15, N'Giáo trình Lập trình căn bản C++', N'Sách mua năm ngoái, còn rất mới, chưa ghi chú gì bên trong.', 50000, 0, 0, GETDATE()),
(@UserB, 16, N'Sách bài tập Cấu trúc dữ liệu và giải thuật', N'Bản photo trắng đen, thích hợp để làm bài tập rèn luyện.', 25000, 3, 0, GETDATE()),
(@UserC, 17, N'Giáo trình Cơ sở dữ liệu SQL Server', N'Sách gốc đã qua sử dụng, có ghi chú bằng bút chì vài trang.', 75000, 1, 0, GETDATE()),
(@UserA, 18, N'Tài liệu Mạng máy tính Cisco', N'Sách mới 100%, mua nhưng không dùng tới.', 120000, 0, 0, GETDATE()),
(@UserB, 19, N'Giáo trình Kế toán tài chính tập 1', N'Sách gốc cũ, bìa hơi sờn nhưng chữ còn rõ ràng.', 40000, 1, 0, GETDATE()),
(@UserC, 20, N'Bài tập Nguyên lý kế toán', N'Bản photo mới in, chưa sử dụng, rõ nét.', 35000, 2, 0, GETDATE()),
(@UserA, 22, N'Marketing căn bản - Philip Kotler', N'Sách gốc cực xịn, phù hợp cho sinh viên chuyên ngành kinh tế.', 250000, 0, 0, GETDATE()),
(@UserB, 25, N'Công nghệ thực phẩm đại cương', N'Sách bản photo đã sử dụng, có gạch dưới bằng bút dạ quang.', 20000, 3, 0, GETDATE()),
(@UserC, 26, N'Hoá học đại cương tập 2', N'Sách gốc, mua ở nhà sách, bìa bọc nylon cẩn thận.', 80000, 1, 0, GETDATE()),
(@UserA, 27, N'Nguyên lý kinh tế học vĩ mô', N'Sách photo mới 90%.', 30000, 2, 0, GETDATE()),
(@UserB, 15, N'Tài liệu thực hành Lập trình C#', N'Dành cho sinh viên CNTT, sách gốc xịn.', 95000, 0, 0, GETDATE()),
(@UserC, 16, N'Thuật toán chuyên sâu', N'Sách khá hiếm, đã qua sử dụng nhưng còn rất tốt.', 150000, 1, 0, GETDATE());
GO

-- 3. Insert ListingImages
-- Get the IDs of the inserted listings
DECLARE @Listing1 INT = (SELECT TOP 1 ListingId FROM Listings WHERE Title = N'Giáo trình Lập trình căn bản C++' ORDER BY ListingId DESC);
DECLARE @Listing2 INT = (SELECT TOP 1 ListingId FROM Listings WHERE Title = N'Sách bài tập Cấu trúc dữ liệu và giải thuật' ORDER BY ListingId DESC);
DECLARE @Listing3 INT = (SELECT TOP 1 ListingId FROM Listings WHERE Title = N'Giáo trình Cơ sở dữ liệu SQL Server' ORDER BY ListingId DESC);
DECLARE @Listing4 INT = (SELECT TOP 1 ListingId FROM Listings WHERE Title = N'Tài liệu Mạng máy tính Cisco' ORDER BY ListingId DESC);
DECLARE @Listing5 INT = (SELECT TOP 1 ListingId FROM Listings WHERE Title = N'Giáo trình Kế toán tài chính tập 1' ORDER BY ListingId DESC);
DECLARE @Listing6 INT = (SELECT TOP 1 ListingId FROM Listings WHERE Title = N'Bài tập Nguyên lý kế toán' ORDER BY ListingId DESC);
DECLARE @Listing7 INT = (SELECT TOP 1 ListingId FROM Listings WHERE Title = N'Marketing căn bản - Philip Kotler' ORDER BY ListingId DESC);
DECLARE @Listing8 INT = (SELECT TOP 1 ListingId FROM Listings WHERE Title = N'Công nghệ thực phẩm đại cương' ORDER BY ListingId DESC);
DECLARE @Listing9 INT = (SELECT TOP 1 ListingId FROM Listings WHERE Title = N'Hoá học đại cương tập 2' ORDER BY ListingId DESC);
DECLARE @Listing10 INT = (SELECT TOP 1 ListingId FROM Listings WHERE Title = N'Nguyên lý kinh tế học vĩ mô' ORDER BY ListingId DESC);
DECLARE @Listing11 INT = (SELECT TOP 1 ListingId FROM Listings WHERE Title = N'Tài liệu thực hành Lập trình C#' ORDER BY ListingId DESC);
DECLARE @Listing12 INT = (SELECT TOP 1 ListingId FROM Listings WHERE Title = N'Thuật toán chuyên sâu' ORDER BY ListingId DESC);

INSERT INTO ListingImages (ListingId, ImageUrl, SortOrder)
VALUES 
(@Listing1, 'https://picsum.photos/seed/lst1a/400/500', 1),
(@Listing1, 'https://picsum.photos/seed/lst1b/400/500', 2),
(@Listing2, 'https://picsum.photos/seed/lst2a/400/500', 1),
(@Listing3, 'https://picsum.photos/seed/lst3a/400/500', 1),
(@Listing3, 'https://picsum.photos/seed/lst3b/400/500', 2),
(@Listing4, 'https://picsum.photos/seed/lst4a/400/500', 1),
(@Listing5, 'https://picsum.photos/seed/lst5a/400/500', 1),
(@Listing6, 'https://picsum.photos/seed/lst6a/400/500', 1),
(@Listing6, 'https://picsum.photos/seed/lst6b/400/500', 2),
(@Listing7, 'https://picsum.photos/seed/lst7a/400/500', 1),
(@Listing8, 'https://picsum.photos/seed/lst8a/400/500', 1),
(@Listing9, 'https://picsum.photos/seed/lst9a/400/500', 1),
(@Listing10, 'https://picsum.photos/seed/lst10a/400/500', 1),
(@Listing11, 'https://picsum.photos/seed/lst11a/400/500', 1),
(@Listing11, 'https://picsum.photos/seed/lst11b/400/500', 2),
(@Listing12, 'https://picsum.photos/seed/lst12a/400/500', 1);
GO
