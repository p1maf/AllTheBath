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
    public partial class Category : Form
    {
        //Объявление переменных
        public string connectionString = "host=localhost;uid=root;pwd=;database=trade";
        public string countInDb;
        public string ID;
        public Category()
        {
            InitializeComponent();
        }

        private void Category_Load(object sender, EventArgs e)
        {
            update();
        }
        // Основной запрос к базе
        void update()
        {
            try
            {
                string query = $@"SELECT CategoryID, CategoryName as 'Категории' FROM Category";


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
                    dataGridView1.Columns["CategoryID"].Visible = false;
                }

                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand($@"SELECT count(*) FROM Category;", con);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        countInDb = rdr[0].ToString();
                    }
                }
                label2.Text = $"{dataGridView1.Rows.Count}/{countInDb}";
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
        //Добавление
        private void Button1_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Заполните все поля");
            }
            else
            {
                try
                {
                    using (MySqlConnection con = new MySqlConnection())
                    {
                        con.ConnectionString = connectionString;
                        con.Open();

                        string query = $@"SELECT CategoryName FROM Category WHERE CategoryName = '{textBox1.Text.Trim()}'";
                        MySqlCommand cmd = new MySqlCommand(query, con);
                        object asdf = cmd.ExecuteScalar();
                        if (cmd.ExecuteScalar() == null)
                        {
                            query = $@"INSERT INTO `trade`.`Category` (`CategoryID`, `CategoryName`) VALUES (null,'{textBox1.Text.Trim()}');";
                            cmd = new MySqlCommand(query, con);
                            if (cmd.ExecuteNonQuery() == 1)
                            {
                                MessageBox.Show("Категория добавлена");
                            }
                            else
                            {
                                MessageBox.Show("Ошибка");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Данная категория уже есть.");
                        }
                    }
                    update();
                    textBox1.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка");
                }
            }
        }
        //Удаление
        private void Button3_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Выберите одну из строк");
            }
            else
            {
                try
                {
                    using (MySqlConnection con = new MySqlConnection())
                    {
                        con.ConnectionString = connectionString;
                        con.Open();

                        string query = $@"DELETE FROM Category WHERE CategoryName = '{textBox1.Text.Trim()}'";
                        MySqlCommand cmd = new MySqlCommand(query, con);
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            MessageBox.Show("Успешно");
                        }
                        else
                        {
                            MessageBox.Show("Ошибка");
                        }
                    }
                    update();
                    textBox1.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка");
                }
            }
        }
        //заполнение элементов формы
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            try
            {
                textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells["Категории"].Value.ToString();
                ID = dataGridView1.Rows[e.RowIndex].Cells["CategoryID"].Value.ToString();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Выберите ячейку, а не колонку");
            }
        }
        // Редактирование
        private void Button2_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Заполните все поля");
            }
            else
            {
                try
                {
                    using (MySqlConnection con = new MySqlConnection())
                    {
                        con.ConnectionString = connectionString;
                        con.Open();

                        string query = $@"UPDATE Category SET `CategoryName` = '{textBox1.Text.Trim()}' WHERE CategoryID = '{ID}'";
                        MySqlCommand cmd = new MySqlCommand(query, con);
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            MessageBox.Show("Успешно");
                        }
                        else
                        {
                            MessageBox.Show("Ошибка");
                        }
                    }
                    update();
                    textBox1.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка");
                }
            }
        }
    }
}
