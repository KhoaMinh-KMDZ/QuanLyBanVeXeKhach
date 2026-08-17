using System;
using System.Collections.Generic;
using QuanLyBanVeXeKhach.Models;

namespace QuanLyBanVeXeKhach.Helpers
{
    public static class TienIchSoDoGhe
    {
        public static KetQuaSoDo TaoSoDoGhe(string tenLoaiXe, HashSet<string> danhSachGheDaBan)
        {
            tenLoaiXe = tenLoaiXe.ToLower();
            KetQuaSoDo kq = new KetQuaSoDo();

            if (tenLoaiXe.Contains("29"))
            {
                // Xe 29 chỗ (Hình 1)
                kq.SoCotTang1 = 5;
                int soThuTu = 1;
                for (int i = 0; i < 29 + 6; i++) // 29 ghế + 6 lối đi = 35 ô, wait! 7 rows of 4 + 1 driver.
                {
                    // Lối đi ở cột giữa (index 2)
                    if (kq.Tang1.Count % 5 == 2)
                    {
                        kq.Tang1.Add(new GheXe { IsHidden = true, TenGhe = "" });
                    }
                    else
                    {
                        if (soThuTu <= 29)
                        {
                            kq.Tang1.Add(TaoGheTheoTen(soThuTu.ToString("D2"), danhSachGheDaBan, 1));
                            soThuTu++;
                        }
                    }
                }
            }
            else if (tenLoaiXe.Contains("16"))
            {
                // Xe 16 chỗ (Hình 2)
                kq.SoCotTang1 = 4;
                // Khoang lái (hàng 1)
                kq.Tang1.Add(new GheXe { IsHidden = true }); 
                kq.Tang1.Add(new GheXe { IsHidden = true });
                kq.Tang1.Add(TaoGheTheoTen("02", danhSachGheDaBan, 1));
                kq.Tang1.Add(TaoGheTheoTen("01", danhSachGheDaBan, 1));

                // Các hàng sau
                int[] gheList = { 5, 4, 0, 3, 8, 7, 0, 6, 11, 10, 0, 9, 15, 14, 13, 12 };
                foreach (int g in gheList)
                {
                    if (g == 0) kq.Tang1.Add(new GheXe { IsHidden = true });
                    else kq.Tang1.Add(TaoGheTheoTen(g.ToString("D2"), danhSachGheDaBan, 1));
                }
            }
            else if (tenLoaiXe.Contains("9"))
            {
                // Limousine 9 chỗ (Hình 3)
                kq.SoCotTang1 = 3;
                kq.Tang1.Add(new GheXe { IsHidden = true });
                kq.Tang1.Add(TaoGheTheoTen("01", danhSachGheDaBan, 1));
                kq.Tang1.Add(TaoGheTheoTen("02", danhSachGheDaBan, 1));

                kq.Tang1.Add(TaoGheTheoTen("04", danhSachGheDaBan, 1));
                kq.Tang1.Add(new GheXe { IsHidden = true });
                kq.Tang1.Add(TaoGheTheoTen("03", danhSachGheDaBan, 1));

                kq.Tang1.Add(TaoGheTheoTen("06", danhSachGheDaBan, 1));
                kq.Tang1.Add(new GheXe { IsHidden = true });
                kq.Tang1.Add(TaoGheTheoTen("05", danhSachGheDaBan, 1));

                kq.Tang1.Add(TaoGheTheoTen("09", danhSachGheDaBan, 1));
                kq.Tang1.Add(TaoGheTheoTen("08", danhSachGheDaBan, 1));
                kq.Tang1.Add(TaoGheTheoTen("07", danhSachGheDaBan, 1));
            }
            else if (tenLoaiXe.Contains("45"))
            {
                // Giường nằm 45 chỗ (Hình 4)
                kq.CoHaiTang = true;
                kq.SoCotTang1 = 5; // A1, Lối đi, A2, Lối đi, A3
                kq.SoCotTang2 = 5;

                for (int t = 1; t <= 2; t++)
                {
                    string prefix = t == 1 ? "A" : "B";
                    List<GheXe> tang = t == 1 ? kq.Tang1 : kq.Tang2;
                    
                    int soThuTu = 1;
                    // Hàng 1-7
                    for (int r = 0; r < 7; r++)
                    {
                        tang.Add(TaoGheTheoTen(prefix + soThuTu++, danhSachGheDaBan, t)); // Trái
                        tang.Add(new GheXe { IsHidden = true }); // Lối đi
                        tang.Add(TaoGheTheoTen(prefix + soThuTu++, danhSachGheDaBan, t)); // Giữa
                        tang.Add(new GheXe { IsHidden = true }); // Lối đi
                        tang.Add(TaoGheTheoTen(prefix + soThuTu++, danhSachGheDaBan, t)); // Phải
                    }
                    // Hàng cuối (3 ghế sát nhau)
                    tang.Add(TaoGheTheoTen(prefix + soThuTu++, danhSachGheDaBan, t));
                    tang.Add(new GheXe { IsHidden = true });
                    tang.Add(TaoGheTheoTen(prefix + soThuTu++, danhSachGheDaBan, t));
                    tang.Add(new GheXe { IsHidden = true });
                    tang.Add(TaoGheTheoTen(prefix + soThuTu++, danhSachGheDaBan, t));
                }
            }
            else if (tenLoaiXe.Contains("24"))
            {
                // Giường nằm 24 chỗ VIP (Hình 5)
                kq.CoHaiTang = true;
                kq.SoCotTang1 = 3;
                kq.SoCotTang2 = 3;

                for (int t = 1; t <= 2; t++)
                {
                    string leftPrefix = t == 1 ? "D" : "A";
                    string rightPrefix = t == 1 ? "C" : "B";
                    List<GheXe> tang = t == 1 ? kq.Tang1 : kq.Tang2;

                    for (int r = 1; r <= 6; r++)
                    {
                        tang.Add(TaoGheTheoTen(r.ToString() + leftPrefix, danhSachGheDaBan, t));
                        tang.Add(new GheXe { IsHidden = true }); // Lối lên
                        tang.Add(TaoGheTheoTen(r.ToString() + rightPrefix, danhSachGheDaBan, t));
                    }
                }
            }

            return kq;
        }

        private static GheXe TaoGheTheoTen(string tenGhe, HashSet<string> gheDaBan, int tang)
        {
            return new GheXe 
            { 
                TenGhe = tenGhe, 
                DaDat = gheDaBan.Contains(tenGhe),
                Tang = tang
            };
        }
    }
}
