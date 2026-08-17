using System;

namespace QuanLyBanVeXeKhach.Models
{
    public class LichSuGiaoDich
    {
        public int MaNhatKy { get; set; }
        public int? MaVe { get; set; }
        public int? MaNV { get; set; }
        public string HanhDong { get; set; } = "";
        public DateTime ThoiGian { get; set; }
        public string GhiChu { get; set; } = "";
        
        // Dữ liệu hiển thị (Join)
        public string TenNhanVien { get; set; } = "";
        
        public string ThoiGianText => ThoiGian.ToString("dd/MM/yyyy HH:mm:ss");
    }
}
