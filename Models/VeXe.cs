namespace QuanLyBanVeXeKhach.Models
{
    public class VeXe
    {
        public int MaVe { get; set; }
        public int MaChuyen { get; set; }
        public int MaKH { get; set; }
        public int MaNV { get; set; }
        public string SoGhe { get; set; } = "";
        public decimal GiaVe { get; set; }
        public DateTime NgayBan { get; set; }
        public int TrangThai { get; set; }  // 0:ĐặtChỗ 1:ĐãThanhToán 2:ĐãHủy
        public string? GhiChu { get; set; }

        // Display helpers (populated via JOIN)
        public string TenKhachHang { get; set; } = "";
        public string SDTKhachHang { get; set; } = "";
        public string TenTuyen { get; set; } = "";
        public string ThoiGianXuatBenText { get; set; } = "";
        public string TenNhanVien { get; set; } = "";

        public string GiaVeText => $"{GiaVe:N0} đ";
        public string NgayBanText => NgayBan.ToString("dd/MM/yyyy HH:mm");

        public string TrangThaiText => TrangThai switch
        {
            0 => "Đặt chỗ",
            1 => "Đã thanh toán",
            2 => "Đã hủy",
            _ => "Không xác định"
        };
    }
}
