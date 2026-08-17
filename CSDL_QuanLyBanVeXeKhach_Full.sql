-- ==============================================================
-- CƠ SỞ DỮ LIỆU QUẢN LÝ BÁN VÉ XE KHÁCH (PHIÊN BẢN NÂNG CẤP)
-- Nhóm 7 - .NET WPF MVVM
-- ==============================================================

-- Tạo lại Database
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'QuanLyBanVeXeKhach')
BEGIN
    ALTER DATABASE QuanLyBanVeXeKhach SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanLyBanVeXeKhach;
END
GO

CREATE DATABASE QuanLyBanVeXeKhach;
GO

-- Kích hoạt Service Broker để hỗ trợ SqlDependency (Đồng bộ thời gian thực)
ALTER DATABASE QuanLyBanVeXeKhach SET ENABLE_BROKER WITH ROLLBACK IMMEDIATE;
GO

USE QuanLyBanVeXeKhach;
GO

-- ==============================================================
-- 1. BẢNG DỮ LIỆU (TABLES)
-- ==============================================================

-- Bảng Vai Trò (phân quyền chi tiết)
CREATE TABLE VaiTro (
    MaVaiTro             INT IDENTITY(1,1) PRIMARY KEY,
    TenVaiTro            NVARCHAR(50)  NOT NULL,
    MoTa                 NVARCHAR(200),
    QuyenBanVe           BIT DEFAULT 0,
    QuyenQuanLyChuyenXe  BIT DEFAULT 0,
    QuyenQuanLyDanhMuc   BIT DEFAULT 0,
    QuyenBaoCao          BIT DEFAULT 0,
    QuyenQuanLyNhanVien  BIT DEFAULT 0,
    NgayTao              DATETIME DEFAULT GETDATE()
);
GO

-- Bảng Nhân Viên (bao gồm Admin, Bán Vé, Tài Xế)
CREATE TABLE NhanVien (
    MaNV         INT IDENTITY(1,1) PRIMARY KEY,
    HoTen        NVARCHAR(100) NOT NULL,
    SDT          VARCHAR(15)   NOT NULL,
    Email        NVARCHAR(100),
    TaiKhoan     VARCHAR(50),              -- Bỏ UNIQUE ở đây để xử lý bằng Filtered Index hoặc code
    MatKhauHash  VARCHAR(64),              -- Mật khẩu SHA-256
    MaVaiTro     INT           NOT NULL DEFAULT 2,
    TrangThai    BIT           DEFAULT 1,  -- 1: Đang làm, 0: Nghỉ việc
    NgayTao      DATETIME      DEFAULT GETDATE(),
    NgayCapNhat  DATETIME,
    CONSTRAINT FK_NhanVien_VaiTro FOREIGN KEY (MaVaiTro) REFERENCES VaiTro(MaVaiTro)
);
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO
CREATE UNIQUE INDEX UQ_NhanVien_TaiKhoan ON NhanVien(TaiKhoan) WHERE TaiKhoan IS NOT NULL;
GO

-- Bảng Khách Hàng (nâng cấp thêm Email, DiaChi, kiểm tra SDT)
CREATE TABLE KhachHang (
    MaKH       INT IDENTITY(1,1) PRIMARY KEY,
    HoTen      NVARCHAR(100) NOT NULL,
    SDT        VARCHAR(15)   NOT NULL UNIQUE,
    Email      NVARCHAR(100),
    DiaChi     NVARCHAR(200),
    NgayDangKy DATETIME DEFAULT GETDATE(),
    GhiChu     NVARCHAR(500),
    CONSTRAINT CHK_KhachHang_SDT CHECK (LEN(SDT) >= 10)
);
GO

-- Bảng Loại Xe
CREATE TABLE LoaiXe (
    MaLoaiXe  INT IDENTITY(1,1) PRIMARY KEY,
    TenLoaiXe NVARCHAR(100) NOT NULL,
    SoGhe     INT           NOT NULL,
    MoTa      NVARCHAR(200)
);
GO

