using System.Collections.ObjectModel;
using System.Windows.Input;
using QuanLyBanVeXeKhach.Helpers;
using QuanLyBanVeXeKhach.Models;
using QuanLyBanVeXeKhach.Repositories;

namespace QuanLyBanVeXeKhach.ViewModels
{
    public class QuanLyVeViewModel : BaseViewModel
    {
        private string _sdtKhachHang = "";
        public string SDTKhachHang
        {
            get => _sdtKhachHang;
            set => SetProperty(ref _sdtKhachHang, value);
        }

        private ObservableCollection<VeXe> _danhSachVe = new();
        public ObservableCollection<VeXe> DanhSachVe
        {
            get => _danhSachVe;
            set => SetProperty(ref _danhSachVe, value);
        }

        private VeXe? _veDuocChon;
        public VeXe? VeDuocChon
        {
            get => _veDuocChon;
            set => SetProperty(ref _veDuocChon, value);
        }

        public ICommand TimKiemCommand { get; }
        public ICommand HuyVeCommand { get; }

        private readonly VeXeRepository _veRepo = new();

        public QuanLyVeViewModel()
        {
            TimKiemCommand = new RelayCommand(_ => TimKiem());
            HuyVeCommand = new RelayCommand(_ => HuyVe());
        }

        private void TimKiem()
        {
            List<VeXe> ds;
            if (string.IsNullOrWhiteSpace(SDTKhachHang))
            {
                ds = _veRepo.GetAllVeXe();
            }
            else
            {
                ds = _veRepo.GetBySoDienThoai(SDTKhachHang.Trim());
            }

            DanhSachVe = new ObservableCollection<VeXe>(ds);

            if (DanhSachVe.Count == 0)
            {
                TienIchThongBao.HienThiThongTin("Không tìm thấy vé nào hợp lệ.");
            }
        }

        private void HuyVe()
        {
            if (VeDuocChon == null)
            {
                TienIchThongBao.HienThiCanhBao("Vui lòng chọn một vé để hủy!");
                return;
            }

            if (VeDuocChon.TrangThai == 2)
            {
                TienIchThongBao.HienThiCanhBao("Vé này đã bị hủy trước đó rồi!");
                return;
            }

            bool xacNhan = TienIchThongBao.HoiDap(
                $"Bạn có chắc chắn muốn hủy vé #{VeDuocChon.MaVe} của khách {VeDuocChon.TenKhachHang} không?", 
                "Xác Nhận Hủy Vé");

            if (xacNhan)
            {
                bool thanhCong = _veRepo.HuyVe(VeDuocChon.MaVe);
                if (thanhCong)
                {
                    TienIchThongBao.HienThiThanhCong("Đã hủy vé thành công. Vui lòng hoàn lại tiền cho khách.");
                    TimKiem(); // Tải lại danh sách
                }
                else
                {
                    TienIchThongBao.HienThiLoi("Lỗi khi hủy vé. Vui lòng thử lại.");
                }
            }
        }
    }
}
