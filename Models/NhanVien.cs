namespace QuanLyBanVeXeKhach.Models
{
    public class NhanVien
    {
        public int MaNV { get; set; }
        public string HoTen { get; set; } = "";
        public string SDT { get; set; } = "";
        public string? Email { get; set; }
        public string? TaiKhoan { get; set; }
        public string MatKhauHash { get; set; } = "";
        public int MaVaiTro { get; set; }
        public bool TrangThai { get; set; } = true;
        public DateTime NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }

        // Navigation
        public VaiTro? VaiTro { get; set; }
        public string TenVaiTro => VaiTro?.TenVaiTro ?? "";
        public string TrangThaiText => TrangThai ? "Đang làm" : "Nghỉ việc";
    }
}
