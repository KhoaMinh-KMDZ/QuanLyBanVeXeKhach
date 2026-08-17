namespace QuanLyBanVeXeKhach.Models
{
    public class VaiTro
    {
        public int MaVaiTro { get; set; }
        public string TenVaiTro { get; set; } = "";
        public string? MoTa { get; set; }
        public bool QuyenBanVe          { get; set; }
        public bool QuyenQuanLyChuyenXe { get; set; }
        public bool QuyenQuanLyDanhMuc  { get; set; }
        public bool QuyenBaoCao         { get; set; }
        public bool QuyenQuanLyNhanVien { get; set; }
        public DateTime NgayTao { get; set; }
    }
}
