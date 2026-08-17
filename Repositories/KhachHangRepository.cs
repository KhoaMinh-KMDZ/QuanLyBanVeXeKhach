using QuanLyBanVeXeKhach.Helpers;
using QuanLyBanVeXeKhach.Models;

namespace QuanLyBanVeXeKhach.Repositories
{
    public class KhachHangRepository : BaseRepository
    {
        private static KhachHang Map(Microsoft.Data.SqlClient.SqlDataReader r) => new KhachHang
        {
            MaKH       = I(r, "MaKH"),
            HoTen      = S(r, "HoTen"),
            SDT        = S(r, "SDT"),
            Email      = S(r, "Email"),
            DiaChi     = S(r, "DiaChi"),
            NgayDangKy = DT(r, "NgayDangKy"),
            GhiChu     = S(r, "GhiChu"),
        };

        public List<KhachHang> GetAll(string? keyword = null)
        {
            const string sql = @"
                SELECT * FROM KhachHang
                WHERE @Keyword IS NULL OR HoTen LIKE '%'+@Keyword+'%' OR SDT LIKE '%'+@Keyword+'%'
                ORDER BY HoTen";
            return DatabaseHelper.ExecuteQuery(sql, Map, [P("@Keyword", keyword)]);
        }

        public KhachHang? GetBySDT(string sdt)
        {
            const string sql = "SELECT * FROM KhachHang WHERE SDT = @SDT";
            return DatabaseHelper.ExecuteQuerySingle(sql, Map, [P("@SDT", sdt)]);
        }

        public int AddOrUpdate(KhachHang kh)
        {
            var existing = GetBySDT(kh.SDT);
            if (existing != null)
            {
                // Update tên nếu cần
                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE KhachHang SET HoTen=@HoTen, Email=@Email, DiaChi=@DiaChi WHERE MaKH=@MaKH",
                    [P("@HoTen", kh.HoTen), P("@Email", kh.Email), P("@DiaChi", kh.DiaChi), P("@MaKH", existing.MaKH)]);
                return existing.MaKH;
            }

            var r = DatabaseHelper.ExecuteScalar(
                "INSERT INTO KhachHang(HoTen,SDT,Email,DiaChi) VALUES(@HoTen,@SDT,@Email,@DiaChi); SELECT SCOPE_IDENTITY();",
                [P("@HoTen", kh.HoTen), P("@SDT", kh.SDT), P("@Email", kh.Email), P("@DiaChi", kh.DiaChi)]);
            return r == null ? 0 : Convert.ToInt32(r);
        }

        public int Add(KhachHang kh)
        {
            var r = DatabaseHelper.ExecuteScalar(
                "INSERT INTO KhachHang(HoTen,SDT,Email,DiaChi,GhiChu) VALUES(@HoTen,@SDT,@Email,@DiaChi,@GhiChu); SELECT SCOPE_IDENTITY();",
                [P("@HoTen", kh.HoTen), P("@SDT", kh.SDT), P("@Email", kh.Email), P("@DiaChi", kh.DiaChi), P("@GhiChu", kh.GhiChu)]);
            return r == null ? 0 : Convert.ToInt32(r);
        }

        public bool Update(KhachHang kh)
        {
            const string sql = "UPDATE KhachHang SET HoTen=@HoTen, SDT=@SDT, Email=@Email, DiaChi=@DiaChi, GhiChu=@GhiChu WHERE MaKH=@MaKH";
            return DatabaseHelper.ExecuteNonQuery(sql, [
                P("@HoTen", kh.HoTen), P("@SDT", kh.SDT), P("@Email", kh.Email),
                P("@DiaChi", kh.DiaChi), P("@GhiChu", kh.GhiChu), P("@MaKH", kh.MaKH)]) > 0;
        }

        public bool Delete(int maKH)
        {
            return DatabaseHelper.ExecuteNonQuery("DELETE FROM KhachHang WHERE MaKH=@MaKH",
                [P("@MaKH", maKH)]) > 0;
        }
    }
}
