using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace MapAPP
{
    public class DBHelper
    {
        private static DBHelper instance;
        public static DBHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DBHelper();
                }
                return instance;
            }
        }

        private string dbConStr;
        public SqlConnection Conn;
        public DBHelper()
        {
            dbConStr = System.Configuration.ConfigurationManager.AppSettings["SqlConStr"];
            Conn = new SqlConnection(dbConStr);
        }

        public DataTable GetDataTable(String SqlStr)
        {
            SqlDataAdapter dap = new SqlDataAdapter(SqlStr, Conn);
            DataTable dt = new DataTable();
            dap.Fill(dt);
            return dt;
        }

        public bool ExcuteSql(String SqlStr)
        {
            SqlCommand cmd = new SqlCommand(SqlStr, Conn);
            this.Conn.Open();
            int result = cmd.ExecuteNonQuery();
            this.Conn.Close();
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
