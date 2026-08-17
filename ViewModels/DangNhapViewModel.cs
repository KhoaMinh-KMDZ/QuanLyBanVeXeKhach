using System.Windows;
using System.Windows.Input;
using QuanLyBanVeXeKhach.Helpers;
using QuanLyBanVeXeKhach.Repositories;

namespace QuanLyBanVeXeKhach.ViewModels
{
    public class DangNhapViewModel : BaseViewModel
    {
        private string _tenDangNhap = "";
        public string TenDangNhap
        {
            get => _tenDangNhap;
            set => SetProperty(ref _tenDangNhap, value);
        }

        private string _thongBaoLoi = "";
        public string ThongBaoLoi
        {
            get => _thongBaoLoi;
            set 
            { 
                SetProperty(ref _thongBaoLoi, value); 
                OnPropertyChanged(nameof(CoLoi)); 
            }
        }

        public bool CoLoi => !string.IsNullOrEmpty(ThongBaoLoi);

        private bool _dangXuLy;
        public bool DangXuLy
        {
            get => _dangXuLy;
            set => SetProperty(ref _dangXuLy, value);
        }

        public ICommand DangNhapCommand { get; }
        public Action? DangNhapThanhCong { get; set; }

        private readonly NhanVienRepository _repo = new();

        public DangNhapViewModel()
        {
            DangNhapCommand = new RelayCommand(XuLyDangNhap);
        }

        private void XuLyDangNhap(object? matKhauParam)
        {
            ThongBaoLoi = "";

            if (string.IsNullOrWhiteSpace(TenDangNhap))
            {
                ThongBaoLoi = "Vui lòng nhập tên đăng nhập!";
                return;
            }

            var matKhau = matKhauParam?.ToString() ?? "";
            if (string.IsNullOrEmpty(matKhau))
            {
                ThongBaoLoi = "Vui lòng nhập mật khẩu!";
                return;
            }

            // Hash mật khẩu và kiểm tra
            var hash = HashHelper.SHA256Hash(matKhau);
            var nhanVien = _repo.DangNhap(TenDangNhap.Trim(), hash);

            if (nhanVien != null && nhanVien.VaiTro != null)
            {
                SessionManager.Login(nhanVien, nhanVien.VaiTro);
                DangNhapThanhCong?.Invoke();
            }
            else
            {
                ThongBaoLoi = "Tên đăng nhập hoặc mật khẩu không đúng!";
            }
        }
    }
}
