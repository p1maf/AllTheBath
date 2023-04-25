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
using System.Threading;
namespace Все_для_бани
{
    public partial class Authorization : Form
    {
        //объявление переменных
        public string connectionString = "host=localhost;uid=root;pwd=;database=trade";
        public string ID;
        public string userPassword;
        public string userRole;
        public int countTryIn;
        public string userName;
        public string superUserFromSpecial = "superuser";
        public string superPasswordFromSpecial = "superpassword";
        public Authorization()
        {
            InitializeComponent();
        }
        //Основная функция по проверке и авторизации в систему.
        private void Button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                if (countTryIn >= 2)
                {
                    MessageBox.Show("Защита от спама. Вы были заблокированы на 5 секунд");
                    Thread.Sleep(5000);
                }
                MessageBox.Show("Заполните поля");
                countTryIn++;
            }
            else
            {
                if(textBox1.Text == "superuser" && textBox2.Text == "superpassword")
                {
                    SpecFeatures specFeatures = new SpecFeatures();
                    this.Hide();
                    specFeatures.ShowDialog();
                    this.Show();
                }
                else
                {
                    try
                    {
                        using (MySqlConnection con = new MySqlConnection())
                        {
                            con.ConnectionString = connectionString;
                            con.Open();
                            MySqlCommand cmd = new MySqlCommand($"SELECT UserPassword, UserRole, concat(UserSurname, ' ', UserName,' ', UserPatronymic) FROM user WHERE UserLogin = '{textBox1.Text}';", con);
                            MySqlDataReader rdr = cmd.ExecuteReader();
                            while (rdr.Read())
                            {
                                userPassword = rdr[0].ToString();
                                userRole = rdr[1].ToString();
                                userName = rdr[2].ToString();
                            }
                        }
                        if (string.IsNullOrWhiteSpace(userPassword))
                        {
                            if (countTryIn == 2)
                            {
                                MessageBox.Show("Защита от спама. Вы были заблокированы на 5 секунд");
                                Thread.Sleep(5000);
                            }
                            else
                            {
                                MessageBox.Show("Данного пользователя не сущетствует");
                                textBox1.Clear();
                                textBox2.Clear();
                            }
                        }
                        else
                        {
                            if (textBox2.Text == userPassword)
                            {
                                MessageBox.Show("Вы успешно вошли");
                                MainMenu mainMenu = new MainMenu(userRole, userName);
                                this.Hide();
                                mainMenu.ShowDialog();
                                this.Show();
                                textBox1.Clear();
                                textBox2.Clear();
                            }
                            else
                            {
                                if (countTryIn == 2)
                                {
                                    MessageBox.Show("Защита от спама. Вы были заблокированы на 5 секунд");
                                    Thread.Sleep(5000);
                                }
                                MessageBox.Show("Вы ввели неверный пароль, подтвердите что вы не робот");
                                textBox1.Enabled = false;
                                textBox2.Enabled = false;
                                button1.Enabled = false;
                                pictureBox2.Visible = true;
                                label3.Visible = true;
                                label4.Visible = true;
                                label5.Visible = true;
                                label6.Visible = true;
                                textBox3.Visible = true;
                                button2.Visible = true;
                                generateCaptcha();

                            }
                        }
                        countTryIn++;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }
        //Генерация каптчи
        void generateCaptcha()
        {
            string symbolCaptch = "123456789qwertyuiopasdfghjklzxcvbnm";
            Random rnd = new Random();
            label3.Text = symbolCaptch[rnd.Next(Convert.ToInt32(symbolCaptch.Length))].ToString();
            label4.Text = symbolCaptch[rnd.Next(Convert.ToInt32(symbolCaptch.Length))].ToString();
            label5.Text = symbolCaptch[rnd.Next(Convert.ToInt32(symbolCaptch.Length))].ToString();
            label6.Text = symbolCaptch[rnd.Next(Convert.ToInt32(symbolCaptch.Length))].ToString();
        }
        //Скрытие элементов перед загрузкой
        private void Authorization_Load(object sender, EventArgs e)
        {
            pictureBox2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            textBox3.Visible = false;
            button2.Visible = false;
        }
        //Проверка на капчу
        private void Button2_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == $"{label3.Text}{label4.Text}{label5.Text}{label6.Text}")
            {
                MessageBox.Show("Вы успешно прошли каптчу");
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                button1.Enabled = true;
                pictureBox2.Visible = false;
                label3.Visible = false;
                label4.Visible = false;
                label5.Visible = false;
                label6.Visible = false;
                textBox3.Visible = false;
                button2.Visible = false;
                textBox3.Clear();
                countTryIn++;
            }
            else
            {
                MessageBox.Show("Вы ввели неверно, попробуйте еще раз");
                generateCaptcha();
            }
        }
    }
}