-- Bảng Xe (nâng cấp thêm NamSanXuat, GhiChu)
CREATE TABLE Xe (
    MaXe       INT IDENTITY(1,1) PRIMARY KEY,
    BienSo     VARCHAR(20)   NOT NULL UNIQUE,
    MaLoaiXe   INT           NOT NULL,
    NamSanXuat INT,
    TrangThai  NVARCHAR(50)  DEFAULT N'Hoạt động',
    GhiChu     NVARCHAR(500),
    CONSTRAINT FK_Xe_LoaiXe FOREIGN KEY (MaLoaiXe) REFERENCES LoaiXe(MaLoaiXe)
);
GO

-- Bảng Bến Xe (MỚI)
CREATE TABLE BenXe (
    MaBenXe INT IDENTITY(1,1) PRIMARY KEY,
    TenBenXe NVARCHAR(200) NOT NULL,
    DiaChi NVARCHAR(500),
    TinhThanh NVARCHAR(100),
    TrangThai BIT DEFAULT 1
);
GO

-- Bảng Tuyến Xe
CREATE TABLE TuyenXe (
    MaTuyen INT IDENTITY(1,1) PRIMARY KEY,
    TenTuyen NVARCHAR(200) NOT NULL,
    MaBenDi INT,
    MaBenDen INT,
    KhoangCach DECIMAL(10,1),      
    ThoiGianDiChuyen NVARCHAR(50), 
    GhiChu NVARCHAR(500),
    TrangThai BIT DEFAULT 1,       
	CONSTRAINT FK_TuyenXe_BenDi FOREIGN KEY (MaBenDi) REFERENCES BenXe(MaBenXe),
	CONSTRAINT FK_TuyenXe_BenDen FOREIGN KEY (MaBenDen) REFERENCES BenXe(MaBenXe)
);
GO

-- Bảng Chuyến Xe (lịch trình)
CREATE TABLE ChuyenXe (
    MaChuyen          INT IDENTITY(1,1) PRIMARY KEY,
    MaTuyen           INT           NOT NULL,
    MaXe              INT           NOT NULL,
    MaTaiXe           INT           NOT NULL,
    ThoiGianXuatBen   DATETIME      NOT NULL,
    ThoiGianDuKienDen DATETIME,
    GiaVe             DECIMAL(18,0) NOT NULL,
    TrangThai         INT DEFAULT 0, -- 0:SắpChạy 1:ĐangChạy 2:HoànThành 3:ĐãHủy
    GhiChu            NVARCHAR(500),
    NgayTao           DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_ChuyenXe_TuyenXe FOREIGN KEY (MaTuyen)  REFERENCES TuyenXe(MaTuyen),
    CONSTRAINT FK_ChuyenXe_Xe      FOREIGN KEY (MaXe)     REFERENCES Xe(MaXe),
    CONSTRAINT FK_ChuyenXe_TaiXe   FOREIGN KEY (MaTaiXe)  REFERENCES NhanVien(MaNV),
    CONSTRAINT CHK_ChuyenXe_ThoiGian CHECK (ThoiGianDuKienDen IS NULL OR ThoiGianXuatBen < ThoiGianDuKienDen),
    CONSTRAINT CHK_ChuyenXe_GiaVe CHECK (GiaVe >= 0)
);
GO

-- Bảng Vé Xe (nghiệp vụ cốt lõi - nâng cấp thêm GiaVe, GhiChu)
CREATE TABLE VeXe (
    MaVe      INT IDENTITY(1,1) PRIMARY KEY,
    MaChuyen  INT           NOT NULL,
    MaKH      INT           NOT NULL,
    MaNV      INT           NOT NULL,
    SoGhe     VARCHAR(10)   NOT NULL,
    GiaVe     DECIMAL(18,0) NOT NULL,
    NgayBan   DATETIME      DEFAULT GETDATE(),
    TrangThai INT DEFAULT 1, -- 0:ĐặtChỗ 1:ĐãThanhToán 2:ĐãHủy
    GhiChu    NVARCHAR(500),
    CONSTRAINT FK_VeXe_ChuyenXe FOREIGN KEY (MaChuyen) REFERENCES ChuyenXe(MaChuyen),
    CONSTRAINT FK_VeXe_KhachHang FOREIGN KEY (MaKH)    REFERENCES KhachHang(MaKH),
    CONSTRAINT FK_VeXe_NhanVien  FOREIGN KEY (MaNV)    REFERENCES NhanVien(MaNV),
    CONSTRAINT CHK_VeXe_GiaVe CHECK (GiaVe >= 0)
);
GO

