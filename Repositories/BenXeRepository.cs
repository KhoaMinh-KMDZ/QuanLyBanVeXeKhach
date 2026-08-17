using QuanLyBanVeXeKhach.Helpers;
using QuanLyBanVeXeKhach.Models;
using System;
using System.Collections.Generic;

namespace QuanLyBanVeXeKhach.Repositories
{
    public class BenXeRepository : BaseRepository
    {
        private static BenXe Map(Microsoft.Data.SqlClient.SqlDataReader r) => new BenXe
        {
            MaBenXe   = I(r, "MaBenXe"),
            TenBenXe  = S(r, "TenBenXe"),
            DiaChi    = S(r, "DiaChi"),
            TinhThanh = S(r, "TinhThanh"),
            TrangThai = B(r, "TrangThai"),
        };

        public List<BenXe> GetAll(string? keyword = null)
        {
            const string sql = @"
                SELECT * FROM BenXe
                WHERE @Keyword IS NULL OR TenBenXe LIKE '%'+@Keyword+'%'
                   OR TinhThanh LIKE '%'+@Keyword+'%'
                ORDER BY MaBenXe";
            return DatabaseHelper.ExecuteQuery(sql, Map, [P("@Keyword", keyword)]);
        }

        public List<BenXe> GetActive()
            => DatabaseHelper.ExecuteQuery("SELECT * FROM BenXe WHERE TrangThai=1 ORDER BY TenBenXe", Map);

        public int Add(BenXe t)
        {
            var r = DatabaseHelper.ExecuteScalar(@"
                INSERT INTO BenXe(TenBenXe,DiaChi,TinhThanh)
                VALUES(@TenBenXe,@DiaChi,@TinhThanh);
                SELECT SCOPE_IDENTITY();",
                [P("@TenBenXe", t.TenBenXe), P("@DiaChi", t.DiaChi), P("@TinhThanh", t.TinhThanh)]);
            return r == null ? 0 : Convert.ToInt32(r);
        }

        public bool Update(BenXe t)
            => DatabaseHelper.ExecuteNonQuery(@"
                UPDATE BenXe
                SET TenBenXe=@TenBenXe, DiaChi=@DiaChi, TinhThanh=@TinhThanh, TrangThai=@TrangThai
                WHERE MaBenXe=@MaBenXe",
                [P("@TenBenXe", t.TenBenXe), P("@DiaChi", t.DiaChi), P("@TinhThanh", t.TinhThanh),
                 P("@TrangThai", t.TrangThai), P("@MaBenXe", t.MaBenXe)]) > 0;

        public bool Delete(int maBenXe)
        {
            try
            {
                // Xóa mềm nếu cần
                return DatabaseHelper.ExecuteNonQuery("UPDATE BenXe SET TrangThai=0 WHERE MaBenXe=@MaBenXe", [P("@MaBenXe", maBenXe)]) >= 0;
            }
            catch { return false; }
        }
    }
}
