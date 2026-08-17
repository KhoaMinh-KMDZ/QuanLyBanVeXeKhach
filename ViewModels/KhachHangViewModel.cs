using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using QuanLyBanVeXeKhach.Models;
using QuanLyBanVeXeKhach.Repositories;

namespace QuanLyBanVeXeKhach.ViewModels
{
    public class KhachHangViewModel : BaseViewModel
    {
        private ObservableCollection<KhachHang> _danhSach = new();
        public ObservableCollection<KhachHang> DanhSach
        {
            get => _danhSach;
            set => SetProperty(ref _danhSach, value);
        }

        private KhachHang? _duocChon;
        public KhachHang? DuocChon
        {
            get => _duocChon;
            set { SetProperty(ref _duocChon, value); if (value != null) DienForm(value); }
        }

        private string _hoTen = "", _sdt = "", _email = "", _diaChi = "", _ghiChu = "";
        public string HoTen  { get => _hoTen;  set => SetProperty(ref _hoTen,  value); }
        public string SDT    { get => _sdt;    set => SetProperty(ref _sdt,    value); }
        public string Email  { get => _email;  set => SetProperty(ref _email,  value); }
        public string DiaChi { get => _diaChi; set => SetProperty(ref _diaChi, value); }
        public string GhiChu { get => _ghiChu; set => SetProperty(ref _ghiChu, value); }

        private bool _dangSua;
        public bool DangSua { get => _dangSua; set => SetProperty(ref _dangSua, value); }

        private string _keyword = "";
        public string Keyword { get => _keyword; set { SetProperty(ref _keyword, value); TimKiem(); } }

        public ICommand ThemMoiCommand { get; }
        public ICommand LuuCommand     { get; }
        public ICommand XoaCommand     { get; }
        public ICommand HuyFormCommand { get; }

        private readonly KhachHangRepository _repo = new();

        public KhachHangViewModel()
        {
            ThemMoiCommand = new RelayCommand(_ => BatDauThem());
            LuuCommand     = new RelayCommand(_ => Luu());
            XoaCommand     = new RelayCommand(_ => Xoa(), _ => DuocChon != null);
            HuyFormCommand = new RelayCommand(_ => { DangSua = false; DuocChon = null; });
            TimKiem();
        }

        private void TimKiem()
        {
            DanhSach = new ObservableCollection<KhachHang>(
                _repo.GetAll(string.IsNullOrWhiteSpace(Keyword) ? null : Keyword));
        }

        private void DienForm(KhachHang kh)
        {
            HoTen = kh.HoTen; SDT = kh.SDT; Email = kh.Email ?? ""; DiaChi = kh.DiaChi ?? ""; GhiChu = kh.GhiChu ?? "";
            DangSua = true;
        }

        private void BatDauThem()
        {
            DuocChon = null; HoTen = ""; SDT = ""; Email = ""; DiaChi = ""; GhiChu = "";
            DangSua = true;
        }

        private void Luu()
        {
            if (string.IsNullOrWhiteSpace(HoTen) || string.IsNullOrWhiteSpace(SDT))
            { MessageBox.Show("Vui lòng nhập họ tên và SĐT!", "Thiếu dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            var kh = new KhachHang { MaKH = DuocChon?.MaKH ?? 0, HoTen = HoTen.Trim(), SDT = SDT.Trim(), Email = Email, DiaChi = DiaChi, GhiChu = GhiChu };
            bool ok = DuocChon == null ? _repo.Add(kh) > 0 : _repo.Update(kh);
            if (ok) { DangSua = false; DuocChon = null; TimKiem(); }
            else MessageBox.Show("SĐT đã tồn tại hoặc lỗi lưu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Xoa()
        {
            if (DuocChon == null) return;
            if (MessageBox.Show($"Xóa khách hàng '{DuocChon.HoTen}'?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                bool ok = _repo.Delete(DuocChon.MaKH);
                if (!ok) MessageBox.Show("Không thể xóa! Khách đã có lịch sử mua vé.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                else { DuocChon = null; TimKiem(); }
            }
        }
    }
}
