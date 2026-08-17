namespace QuanLyBanVeXeKhach.Models
{
    public class BenXe
    {
        public int MaBenXe { get; set; }
        public string TenBenXe { get; set; } = "";
        public string DiaChi { get; set; } = "";
        public string TinhThanh { get; set; } = "";
        public bool TrangThai { get; set; } = true;
        
        public string TrangThaiText => TrangThai ? "Hoạt động" : "Tạm dừng";
    }
}
