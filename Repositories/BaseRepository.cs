using Microsoft.Data.SqlClient;
using QuanLyBanVeXeKhach.Helpers;
using static QuanLyBanVeXeKhach.Helpers.DatabaseHelper;

namespace QuanLyBanVeXeKhach.Repositories
{
    // Alias helpers cho gọn
    public abstract class BaseRepository
    {
        protected static string S(SqlDataReader r, string col) => SafeString(r, col);
        protected static int    I(SqlDataReader r, string col) => SafeInt(r, col);
        protected static decimal D(SqlDataReader r, string col) => SafeDecimal(r, col);
        protected static bool   B(SqlDataReader r, string col) => SafeBool(r, col);
        protected static DateTime DT(SqlDataReader r, string col) => SafeDateTime(r, col);
        protected static DateTime? DTN(SqlDataReader r, string col) => SafeDateTimeNull(r, col);
        protected static int?    IN(SqlDataReader r, string col) => SafeIntNull(r, col);
        protected static decimal? DN(SqlDataReader r, string col) => SafeDecimalNull(r, col);

        protected static SqlParameter P(string name, object? value)
            => new SqlParameter(name, value ?? DBNull.Value);
    }
}