-- Filtered Unique Index: mỗi ghế chỉ được đặt 1 lần cho 1 chuyến (trừ vé đã hủy)
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO
CREATE UNIQUE INDEX UQ_VeXe_Ghe_Active
ON VeXe (MaChuyen, SoGhe)
WHERE TrangThai IN (0, 1);
GO

-- Bảng Nhật Ký Hệ Thống (để trigger ghi log)
CREATE TABLE NhatKyHeThong (
    MaNhatKy  INT IDENTITY(1,1) PRIMARY KEY,
    MaVe      INT,
    MaNV      INT,
    HanhDong  NVARCHAR(50),
    ThoiGian  DATETIME DEFAULT GETDATE(),
    GhiChu    NVARCHAR(500)
);
GO

-- ==============================================================
-- 2. TRIGGERS (RÀNG BUỘC & GHI LOG)
-- ==============================================================

-- Trigger ghi log khi bán vé
CREATE TRIGGER trg_NhatKyBanVe
ON VeXe
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO NhatKyHeThong (MaVe, MaNV, HanhDong, GhiChu)
    SELECT  i.MaVe, i.MaNV,
            N'Bán vé',
            N'Chuyến #' + CAST(i.MaChuyen AS NVARCHAR) + N', Ghế ' + i.SoGhe
    FROM inserted i;
END
GO

-- Trigger ghi log khi cập nhật trạng thái vé
CREATE TRIGGER trg_NhatKyCapNhatVe
ON VeXe
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(TrangThai)
    BEGIN
        INSERT INTO NhatKyHeThong (MaVe, MaNV, HanhDong, GhiChu)
        SELECT  i.MaVe, i.MaNV,
                CASE i.TrangThai
                    WHEN 2 THEN N'Hủy vé'
                    WHEN 1 THEN N'Xác nhận vé'
                    ELSE N'Cập nhật vé'
                END,
                N'Ghế ' + i.SoGhe + N', Chuyến #' + CAST(i.MaChuyen AS NVARCHAR)
        FROM inserted i;
    END
END
GO

-- Trigger ngăn xóa Khách Hàng đã có lịch sử mua vé
CREATE TRIGGER trg_NganXoaKhachHang
ON KhachHang
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM deleted d JOIN VeXe v ON d.MaKH = v.MaKH)
    BEGIN
        RAISERROR(N'Không thể xóa khách hàng đã có lịch sử mua vé trong hệ thống!', 16, 1);
        RETURN;
    END
    DELETE FROM KhachHang WHERE MaKH IN (SELECT MaKH FROM deleted);
END
GO

-- ==============================================================
-- 3. FUNCTIONS (HÀM TÍNH TOÁN)
-- ==============================================================

-- Hàm tính doanh thu theo ngày
CREATE FUNCTION fn_DoanhThuTheoNgay(@Ngay DATE)
RETURNS DECIMAL(18,0)
AS
BEGIN
    DECLARE @DT DECIMAL(18,0);
    SELECT @DT = ISNULL(SUM(GiaVe), 0)
    FROM VeXe
    WHERE CAST(NgayBan AS DATE) = @Ngay AND TrangThai = 1;
    RETURN ISNULL(@DT, 0);
END
GO

-- Hàm đếm số ghế trống của một chuyến
CREATE FUNCTION fn_SoGheTrong(@MaChuyen INT)
RETURNS INT
AS
BEGIN
    DECLARE @SoGhe INT = 0, @SoDaBan INT = 0;
    SELECT @SoGhe = lx.SoGhe
    FROM ChuyenXe cx
    JOIN Xe x       ON cx.MaXe    = x.MaXe
    JOIN LoaiXe lx  ON x.MaLoaiXe = lx.MaLoaiXe
    WHERE cx.MaChuyen = @MaChuyen;

    SELECT @SoDaBan = COUNT(*)
    FROM VeXe
    WHERE MaChuyen = @MaChuyen AND TrangThai IN (0, 1);

    RETURN ISNULL(@SoGhe, 0) - ISNULL(@SoDaBan, 0);
