using System.Collections.ObjectModel;
using System.Windows.Input;
using QuanLyBanVeXeKhach.Helpers;
using QuanLyBanVeXeKhach.Models;
using QuanLyBanVeXeKhach.Repositories;

namespace QuanLyBanVeXeKhach.ViewModels
{
    public class TrangChuViewModel : BaseViewModel
    {
        // Stats
        private int    _soVeHomNay;
        private decimal _doanhThuHomNay;
        private int    _soChuyenSapChay;
        private int    _soChuyenDangChay;

        public int SoVeHomNay
        {
            get => _soVeHomNay;
            set => SetProperty(ref _soVeHomNay, value);
        }
        public decimal DoanhThuHomNay
        {
            get => _doanhThuHomNay;
            set => SetProperty(ref _doanhThuHomNay, value);
        }
        public int SoChuyenSapChay
        {
            get => _soChuyenSapChay;
            set => SetProperty(ref _soChuyenSapChay, value);
        }
        public int SoChuyenDangChay
        {
            get => _soChuyenDangChay;
            set => SetProperty(ref _soChuyenDangChay, value);
        }

        public string DoanhThuText => $"{DoanhThuHomNay:N0} đ";
        public string ChaoMung => $"Xin chào, {SessionManager.CurrentUser?.HoTen ?? ""}!";
        public string NgayHienTai => DateTime.Now.ToString("dddd, dd/MM/yyyy");

        private ObservableCollection<ChuyenXe> _chuyenSapChay = new();
        public ObservableCollection<ChuyenXe> ChuyenSapChay
        {
            get => _chuyenSapChay;
            set => SetProperty(ref _chuyenSapChay, value);
        }

        public ICommand LamMoiCommand { get; }

        private readonly VeXeRepository      _veRepo     = new();
        private readonly ChuyenXeRepository  _chuyenRepo = new();

        public TrangChuViewModel()
        {
            LamMoiCommand = new RelayCommand(_ => Load());
            Load();
        }

        public void Load()
        {
            SoVeHomNay       = _veRepo.GetSoVeHomNay();
            DoanhThuHomNay   = _veRepo.GetDoanhThuHomNay();
            SoChuyenSapChay  = _chuyenRepo.GetSoChuyenSapChay();
            SoChuyenDangChay = _chuyenRepo.GetSoChuyenDangChay();
            OnPropertyChanged(nameof(DoanhThuText));

            var list = _chuyenRepo.GetSapChay(3);
            ChuyenSapChay = new ObservableCollection<ChuyenXe>(list);
        }
    }
}
