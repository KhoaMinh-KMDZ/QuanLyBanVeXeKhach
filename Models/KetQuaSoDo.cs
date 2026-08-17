using System.Collections.Generic;

namespace QuanLyBanVeXeKhach.Models
{
    public class KetQuaSoDo
    {
        public bool CoHaiTang { get; set; } = false;
        
        public int SoCotTang1 { get; set; } = 0;
        public List<GheXe> Tang1 { get; set; } = new List<GheXe>();
        
        public int SoCotTang2 { get; set; } = 0;
        public List<GheXe> Tang2 { get; set; } = new List<GheXe>();
    }
}
