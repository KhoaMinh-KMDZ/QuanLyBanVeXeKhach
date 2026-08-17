using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using QuanLyBanVeXeKhach.Repositories;
using QuanLyBanVeXeKhach.Services;

namespace QuanLyBanVeXeKhach.ViewModels
{
    public class BaoCaoViewModel : BaseViewModel
    {
        private DateTime _tuNgay  = DateTime.Today.AddDays(-30);
        private DateTime _denNgay = DateTime.Today;

        public DateTime TuNgay
        {
            get => _tuNgay;
            set { SetProperty(ref _tuNgay, value); }
        }

        public DateTime DenNgay
        {
            get => _denNgay;
            set { SetProperty(ref _denNgay, value); }
        }

        private ObservableCollection<BaoCaoNgay> _danhSachNgay = new();
        public ObservableCollection<BaoCaoNgay> DanhSachNgay
        {
            get => _danhSachNgay;
            set => SetProperty(ref _danhSachNgay, value);
        }

        private ObservableCollection<BaoCaoTuyen> _danhSachTuyen = new();
        public ObservableCollection<BaoCaoTuyen> DanhSachTuyen
        {
            get => _danhSachTuyen;
            set => SetProperty(ref _danhSachTuyen, value);
        }

        private decimal _tongDoanhThu;
        private int     _tongSoVe;

        public decimal TongDoanhThu
        {
            get => _tongDoanhThu;
            set { SetProperty(ref _tongDoanhThu, value); OnPropertyChanged(nameof(TongDoanhThuText)); }
        }

        public int TongSoVe
        {
            get => _tongSoVe;
            set => SetProperty(ref _tongSoVe, value);
        }

        public string TongDoanhThuText => $"{TongDoanhThu:N0} đ";

        public ICommand XemBaoCaoCommand   { get; }
        public ICommand XuatExcelCommand   { get; }

        private readonly BaoCaoRepository _repo = new();

        public BaoCaoViewModel()
        {
            XemBaoCaoCommand = new RelayCommand(_ => Load());
            XuatExcelCommand = new RelayCommand(_ => XuatExcel(),
                _ => DanhSachNgay.Any());
            Load();
        }

        public void Load()
        {
            var data  = _repo.GetBaoCaoNgay(TuNgay, DenNgay);
            var tuyen = _repo.GetBaoCaoTheoTuyen(TuNgay, DenNgay);
            var (dt, soVe) = _repo.GetTongKet(TuNgay, DenNgay);

            DanhSachNgay  = new ObservableCollection<BaoCaoNgay>(data);
            DanhSachTuyen = new ObservableCollection<BaoCaoTuyen>(tuyen);
            TongDoanhThu  = dt;
            TongSoVe      = soVe;
        }

        private void XuatExcel()
        {
            try
            {
                var path = ExportService.XuatBaoCaoExcel(DanhSachNgay.ToList(), TuNgay, DenNgay);
                MessageBox.Show($"Xuất thành công!\n{path}", "Thành Công",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất Excel: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
