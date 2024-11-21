
CREATE DATABASE FastFood
GO 

USE FastFood
GO

-- Bảng Admin
CREATE TABLE Admins (
    ID INT IDENTITY(1, 1),
    MaAdmin VARCHAR(10) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    MatKhau VARCHAR(50) NOT NULL,
    TinhTrang NVARCHAR(50) NOT NULL
);

-- Bảng Khách Hàng
CREATE TABLE Customer (
    ID INT IDENTITY(1, 1),
    MaKhachHang VARCHAR(10) PRIMARY KEY, 
    TenKhachHang NVARCHAR(100) NOT NULL,
    DienThoai VARCHAR(10),
    Email VARCHAR(100),
    DiaChi NVARCHAR(100),
    MatKhau VARCHAR(50),
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- Bảng Danh Mục
CREATE TABLE Categories (
    ID INT IDENTITY(1, 1),
    MaDanhMuc VARCHAR(10) PRIMARY KEY,
    TenDanhMuc NVARCHAR(100),
    MoTaDanhMuc NVARCHAR(100)
);

-- Bảng Sản Phẩm
CREATE TABLE Products (
    ID INT IDENTITY(1, 1),
    MaSanPham VARCHAR(10) PRIMARY KEY,
    HinhSanPham VARCHAR(100),
    TenSanPham NVARCHAR(50),
    GiaSanPham DECIMAL(10, 3),
    SoLuongSanPham INT,
    MoTaSanPham NVARCHAR(100),
    TrangThai NVARCHAR(50),
    MaDanhMuc VARCHAR(10),
    FOREIGN KEY (MaDanhMuc) REFERENCES Categories (MaDanhMuc)
);

-- Bảng Giỏ Hàng
CREATE TABLE Carts ( 
    ID INT IDENTITY(1, 1),
    MaGioHang VARCHAR(10) PRIMARY KEY,
    MaKhachHang VARCHAR(10),
    TongTien DECIMAL(10, 3),
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MaKhachHang) REFERENCES Customer (MaKhachHang)
);

-- Bảng Chi Tiết Giỏ Hàng
CREATE TABLE CartDetails ( 
    MaChiTiet INT IDENTITY(1, 1) PRIMARY KEY,  -- Sử dụng INT hoặc GUID cho khóa chính
    MaGioHang VARCHAR(10),
    MaSanPham VARCHAR(10),
    SoLuongSanPhamMua INT,
    Gia DECIMAL(10, 3), 
    FOREIGN KEY (MaSanPham) REFERENCES Products (MaSanPham),
    FOREIGN KEY (MaGioHang) REFERENCES Carts (MaGioHang)
);




-- ****************** Bảng dữ liệu *********************

-- Bảng Admin
INSERT INTO Admins (MaAdmin, HoTen, Email, MatKhau, TinhTrang)
VALUES
('AD01', N'Đình Vương', 'admin1@gmail.com', '123', N'Hoạt động');

SELECT * FROM Admins;

-- Bảng Customer
INSERT INTO Customers (MaKhachHang, TenKhachHang, DienThoai, Email, DiaChi, MatKhau, CreatedDate) 
VALUES 
('KH01', N'Nguyễn Đình Vương', '0333505127', 'vuong@gmail.com', N'Đồng Nai', '123', '2024-11-09');

SELECT * FROM Customers;

-- Bảng Categories
INSERT INTO Categories (MaDanhMuc, TenDanhMuc)
VALUES
('DM01', N'Gà Rán - Gà Quay'),
('DM02', N'Burger - Cơm - Mì Ý'),
('DM03', N'Thức Ăn Nhẹ'),
('DM04', N'Thức Uống & Tráng Miệng'),
('DM05', N'Combo 1 Người'),
('DM06', N'Combo Nhóm'),
('DM07', N'Sushi'),
('DM08', N'Bánh Kem'),
('DM09', N'Kem Socola'),
('DM10', N'Bắp Rang');

delete Products 

-- Bảng Products
INSERT INTO Products (MaSanPham, HinhSanPham, TenSanPham, GiaSanPham, SoLuongSanPham, MoTaSanPham, TrangThai, MaDanhMuc)
VALUES
('PD001', '/LayoutUser/img/product/product-1.jpg', N'Gà rán giòn', '129', 200, N'Gà rán giòn với lớp vỏ giòn rụm và hương vị đậm đà', N'Còn hàng', 'DM01'),
('PD002', '/LayoutUser/img/product/product-2.jpg', N'Combo gà rán giòn', '135', 150, N'Combo Gà rán ngon với lớp vỏ ngoài giòn tan', N'Còn hàng', 'DM01'),
('PD003', '/LayoutUser/img/product/product-3.png', N'Mì ý', '125', 180, N'Mì ý với sợi mì dai, đậm đà gia vị', N'Còn hàng', 'DM02'),
('PD004', '/LayoutUser/img/product/product-4.jpg', N'Combo mì ý', '139', 120, N'Combo 2 người mì ý chuẩn kiểu ý', N'Còn hàng', 'DM02'),
('PD005', '/LayoutUser/img/product/product-5.jpg', N'Burger Bò', '142', 250, N'Burger Bò, thơm ngon', N'Còn hàng', 'DM02'),
('PD006', '/LayoutUser/img/product/product-6.jpg', N'SuShi', '120', 300, N'SuShi, ngọt thơm tự nhiên', N'Còn hàng', 'DM01'),
('PD007', '/LayoutUser/img/product/product-7.jpg', N'Bánh kem', '133', 100, N'Bánh kem, lớp vỏ kem mỏng và béo ngậy', N'Còn hàng', 'DM04'),
('PD008', '/LayoutUser/img/product/product-8.png', N'Kem ốc quế', '127', 220, N'Kem ốc quế lớp vỏ mỏng và vị socola', N'Còn hàng', 'DM09'),
('PD009', '/LayoutUser/img/product/product-9.png', N'Khoai tây chiên', '130', 90, N'Khoai tây chiên vàng ươm, thơm nức mũi', N'Còn hàng', 'DM04'),
('PD010', '/LayoutUser/img/product/product-10.jpg', N'1 đùi gà', '145', 160, N'1 đùi gà combo 1 người', N'Còn hàng', 'DM05'),
('PD011', '/LayoutUser/img/product/product-11.jpg', N'Nước cam ép', '138', 110, N'Nước cam ép lon ', N'Còn hàng', 'DM04'),
('PD012', '/LayoutUser/img/product/product-12.png', N'Khoai tây lắc', '122', 130, N'Khoai tây lắc phô mai, thơm đậm vị', N'Còn hàng', 'DM01'),
('PD013', '/LayoutUser/img/product/product-13.png', N'Cơm gà trứng', '140', 210, N'Cơm gà trứng tẩm ướp gia vị đặc biệt', N'Còn hàng', 'DM02'),
('PD014', '/LayoutUser/img/product/product-14.png', N'Cơm ức gà', '128', 140, N'Cơm ức gà với hương vị sốt me đậm đà', N'Còn hàng', 'DM02'),
('PD015', '/LayoutUser/img/product/product-15.png', N'Gà quay', '137', 170, N'Món gà quay thơm ngon, đậm đà', N'Còn hàng', 'DM01'),
('PD016', '/LayoutUser/img/product/product-16.jpg', N'Burger', '134', 190, N'Burger nồng hấp dẫn', N'Còn hàng', 'DM02');


-- Kiểm tra dữ liệu đã thêm vào bảng Products
SELECT * FROM Products;
