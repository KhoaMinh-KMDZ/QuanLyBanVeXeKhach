using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using QuanLyBanVeXeKhach.Models;
using QuanLyBanVeXeKhach.Repositories;

namespace QuanLyBanVeXeKhach.ViewModels
{
    public class ChuyenXeViewModel : BaseViewModel
    {
        private ObservableCollection<ChuyenXe> _danhSach = new();
        public ObservableCollection<ChuyenXe> DanhSach
        {
            get => _danhSach;
            set => SetProperty(ref _danhSach, value);
        }

        private ChuyenXe? _duocChon;
        public ChuyenXe? DuocChon
        {
            get => _duocChon;
            set { SetProperty(ref _duocChon, value); if (value != null) DienForm(value); }
        }

        // Form fields
        public List<Models.TuyenXe> DanhSachTuyen { get; set; } = new();
        public List<Models.Xe> DanhSachXe { get; set; } = new();
        public List<NhanVien> DanhSachTaiXe { get; set; } = new();

        private Models.TuyenXe? _tuyenChon;
        public Models.TuyenXe? TuyenChon { get => _tuyenChon; set => SetProperty(ref _tuyenChon, value); }

        private Models.Xe? _xeChon;
        public Models.Xe? XeChon { get => _xeChon; set => SetProperty(ref _xeChon, value); }

        private NhanVien? _taiXeChon;
        public NhanVien? TaiXeChon { get => _taiXeChon; set => SetProperty(ref _taiXeChon, value); }

        private DateTime? _ngayXuatBen = DateTime.Today.AddDays(1);
        public DateTime? NgayXuatBen { get => _ngayXuatBen; set => SetProperty(ref _ngayXuatBen, value); }

        private string _gioXuatBen = "08:00";
        public string GioXuatBen { get => _gioXuatBen; set => SetProperty(ref _gioXuatBen, value); }

        private string _gioVe = "";
        public string GioVe { get => _gioVe; set => SetProperty(ref _gioVe, value); }

        private string _giaVeText = "";
        public string GiaVeText { get => _giaVeText; set => SetProperty(ref _giaVeText, value); }

        private string _ghiChu = "";
        public string GhiChu { get => _ghiChu; set => SetProperty(ref _ghiChu, value); }

        private bool _dangSua;
        public bool DangSua { get => _dangSua; set => SetProperty(ref _dangSua, value); }

        private string _keyword = "";
        public string Keyword { get => _keyword; set { SetProperty(ref _keyword, value); TimKiem(); } }

        public ICommand ThemMoiCommand     { get; }
        public ICommand LuuCommand         { get; }
        public ICommand XoaCommand         { get; }
        public ICommand HuyFormCommand     { get; }
        public ICommand BatDauChayCommand  { get; }
        public ICommand HoanThanhCommand   { get; }
        public ICommand HuyChuyenCommand   { get; }
        public ICommand LamMoiCommand      { get; }

        private readonly ChuyenXeRepository _repo    = new();
        private readonly TuyenXeRepository  _tuyenRepo = new();
        private readonly XeRepository       _xeRepo  = new();
        private readonly NhanVienRepository _nvRepo  = new();

        public ChuyenXeViewModel()
        {
            ThemMoiCommand    = new RelayCommand(_ => BatDauThem());
            LuuCommand        = new RelayCommand(_ => Luu());
            XoaCommand        = new RelayCommand(_ => Xoa(), _ => DuocChon != null);
            HuyFormCommand    = new RelayCommand(_ => { DangSua = false; DuocChon = null; });
            BatDauChayCommand = new RelayCommand(_ => CapNhatTT(1), _ => DuocChon?.TrangThai == 0);
            HoanThanhCommand  = new RelayCommand(_ => CapNhatTT(2), _ => DuocChon?.TrangThai == 1);
            HuyChuyenCommand  = new RelayCommand(_ => CapNhatTT(3), _ => DuocChon?.TrangThai is 0 or 1);
            LamMoiCommand     = new RelayCommand(_ => Load());
            Load();
        }