END
GO

-- ==============================================================
-- 4. STORED PROCEDURES
-- ==============================================================

-- SP Đăng nhập (có JOIN VaiTro để lấy quyền)
CREATE PROC sp_DangNhap
    @TaiKhoan   VARCHAR(50),
    @MatKhauHash VARCHAR(64)
AS
BEGIN
    SELECT nv.*,
           vt.TenVaiTro,
           vt.QuyenBanVe,
           vt.QuyenQuanLyChuyenXe,
           vt.QuyenQuanLyDanhMuc,
           vt.QuyenBaoCao,
           vt.QuyenQuanLyNhanVien
    FROM   NhanVien nv
    JOIN   VaiTro   vt ON nv.MaVaiTro = vt.MaVaiTro
    WHERE  nv.TaiKhoan = @TaiKhoan
      AND  nv.MatKhauHash = @MatKhauHash
      AND  nv.TrangThai = 1;
END
GO

-- SP Thêm Chuyến Xe
CREATE PROC sp_ThemChuyenXe
    @MaTuyen          INT,
    @MaXe             INT,
    @MaTaiXe          INT,
    @ThoiGianXuatBen  DATETIME,
    @ThoiGianDuKienDen DATETIME,
    @GiaVe            DECIMAL(18,0),
    @GhiChu           NVARCHAR(500)
AS
BEGIN
    INSERT INTO ChuyenXe (MaTuyen, MaXe, MaTaiXe, ThoiGianXuatBen, ThoiGianDuKienDen, GiaVe, GhiChu)
    VALUES (@MaTuyen, @MaXe, @MaTaiXe, @ThoiGianXuatBen, @ThoiGianDuKienDen, @GiaVe, @GhiChu);
    SELECT SCOPE_IDENTITY() AS MaChuyen;
END
GO

-- SP Cập nhật trạng thái chuyến xe
CREATE PROC sp_CapNhatTrangThaiChuyen
    @MaChuyen  INT,
    @TrangThai INT
AS
BEGIN
    UPDATE ChuyenXe SET TrangThai = @TrangThai WHERE MaChuyen = @MaChuyen;
END
GO

-- SP Bán vé (Sử dụng Transaction để gộp lưu Khách hàng và Vé xe, xử lý tranh chấp)
CREATE PROC sp_ThanhToanVeXe
    @MaChuyen INT,
    @HoTenKH  NVARCHAR(100),
    @SDTKH    VARCHAR(15),
    @MaNV     INT,
    @SoGhe    VARCHAR(10),
    @GiaVe    DECIMAL(18,0),
    @GhiChu   NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Bắt đầu một giao dịch an toàn. Nếu có lỗi ở bất kỳ đâu, sẽ hoàn tác lại toàn bộ.
    BEGIN TRY
        BEGIN TRAN;

        -- 1. Kiểm tra tranh chấp ghế (Race Condition)
        -- Dùng khóa (UPDLOCK) để phong tỏa dòng dữ liệu này trong lúc kiểm tra, 
        -- ngăn không cho người khác giành mất ghế cùng lúc.
        IF EXISTS (
            SELECT 1 FROM VeXe WITH (UPDLOCK, SERIALIZABLE)
            WHERE MaChuyen = @MaChuyen AND SoGhe = @SoGhe AND TrangThai IN (0, 1)
        )
        BEGIN
            RAISERROR(N'Ghế này đã bị người khác đặt mất! Vui lòng chọn ghế khác.', 16, 1);
        END

        -- 2. Xử lý thông tin Khách hàng (Thêm mới hoặc Cập nhật nếu đã có số điện thoại)
        DECLARE @MaKH INT;
        SELECT @MaKH = MaKH FROM KhachHang WHERE SDT = @SDTKH;

        IF @MaKH IS NULL
        BEGIN
            -- Nếu khách chưa tồn tại, tạo khách mới
            INSERT INTO KhachHang (HoTen, SDT) VALUES (@HoTenKH, @SDTKH);
            SET @MaKH = SCOPE_IDENTITY();
        END
        ELSE
        BEGIN
            -- Nếu khách đã tồn tại, cập nhật lại tên mới nhất
            UPDATE KhachHang SET HoTen = @HoTenKH WHERE MaKH = @MaKH;
        END

        -- 3. Tạo Vé Xe
        INSERT INTO VeXe (MaChuyen, MaKH, MaNV, SoGhe, GiaVe, TrangThai, GhiChu)
        VALUES (@MaChuyen, @MaKH, @MaNV, @SoGhe, @GiaVe, 1, @GhiChu);
        
        DECLARE @MaVe INT = SCOPE_IDENTITY();

        -- 4. Nếu cả Khách hàng và Vé xe đều tạo thành công thì lưu vào DB vĩnh viễn (Commit)
        COMMIT TRAN;

        -- Trả về mã vé vừa tạo
        SELECT @MaVe AS MaVe;
    END TRY
    BEGIN CATCH
        -- Nếu xảy ra lỗi ở bất kỳ bước nào (bị trùng ghế, mất kết nối...), 
        -- hủy bỏ toàn bộ các thay đổi ở trên (Rollback) để CSDL không bị rác.
        IF @@TRANCOUNT > 0 
            ROLLBACK TRAN;

        -- Thông báo lỗi chi tiết ra bên ngoài cho C# bắt lấy
        DECLARE @ErrorMsg NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMsg, 16, 1);
    END CATCH
