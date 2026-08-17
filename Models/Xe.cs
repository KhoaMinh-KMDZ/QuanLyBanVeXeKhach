namespace QuanLyBanVeXeKhach.Models
{
    public class Xe
    {
        public int MaXe { get; set; }
        public string BienSo { get; set; } = "";
        public int MaLoaiXe { get; set; }
        public int? NamSanXuat { get; set; }
        public string TrangThai { get; set; } = "Hoạt động";
        public string? GhiChu { get; set; }

        // Navigation
        public LoaiXe? LoaiXe { get; set; }
        public string TenLoaiXe => LoaiXe?.TenLoaiXe ?? "";
        public int SoGhe => LoaiXe?.SoGhe ?? 0;
        public string ThongTin => $"{BienSo} ({TenLoaiXe} - {SoGhe} chỗ)";
    }
}