        public void Load()
        {
            DanhSachTuyen  = _tuyenRepo.GetActive();
            DanhSachXe     = _xeRepo.GetActive();
            DanhSachTaiXe  = _nvRepo.GetTaiXe();
            OnPropertyChanged(nameof(DanhSachTuyen));
            OnPropertyChanged(nameof(DanhSachXe));
            OnPropertyChanged(nameof(DanhSachTaiXe));
            TimKiem();
        }

        private void TimKiem()
        {
            var list = _repo.GetAll(keyword: string.IsNullOrWhiteSpace(Keyword) ? null : Keyword);
            DanhSach = new ObservableCollection<ChuyenXe>(list);
        }

        private void DienForm(ChuyenXe cx)
        {
            TuyenChon   = DanhSachTuyen.FirstOrDefault(t => t.MaTuyen == cx.MaTuyen);
            XeChon      = DanhSachXe.FirstOrDefault(x => x.MaXe == cx.MaXe);
            TaiXeChon   = DanhSachTaiXe.FirstOrDefault(nv => nv.MaNV == cx.MaTaiXe);
            NgayXuatBen = cx.ThoiGianXuatBen.Date;
            GioXuatBen  = cx.ThoiGianXuatBen.ToString("HH:mm");
            GioVe       = cx.ThoiGianDuKienDen?.ToString("HH:mm") ?? "";
            GiaVeText   = cx.GiaVe.ToString();
            GhiChu      = cx.GhiChu ?? "";
            DangSua     = true;
        }

        private void BatDauThem()
        {
            DuocChon    = null;
            TuyenChon   = DanhSachTuyen.FirstOrDefault();
            XeChon      = DanhSachXe.FirstOrDefault();
            TaiXeChon   = DanhSachTaiXe.FirstOrDefault();
            NgayXuatBen = DateTime.Today.AddDays(1);
            GioXuatBen  = "08:00";
            GioVe       = "";
            GiaVeText   = "250000";
            GhiChu      = "";
            DangSua     = true;
        }

        private void Luu()
        {
            if (TuyenChon == null || XeChon == null || TaiXeChon == null || NgayXuatBen == null)
            { MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Thiếu dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            if (!TimeSpan.TryParse(GioXuatBen, out var gio))
            { MessageBox.Show("Giờ xuất bến không hợp lệ (HH:mm)!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            if (!decimal.TryParse(GiaVeText, out var giaVe))
            { MessageBox.Show("Giá vé không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            DateTime? tgDen = null;
            if (!string.IsNullOrWhiteSpace(GioVe) && TimeSpan.TryParse(GioVe, out var gioDen))
                tgDen = NgayXuatBen.Value.Date.AddDays(gioDen < gio ? 1 : 0) + gioDen;

            var cx = new ChuyenXe
            {
                MaChuyen         = DuocChon?.MaChuyen ?? 0,
                MaTuyen          = TuyenChon.MaTuyen,
                MaXe             = XeChon.MaXe,
                MaTaiXe          = TaiXeChon.MaNV,
                ThoiGianXuatBen  = NgayXuatBen.Value.Date + gio,
                ThoiGianDuKienDen= tgDen,
                GiaVe            = giaVe,
                GhiChu           = GhiChu,
            };

            bool ok = DuocChon == null ? _repo.Add(cx) > 0 : _repo.Update(cx);
            if (ok)
            {
                MessageBox.Show("Lưu thành công!", "OK", MessageBoxButton.OK, MessageBoxImage.Information);
                DangSua = false; DuocChon = null; Load();
            }
            else
                MessageBox.Show("Lưu thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Xoa()
        {
            if (DuocChon == null) return;
            if (MessageBox.Show($"Hủy chuyến '{DuocChon.ThongTin}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _repo.CapNhatTrangThai(DuocChon.MaChuyen, 3);
                Load();
            }
        }

        private void CapNhatTT(int tt)
        {
            if (DuocChon == null) return;
            string[] labels = { "sắp chạy", "đang chạy", "hoàn thành", "đã hủy" };
            if (MessageBox.Show($"Đặt trạng thái '{labels[tt]}'?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _repo.CapNhatTrangThai(DuocChon.MaChuyen, tt);
                Load();
            }
        }
    }
}
