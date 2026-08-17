using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using QuanLyBanVeXeKhach.Models;
using QuanLyBanVeXeKhach.Repositories;

namespace QuanLyBanVeXeKhach.ViewModels
{
    public class TuyenXeViewModel : BaseViewModel
    {
        private ObservableCollection<TuyenXe> _danhSach = new();
        public ObservableCollection<TuyenXe> DanhSach
        {
            get => _danhSach;
            set => SetProperty(ref _danhSach, value);
        }

        private ObservableCollection<BenXe> _danhSachBenXe = new();
        public ObservableCollection<BenXe> DanhSachBenXe
        {
            get => _danhSachBenXe;
            set => SetProperty(ref _danhSachBenXe, value);
        }

        private TuyenXe? _duocChon;
        public TuyenXe? DuocChon
        {
            get => _duocChon;
            set { SetProperty(ref _duocChon, value); if (value != null) DienForm(value); }
        }

        // Form
        private string _tenTuyen = "";
        public string TenTuyen { get => _tenTuyen; set => SetProperty(ref _tenTuyen, value); }

        private int _maBenDi;
        public int MaBenDi { get => _maBenDi; set => SetProperty(ref _maBenDi, value); }

        private int _maBenDen;
        public int MaBenDen { get => _maBenDen; set => SetProperty(ref _maBenDen, value); }

        private string _khoangCach = "";
        public string KhoangCach { get => _khoangCach; set => SetProperty(ref _khoangCach, value); }

        private string _thoiGianDiChuyen = "";
        public string ThoiGianDiChuyen { get => _thoiGianDiChuyen; set => SetProperty(ref _thoiGianDiChuyen, value); }

        private bool _trangThai = true;
        public bool TrangThai { get => _trangThai; set => SetProperty(ref _trangThai, value); }

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
        public ICommand LamMoiCommand  { get; }

        private readonly TuyenXeRepository _repo = new();
        private readonly BenXeRepository _benXeRepo = new();

        public TuyenXeViewModel()
        {
            ThemMoiCommand = new RelayCommand(_ => BatDauThem());
            LuuCommand     = new RelayCommand(_ => Luu());
            XoaCommand     = new RelayCommand(_ => Xoa(), _ => DuocChon != null);
            HuyFormCommand = new RelayCommand(_ => { DangSua = false; DuocChon = null; });
            LamMoiCommand  = new RelayCommand(_ => TimKiem());
            TimKiem();
        }

        private void TimKiem()
        {
            var list = _repo.GetAll(string.IsNullOrWhiteSpace(Keyword) ? null : Keyword);
            DanhSach = new ObservableCollection<TuyenXe>(list);

            var bxList = _benXeRepo.GetActive();
            DanhSachBenXe = new ObservableCollection<BenXe>(bxList);
        }

        private void DienForm(TuyenXe t)
        {
            TenTuyen         = t.TenTuyen;
            MaBenDi          = t.MaBenDi;
            MaBenDen         = t.MaBenDen;
            KhoangCach       = t.KhoangCach?.ToString() ?? "";
            ThoiGianDiChuyen = t.ThoiGianDiChuyen ?? "";
            TrangThai        = t.TrangThai;
            GhiChu           = t.GhiChu ?? "";
            DangSua          = true;
        }

        private void BatDauThem()
        {
            DuocChon = null; TenTuyen = ""; MaBenDi = 0; MaBenDen = 0;
            KhoangCach = ""; ThoiGianDiChuyen = ""; TrangThai = true; GhiChu = "";
            DangSua = true;
        }

        private void Luu()
        {
            if (string.IsNullOrWhiteSpace(TenTuyen) || MaBenDi == 0 || MaBenDen == 0)
            { MessageBox.Show("Vui lòng điền đầy đủ thông tin (Tên tuyến và 2 Bến xe)!", "Thiếu dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            if (MaBenDi == MaBenDen)
            { MessageBox.Show("Bến đi và Bến đến không được trùng nhau!", "Lỗi hợp logic", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            decimal.TryParse(KhoangCach, out var kc);
            var t = new TuyenXe
            {
                MaTuyen          = DuocChon?.MaTuyen ?? 0,
                TenTuyen         = TenTuyen.Trim(),
                MaBenDi          = MaBenDi,
                MaBenDen         = MaBenDen,
                KhoangCach       = kc > 0 ? kc : null,
                ThoiGianDiChuyen = ThoiGianDiChuyen.Trim(),
                TrangThai        = TrangThai,
                GhiChu           = GhiChu.Trim(),
            };

            bool ok = DuocChon == null ? _repo.Add(t) > 0 : _repo.Update(t);
            if (ok) { DangSua = false; DuocChon = null; TimKiem(); }
            else MessageBox.Show("Lưu thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Xoa()
        {
            if (DuocChon == null) return;
            if (MessageBox.Show($"Xóa tuyến '{DuocChon.TenTuyen}'?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                bool ok = _repo.Delete(DuocChon.MaTuyen);
                if (ok) { DuocChon = null; TimKiem(); }
                else MessageBox.Show("Không thể xóa tuyến đã có chuyến xe!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
