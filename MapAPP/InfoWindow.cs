using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MapAPP
{
    public partial class InfoWindow : Form
    {
        private int _id=-1;
        public InfoWindow(int id)
        {
            InitializeComponent();
            _id=id;
        }
        
        private void InfoWindow_Load(object sender, EventArgs e)
        {
            string sql = string.Format("select * from t_point where id={0}", _id);
            DataTable dt = DBHelper.Instance.GetDataTable(sql);
            if (dt == null || dt.Rows.Count <= 0)
            {
                MessageBox.Show("查找错误或者数据记录不存在！");
            }
            else
            {
                try
                {
                    tbid.Text = dt.Rows[0]["id"].ToString();
                }
                catch
                { }

                try
                {
                    tbx.Text = dt.Rows[0]["x"].ToString();
                }
                catch
                { }

                try
                {
                    tby.Text = dt.Rows[0]["y"].ToString();
                }
                catch
                { }

                try
                {
                    tbt.Text = dt.Rows[0]["t"].ToString();
                }
                catch
                { }

                try
                {
                    tbt1.Text = dt.Rows[0]["t1"].ToString();
                }
                catch
                { }
                try
                {
                    tbt2.Text = dt.Rows[0]["t2"].ToString();
                }
                catch
                { }

                try
                {
                    tbt3.Text = dt.Rows[0]["t3"].ToString();
                }
                catch
                { }

                try
                {
                    tbt4.Text = dt.Rows[0]["t4"].ToString();
                }
                catch
                { }

                try
                {
                    tbt5.Text = dt.Rows[0]["t5"].ToString();
                }
                catch
                { }

                try
                {
                    tbt6.Text = dt.Rows[0]["t6"].ToString();
                }
                catch
                { }

                try
                {
                    tbt7.Text = dt.Rows[0]["t7"].ToString();
                }
                catch
                { }

                try
                {
                    tbt8.Text = dt.Rows[0]["t8"].ToString();
                }
                catch
                { }

                try
                {
                    tbt9.Text = dt.Rows[0]["t9"].ToString();
                }
                catch
                { }

                try
                {
                    tbt10.Text = dt.Rows[0]["t10"].ToString();
                }
                catch
                { }

            }
        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}