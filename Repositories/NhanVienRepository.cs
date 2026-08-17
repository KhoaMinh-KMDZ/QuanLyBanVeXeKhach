using Microsoft.Data.SqlClient;
using QuanLyBanVeXeKhach.Helpers;
using QuanLyBanVeXeKhach.Models;

namespace QuanLyBanVeXeKhach.Repositories
{
    public class NhanVienRepository : BaseRepository
    {
        private static NhanVien Map(SqlDataReader r) => new NhanVien
        {
            MaNV        = I(r, "MaNV"),
            HoTen       = S(r, "HoTen"),
            SDT         = S(r, "SDT"),
            Email       = S(r, "Email"),
            TaiKhoan    = S(r, "TaiKhoan"),
            MatKhauHash = S(r, "MatKhauHash"),
            MaVaiTro    = I(r, "MaVaiTro"),
            TrangThai   = B(r, "TrangThai"),
            NgayTao     = DT(r, "NgayTao"),
            NgayCapNhat = DTN(r, "NgayCapNhat"),
            VaiTro = TryMapVaiTro(r)
        };

        private static VaiTro? TryMapVaiTro(SqlDataReader r)
        {
            try
            {
                r.GetOrdinal("TenVaiTro"); // throws if not present
                return new VaiTro
                {
                    TenVaiTro            = DatabaseHelper.SafeString(r, "TenVaiTro"),
                    QuyenBanVe           = DatabaseHelper.SafeBool(r, "QuyenBanVe"),
                    QuyenQuanLyChuyenXe  = DatabaseHelper.SafeBool(r, "QuyenQuanLyChuyenXe"),
                    QuyenQuanLyDanhMuc   = DatabaseHelper.SafeBool(r, "QuyenQuanLyDanhMuc"),
                    QuyenBaoCao          = DatabaseHelper.SafeBool(r, "QuyenBaoCao"),
                    QuyenQuanLyNhanVien  = DatabaseHelper.SafeBool(r, "QuyenQuanLyNhanVien"),
                };
            }
            catch { return null; }
        }

        // Đăng nhập qua Stored Procedure (gọi sp_DangNhap)
        public NhanVien? DangNhap(string taiKhoan, string matKhauHash)
        {
            const string sql = "EXEC sp_DangNhap @TaiKhoan, @MatKhauHash";
            return DatabaseHelper.ExecuteQuerySingle(sql, Map,
                [P("@TaiKhoan", taiKhoan), P("@MatKhauHash", matKhauHash)]);
        }

        public List<NhanVien> GetAll(string? keyword = null)
        {
            const string sql = @"
                SELECT nv.*, vt.TenVaiTro,
                       vt.QuyenBanVe, vt.QuyenQuanLyChuyenXe, vt.QuyenQuanLyDanhMuc,
                       vt.QuyenBaoCao, vt.QuyenQuanLyNhanVien
                FROM   NhanVien nv
                JOIN   VaiTro   vt ON nv.MaVaiTro = vt.MaVaiTro
                WHERE  (@Keyword IS NULL OR nv.HoTen LIKE '%'+@Keyword+'%'
                        OR nv.TaiKhoan LIKE '%'+@Keyword+'%')
                ORDER BY nv.MaNV";
            return DatabaseHelper.ExecuteQuery(sql, Map, [P("@Keyword", keyword)]);
        }

        public List<NhanVien> GetTaiXe()
        {
            const string sql = "SELECT *, NULL AS TenVaiTro, 0 AS QuyenBanVe, 0 AS QuyenQuanLyChuyenXe, 0 AS QuyenQuanLyDanhMuc, 0 AS QuyenBaoCao, 0 AS QuyenQuanLyNhanVien FROM NhanVien WHERE TrangThai = 1 ORDER BY HoTen";
            return DatabaseHelper.ExecuteQuery(sql, Map);
        }

        public List<VaiTro> GetAllVaiTro()
        {
            const string sql = "SELECT * FROM VaiTro ORDER BY MaVaiTro";
            return DatabaseHelper.ExecuteQuery(sql, r => new VaiTro
            {
                MaVaiTro            = I(r, "MaVaiTro"),
                TenVaiTro           = S(r, "TenVaiTro"),
                MoTa                = S(r, "MoTa"),
                QuyenBanVe          = B(r, "QuyenBanVe"),
                QuyenQuanLyChuyenXe = B(r, "QuyenQuanLyChuyenXe"),
                QuyenQuanLyDanhMuc  = B(r, "QuyenQuanLyDanhMuc"),
                QuyenBaoCao         = B(r, "QuyenBaoCao"),
                QuyenQuanLyNhanVien = B(r, "QuyenQuanLyNhanVien"),
            });
        }

        public int Add(NhanVien nv)
        {
            const string sql = @"INSERT INTO NhanVien (HoTen, SDT, Email, TaiKhoan, MatKhauHash, MaVaiTro)
                                  VALUES (@HoTen, @SDT, @Email, @TaiKhoan, @MatKhauHash, @MaVaiTro);
                                  SELECT SCOPE_IDENTITY();";
            var r = DatabaseHelper.ExecuteScalar(sql, [
                P("@HoTen", nv.HoTen), P("@SDT", nv.SDT), P("@Email", nv.Email),
                P("@TaiKhoan", nv.TaiKhoan), P("@MatKhauHash", nv.MatKhauHash),
                P("@MaVaiTro", nv.MaVaiTro)]);
            return r == null ? 0 : Convert.ToInt32(r);
        }

        public bool Update(NhanVien nv)
        {
            const string sql = @"UPDATE NhanVien
                                 SET HoTen=@HoTen, SDT=@SDT, Email=@Email,
                                     TaiKhoan=@TaiKhoan, MaVaiTro=@MaVaiTro,
                                     NgayCapNhat=GETDATE()
                                 WHERE MaNV=@MaNV";
            return DatabaseHelper.ExecuteNonQuery(sql, [
                P("@HoTen", nv.HoTen), P("@SDT", nv.SDT), P("@Email", nv.Email),
                P("@TaiKhoan", nv.TaiKhoan), P("@MaVaiTro", nv.MaVaiTro),
                P("@MaNV", nv.MaNV)]) > 0;
        }

        public bool DoiMatKhau(int maNV, string matKhauHash)
        {
            const string sql = "UPDATE NhanVien SET MatKhauHash=@MatKhauHash, NgayCapNhat=GETDATE() WHERE MaNV=@MaNV";
            return DatabaseHelper.ExecuteNonQuery(sql, [P("@MatKhauHash", matKhauHash), P("@MaNV", maNV)]) > 0;
        }

        public bool KhoaTaiKhoan(int maNV, bool trangThai)
        {
            const string sql = "UPDATE NhanVien SET TrangThai=@TrangThai WHERE MaNV=@MaNV";
            return DatabaseHelper.ExecuteNonQuery(sql, [P("@TrangThai", trangThai), P("@MaNV", maNV)]) > 0;
        }
    }
}
