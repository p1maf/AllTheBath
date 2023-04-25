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
    public partial class Basket : Form
    {
        //объявление переменных
        public string connectionString = "host=localhost;uid=root;pwd=;database=trade";
        public string countInDb;
        public string userRole;
        public string Articl, Description, Category, Photo, defaulPhoto, NameProd, PriceProd;
        public string userName;
        public int countBefore = 0;
        public int countAfet = 0;
        public int sumPriceProd = 0;
        public List<string> card = new List<string>();
        public Basket(string userNames)
        {
            InitializeComponent();
            userName = userNames;
        }
        // Удаление товара из корзины
        private void Button2_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedCells.Count > 0)
            {
                int indexRow = dataGridView2.CurrentCell.RowIndex;
                sumPriceProd -=  Convert.ToInt32(dataGridView2.Rows[indexRow].Cells[3].Value);
                dataGridView2.Rows.RemoveAt(dataGridView2.CurrentCell.RowIndex);
                label9.Text = $"Сумма заказа: {sumPriceProd}";
            }
            else
            {
                MessageBox.Show("Выберите товар для удаления");
            }
        }
        //Переход в главное меню
        private void Button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //Переход на форму просмотра заказа
        private void Button3_Click(object sender, EventArgs e)
        {
            if(dataGridView2.Rows.Count == 0)
            {
                MessageBox.Show("Корзина пуста, добавьте что-то");
            }
            else
            {
                if (dataGridView2.Rows.Count != card.Count)
                {
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (!card.Contains($"{row.Cells[0].Value.ToString()};{row.Cells[1].Value.ToString()};{row.Cells[2].Value.ToString()};{row.Cells[3].Value.ToString()}"))
                        {
                            card.Add($"{row.Cells[0].Value.ToString()};{row.Cells[1].Value.ToString()};{row.Cells[2].Value.ToString()};{row.Cells[3].Value.ToString()}");
                        }
                    }
                }
                Order order = new Order(card, userName);
                this.Hide();
                order.ShowDialog();
                this.Show();
            }
        }
        //Добавление в корзину
        private void Button1_Click(object sender, EventArgs e)
        {
            int countProdInDB = 0;
            bool haveInGrid = false;
            if (label7.Text != "")
            {
                try
                {
                    if (dataGridView2.Rows.Count >= 1)
                    {
                        using (MySqlConnection con = new MySqlConnection())
                        {
                            con.ConnectionString = connectionString;
                            con.Open();
                            string query = $"SELECT ProductCount FROM Product WHERE ProductArticleNumber = '{Articl}'";
                            MySqlCommand cmd = new MySqlCommand(query, con);
                            countProdInDB = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        foreach (DataGridViewRow row in dataGridView2.Rows)
                        {
                            if (row.Cells[0].Value.ToString() == Articl)
                            {
                                haveInGrid = true;
                                int rowCount = Convert.ToInt32(row.Cells[2].Value.ToString());
                                if ((numericUpDown1.Value + rowCount) > countProdInDB)
                                {
                                    MessageBox.Show("Такого количества продукта нет на складе");
                                }
                                else
                                {
                                    row.Cells[2].Value = numericUpDown1.Value + rowCount;
                                    row.Cells[3].Value = Convert.ToInt32(row.Cells[2].Value) * Convert.ToInt32(PriceProd);
                                }
                            }
                        }
                        if (haveInGrid == false)
                        {
                            dataGridView2.Rows.Add(Articl, NameProd, numericUpDown1.Value, PriceProd);
                        }
                    }
                    else
                    {
                        dataGridView2.Rows.Add(Articl, NameProd, numericUpDown1.Value, PriceProd);
                    }
                    int indexRow = dataGridView1.CurrentCell.RowIndex;

                    sumPriceProd += Convert.ToInt32(numericUpDown1.Value) * Convert.ToInt32(dataGridView1.Rows[indexRow].Cells["Цена"].Value);
                    
                    label9.Text = $"Сумма заказа: {sumPriceProd}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка");
                }
            }
            else
            {
                MessageBox.Show("Выберите запись");
            }
        }

        private void DataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        //Заполнение элементов на форме
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Articl = dataGridView1.Rows[e.RowIndex].Cells["Артикул"].Value.ToString();
                NameProd = dataGridView1.Rows[e.RowIndex].Cells["Наименование"].Value.ToString();
                PriceProd = dataGridView1.Rows[e.RowIndex].Cells["Цена"].Value.ToString();
                textBox2.Text = dataGridView1.Rows[e.RowIndex].Cells["Описание"].Value.ToString();
                //textBox6.Text = dataGridView1.Rows[e.RowIndex].Cells["Производитель"].Value.ToString();
                label7.Text = dataGridView1.Rows[e.RowIndex].Cells["Категория"].Value.ToString();
                defaulPhoto = dataGridView1.Rows[e.RowIndex].Cells["ProductPhoto"].Value.ToString();
                if (dataGridView1.Rows[e.RowIndex].Cells["ProductPhoto"].Value.ToString() == "")
                {
                    pictureBox1.Image = Image.FromFile("./Image/picture.png");
                }
                else
                {
                    pictureBox1.Image = Image.FromFile("./Image/" + dataGridView1.Rows[e.RowIndex].Cells["ProductPhoto"].Value.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Выберите ячейку, а не заголовок");
            }
        }
        //Поиск по артиклу
        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            update();
        }
        //Применение параметров при загрузке
        private void Basket_Load(object sender, EventArgs e)
        {
            label3.Text = $"Менеджер {userName}";
            DateTime dateTime = DateTime.Now;
            label4.Text = "Дата приема " + dateTime.ToShortDateString();
            update();
        }
        //Основной запрос к базе
        void update()
        {
            try
            {
                string query = $@"SELECT ProductArticleNumber as 'Артикул', ProductName as 'Наименование', ProductDescription as 'Описание',
                 Category.CategoryName as 'Категория', ProductPhoto, ProductManufacturer as 'Производитель', ProductPrice as 'Цена' FROM Product
                 INNER JOIN Category ON Product.ProductCategory = Category.CategoryID WHERE ProductArticleNumber LIKE '%{textBox1.Text.Trim()}%'";



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
                    dataGridView1.Columns["ProductPhoto"].Visible = false;
                    dataGridView1.Columns["Описание"].Visible = false;
                    dataGridView1.Columns["Категория"].Visible = false;
                    dataGridView1.Columns["Производитель"].Visible = false;
                    dataGridView1.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }
    }
}
