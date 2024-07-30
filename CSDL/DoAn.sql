create database [QL_SANBONG_TEST1]
go
USE [QL_SANBONG_TEST1]
GO
drop database [QL_SANBONG_TEST1]
/****** Object:  Table [dbo].[Account]    Script Date: 11/5/2023 9:34:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Account](
	[Username] [varchar](50) NOT NULL,
	[Passworduser] [varchar](50) NULL,
	[DisplayName] [nvarchar](50) NULL,
	[UserType] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DATSAN]    Script Date: 11/5/2023 9:34:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DATSAN](
	[MaSanBong] [char](10) NOT NULL,
	[MaKH] [char](10) NOT NULL,
	[BatDau] [datetime] NULL,
	[KetThuc] [datetime] NULL,
	[LoaiThue] [nvarchar](50) NULL,
	[DonGia] [float] NULL,
PRIMARY KEY CLUSTERED 
(
	[MaSanBong] ASC,
	[MaKH] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HOADON]    Script Date: 11/5/2023 9:34:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HOADON](
    [MaKH] [char](10) NOT NULL,
    [MaSanBong] [char](10) NOT NULL,
    [GioDa] [int] NULL,
    [DonGia] [float] NULL,
    [ThanhTien] [float] NULL, 
PRIMARY KEY CLUSTERED 
(
    [MaKH] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[KHACHHANG]    Script Date: 11/5/2023 9:34:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KHACHHANG](
	[MaKH] [char](10) NOT NULL,
	[TenKH] [nvarchar](50) NULL,
	[GioiTinh] [nvarchar](10) NULL,
	[SDT] [varchar](15) NULL,
	[DiaChi] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaKH] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LOAISAN]    Script Date: 11/5/2023 9:34:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LOAISAN](
	[MaLoaiSan] [char](10) NOT NULL,
	[TenLoaiSan] [nvarchar](50) NULL,
	[GiaTien] [float] NULL,
PRIMARY KEY CLUSTERED 
(
	[MaLoaiSan] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SANBONG]    Script Date: 11/5/2023 9:34:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SANBONG](
	[MaSanBong] [char](10) NOT NULL,
	[MaLoaiSan] [char](10) NULL,
	[TenSanBong] [nvarchar](50) NULL,
	[TinhTrang] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaSanBong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[DATSAN]  WITH CHECK ADD FOREIGN KEY([MaKH])
REFERENCES [dbo].[KHACHHANG] ([MaKH])
GO
ALTER TABLE [dbo].[DATSAN]  WITH CHECK ADD FOREIGN KEY([MaSanBong])
REFERENCES [dbo].[SANBONG] ([MaSanBong])
GO
ALTER TABLE [dbo].[HOADON]  WITH CHECK ADD FOREIGN KEY([MaKH])
REFERENCES [dbo].[KHACHHANG] ([MaKH])
GO
ALTER TABLE [dbo].[HOADON]  WITH CHECK ADD FOREIGN KEY([MaSanBong])	
REFERENCES [dbo].[SANBONG] ([MaSanBong])
GO
ALTER TABLE [dbo].[SANBONG]  WITH CHECK ADD FOREIGN KEY([MaLoaiSan])
REFERENCES [dbo].[LOAISAN] ([MaLoaiSan])
GO

CREATE TRIGGER trgAfterInsert_DATSAN
ON DATSAN
AFTER INSERT
AS
BEGIN
    INSERT INTO HOADON (MaKH, MaSanBong, GioDa, DonGia, ThanhTien)
    SELECT
        i.MaKH,
        i.MaSanBong,
        DATEDIFF(MINUTE, i.BatDau, i.KetThuc) AS GioDa,
        i.DonGia,
        i.DonGia * DATEDIFF(MINUTE, i.BatDau, i.KetThuc) / 60.0 AS ThanhTien
    FROM
        INSERTED i
END;


Drop trigger trgAfterInsert_DATSAN

GO
CREATE TRIGGER trgAfterDelete_DATSAN
ON DATSAN
AFTER DELETE
AS
BEGIN
    DELETE FROM HOADON
    WHERE MaSanBong IN (SELECT MaSanBong FROM DELETED)
END;
GO


INSERT [dbo].[Account] ([Username], [Passworduser], [DisplayName], [UserType]) VALUES (N'admin1', N'adminpass1', N'Quản trị viên 1', N'Quản trị viên')
INSERT [dbo].[Account] ([Username], [Passworduser], [DisplayName], [UserType]) VALUES (N'user1', N'password1', N'Người dùng 1', N'Người dùng thường')
INSERT [dbo].[Account] ([Username], [Passworduser], [DisplayName], [UserType]) VALUES (N'user2', N'password2', N'Người dùng 2', N'Người dùng thường')
GO
INSERT [dbo].[KHACHHANG] ([MaKH], [TenKH], [GioiTinh], [SDT], [DiaChi]) VALUES (N'KH001', N'Nguyễn Văn A', N'Nam', N'0123456789', N'123 Đường ABC, Quận 1, TP.HCM')
INSERT [dbo].[KHACHHANG] ([MaKH], [TenKH], [GioiTinh], [SDT], [DiaChi]) VALUES (N'KH002', N'Trần Thị B', N'Nữ', N'0987654321', N'456 Đường XYZ, Quận 2, TP.HCM')
INSERT [dbo].[KHACHHANG] ([MaKH], [TenKH], [GioiTinh], [SDT], [DiaChi]) VALUES (N'KH003', N'Lê Văn C', N'Nam', N'0369852147', N'789 Đường XYZ, Quận 3, TP.HCM')
INSERT [dbo].[KHACHHANG] ([MaKH], [TenKH], [GioiTinh], [SDT], [DiaChi]) VALUES (N'KH004', N'Phạm Thị D', N'Nữ', N'0758123645', N'987 Đường UVW, Quận 4, TP.HCM')
INSERT [dbo].[KHACHHANG] ([MaKH], [TenKH], [GioiTinh], [SDT], [DiaChi]) VALUES (N'KH005', N'Hoàng Văn E', N'Nam', N'0582467301', N'654 Đường XYZ, Quận 5, TP.HCM')
INSERT [dbo].[KHACHHANG] ([MaKH], [TenKH], [GioiTinh], [SDT], [DiaChi]) VALUES (N'KH006', N'Đỗ Thị F', N'Nữ', N'0695748210', N'987 Đường KLM, Quận 6, TP.HCM')
INSERT [dbo].[KHACHHANG] ([MaKH], [TenKH], [GioiTinh], [SDT], [DiaChi]) VALUES (N'KH007', N'Vũ Văn G', N'Nam', N'0123456789', N'123 Đường ABC, Quận 7, TP.HCM')
INSERT [dbo].[KHACHHANG] ([MaKH], [TenKH], [GioiTinh], [SDT], [DiaChi]) VALUES (N'KH008', N'Nguyễn Thị H', N'Nữ', N'0987654321', N'456 Đường XYZ, Quận 8, TP.HCM')
INSERT [dbo].[KHACHHANG] ([MaKH], [TenKH], [GioiTinh], [SDT], [DiaChi]) VALUES (N'KH009', N'Lê Văn I', N'Nam', N'0369852147', N'789 Đường XYZ, Quận 9, TP.HCM')
INSERT [dbo].[KHACHHANG] ([MaKH], [TenKH], [GioiTinh], [SDT], [DiaChi]) VALUES (N'KH010', N'Trần Văn K', N'Nam', N'0758123645', N'123 Đường KLM, Quận 10, TP.HCM')
INSERT [dbo].[KHACHHANG] ([MaKH], [TenKH], [GioiTinh], [SDT], [DiaChi]) VALUES (N'KH011', N'Trần Văn K', N'Nam', N'0758123645', N'123 Đường KLM, Quận 10, TP.HCM')
INSERT [dbo].[KHACHHANG] ([MaKH], [TenKH], [GioiTinh], [SDT], [DiaChi]) VALUES (N'KH012', N'Trần Văn K', N'Nam', N'0758123645', N'123 Đường KLM, Quận 10, TP.HCM')
GO
INSERT [dbo].[LOAISAN] ([MaLoaiSan], [TenLoaiSan], [GiaTien]) VALUES (N'LS001', N'Sân 5', 25)
INSERT [dbo].[LOAISAN] ([MaLoaiSan], [TenLoaiSan], [GiaTien]) VALUES (N'LS002', N'Sân 7', 30)
INSERT [dbo].[LOAISAN] ([MaLoaiSan], [TenLoaiSan], [GiaTien]) VALUES (N'LS003', N'Sân 11', 50)
GO
INSERT [dbo].[SANBONG] ([MaSanBong], [MaLoaiSan], [TenSanBong], [TinhTrang]) VALUES (N'SB001', N'LS001', N'Sân 1', N'Trống')
INSERT [dbo].[SANBONG] ([MaSanBong], [MaLoaiSan], [TenSanBong], [TinhTrang]) VALUES (N'SB002', N'LS002', N'Sân 2', N'Đã đặt')
INSERT [dbo].[SANBONG] ([MaSanBong], [MaLoaiSan], [TenSanBong], [TinhTrang]) VALUES (N'SB003', N'LS003', N'Sân 3', N'Đã đặt')
INSERT [dbo].[SANBONG] ([MaSanBong], [MaLoaiSan], [TenSanBong], [TinhTrang]) VALUES (N'SB004', N'LS001', N'Sân 4', N'Trống')
INSERT [dbo].[SANBONG] ([MaSanBong], [MaLoaiSan], [TenSanBong], [TinhTrang]) VALUES (N'SB005', N'LS001', N'Sân 5', N'Đã đặt')
INSERT [dbo].[SANBONG] ([MaSanBong], [MaLoaiSan], [TenSanBong], [TinhTrang]) VALUES (N'SB006', N'LS003', N'Sân 6', N'Đã đặt')
INSERT [dbo].[SANBONG] ([MaSanBong], [MaLoaiSan], [TenSanBong], [TinhTrang]) VALUES (N'SB007', N'LS002', N'Sân 7', N'Trống')
INSERT [dbo].[SANBONG] ([MaSanBong], [MaLoaiSan], [TenSanBong], [TinhTrang]) VALUES (N'SB008', N'LS002', N'Sân 8', N'Trống')
INSERT [dbo].[SANBONG] ([MaSanBong], [MaLoaiSan], [TenSanBong], [TinhTrang]) VALUES (N'SB009', N'LS001', N'Sân 9', N'Đã đặt')
INSERT [dbo].[SANBONG] ([MaSanBong], [MaLoaiSan], [TenSanBong], [TinhTrang]) VALUES (N'SB010', N'LS001', N'Sân 10', N'Trống')
INSERT [dbo].[SANBONG] ([MaSanBong], [MaLoaiSan], [TenSanBong], [TinhTrang]) VALUES (N'SB011', N'LS001', N'Sân 11', N'Đã đặt')
GO
INSERT [dbo].[DATSAN] ([MaSanBong], [MaKH], [BatDau], [KetThuc], [LoaiThue], [DonGia]) VALUES (N'SB001', N'KH001', CAST(N'2023-10-25T10:00:00.000' AS DateTime), CAST(N'2023-10-25T11:00:00.000' AS DateTime), N'Đặt trước', 25)
INSERT [dbo].[DATSAN] ([MaSanBong], [MaKH], [BatDau], [KetThuc], [LoaiThue], [DonGia]) VALUES (N'SB002', N'KH002', CAST(N'2023-10-25T14:00:00.000' AS DateTime), CAST(N'2023-10-25T15:30:00.000' AS DateTime), N'Trực tiếp', 30)
INSERT [dbo].[DATSAN] ([MaSanBong], [MaKH], [BatDau], [KetThuc], [LoaiThue], [DonGia]) VALUES (N'SB003', N'KH003', CAST(N'2023-10-26T09:00:00.000' AS DateTime), CAST(N'2023-10-26T10:30:00.000' AS DateTime), N'Đặt trước', 35)
INSERT [dbo].[DATSAN] ([MaSanBong], [MaKH], [BatDau], [KetThuc], [LoaiThue], [DonGia]) VALUES (N'SB004', N'KH004', CAST(N'2023-10-26T16:00:00.000' AS DateTime), CAST(N'2023-10-26T17:00:00.000' AS DateTime), N'Trực tiếp', 40)
INSERT [dbo].[DATSAN] ([MaSanBong], [MaKH], [BatDau], [KetThuc], [LoaiThue], [DonGia]) VALUES (N'SB005', N'KH005', CAST(N'2023-10-27T08:00:00.000' AS DateTime), CAST(N'2023-10-27T09:30:00.000' AS DateTime), N'Đặt trước', 45)
INSERT [dbo].[DATSAN] ([MaSanBong], [MaKH], [BatDau], [KetThuc], [LoaiThue], [DonGia]) VALUES (N'SB006', N'KH006', CAST(N'2023-10-27T11:00:00.000' AS DateTime), CAST(N'2023-10-27T12:00:00.000' AS DateTime), N'Trực tiếp', 50)
INSERT [dbo].[DATSAN] ([MaSanBong], [MaKH], [BatDau], [KetThuc], [LoaiThue], [DonGia]) VALUES (N'SB007', N'KH007', CAST(N'2023-10-28T13:00:00.000' AS DateTime), CAST(N'2023-10-28T14:30:00.000' AS DateTime), N'Trực tiếp', 55)
INSERT [dbo].[DATSAN] ([MaSanBong], [MaKH], [BatDau], [KetThuc], [LoaiThue], [DonGia]) VALUES (N'SB008', N'KH008', CAST(N'2023-10-28T10:00:00.000' AS DateTime), CAST(N'2023-10-28T11:00:00.000' AS DateTime), N'Trực tiếp', 60)
INSERT [dbo].[DATSAN] ([MaSanBong], [MaKH], [BatDau], [KetThuc], [LoaiThue], [DonGia]) VALUES (N'SB009', N'KH009', CAST(N'2023-10-29T15:00:00.000' AS DateTime), CAST(N'2023-10-29T16:30:00.000' AS DateTime), N'Đặt trước', 65)
INSERT [dbo].[DATSAN] ([MaSanBong], [MaKH], [BatDau], [KetThuc], [LoaiThue], [DonGia]) VALUES (N'SB010', N'KH010', CAST(N'2023-10-29T09:00:00.000' AS DateTime), CAST(N'2023-10-29T10:00:00.000' AS DateTime), N'Đặt trước', 70)
GO


SELECT ds.[MaSanBong], sb.[TenSanBong], ds.[MaKH], kh.[TenKH], ds.[BatDau], ds.[KetThuc], ds.[LoaiThue], ds.[DonGia]
FROM [DATSAN] ds
INNER JOIN [SANBONG] sb ON ds.[MaSanBong] = sb.[MaSanBong]
INNER JOIN [KHACHHANG] kh ON ds.[MaKH] = kh.[MaKH]

drop table hoadon
drop table datsan

SELECT * FROM [dbo].[HOADON]
SELECT * FROM [dbo].[DATSAN]
INSERT [dbo].[DATSAN] ([MaSanBong], [MaKH], [BatDau], [KetThuc], [LoaiThue], [DonGia]) 
VALUES (N'SB011', N'KH011', CAST(N'2023-10-30T09:00:00.000' AS DateTime), CAST(N'2023-10-30T10:00:00.000' AS DateTime), N'Đặt trước', 35)
DELETE FROM [DATSAN]
WHERE [MaSanBong] = 'SB011' AND [MaKH] = 'KH011'


SELECT
    YEAR(d.BatDau) AS Nam,
    MONTH(d.BatDau) AS Thang,
    SUM(h.ThanhTien) AS TongDoanhThu
FROM
    DATSAN d
JOIN
    HOADON h ON d.MaSanBong = h.MaSanBong AND d.MaKH = h.MaKH
WHERE
    d.BatDau IS NOT NULL
GROUP BY
    YEAR(d.BatDau),
    MONTH(d.BatDau)
ORDER BY
    Nam, Thang;


	-- Tạo stored procedure
CREATE PROCEDURE spTinhDoanhThu
    @Thang INT,
    @Nam INT,
    @TongDoanhThu FLOAT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Khai báo biến để lưu trữ tổng doanh thu
    DECLARE @DoanhThu FLOAT;

    -- Tính tổng doanh thu từ bảng HoaDon
    SELECT @DoanhThu = SUM(ThanhTien)
    FROM HOADON
    WHERE MONTH(GioDa) = @Thang AND YEAR(GioDa) = @Nam;

    -- Đặt giá trị tổng doanh thu vào biến đầu ra
    SET @TongDoanhThu = ISNULL(@DoanhThu, 0);
END;

-- Khai báo biến để lưu trữ tổng doanh thu
DECLARE @TongDoanhThu FLOAT;

-- Gọi stored procedure
EXEC spTinhDoanhThu @Thang = 10, @Nam = 2023, @TongDoanhThu = @TongDoanhThu OUTPUT;

-- Hiển thị tổng doanh thu
PRINT 'Tổng doanh thu: ' + CONVERT(VARCHAR, @TongDoanhThu);
