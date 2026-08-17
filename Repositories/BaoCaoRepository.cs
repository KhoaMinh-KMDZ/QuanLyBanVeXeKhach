using QuanLyBanVeXeKhach.Helpers;

namespace QuanLyBanVeXeKhach.Repositories
{
    public class BaoCaoNgay
    {
        public DateTime Ngay { get; set; }
        public int SoVe { get; set; }
        public decimal DoanhThu { get; set; }
        public string NgayText => Ngay.ToString("dd/MM/yyyy");
        public string DoanhThuText => $"{DoanhThu:N0} đ";
    }

    public class BaoCaoTuyen
    {
        public string TenTuyen { get; set; } = "";
        public int SoVe { get; set; }
        public decimal DoanhThu { get; set; }
        public string DoanhThuText => $"{DoanhThu:N0} đ";
    }

    public class BaoCaoRepository : BaseRepository
    {
        public List<BaoCaoNgay> GetBaoCaoNgay(DateTime tuNgay, DateTime denNgay)
        {
            const string sql = @"
                SELECT CAST(v.NgayBan AS DATE) AS Ngay,
                       COUNT(*)                AS SoVe,
                       SUM(v.GiaVe)            AS DoanhThu
                FROM   VeXe v
                WHERE  v.TrangThai = 1
                  AND  CAST(v.NgayBan AS DATE) BETWEEN @TuNgay AND @DenNgay
                GROUP BY CAST(v.NgayBan AS DATE)
                ORDER BY Ngay";
            return DatabaseHelper.ExecuteQuery(sql, r => new BaoCaoNgay
            {
                Ngay      = DT(r, "Ngay"),
                SoVe      = I(r, "SoVe"),
                DoanhThu  = D(r, "DoanhThu"),
            }, [P("@TuNgay", tuNgay.Date), P("@DenNgay", denNgay.Date)]);
        }

        public List<BaoCaoTuyen> GetBaoCaoTheoTuyen(DateTime tuNgay, DateTime denNgay)
        {
            const string sql = @"
                SELECT t.TenTuyen, COUNT(*) AS SoVe, SUM(v.GiaVe) AS DoanhThu
                FROM   VeXe v
                JOIN   ChuyenXe cx ON v.MaChuyen = cx.MaChuyen
                JOIN   TuyenXe  t  ON cx.MaTuyen = t.MaTuyen
                WHERE  v.TrangThai = 1
                  AND  CAST(v.NgayBan AS DATE) BETWEEN @TuNgay AND @DenNgay
                GROUP BY t.TenTuyen
                ORDER BY DoanhThu DESC";
            return DatabaseHelper.ExecuteQuery(sql, r => new BaoCaoTuyen
            {
                TenTuyen = S(r, "TenTuyen"),
                SoVe     = I(r, "SoVe"),
                DoanhThu = D(r, "DoanhThu"),
            }, [P("@TuNgay", tuNgay.Date), P("@DenNgay", denNgay.Date)]);
        }

        public (decimal doanhThu, int soVe) GetTongKet(DateTime tuNgay, DateTime denNgay)
        {
            const string sql = @"
                SELECT ISNULL(SUM(GiaVe),0) AS DoanhThu, COUNT(*) AS SoVe
                FROM VeXe
                WHERE TrangThai=1
                  AND CAST(NgayBan AS DATE) BETWEEN @TuNgay AND @DenNgay";
            var result = DatabaseHelper.ExecuteQuery(sql, r => (
                DoanhThu: D(r, "DoanhThu"),
                SoVe:     I(r, "SoVe")),
                [P("@TuNgay", tuNgay.Date), P("@DenNgay", denNgay.Date)]).FirstOrDefault();
            return (result.DoanhThu, result.SoVe);
        }
    }
}
