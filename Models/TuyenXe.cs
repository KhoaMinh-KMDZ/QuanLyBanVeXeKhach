namespace QuanLyBanVeXeKhach.Models
{
    public class TuyenXe
    {
        public int MaTuyen { get; set; }
        public string TenTuyen { get; set; } = "";
        public int MaBenDi { get; set; }
        public int MaBenDen { get; set; }
        public decimal? KhoangCach { get; set; }
        public string? ThoiGianDiChuyen { get; set; }
        public string? GhiChu { get; set; }
        public bool TrangThai { get; set; } = true;

        public string TenBenDi { get; set; } = "";
        public string TenBenDen { get; set; } = "";

        public string TrangThaiText => TrangThai ? "Hoạt động" : "Tạm dừng";
        public string ThongTin => $"{TenBenDi} → {TenBenDen}";
    }
}
