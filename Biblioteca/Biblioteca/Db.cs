using System.Data;
using System.Data.SqlClient;

namespace Biblioteca
{
    public static class Db
    {
        // Se usar SQL Express, troque por: Server=.\SQLEXPRESS;Database=Biblioteca;Integrated Security=true;
        public static string ConnStr =
            @"Server=(localdb)\MSSQLLocalDB;Database=Biblioteca;Integrated Security=true;";

        public static DataTable Consultar(string sql, params SqlParameter[] ps)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql, con))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddRange(ps);
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public static int Executar(string sql, params SqlParameter[] ps)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddRange(ps);
                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static object Escalar(string sql, params SqlParameter[] ps)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddRange(ps);
                con.Open();
                return cmd.ExecuteScalar();
            }
        }
    }
}
