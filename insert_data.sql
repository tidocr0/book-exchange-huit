DELETE FROM Subjects;
DELETE FROM Faculties;
GO

INSERT INTO Faculties (Name) VALUES
(N'Công nghệ Thông tin'),
(N'Thương mại'),
(N'Ngoại ngữ'),
(N'Kinh tế và Quản lý công nghiệp'),
(N'Sinh học và Môi trường'),
(N'Công nghệ Thực phẩm'),
(N'Công nghệ Hoá học'),
(N'Cơ khí'),
(N'Công nghệ Điện - Điện tử'),
(N'Du lịch'),
(N'Tài chính - Kế toán'),
(N'Quản trị Kinh doanh'),
(N'Luật'),
(N'Công nghệ May - Thời trang');
GO

INSERT INTO Subjects (Name, FacultyId) VALUES
(N'Lập trình căn bản', (SELECT FacultyId FROM Faculties WHERE Name = N'Công nghệ Thông tin')),
(N'Cấu trúc dữ liệu và giải thuật', (SELECT FacultyId FROM Faculties WHERE Name = N'Công nghệ Thông tin')),
(N'Cơ sở dữ liệu', (SELECT FacultyId FROM Faculties WHERE Name = N'Công nghệ Thông tin')),
(N'Mạng máy tính', (SELECT FacultyId FROM Faculties WHERE Name = N'Công nghệ Thông tin')),
(N'Kế toán tài chính', (SELECT FacultyId FROM Faculties WHERE Name = N'Tài chính - Kế toán')),
(N'Nguyên lý kế toán', (SELECT FacultyId FROM Faculties WHERE Name = N'Tài chính - Kế toán')),
(N'Quản trị học', (SELECT FacultyId FROM Faculties WHERE Name = N'Quản trị Kinh doanh')),
(N'Marketing căn bản', (SELECT FacultyId FROM Faculties WHERE Name = N'Quản trị Kinh doanh')),
(N'Tiếng Anh 1', (SELECT FacultyId FROM Faculties WHERE Name = N'Ngoại ngữ')),
(N'Tiếng Anh 2', (SELECT FacultyId FROM Faculties WHERE Name = N'Ngoại ngữ')),
(N'Công nghệ thực phẩm đại cương', (SELECT FacultyId FROM Faculties WHERE Name = N'Công nghệ Thực phẩm')),
(N'Hoá học đại cương', (SELECT FacultyId FROM Faculties WHERE Name = N'Công nghệ Hoá học')),
(N'Nguyên lý kinh tế', (SELECT FacultyId FROM Faculties WHERE Name = N'Kinh tế và Quản lý công nghiệp')),
(N'Pháp luật đại cương', (SELECT FacultyId FROM Faculties WHERE Name = N'Luật'));
GO
