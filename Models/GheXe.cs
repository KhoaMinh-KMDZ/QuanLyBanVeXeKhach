using System.ComponentModel;

namespace QuanLyBanVeXeKhach.Models
{
    public class GheXe : INotifyPropertyChanged
    {
        public string TenGhe { get; set; } = "";
        public bool IsAisle { get; set; }
        public bool IsHidden { get; set; }
        public int Tang { get; set; } = 1;

        private bool _daDat;
        public bool DaDat
        {
            get => _daDat;
            set { _daDat = value; OnChanged(nameof(DaDat)); OnChanged(nameof(MauNen)); OnChanged(nameof(MauChu)); OnChanged(nameof(MauVien)); }
        }

        private bool _duocChon;
        public bool DuocChon
        {
            get => _duocChon;
            set { _duocChon = value; OnChanged(nameof(DuocChon)); OnChanged(nameof(MauNen)); OnChanged(nameof(MauChu)); OnChanged(nameof(MauVien)); }
        }

        // Đang chọn: Xanh lá mượt (#4CAF50), Đã bán: Xám nhạt (#E0E0E0), Trống: Trắng (#FFFFFF)
        public string MauNen => DuocChon ? "#4CAF50" : DaDat ? "#E0E0E0" : "#FFFFFF";
        public string MauChu => DuocChon ? "#FFFFFF" : DaDat ? "#9E9E9E" : "#757575";
        public string MauVien => DuocChon ? "#4CAF50" : DaDat ? "#E0E0E0" : "#BDBDBD";

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
