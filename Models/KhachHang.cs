namespace QuanLyBanVeXeKhach.Models
{
    public class KhachHang
    {
        public int MaKH { get; set; }
        public string HoTen { get; set; } = "";
        public string SDT { get; set; } = "";
        public string? Email { get; set; }
        public string? DiaChi { get; set; }
        public DateTime NgayDangKy { get; set; }
        public string? GhiChu { get; set; }

        public string ThongTin => $"{HoTen} - {SDT}";
    }
}
