using QuanLyBanVeXeKhach.Helpers;
using QuanLyBanVeXeKhach.Models;

namespace QuanLyBanVeXeKhach.Repositories
{
    public class TuyenXeRepository : BaseRepository
    {
        private static TuyenXe Map(Microsoft.Data.SqlClient.SqlDataReader r) => new TuyenXe
        {
            MaTuyen          = I(r, "MaTuyen"),
            TenTuyen         = S(r, "TenTuyen"),
            MaBenDi          = I(r, "MaBenDi"),
            MaBenDen         = I(r, "MaBenDen"),
            KhoangCach       = DN(r, "KhoangCach"),
            ThoiGianDiChuyen = S(r, "ThoiGianDiChuyen"),
            GhiChu           = S(r, "GhiChu"),
            TrangThai        = B(r, "TrangThai"),
            TenBenDi         = DatabaseHelper.SafeString(r, "TenBenDi"),
            TenBenDen        = DatabaseHelper.SafeString(r, "TenBenDen")
        };

        public List<TuyenXe> GetAll(string? keyword = null)
        {
            const string sql = @"
                SELECT t.*, b1.TenBenXe AS TenBenDi, b2.TenBenXe AS TenBenDen 
                FROM TuyenXe t
                LEFT JOIN BenXe b1 ON t.MaBenDi = b1.MaBenXe
                LEFT JOIN BenXe b2 ON t.MaBenDen = b2.MaBenXe
                WHERE @Keyword IS NULL OR t.TenTuyen LIKE '%'+@Keyword+'%'
                   OR b1.TenBenXe LIKE '%'+@Keyword+'%' OR b2.TenBenXe LIKE '%'+@Keyword+'%'
                ORDER BY t.MaTuyen";
            return DatabaseHelper.ExecuteQuery(sql, Map, [P("@Keyword", keyword)]);
        }

        public List<TuyenXe> GetActive()
        {
            const string sql = @"
                SELECT t.*, b1.TenBenXe AS TenBenDi, b2.TenBenXe AS TenBenDen 
                FROM TuyenXe t
                LEFT JOIN BenXe b1 ON t.MaBenDi = b1.MaBenXe
                LEFT JOIN BenXe b2 ON t.MaBenDen = b2.MaBenXe
                WHERE t.TrangThai=1 ORDER BY t.TenTuyen";
            return DatabaseHelper.ExecuteQuery(sql, Map);
        }

        public int Add(TuyenXe t)
        {
            var r = DatabaseHelper.ExecuteScalar(@"
                INSERT INTO TuyenXe(TenTuyen,MaBenDi,MaBenDen,KhoangCach,ThoiGianDiChuyen,GhiChu)
                VALUES(@TenTuyen,@MaBenDi,@MaBenDen,@KhoangCach,@ThoiGianDiChuyen,@GhiChu);
                SELECT SCOPE_IDENTITY();",
                [P("@TenTuyen", t.TenTuyen), P("@MaBenDi", t.MaBenDi), P("@MaBenDen", t.MaBenDen),
                 P("@KhoangCach", t.KhoangCach), P("@ThoiGianDiChuyen", t.ThoiGianDiChuyen), P("@GhiChu", t.GhiChu)]);
            return r == null ? 0 : Convert.ToInt32(r);
        }

        public bool Update(TuyenXe t)
            => DatabaseHelper.ExecuteNonQuery(@"
                UPDATE TuyenXe
                SET TenTuyen=@TenTuyen,MaBenDi=@MaBenDi,MaBenDen=@MaBenDen,
                    KhoangCach=@KhoangCach,ThoiGianDiChuyen=@ThoiGianDiChuyen,
                    GhiChu=@GhiChu,TrangThai=@TrangThai
                WHERE MaTuyen=@MaTuyen",
                [P("@TenTuyen", t.TenTuyen), P("@MaBenDi", t.MaBenDi), P("@MaBenDen", t.MaBenDen),
                 P("@KhoangCach", t.KhoangCach), P("@ThoiGianDiChuyen", t.ThoiGianDiChuyen),
                 P("@GhiChu", t.GhiChu), P("@TrangThai", t.TrangThai), P("@MaTuyen", t.MaTuyen)]) > 0;

        public bool Delete(int maTuyen)
        {
            try
            {
                return DatabaseHelper.ExecuteNonQuery("EXEC sp_XoaTuyenXe @MaTuyen", [P("@MaTuyen", maTuyen)]) >= 0;
            }
            catch { return false; }
        }
    }
}
