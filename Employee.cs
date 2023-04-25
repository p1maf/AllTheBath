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
    public partial class Employee : Form
    {
        //Объявление переменных
        public string connectionString = "host=localhost;uid=root;pwd=;database=trade";
        public string countInDb;
        public string ID;
        public Employee()
        {
            InitializeComponent();
        }

        private void Employee_Load(object sender, EventArgs e)
        {
            update();
        }
        //Заполнение таблицы на форме
        void update()
        {
            try
            {
                string query = $@"SELECT FIO as 'ФИО', Post as 'Должность', PhoneNumber as 'Телефон' FROM Employee";


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
                }

                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand($@"SELECT count(*) FROM Employee;", con);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        countInDb = rdr[0].ToString();
                    }
                }
                label4.Text = $"{dataGridView1.Rows.Count}/{countInDb}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }
        //Проверки
        private void TextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBox2.Text.Length == 0)
            {
                if (e.KeyChar == '0')
                {
                    e.Handled = true;
                }
            }
            if (!Char.IsControl(e.KeyChar))
            {
                if (Char.IsDigit(e.KeyChar) || e.KeyChar == '-')
                {
                    e.Handled = true;
                }
            }
        }
        //Переход назад
        private void Button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //Добавление
        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(maskedTextBox1.Text) || !maskedTextBox1.MaskFull)
                {
                    MessageBox.Show("Заполните все поля");
                }
                else
                {
                    using (MySqlConnection con = new MySqlConnection())
                    {
                        con.ConnectionString = connectionString;
                        con.Open();

                        string query = $@"SELECT FIO FROM Employee WHERE PhoneNumber = '{maskedTextBox1.Text.Trim()}'";
                        MySqlCommand cmd = new MySqlCommand(query, con);
                        if (cmd.ExecuteScalar() == null)
                        {
                            query = $@"INSERT INTO `trade`.`Employee` (`PhoneNumber`, `FIO`, `Post`) VALUES ('{maskedTextBox1.Text.Trim()}','{textBox2.Text.Trim()}', '{textBox1.Text.Trim()}');";
                            cmd = new MySqlCommand(query, con);
                            if (cmd.ExecuteNonQuery() == 1)
                            {
                                MessageBox.Show("Сотрудник добавлен");
                            }
                            else
                            {
                                MessageBox.Show("Ошибка");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Данный сотрудник уже есть.");
                        }
                    }
                    update();
                    textBox1.Clear();
                    textBox2.Clear();
                    maskedTextBox1.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }
        //Обновление
        private void Button2_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(maskedTextBox1.Text))
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

                        string query = $@"UPDATE Employee SET `FIO` = '{textBox2.Text.Trim()}', `Post` = '{textBox1.Text.Trim()}', `PhoneNumber` = '{maskedTextBox1.Text}' WHERE PhoneNumber = '{maskedTextBox1.Text}'";
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
                    textBox2.Clear();
                    maskedTextBox1.Clear();
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
            if(string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(maskedTextBox1.Text))
            {
                MessageBox.Show("Заполните поля");
            }
            else
            {
                try
                {
                    using (MySqlConnection con = new MySqlConnection())
                    {
                        con.ConnectionString = connectionString;
                        con.Open();

                        string query = $@"DELETE FROM Employee WHERE PhoneNumber = '{maskedTextBox1.Text.Trim()}'";
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
                    textBox2.Clear();
                    maskedTextBox1.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка");
                }
            }
        }
        //Заполнение элементов формы
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                maskedTextBox1.Text = dataGridView1.Rows[e.RowIndex].Cells["Телефон"].Value.ToString();
                textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells["Должность"].Value.ToString();
                textBox2.Text = dataGridView1.Rows[e.RowIndex].Cells["ФИО"].Value.ToString();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Выберите поле");
            }
        }
    }
}