END
GO

-- SP Thêm Tuyến Xe
CREATE PROC sp_ThemTuyenXe
    @TenTuyen NVARCHAR(200),
    @MaBenDi   INT,
    @MaBenDen  INT,
    @KhoangCach DECIMAL(10,1),
    @ThoiGianDiChuyen NVARCHAR(50)
AS
BEGIN
    INSERT INTO TuyenXe (TenTuyen, MaBenDi, MaBenDen, KhoangCach, ThoiGianDiChuyen)
    VALUES (@TenTuyen, @MaBenDi, @MaBenDen, @KhoangCach, @ThoiGianDiChuyen);
    SELECT SCOPE_IDENTITY() AS MaTuyen;
END
GO

-- SP Xóa Tuyến Xe (kiểm tra ràng buộc)
CREATE PROC sp_XoaTuyenXe
    @MaTuyen INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM ChuyenXe WHERE MaTuyen = @MaTuyen)
    BEGIN
        RAISERROR(N'Không thể xóa tuyến đã có chuyến xe. Hãy vô hiệu hóa thay thế!', 16, 1);
        RETURN;
    END
    DELETE FROM TuyenXe WHERE MaTuyen = @MaTuyen;
END
GO

-- ==============================================================
-- 5. CURSOR — Báo cáo doanh thu từng tuyến
-- ==============================================================
CREATE PROC sp_BaoCaoDoanhThuTuyen
AS
BEGIN
    DECLARE @MaTuyen  INT;
    DECLARE @TenTuyen NVARCHAR(200);
    DECLARE @SoVe     INT;
    DECLARE @DoanhThu DECIMAL(18,0);

    DECLARE cur_Tuyen CURSOR FOR
        SELECT MaTuyen, TenTuyen FROM TuyenXe WHERE TrangThai = 1;

    OPEN cur_Tuyen;
    FETCH NEXT FROM cur_Tuyen INTO @MaTuyen, @TenTuyen;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SELECT @SoVe     = ISNULL(COUNT(*), 0),
               @DoanhThu = ISNULL(SUM(v.GiaVe), 0)
        FROM   VeXe v
        JOIN   ChuyenXe cx ON v.MaChuyen = cx.MaChuyen
        WHERE  cx.MaTuyen = @MaTuyen AND v.TrangThai = 1;

        PRINT N'Tuyến: ' + @TenTuyen
            + N' | Số vé: '    + CAST(ISNULL(@SoVe, 0) AS NVARCHAR)
            + N' | Doanh thu: ' + CAST(ISNULL(@DoanhThu, 0) AS NVARCHAR) + N' VNĐ';

        FETCH NEXT FROM cur_Tuyen INTO @MaTuyen, @TenTuyen;
    END

    CLOSE cur_Tuyen;
    DEALLOCATE cur_Tuyen;
