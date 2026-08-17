namespace QuanLyBanVeXeKhach.Models
{
    public class ChuyenXe
    {
        public int MaChuyen { get; set; }
        public int MaTuyen { get; set; }
        public int MaXe { get; set; }
        public int MaTaiXe { get; set; }
        public DateTime ThoiGianXuatBen { get; set; }
        public DateTime? ThoiGianDuKienDen { get; set; }
        public decimal GiaVe { get; set; }
        public int TrangThai { get; set; }   // 0:SắpChạy 1:ĐangChạy 2:HoànThành 3:ĐãHủy
        public string? GhiChu { get; set; }
        public DateTime NgayTao { get; set; }

        // Display helpers (populated via JOIN)
        public string TenTuyen { get; set; } = "";
        public string DiemDi { get; set; } = "";
        public string DiemDen { get; set; } = "";
        public string BienSo { get; set; } = "";
        public string TenTaiXe { get; set; } = "";
        public string TenLoaiXe { get; set; } = "";
        public int SoGhe { get; set; }
        public int SoGheDaDat { get; set; }

        public int SoGheTrong => SoGhe - SoGheDaDat;
        public string GiaVeText => $"{GiaVe:N0} đ";

        public string TrangThaiText => TrangThai switch
        {
            0 => "Sắp chạy",
            1 => "Đang chạy",
            2 => "Hoàn thành",
            3 => "Đã hủy",
            _ => "Không xác định"
        };

        public string ThongTin => $"{TenTuyen} — {ThoiGianXuatBen:dd/MM/yyyy HH:mm}";
    }
}
