using QuanLyBanVeXeKhach.Helpers;
using QuanLyBanVeXeKhach.Models;

namespace QuanLyBanVeXeKhach.Repositories
{
    public class VeXeRepository : BaseRepository
    {
        private static VeXe Map(Microsoft.Data.SqlClient.SqlDataReader r) => new VeXe
        {
            MaVe       = I(r, "MaVe"),
            MaChuyen   = I(r, "MaChuyen"),
            MaKH       = I(r, "MaKH"),
            MaNV       = I(r, "MaNV"),
            SoGhe      = S(r, "SoGhe"),
            GiaVe      = D(r, "GiaVe"),
            NgayBan    = DT(r, "NgayBan"),
            TrangThai  = I(r, "TrangThai"),
            GhiChu     = S(r, "GhiChu"),
            TenKhachHang       = DatabaseHelper.SafeString(r, "TenKhachHang"),
            SDTKhachHang       = DatabaseHelper.SafeString(r, "SDTKhachHang"),
            TenTuyen           = DatabaseHelper.SafeString(r, "TenTuyen"),
            ThoiGianXuatBenText = DatabaseHelper.SafeDateTime(r, "ThoiGianXuatBen") == DateTime.MinValue
                                  ? "" : DatabaseHelper.SafeDateTime(r, "ThoiGianXuatBen").ToString("dd/MM/yyyy HH:mm"),
            TenNhanVien        = DatabaseHelper.SafeString(r, "TenNhanVien"),
        };

        // Lấy danh sách ghế đã đặt (chỉ ghế đang hoạt động)
        public List<string> GetGheDaBan(int maChuyen)
        {
            return DatabaseHelper.ExecuteQuery(
                "SELECT SoGhe FROM VeXe WHERE MaChuyen=@MaChuyen AND TrangThai IN (0,1)",
                r => DatabaseHelper.SafeString(r, "SoGhe"),
                [P("@MaChuyen", maChuyen)]);
        }

        public List<VeXe> GetByChuyen(int maChuyen)
        {
            const string sql = @"
                SELECT v.*, kh.HoTen AS TenKhachHang, kh.SDT AS SDTKhachHang,
                       t.TenTuyen, cx.ThoiGianXuatBen, nv.HoTen AS TenNhanVien
                FROM   VeXe v
                JOIN   KhachHang kh ON v.MaKH    = kh.MaKH
                JOIN   ChuyenXe  cx ON v.MaChuyen = cx.MaChuyen
                JOIN   TuyenXe   t  ON cx.MaTuyen = t.MaTuyen
                JOIN   NhanVien  nv ON v.MaNV     = nv.MaNV
                WHERE  v.MaChuyen = @MaChuyen
                ORDER BY v.SoGhe";
            return DatabaseHelper.ExecuteQuery(sql, Map, [P("@MaChuyen", maChuyen)]);
        }

        public List<VeXe> GetBySoDienThoai(string sdt)
        {
            const string sql = @"
                SELECT v.*, kh.HoTen AS TenKhachHang, kh.SDT AS SDTKhachHang,
                       t.TenTuyen, cx.ThoiGianXuatBen, nv.HoTen AS TenNhanVien
                FROM   VeXe v
                JOIN   KhachHang kh ON v.MaKH    = kh.MaKH
                JOIN   ChuyenXe  cx ON v.MaChuyen = cx.MaChuyen
                JOIN   TuyenXe   t  ON cx.MaTuyen = t.MaTuyen
                JOIN   NhanVien  nv ON v.MaNV     = nv.MaNV
                WHERE  kh.SDT = @SDT
                ORDER BY v.NgayBan DESC";
            return DatabaseHelper.ExecuteQuery(sql, Map, [P("@SDT", sdt)]);
        }

        public List<VeXe> GetAllVeXe()
        {
            const string sql = @"
                SELECT v.*, kh.HoTen AS TenKhachHang, kh.SDT AS SDTKhachHang,
                       t.TenTuyen, cx.ThoiGianXuatBen, nv.HoTen AS TenNhanVien
                FROM   VeXe v
                JOIN   KhachHang kh ON v.MaKH    = kh.MaKH
                JOIN   ChuyenXe  cx ON v.MaChuyen = cx.MaChuyen
                JOIN   TuyenXe   t  ON cx.MaTuyen = t.MaTuyen
                JOIN   NhanVien  nv ON v.MaNV     = nv.MaNV
                ORDER BY v.NgayBan DESC";
            return DatabaseHelper.ExecuteQuery(sql, Map);
        }

        /// <summary>
        /// Gọi Stored Procedure để thanh toán. Mọi thao tác lưu Khách hàng và Vé xe 
        /// đều được gộp chung trong 1 Transaction tại SQL Server.
        /// </summary>
        public int ThucHienThanhToan(VeXe ve, string hoTenKH, string sdtKH)
        {
            // Tên của Stored Procedure vừa tạo trong CSDL
            
            try
            {
                // Gọi hàm ExecuteScalar (chạy lệnh và lấy về cột đầu tiên của dòng đầu tiên)
                // Lưu ý: Đối với việc gọi Stored Procedure có trả về Exception qua RAISERROR,
                // hàm ExecuteScalar trong DatabaseHelper có the catch lỗi và trả về null,
                // nhưng nếu ta muốn ném lỗi lên UI (tới ViewModel) thì ta có thể sửa DatabaseHelper
                // hoặc gọi trực tiếp ở đây. Tạm thời vẫn dùng DatabaseHelper, 
                // nhưng trong đồ án thực tế, ta nên ném (throw) lỗi lên để ViewModel hiển thị.
                
                // Vì DatabaseHelper.ExecuteScalar của bạn đang dùng chuỗi sql trơn,
                // để chạy được Stored Proc qua ExecuteScalar, ta viết lệnh EXEC:
                const string execSql = "EXEC sp_ThanhToanVeXe @MaChuyen, @HoTenKH, @SDTKH, @MaNV, @SoGhe, @GiaVe, @GhiChu";
                
                var r = DatabaseHelper.ExecuteScalar(execSql, [
                    P("@MaChuyen", ve.MaChuyen), 
                    P("@HoTenKH", hoTenKH), 
                    P("@SDTKH", sdtKH),
                    P("@MaNV", ve.MaNV),
                    P("@SoGhe", ve.SoGhe), 
                    P("@GiaVe", ve.GiaVe), 
                    P("@GhiChu", ve.GhiChu)
                ]);
                
                return r == null ? 0 : Convert.ToInt32(r);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                // Ném lỗi do SQL Server báo về (ví dụ: Trùng ghế) lên thẳng ViewModel
                throw new Exception(ex.Message);
            }
        }

        public bool HuyVe(int maVe)
            => DatabaseHelper.ExecuteNonQuery("UPDATE VeXe SET TrangThai=2 WHERE MaVe=@MaVe",
                [P("@MaVe", maVe)]) > 0;

        public decimal GetDoanhThuHomNay()
        {
            var r = DatabaseHelper.ExecuteScalar(
                "SELECT ISNULL(SUM(GiaVe),0) FROM VeXe WHERE TrangThai=1 AND CAST(NgayBan AS DATE)=CAST(GETDATE() AS DATE)");
            return r == null ? 0m : Convert.ToDecimal(r);
        }

        public int GetSoVeHomNay()
        {
            var r = DatabaseHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM VeXe WHERE TrangThai=1 AND CAST(NgayBan AS DATE)=CAST(GETDATE() AS DATE)");
            return r == null ? 0 : Convert.ToInt32(r);
        }
    }
}