END
GO

-- ==============================================================
-- 6. DỮ LIỆU MẪU
-- ==============================================================

-- VaiTro
INSERT INTO VaiTro (TenVaiTro, MoTa, QuyenBanVe, QuyenQuanLyChuyenXe, QuyenQuanLyDanhMuc, QuyenBaoCao, QuyenQuanLyNhanVien)
VALUES
    (N'Admin',           N'Quản trị viên hệ thống toàn quyền', 1, 1, 1, 1, 1),
    (N'NhanVienBanVe',   N'Nhân viên bán vé',                   1, 0, 0, 0, 0),
    (N'QuanLyDieuHanh',  N'Quản lý điều hành',                  1, 1, 1, 1, 0);
GO

-- NhanVien (MatKhau: 123456 => SHA256: 8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92)
INSERT INTO NhanVien (HoTen, SDT, Email, TaiKhoan, MatKhauHash, MaVaiTro)
VALUES
    (N'Admin Hệ Thống',      '0909111222', 'admin@xekhach.com',    'admin',      '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 1),
    (N'Nguyễn Thị Bán Vé',   '0912333444', 'banve1@xekhach.com',   'nhanvien1',  '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 2),
    (N'Trần Văn Quản Lý',    '0988777666', 'quanly@xekhach.com',   'quanly',     '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 3),
    (N'Lê Minh Tài Xế',      '0977555444', NULL,                   NULL,         NULL,                                                             2),
    (N'Phạm Văn Tài Xế',     '0966444333', NULL,                   NULL,         NULL,                                                             2);
GO

-- LoaiXe
INSERT INTO LoaiXe (TenLoaiXe, SoGhe, MoTa)
VALUES
    (N'Limousine 11 chỗ',  11, N'Xe VIP Limousine 11 chỗ'),
    (N'Limousine 16 chỗ',  16, N'Xe Limousine 16 chỗ'),
    (N'Xe 29 chỗ',  29, N'Xe ghế ngồi 29 chỗ'),
    (N'Xe 45 chỗ',  45, N'Xe khách 45 chỗ');
GO

-- Xe
INSERT INTO Xe (BienSo, MaLoaiXe, NamSanXuat)
VALUES
    ('51B-111.11', 1, 2023), -- Limousine 11 chỗ
    ('51B-161.61', 2, 2023), -- Limousine 16 chỗ
    ('51B-292.92', 3, 2020), -- Xe 29 chỗ
    ('51B-454.54', 4, 2023); -- Xe 45 chỗ
GO

-- Bến Xe
INSERT INTO BenXe (TenBenXe, TinhThanh) VALUES
(N'Bến xe Miền Đông', N'TP.HCM'),
(N'Bến xe Miền Tây', N'TP.HCM'),
(N'Bến xe An Sương', N'TP.HCM'),
(N'Bến xe Liên tỉnh Đà Lạt', N'Lâm Đồng'),
(N'Bến xe Vũng Tàu', N'Bà Rịa - Vũng Tàu'),
(N'Bến xe Tây Ninh', N'Tây Ninh');
GO

-- Tuyến Xe
INSERT INTO TuyenXe (TenTuyen, MaBenDi, MaBenDen, KhoangCach, ThoiGianDiChuyen) VALUES
(N'TP.HCM - Đà Lạt', 1, 4, 300, N'8 tiếng'),
(N'TP.HCM - Vũng Tàu', 1, 5, 120, N'2.5 tiếng'),
(N'TP.HCM - Tây Ninh', 3, 6, 90, N'2 tiếng');
GO

-- KhachHang
INSERT INTO KhachHang (HoTen, SDT, Email)
VALUES
    (N'Nguyễn Văn An',   '0901000001', 'an@email.com'),
    (N'Trần Thị Bình',   '0901000002', 'binh@email.com'),
    (N'Lê Văn Cường',    '0901000003', NULL),
    (N'Phạm Thị Dung',   '0901000004', 'dung@email.com');
GO

-- ChuyenXe
INSERT INTO ChuyenXe (MaTuyen, MaXe, MaTaiXe, ThoiGianXuatBen, ThoiGianDuKienDen, GiaVe, TrangThai)
VALUES
    (1, 1, 4, DATEADD(HOUR, 3, GETDATE()),   DATEADD(HOUR, 10, GETDATE()),  250000, 0), -- Xe 1 (11 chỗ)
    (2, 2, 5, DATEADD(HOUR, 5, GETDATE()),   DATEADD(HOUR, 14, GETDATE()),  350000, 0), -- Xe 2 (16 chỗ)
    (3, 3, 4, DATEADD(DAY,  1, GETDATE()),   DATEADD(HOUR, 27, GETDATE()),  150000, 0), -- Xe 3 (29 chỗ)
    (4, 4, 5, DATEADD(HOUR,-2, GETDATE()),   DATEADD(HOUR,  2, GETDATE()),  180000, 1), -- Xe 4 (45 chỗ)
    (1, 1, 4, DATEADD(DAY, -1, GETDATE()),   DATEADD(HOUR,-17, GETDATE()),  250000, 2); -- Xe 1 (11 chỗ)
GO

-- VeXe (vé mẫu cho chuyến đã hoàn thành)
INSERT INTO VeXe (MaChuyen, MaKH, MaNV, SoGhe, GiaVe, NgayBan, TrangThai)
VALUES
    (5, 1, 2, '01', 250000, DATEADD(DAY,-2, GETDATE()), 1),
    (5, 2, 2, '02', 250000, DATEADD(DAY,-2, GETDATE()), 1),
    (5, 3, 2, '03', 250000, DATEADD(DAY,-2, GETDATE()), 2),
    (5, 4, 2, '04', 250000, DATEADD(DAY,-2, GETDATE()), 1),
    (4, 1, 2, '1', 180000, DATEADD(HOUR,-3, GETDATE()), 1),
    (4, 2, 2, '5', 180000, DATEADD(HOUR,-3, GETDATE()), 1);
GO

-- ==============================================================
-- 7. KỊCH BẢN BACKUP VÀ RESTORE (SAO LƯU VÀ PHỤC HỒI)
-- Chú ý: Đây là script mẫu dùng để chạy bằng tay (Manual) 
-- hoặc đưa vào SQL Server Agent Job để chạy định kỳ.
-- ==============================================================

-- 7.1. Backup Full (Sao lưu toàn bộ CSDL ra ổ đĩa)
/*
BACKUP DATABASE QuanLyBanVeXeKhach
TO DISK = 'C:\BackupDB\QuanLyBanVeXeKhach_Full.bak'
WITH FORMAT, 
     MEDIANAME = 'SQLServerBackups', 
     NAME = 'Full Backup of QuanLyBanVeXeKhach';
GO
*/

-- 7.2. Backup Differential (Sao lưu các thay đổi kể từ lần Backup Full gần nhất)
/*
BACKUP DATABASE QuanLyBanVeXeKhach
TO DISK = 'C:\BackupDB\QuanLyBanVeXeKhach_Diff.bak'
WITH DIFFERENTIAL,
     NAME = 'Differential Backup of QuanLyBanVeXeKhach';
GO
*/

-- 7.3. Restore Database (Phục hồi CSDL từ file Backup)
-- LƯU Ý KHI CHẠY LỆNH NÀY: Phải chuyển USE sang master và đóng các kết nối cũ
/*
USE master;
GO
-- Ép ngắt các kết nối đang sử dụng DB để phục hồi
ALTER DATABASE QuanLyBanVeXeKhach SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

-- Phục hồi bản Full trước, chọn NORECOVERY để tiếp tục ghép bản Diff
RESTORE DATABASE QuanLyBanVeXeKhach
FROM DISK = 'C:\BackupDB\QuanLyBanVeXeKhach_Full.bak'
WITH NORECOVERY, REPLACE; 

-- Phục hồi bản Differential, chọn RECOVERY để kết thúc
RESTORE DATABASE QuanLyBanVeXeKhach
FROM DISK = 'C:\BackupDB\QuanLyBanVeXeKhach_Diff.bak'
WITH RECOVERY; 

-- Mở lại chế độ đa người dùng
ALTER DATABASE QuanLyBanVeXeKhach SET MULTI_USER;
GO
*/

