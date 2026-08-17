using QuanLyBanVeXeKhach.Helpers;
using QuanLyBanVeXeKhach.Models;

namespace QuanLyBanVeXeKhach.Repositories
{
    public class ChuyenXeRepository : BaseRepository
    {
        private const string SelectSql = @"
            SELECT cx.*,
                   t.TenTuyen, b1.TenBenXe AS DiemDi, b2.TenBenXe AS DiemDen,
                   x.BienSo, nv.HoTen AS TenTaiXe,
                   lx.TenLoaiXe, lx.SoGhe,
                   (SELECT COUNT(*) FROM VeXe WHERE MaChuyen = cx.MaChuyen AND TrangThai IN (0,1)) AS SoGheDaDat
            FROM   ChuyenXe cx
            JOIN   TuyenXe  t  ON cx.MaTuyen = t.MaTuyen
            LEFT JOIN BenXe b1 ON t.MaBenDi  = b1.MaBenXe
            LEFT JOIN BenXe b2 ON t.MaBenDen = b2.MaBenXe
            JOIN   Xe       x  ON cx.MaXe    = x.MaXe
            JOIN   LoaiXe   lx ON x.MaLoaiXe = lx.MaLoaiXe
            JOIN   NhanVien nv ON cx.MaTaiXe = nv.MaNV";

        private static ChuyenXe Map(Microsoft.Data.SqlClient.SqlDataReader r) => new ChuyenXe
        {
            MaChuyen          = I(r, "MaChuyen"),
            MaTuyen           = I(r, "MaTuyen"),
            MaXe              = I(r, "MaXe"),
            MaTaiXe           = I(r, "MaTaiXe"),
            ThoiGianXuatBen   = DT(r, "ThoiGianXuatBen"),
            ThoiGianDuKienDen = DTN(r, "ThoiGianDuKienDen"),
            GiaVe             = D(r, "GiaVe"),
            TrangThai         = I(r, "TrangThai"),
            GhiChu            = S(r, "GhiChu"),
            NgayTao           = DT(r, "NgayTao"),
            TenTuyen          = S(r, "TenTuyen"),
            DiemDi            = S(r, "DiemDi"),
            DiemDen           = S(r, "DiemDen"),
            BienSo            = S(r, "BienSo"),
            TenTaiXe          = S(r, "TenTaiXe"),
            TenLoaiXe         = S(r, "TenLoaiXe"),
            SoGhe             = I(r, "SoGhe"),
            SoGheDaDat        = I(r, "SoGheDaDat"),
        };

        public List<ChuyenXe> GetAll(int? trangThai = null, string? keyword = null)
        {
            string sql = SelectSql + @"
                WHERE (@TrangThai IS NULL OR cx.TrangThai = @TrangThai)
                  AND (@Keyword IS NULL OR t.TenTuyen LIKE '%'+@Keyword+'%' OR x.BienSo LIKE '%'+@Keyword+'%')
                ORDER BY cx.ThoiGianXuatBen DESC";
            return DatabaseHelper.ExecuteQuery(sql, Map, [P("@TrangThai", trangThai), P("@Keyword", keyword)]);
        }

        public List<ChuyenXe> GetSapChay(int soNgay = 2)
        {
            string sql = SelectSql + @"
                WHERE cx.TrangThai IN (0, 1)
                  AND cx.ThoiGianXuatBen >= GETDATE()
                  AND cx.ThoiGianXuatBen <= DATEADD(DAY, @SoNgay, GETDATE())
                ORDER BY cx.ThoiGianXuatBen ASC";
            return DatabaseHelper.ExecuteQuery(sql, Map, [P("@SoNgay", soNgay)]);
        }

        public int GetSoChuyenSapChay()
        {
            var r = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM ChuyenXe WHERE TrangThai=0");
            return r == null ? 0 : Convert.ToInt32(r);
        }

        public int GetSoChuyenDangChay()
        {
            var r = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM ChuyenXe WHERE TrangThai=1");
            return r == null ? 0 : Convert.ToInt32(r);
        }

        public int Add(ChuyenXe cx)
        {
            const string sql = @"
                INSERT INTO ChuyenXe(MaTuyen,MaXe,MaTaiXe,ThoiGianXuatBen,ThoiGianDuKienDen,GiaVe,GhiChu)
                VALUES(@MaTuyen,@MaXe,@MaTaiXe,@ThoiGianXuatBen,@ThoiGianDuKienDen,@GiaVe,@GhiChu);
                SELECT SCOPE_IDENTITY();";
            var r = DatabaseHelper.ExecuteScalar(sql, [
                P("@MaTuyen", cx.MaTuyen), P("@MaXe", cx.MaXe), P("@MaTaiXe", cx.MaTaiXe),
                P("@ThoiGianXuatBen", cx.ThoiGianXuatBen),
                P("@ThoiGianDuKienDen", cx.ThoiGianDuKienDen),
                P("@GiaVe", cx.GiaVe), P("@GhiChu", cx.GhiChu)]);
            return r == null ? 0 : Convert.ToInt32(r);
        }

        public bool Update(ChuyenXe cx)
            => DatabaseHelper.ExecuteNonQuery(@"
                UPDATE ChuyenXe
                SET MaTuyen=@MaTuyen, MaXe=@MaXe, MaTaiXe=@MaTaiXe,
                    ThoiGianXuatBen=@ThoiGianXuatBen, ThoiGianDuKienDen=@ThoiGianDuKienDen,
                    GiaVe=@GiaVe, GhiChu=@GhiChu
                WHERE MaChuyen=@MaChuyen",
                [P("@MaTuyen", cx.MaTuyen), P("@MaXe", cx.MaXe), P("@MaTaiXe", cx.MaTaiXe),
                 P("@ThoiGianXuatBen", cx.ThoiGianXuatBen),
                 P("@ThoiGianDuKienDen", cx.ThoiGianDuKienDen),
                 P("@GiaVe", cx.GiaVe), P("@GhiChu", cx.GhiChu), P("@MaChuyen", cx.MaChuyen)]) > 0;

        public bool CapNhatTrangThai(int maChuyen, int trangThai)
            => DatabaseHelper.ExecuteNonQuery("EXEC sp_CapNhatTrangThaiChuyen @MaChuyen, @TrangThai",
                [P("@MaChuyen", maChuyen), P("@TrangThai", trangThai)]) >= 0;
    }
}
