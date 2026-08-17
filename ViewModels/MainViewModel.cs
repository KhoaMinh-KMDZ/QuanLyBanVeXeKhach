using System.Windows;
using System.Windows.Input;
using QuanLyBanVeXeKhach.Helpers;

namespace QuanLyBanVeXeKhach.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel _currentView;
        public BaseViewModel CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        // Thông tin user hiện tại
        public string ThongTinUser => SessionManager.CurrentUser != null
            ? $"{SessionManager.CurrentUser.HoTen}  |  {SessionManager.CurrentUser.TenVaiTro}"
            : "";

        // Phân quyền hiển thị menu
        public bool CanBanVe          => SessionManager.CanBanVe;
        public bool CanQuanLyChuyenXe => SessionManager.CanQuanLyChuyenXe;
        public bool CanQuanLyDanhMuc  => SessionManager.CanQuanLyDanhMuc;
        public bool CanBaoCao         => SessionManager.CanBaoCao;
        public bool CanQuanLyNhanVien => SessionManager.CanQuanLyNhanVien;

        // Commands
        public ICommand HienThiTrangChuCommand   { get; }
        public ICommand HienThiBanVeCommand      { get; }
        public ICommand HienThiQuanLyVeCommand   { get; }
        public ICommand HienThiChuyenXeCommand   { get; }
        public ICommand HienThiTuyenXeCommand    { get; }
        public ICommand HienThiBenXeCommand      { get; }
        public ICommand HienThiXeCommand         { get; }
        public ICommand HienThiKhachHangCommand  { get; }
        public ICommand HienThiNhanVienCommand   { get; }
        public ICommand HienThiBaoCaoCommand     { get; }
        public ICommand HienThiLichSuGiaoDichCommand { get; }
        public ICommand DangXuatCommand          { get; }

        public MainViewModel()
        {
            HienThiTrangChuCommand  = new RelayCommand(_ => CurrentView = new TrangChuViewModel());
            HienThiBanVeCommand     = new RelayCommand(_ => CurrentView = new BanVeViewModel());
            HienThiQuanLyVeCommand  = new RelayCommand(_ => CurrentView = new QuanLyVeViewModel());
            HienThiChuyenXeCommand  = new RelayCommand(_ => CurrentView = new ChuyenXeViewModel());
            HienThiTuyenXeCommand   = new RelayCommand(_ => CurrentView = new TuyenXeViewModel());
            HienThiBenXeCommand     = new RelayCommand(_ => CurrentView = new BenXeViewModel());
            HienThiXeCommand        = new RelayCommand(_ => CurrentView = new XeViewModel());
            HienThiKhachHangCommand = new RelayCommand(_ => CurrentView = new KhachHangViewModel());
            HienThiNhanVienCommand  = new RelayCommand(_ => CurrentView = new NhanVienViewModel());
            HienThiBaoCaoCommand    = new RelayCommand(_ => CurrentView = new BaoCaoViewModel());
            HienThiLichSuGiaoDichCommand = new RelayCommand(_ => CurrentView = new LichSuGiaoDichViewModel());
            DangXuatCommand         = new RelayCommand(XuLyDangXuat);

            // Mặc định trang chủ
            _currentView = new TrangChuViewModel();
        }

        private void XuLyDangXuat(object? param)
        {
            var ketQua = MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác Nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (ketQua == MessageBoxResult.Yes)
            {
                SessionManager.Logout();
                var loginWin = new Views.DangNhapView();
                Application.Current.MainWindow = loginWin;
                loginWin.Show();

                if (param is Window w) w.Close();
                else
                {
                    foreach (Window win in Application.Current.Windows)
                    {
                        if (win is Views.MainView) { win.Close(); break; }
                    }
                }
            }
        }
    }
}
