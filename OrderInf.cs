using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace Все_для_бани
{
    public partial class OrderInf : Form
    {
        public string connectionString = "host=localhost;uid=root;pwd=;database=trade";
        public string indeR;
        public OrderInf(string index)
        {
            InitializeComponent();
            indeR = index;
        }

        private void OrderInf_Load(object sender, EventArgs e)
        {
            try
            {
                string query = $@"SELECT article as 'Артикул',
                Product.ProductName as 'Наименование', count as 'Количество', Product.ProductPrice as 'Цена' FROM basket INNER JOIN Product ON article = Product.ProductArticleNumber WHERE id = {indeR};";



                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.ExecuteNonQuery();
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                    dataGridView1.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string query = $@"UPDATE `trade`.`Orders` SET `OrderStatus` = '{comboBox1.Text}' WHERE (`OrderID` = '{indeR}');";



                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    if(cmd.ExecuteNonQuery() == 1)
                    {
                        MessageBox.Show("Статус изменен");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }
    }
}
