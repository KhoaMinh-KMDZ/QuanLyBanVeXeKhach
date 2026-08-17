using QuanLyBanVeXeKhach.Helpers;
using QuanLyBanVeXeKhach.Models;

namespace QuanLyBanVeXeKhach.Repositories
{
    public class XeRepository : BaseRepository
    {
        private static Xe MapXe(Microsoft.Data.SqlClient.SqlDataReader r) => new Xe
        {
            MaXe       = I(r, "MaXe"),
            BienSo     = S(r, "BienSo"),
            MaLoaiXe   = I(r, "MaLoaiXe"),
            NamSanXuat = DatabaseHelper.SafeIntNull(r, "NamSanXuat"),
            TrangThai  = S(r, "TrangThai"),
            GhiChu     = S(r, "GhiChu"),
            LoaiXe = new LoaiXe
            {
                MaLoaiXe  = I(r, "MaLoaiXe"),
                TenLoaiXe = DatabaseHelper.SafeString(r, "TenLoaiXe"),
                SoGhe     = DatabaseHelper.SafeInt(r, "SoGhe"),
            }
        };

        private const string SelectSql = @"
            SELECT x.*, lx.TenLoaiXe, lx.SoGhe
            FROM Xe x JOIN LoaiXe lx ON x.MaLoaiXe = lx.MaLoaiXe";

        public List<Xe> GetAll(string? keyword = null)
        {
            string sql = SelectSql + @"
                WHERE (@Keyword IS NULL OR x.BienSo LIKE '%'+@Keyword+'%' OR lx.TenLoaiXe LIKE '%'+@Keyword+'%')
                ORDER BY x.BienSo";
            return DatabaseHelper.ExecuteQuery(sql, MapXe, [P("@Keyword", keyword)]);
        }

        public List<Xe> GetActive()
        {
            return DatabaseHelper.ExecuteQuery(SelectSql + " WHERE x.TrangThai = N'Hoạt động' ORDER BY x.BienSo", MapXe);
        }

        public int Add(Xe xe)
        {
            var r = DatabaseHelper.ExecuteScalar(
                "INSERT INTO Xe(BienSo,MaLoaiXe,NamSanXuat,GhiChu) VALUES(@BienSo,@MaLoaiXe,@NamSanXuat,@GhiChu); SELECT SCOPE_IDENTITY();",
                [P("@BienSo", xe.BienSo), P("@MaLoaiXe", xe.MaLoaiXe), P("@NamSanXuat", xe.NamSanXuat), P("@GhiChu", xe.GhiChu)]);
            return r == null ? 0 : Convert.ToInt32(r);
        }

        public bool Update(Xe xe)
        {
            return DatabaseHelper.ExecuteNonQuery(
                "UPDATE Xe SET BienSo=@BienSo, MaLoaiXe=@MaLoaiXe, NamSanXuat=@NamSanXuat, TrangThai=@TrangThai, GhiChu=@GhiChu WHERE MaXe=@MaXe",
                [P("@BienSo", xe.BienSo), P("@MaLoaiXe", xe.MaLoaiXe), P("@NamSanXuat", xe.NamSanXuat),
                 P("@TrangThai", xe.TrangThai), P("@GhiChu", xe.GhiChu), P("@MaXe", xe.MaXe)]) > 0;
        }

        public bool Delete(int maXe)
            => DatabaseHelper.ExecuteNonQuery("DELETE FROM Xe WHERE MaXe=@MaXe", [P("@MaXe", maXe)]) > 0;

        // ─── LoaiXe ───────────────────────────────────────────────
        public List<LoaiXe> GetAllLoaiXe()
        {
            return DatabaseHelper.ExecuteQuery("SELECT * FROM LoaiXe ORDER BY MaLoaiXe", r => new LoaiXe
            {
                MaLoaiXe  = I(r, "MaLoaiXe"),
                TenLoaiXe = S(r, "TenLoaiXe"),
                SoGhe     = I(r, "SoGhe"),
                MoTa      = S(r, "MoTa"),
            });
        }

        public int AddLoaiXe(LoaiXe lx)
        {
            var r = DatabaseHelper.ExecuteScalar(
                "INSERT INTO LoaiXe(TenLoaiXe,SoGhe,MoTa) VALUES(@TenLoaiXe,@SoGhe,@MoTa); SELECT SCOPE_IDENTITY();",
                [P("@TenLoaiXe", lx.TenLoaiXe), P("@SoGhe", lx.SoGhe), P("@MoTa", lx.MoTa)]);
            return r == null ? 0 : Convert.ToInt32(r);
        }

        public bool UpdateLoaiXe(LoaiXe lx)
            => DatabaseHelper.ExecuteNonQuery("UPDATE LoaiXe SET TenLoaiXe=@TenLoaiXe, SoGhe=@SoGhe, MoTa=@MoTa WHERE MaLoaiXe=@MaLoaiXe",
                [P("@TenLoaiXe", lx.TenLoaiXe), P("@SoGhe", lx.SoGhe), P("@MoTa", lx.MoTa), P("@MaLoaiXe", lx.MaLoaiXe)]) > 0;
    }
}
