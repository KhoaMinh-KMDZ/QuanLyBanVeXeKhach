using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using QuanLyBanVeXeKhach.Models;
using QuanLyBanVeXeKhach.Repositories;

namespace QuanLyBanVeXeKhach.ViewModels
{
    public class XeViewModel : BaseViewModel
    {
        private ObservableCollection<Xe> _danhSach = new();
        public ObservableCollection<Xe> DanhSach
        {
            get => _danhSach;
            set => SetProperty(ref _danhSach, value);
        }

        private Xe? _duocChon;
        public Xe? DuocChon
        {
            get => _duocChon;
            set { SetProperty(ref _duocChon, value); if (value != null) DienForm(value); }
        }

        public List<LoaiXe> DanhSachLoaiXe { get; set; } = new();

        private string _bienSo = "";
        public string BienSo { get => _bienSo; set => SetProperty(ref _bienSo, value); }

        private LoaiXe? _loaiXeChon;
        public LoaiXe? LoaiXeChon { get => _loaiXeChon; set => SetProperty(ref _loaiXeChon, value); }

        private string _namSanXuat = "";
        public string NamSanXuat { get => _namSanXuat; set => SetProperty(ref _namSanXuat, value); }

        private string _trangThai = "Hoạt động";
        public string TrangThai { get => _trangThai; set => SetProperty(ref _trangThai, value); }

        public List<string> DanhSachTrangThai { get; } = new() { "Hoạt động", "Bảo dưỡng", "Ngừng hoạt động" };

        private string _ghiChu = "";
        public string GhiChu { get => _ghiChu; set => SetProperty(ref _ghiChu, value); }

        private bool _dangSua;
        public bool DangSua { get => _dangSua; set => SetProperty(ref _dangSua, value); }

        private string _keyword = "";
        public string Keyword { get => _keyword; set { SetProperty(ref _keyword, value); TimKiem(); } }

        public ICommand ThemMoiCommand { get; }
        public ICommand LuuCommand     { get; }
        public ICommand XoaCommand     { get; }
        public ICommand HuyFormCommand { get; }

        private readonly XeRepository _repo = new();

        public XeViewModel()
        {
            ThemMoiCommand = new RelayCommand(_ => BatDauThem());
            LuuCommand     = new RelayCommand(_ => Luu());
            XoaCommand     = new RelayCommand(_ => Xoa(), _ => DuocChon != null);
            HuyFormCommand = new RelayCommand(_ => { DangSua = false; DuocChon = null; });
            Load();
        }

        private void Load()
        {
            DanhSachLoaiXe = _repo.GetAllLoaiXe();
            OnPropertyChanged(nameof(DanhSachLoaiXe));
            TimKiem();
        }

        private void TimKiem()
        {
            var list = _repo.GetAll(string.IsNullOrWhiteSpace(Keyword) ? null : Keyword);
            DanhSach = new ObservableCollection<Xe>(list);
        }

        private void DienForm(Xe xe)
        {
            BienSo     = xe.BienSo;
            LoaiXeChon = DanhSachLoaiXe.FirstOrDefault(l => l.MaLoaiXe == xe.MaLoaiXe);
            NamSanXuat = xe.NamSanXuat?.ToString() ?? "";
            TrangThai  = xe.TrangThai;
            GhiChu     = xe.GhiChu ?? "";
            DangSua    = true;
        }

        private void BatDauThem()
        {
            DuocChon = null; BienSo = ""; LoaiXeChon = DanhSachLoaiXe.FirstOrDefault();
            NamSanXuat = DateTime.Now.Year.ToString(); TrangThai = "Hoạt động"; GhiChu = "";
            DangSua = true;
        }

        private void Luu()
        {
            if (string.IsNullOrWhiteSpace(BienSo) || LoaiXeChon == null)
            { MessageBox.Show("Điền đầy đủ biển số và loại xe!", "Thiếu dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            int.TryParse(NamSanXuat, out var nam);
            var xe = new Xe
            {
                MaXe       = DuocChon?.MaXe ?? 0,
                BienSo     = BienSo.Trim().ToUpper(),
                MaLoaiXe   = LoaiXeChon.MaLoaiXe,
                NamSanXuat = nam > 1990 ? nam : null,
                TrangThai  = TrangThai,
                GhiChu     = GhiChu.Trim(),
            };

            bool ok = DuocChon == null ? _repo.Add(xe) > 0 : _repo.Update(xe);
            if (ok) { DangSua = false; DuocChon = null; Load(); }
            else MessageBox.Show("Biển số đã tồn tại hoặc lỗi lưu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Xoa()
        {
            if (DuocChon == null) return;
            if (MessageBox.Show($"Xóa xe '{DuocChon.BienSo}'?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                bool ok = _repo.Delete(DuocChon.MaXe);
                if (ok) { DuocChon = null; Load(); }
                else MessageBox.Show("Không thể xóa xe đang có chuyến!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
