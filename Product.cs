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
using System.IO;
namespace Все_для_бани
{
    public partial class Product : Form
    {
        //Объявление переменных
        public string connectionString = "host=localhost;uid=root;pwd=;database=trade";
        public string countInDb;
        public string userRole;
        public string Articl, Name, Description, Price, Category, Photo, defaulPhoto;
        public Product(string userStatus)
        {
            InitializeComponent();

            userRole = userStatus;
        }
        //Проверка ввода
        private void TextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBox3.Text.Length == 0)
            {
                if (e.KeyChar == '0')
                {
                    e.Handled = true;
                }
            }
            if (!Char.IsControl(e.KeyChar))
            {
                if (!Char.IsDigit(e.KeyChar) || e.KeyChar == '-')
                {
                    e.Handled = true;
                }
            }
        }
        //Поиск
        private void TextBox5_TextChanged(object sender, EventArgs e)
        {
            update();
        }
        //Удаление товара
        private void Button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text) || string.IsNullOrWhiteSpace(textBox4.Text) || string.IsNullOrWhiteSpace(textBox6.Text) || string.IsNullOrWhiteSpace(comboBox1.Text))
            {
                MessageBox.Show("Выберите товар");

            }
            else
            {
                try
                {
                    using (MySqlConnection con = new MySqlConnection())
                    {
                        con.ConnectionString = connectionString;
                        con.Open();

                        string query = $@"DELETE FROM Product WHERE  ProductArticleNumber = '{textBox1.Text.Trim()}'";
                        DialogResult result = MessageBox.Show(
                            "Вы точно хотите удалить запись",
                            "Сообщение",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button1,
                            MessageBoxOptions.DefaultDesktopOnly);

                        if(result == DialogResult.Yes)
                        {

                            MySqlCommand cmd = new MySqlCommand(query, con);
                            if (cmd.ExecuteNonQuery() == 1)
                            {
                                MessageBox.Show("Успешно");
                                clean();
                            }
                            else
                            {
                                MessageBox.Show("Ошибка");
                            }
                        }
                    }
                    update();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка");
                }
            }
        }
        //Переход в меню
        private void Button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //Сортировка
        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                update();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }
        //Фильтрация
        private void ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                update();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
       //Редактирование
        private void Button4_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();

                    string query = $"SELECT CategoryID FROM Category WHERE CategoryName = '{comboBox1.Text.Trim()}'";

                    MySqlCommand cmd = new MySqlCommand(query, con);

                    string categoryID = Convert.ToString(cmd.ExecuteScalar());

                    if(Photo != null)
                    {
                        query = $@"UPDATE Product SET `ProductArticleNumber` = '{textBox1.Text.Trim()}', `ProductName` = '{textBox2.Text.Trim()}', `ProductDescription` = '{textBox4.Text.Trim()}',
                                   `ProductCategory` = {categoryID}, `ProductPhoto` = '{Photo}', `ProductManufacturer` = '{textBox6.Text.Trim()}', `ProductPrice` = '{textBox3.Text.Trim()}', `ProductCount` = '{numericUpDown1.Value}' WHERE ProductArticleNumber = '{textBox1.Text.Trim()}'";
                    }
                    else
                    {
                        query = $@"UPDATE Product SET `ProductArticleNumber` = '{textBox1.Text.Trim()}', `ProductName` = '{textBox2.Text.Trim()}', `ProductDescription` = '{textBox4.Text.Trim()}',
                                   `ProductCategory` = {categoryID}, `ProductPhoto` = '{defaulPhoto}', `ProductManufacturer` = '{textBox6.Text.Trim()}', `ProductPrice` = '{textBox3.Text.Trim()}', `ProductCount` = '{numericUpDown1.Value}' WHERE ProductArticleNumber = '{textBox1.Text.Trim()}'";

                    }

                    DialogResult result = MessageBox.Show(
                        "Вы точно хотите обновить запись",
                        "Сообщение",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.DefaultDesktopOnly);

                    if(result == DialogResult.Yes)
                    {
                        cmd = new MySqlCommand(query, con);
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            MessageBox.Show("Успешно");
                            clean();
                        }
                        else
                        {
                            MessageBox.Show("Ошибка");
                        }
                    }
                }
                update();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }
        //Главный запрос по отображению информации в БД
        void update()
        {
            try
            {
                string query;
                if (comboBox2.SelectedIndex == 0)
                {
                    if(comboBox3.Text == "Все")
                    {
                        query = $@"SELECT ProductArticleNumber as 'Артикул', ProductName as 'Наименование', ProductDescription as 'Описание',
                 Category.CategoryName as 'Категория', ProductPhoto, ProductManufacturer as 'Производитель', ProductPrice as 'Цена', ProductCount as 'Количество' FROM Product
                 INNER JOIN Category ON Product.ProductCategory = Category.CategoryID WHERE ProductName LIKE '%{textBox5.Text.Trim()}%' ORDER BY ProductPrice";
                    }
                    else
                    {
                        query = $@"SELECT ProductArticleNumber as 'Артикул', ProductName as 'Наименование', ProductDescription as 'Описание',
                 Category.CategoryName as 'Категория', ProductPhoto, ProductManufacturer as 'Производитель', ProductPrice as 'Цена', ProductCount as 'Количество' FROM Product
                 INNER JOIN Category ON Product.ProductCategory = Category.CategoryID WHERE ProductName LIKE '%{textBox5.Text.Trim()}%' AND Category.CategoryName = '{comboBox3.Text}' ORDER BY ProductPrice";
                    }

                }
                else if(comboBox2.SelectedIndex == 1)
                {

                    if (comboBox3.Text == "Все")
                    {
                        query = $@"SELECT ProductArticleNumber as 'Артикул', ProductName as 'Наименование', ProductDescription as 'Описание',
                 Category.CategoryName as 'Категория', ProductPhoto, ProductManufacturer as 'Производитель', ProductPrice as 'Цена', ProductCount as 'Количество' FROM Product
                 INNER JOIN Category ON Product.ProductCategory = Category.CategoryID WHERE ProductName LIKE '%{textBox5.Text.Trim()}%' ORDER BY ProductPrice DESC";
                    }
                    else
                    {
                        query = $@"SELECT ProductArticleNumber as 'Артикул', ProductName as 'Наименование', ProductDescription as 'Описание',
                 Category.CategoryName as 'Категория', ProductPhoto, ProductManufacturer as 'Производитель', ProductPrice as 'Цена', ProductCount as 'Количество' FROM Product
                 INNER JOIN Category ON Product.ProductCategory = Category.CategoryID WHERE ProductName LIKE '%{textBox5.Text.Trim()}%' AND Category.CategoryName = '{comboBox3.Text}' ORDER BY ProductPrice DESC";
                    }
                }
                else
                {
                    if(comboBox3.Text == "Все")
                    {
                        query = $@"SELECT ProductArticleNumber as 'Артикул', ProductName as 'Наименование', ProductDescription as 'Описание',
                 Category.CategoryName as 'Категория', ProductPhoto, ProductManufacturer as 'Производитель', ProductPrice as 'Цена', ProductCount as 'Количество' FROM Product
                 INNER JOIN Category ON Product.ProductCategory = Category.CategoryID WHERE ProductName LIKE '%{textBox5.Text.Trim()}%'";
                    }
                    else
                    {
                        query = $@"SELECT ProductArticleNumber as 'Артикул', ProductName as 'Наименование', ProductDescription as 'Описание',
                 Category.CategoryName as 'Категория', ProductPhoto, ProductManufacturer as 'Производитель', ProductPrice as 'Цена', ProductCount as 'Количество' FROM Product
                 INNER JOIN Category ON Product.ProductCategory = Category.CategoryID WHERE ProductName LIKE '%{textBox5.Text.Trim()}%' AND Category.CategoryName = '{comboBox3.Text}'";
                    }
                }
                



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
                }

                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand($@"SELECT count(*) FROM Product;", con);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        countInDb = rdr[0].ToString();
                    }
                }
                label6.Text = $"Количество записей: {dataGridView1.Rows.Count}/{countInDb}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }
        //Заполнение формы перед загрузкой
        private void Product_Load(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand($@"SELECT CategoryName FROM Category;", con);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        comboBox1.Items.Add(rdr[0].ToString());
                        comboBox3.Items.Add(rdr[0].ToString());
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка");
            }

            switch(userRole)
            {
                case "2": textBox5.Visible = false; comboBox2.Visible = false; comboBox3.Visible = false; break;
                case "1": button2.Visible = false; button4.Visible = false; button5.Visible = false;
                    textBox1.Enabled = false; textBox2.Enabled = false; textBox3.Enabled = false; textBox4.Enabled = false; comboBox1.Enabled = false; textBox6.Enabled = false; button1.Enabled = false; break;
            }
            comboBox2.SelectedIndex = 2;
            comboBox3.SelectedIndex = 0;
            update();

        }
        //Получение информации по щелчку
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells["Артикул"].Value.ToString();
                textBox2.Text = dataGridView1.Rows[e.RowIndex].Cells["Наименование"].Value.ToString();
                textBox3.Text = dataGridView1.Rows[e.RowIndex].Cells["Цена"].Value.ToString();
                textBox4.Text = dataGridView1.Rows[e.RowIndex].Cells["Описание"].Value.ToString();
                textBox6.Text = dataGridView1.Rows[e.RowIndex].Cells["Производитель"].Value.ToString();
                comboBox1.Text = dataGridView1.Rows[e.RowIndex].Cells["Категория"].Value.ToString();
                defaulPhoto = dataGridView1.Rows[e.RowIndex].Cells["ProductPhoto"].Value.ToString();
                numericUpDown1.Value = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Количество"].Value.ToString());
                if (dataGridView1.Rows[e.RowIndex].Cells["ProductPhoto"].Value.ToString() == "")
                {
                    pictureBox1.Image = Image.FromFile("./Image/picture.png");
                }
                else
                {
                    pictureBox1.Image = Image.FromFile("./Image/" + dataGridView1.Rows[e.RowIndex].Cells["ProductPhoto"].Value.ToString());
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Выберите ячейку, а не заголовок");
            }
        }
        //Изменение фотографии
        private void Button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open_dialog = new OpenFileDialog(); //создание диалогового окна для выбора файла
            open_dialog.Filter = "Image Files(*.JPG;*.PNG)|*.JPG;*.PNG|All files (*.*)|*.*"; //формат загружаемого файла
            if (open_dialog.ShowDialog() == DialogResult.OK) //если в окне была нажата кнопка "ОК"
            {
                try
                {
                    var words = open_dialog.FileName.Split('\\');
                    pictureBox1.Image = Image.FromFile(open_dialog.FileName);
                    Photo = $"{words[words.Length - 1]}";
                    File.Copy(open_dialog.FileName, $"{Application.StartupPath + @"\Image\" + words[words.Length - 1]}", true);
                }
                catch
                {
                    DialogResult rezult = MessageBox.Show("Невозможно открыть выбранный файл",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //Добавление товара
        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox3.Text) || string.IsNullOrWhiteSpace(textBox4.Text) || string.IsNullOrWhiteSpace(textBox6.Text) || string.IsNullOrWhiteSpace(comboBox1.Text))
                {
                    MessageBox.Show("Заполните все поля");

                }
                else
                {
                    using (MySqlConnection con = new MySqlConnection())
                    {
                        con.ConnectionString = connectionString;
                        con.Open();

                        string query = $@"SELECT ProductName FROM Product WHERE ProductArticleNumber = '{textBox1.Text.Trim()}'";
                        MySqlCommand cmd = new MySqlCommand(query, con);
                        if (cmd.ExecuteScalar() == null)
                        {
                            query = $"SELECT CategoryID FROM Category WHERE CategoryName = '{comboBox1.Text.Trim()}'";

                            cmd = new MySqlCommand(query, con);

                            string categoryID = Convert.ToString(cmd.ExecuteScalar());
                            
                            if(Photo != null)
                            {
                                query = $@"INSERT INTO Product (`ProductArticleNumber`, `ProductName`, `ProductDescription`, `ProductCategory`, `ProductPhoto`, `ProductManufacturer`, `ProductPrice`, `ProductCount`) VALUES
                                                                  ('{textBox1.Text.Trim()}','{textBox2.Text.Trim()}', '{textBox4.Text.Trim()}', '{categoryID}', '{Photo}', '{textBox6.Text.Trim()}', '{textBox3.Text.Trim()}', '{numericUpDown1.Value}');";

                            }
                            else if (defaulPhoto == null)
                            {
                                query = $@"INSERT INTO Product (`ProductArticleNumber`, `ProductName`, `ProductDescription`, `ProductCategory`, `ProductPhoto`, `ProductManufacturer`, `ProductPrice`, `ProductCount`) VALUES
                                                                  ('{textBox1.Text.Trim()}','{textBox2.Text.Trim()}', '{textBox4.Text.Trim()}', '{categoryID}', 'picture.png', '{textBox6.Text.Trim()}', '{textBox3.Text.Trim()}', '{numericUpDown1.Value}');";
                            }
                            else
                            {
                                query = $@"INSERT INTO Product (`ProductArticleNumber`, `ProductName`, `ProductDescription`, `ProductCategory`, `ProductPhoto`, `ProductManufacturer`, `ProductPrice`, `ProductCount`) VALUES
                                                                  ('{textBox1.Text.Trim()}','{textBox2.Text.Trim()}', '{textBox4.Text.Trim()}', '{categoryID}', '{defaulPhoto}', '{textBox6.Text.Trim()}', '{textBox3.Text.Trim()}', '{numericUpDown1.Value}');";

                            }
                            cmd = new MySqlCommand(query, con);
                            if (cmd.ExecuteNonQuery() == 1)
                            {
                                MessageBox.Show("Товар добавлен");
                                clean();
                            }
                            else
                            {
                                MessageBox.Show("Ошибка");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Товар с таким артиклем существует.");
                        }
                    }
                    update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }
        //Очищение формы
        void clean()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox6.Clear();
            pictureBox1.Image = null;
            comboBox1.SelectedIndex = -1;
        }
    }
}
