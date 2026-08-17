using ClosedXML.Excel;
using QuanLyBanVeXeKhach.Repositories;
using System.IO;

namespace QuanLyBanVeXeKhach.Services
{
    public static class ExportService
    {
        public static string XuatBaoCaoExcel(List<BaoCaoNgay> data, DateTime tuNgay, DateTime denNgay)
        {
            var folder   = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "BaoCaoXeKhach");
            Directory.CreateDirectory(folder);
            var fileName = $"BaoCao_{tuNgay:yyyyMMdd}_{denNgay:yyyyMMdd}.xlsx";
            var filePath = Path.Combine(folder, fileName);

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Báo Cáo Doanh Thu");

            // Title
            ws.Cell(1, 1).Value = "BÁO CÁO DOANH THU QUẢN LÝ BÁN VÉ XE KHÁCH";
            ws.Range(1, 1, 1, 5).Merge();
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 16;
            ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(2, 1).Value = $"Từ: {tuNgay:dd/MM/yyyy}  Đến: {denNgay:dd/MM/yyyy}";
            ws.Range(2, 1, 2, 5).Merge();
            ws.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Headers
            string[] headers = { "STT", "Ngày", "Số Vé", "Doanh Thu" };
            for (int c = 0; c < headers.Length; c++)
            {
                ws.Cell(4, c + 1).Value = headers[c];
                ws.Cell(4, c + 1).Style.Font.Bold = true;
                ws.Cell(4, c + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#1e3c72");
                ws.Cell(4, c + 1).Style.Font.FontColor = XLColor.White;
                ws.Cell(4, c + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // Data
            decimal tongDT = 0; int tongVe = 0;
            int row = 4, stt = 1;
            foreach (var d in data)
            {
                row++;
                ws.Cell(row, 1).Value = stt++;
                ws.Cell(row, 2).Value = d.Ngay.ToString("dd/MM/yyyy");
                ws.Cell(row, 3).Value = d.SoVe;
                ws.Cell(row, 4).Value = (double)d.DoanhThu;
                ws.Cell(row, 4).Style.NumberFormat.Format = "#,##0";
                if (stt % 2 == 0)
                    ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#EEF2FF");
                tongDT += d.DoanhThu; tongVe += d.SoVe;
            }

            // Total
            row++;
            ws.Cell(row, 1).Value = "Tổng cộng";
            ws.Range(row, 1, row, 2).Merge();
            ws.Cell(row, 3).Value = tongVe;
            ws.Cell(row, 4).Value = (double)tongDT;
            ws.Cell(row, 4).Style.NumberFormat.Format = "#,##0";
            ws.Row(row).Style.Font.Bold = true;
            ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#2a5298");
            ws.Row(row).Style.Font.FontColor = XLColor.White;

            ws.Columns().AdjustToContents();
            wb.SaveAs(filePath);
            return filePath;
        }
    }
}
