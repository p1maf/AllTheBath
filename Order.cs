using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;
using MySql.Data.MySqlClient;
namespace Все_для_бани
{
    public partial class Order : Form
    {
        //Объявление переменных
        public string connectionString = "host=localhost;uid=root;pwd=;database=trade";
        public List<string> card = new List<string>();
        public string userNameEmployee;
        public int numberOrder;
        public Order(List<string> cards, string userNameEmplo)
        {
            InitializeComponent();
            card = cards;
            userNameEmployee = userNameEmplo;
        }
        //Заполнение формы элементами перед загрузкой
        private void Order_Load(object sender, EventArgs e)
        {
            int sumPriceProd = 0;
            try
            {
                foreach(var item in card)
                {
                    var itemCard = item.Split(';');
                    dataGridView1.Rows.Add(itemCard[0].ToString(), itemCard[1].ToString(), itemCard[2].ToString(), itemCard[3].ToString());
                }
                DateTime dateTime = DateTime.Now;
                label2.Text = dateTime.ToString("yyyy-MM-dd");

                label3.Text = dateTime.AddDays(10).ToString("yyyy-MM-dd");
                using (MySqlConnection con = new MySqlConnection())
                {
                    string query = "SELECT MAX(id) FROM basket";
                    con.ConnectionString = connectionString;
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    numberOrder = Convert.ToInt32(cmd.ExecuteScalar());
                     if (cmd.ExecuteScalar() == null)
                    {
                        label1.Text = "Номер заказа: 1";
                    }
                    else
                    {
                        label1.Text = $"Номер заказа: {numberOrder + 1}";
                    }
                }

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    sumPriceProd += Convert.ToInt32(row.Cells[3].Value);
                }
                label9.Text = $"{sumPriceProd}";
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {

        }
        //Высчитывание цены со скидкой
        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
            {
                if (label8.Text == "")
                {
                    label8.Text = $"{10}";
                    priceWithDiscount();
                }
                else
                {
                    label8.Text = $"{Convert.ToInt32(label8.Text) + 10}";
                    priceWithDiscount();
                }
            }
            else
            {
                label8.Text = $"{Convert.ToInt32(label8.Text) - 10}";
                priceWithDiscount();
            }
        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                if (label8.Text == "")
                {
                    label8.Text = $"{10}";
                    priceWithDiscount();
                }
                else
                {
                    label8.Text = $"{Convert.ToInt32(label8.Text) + 10}";
                    priceWithDiscount();
                }
            }
            else
            {
                label8.Text = $"{Convert.ToInt32(label8.Text) - 10}";
                priceWithDiscount();
            }
        }

        private void CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
            {
                if (label8.Text == "")
                {
                    label8.Text = $"{5}";
                    priceWithDiscount();
                }
                else
                {
                    label8.Text = $"{Convert.ToInt32(label8.Text) + 5}";
                    priceWithDiscount();
                }
            }
            else
            {
                label8.Text = $"{Convert.ToInt32(label8.Text) - 5}";
                priceWithDiscount();
            }
        }

        private void CheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked == true)
            {
                if(label8.Text == "")
                {
                    label8.Text = $"{15}";
                    priceWithDiscount();
                }
                else
                {
                    label8.Text = $"{Convert.ToInt32(label8.Text) + 15}";
                    priceWithDiscount();
                }
            }
            else
            {
                label8.Text = $"{Convert.ToInt32(label8.Text) - 15}";
                priceWithDiscount();
            }
        }

        void priceWithDiscount()
        {
            label10.Text = $"{Convert.ToDouble(label9.Text) - (Convert.ToDouble(label9.Text) * (Convert.ToDouble(label8.Text) / 100))}";
        }
        //Добавление
        private void Button2_Click(object sender, EventArgs e)
        {
            string query;
            try
            {
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    foreach(DataGridViewRow row in dataGridView1.Rows)
                    {
                        query = $"INSERT INTO basket (`id`, `article`, `count`) VALUES ('{numberOrder+1}', '{row.Cells[0].Value}', '{row.Cells[2].Value}');";
                        MySqlCommand cmd = new MySqlCommand(query, con);
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            
                        }
                        else
                        {
                            MessageBox.Show("Ошибка");
                        }
                    }
                }
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    if(label8.Text == "")
                    {
                        query = $@"INSERT INTO `trade`.`Orders` (`OrderID`, `OrderDate`, `OrderDeliveryDate`, `OrderStatus`, `OrderPrice`, `nameEmployee`, `OrderDiscount`) VALUES ('{numberOrder+1}', '{label2.Text}', '{label3.Text}', 'Выполнен', '{Convert.ToDouble(label9.Text)}', '{userNameEmployee}', 0);";
                    }
                    else
                    {
                        query = $@"INSERT INTO `trade`.`Orders` (`OrderID`, `OrderDate`, `OrderDeliveryDate`, `OrderStatus`, `OrderPrice`, `nameEmployee`, `OrderDiscount`) VALUES ('{numberOrder+1}', '{label2.Text}', '{label3.Text}', 'Выполнен', '{label10.Text.Replace(',','.')}', '{userNameEmployee}', '{Convert.ToInt32(label8.Text)}');";
                    }
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        MessageBox.Show("Заказ оформлен");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка");
                    }
                }
                button2.Enabled = false;

                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    foreach(DataGridViewRow row in dataGridView1.Rows)
                    {
                        query = $"SELECT ProductCount FROM Product WHERE ProductArticleNumber = '{row.Cells[0].Value}'";
                        MySqlCommand cmd = new MySqlCommand(query, con);
                        int countInTableProduct = Convert.ToInt32(cmd.ExecuteScalar());
                        query = $"UPDATE `trade`.`Product` SET `ProductCount` = '{countInTableProduct - Convert.ToInt32(row.Cells[2].Value)}' WHERE (`ProductArticleNumber` = '{row.Cells[0].Value}');";
                        cmd = new MySqlCommand(query, con);
                        if (cmd.ExecuteNonQuery() == 1)
                        {

                        }
                        else
                        {
                            MessageBox.Show("Ошибка");
                        }
                    }
                }

            }
            catch(Exception)
            {
                MessageBox.Show("Ошибка");
            }
        }
        //Создание чека
        private void Button3_Click(object sender, EventArgs e)
        {
            int RowCell = 2;
            Word.Application wdApp = new Word.Application();
            Word.Document wdDoc = null;
            Object wdMiss = System.Reflection.Missing.Value;

            wdDoc = wdApp.Documents.Add(ref wdMiss, ref wdMiss, ref wdMiss, ref wdMiss);

            wdDoc.PageSetup.Orientation = Word.WdOrientation.wdOrientPortrait;

            Word.Range wordrange = wdDoc.Range(ref wdMiss, ref wdMiss);
            wordrange.PageSetup.LeftMargin = wdApp.CentimetersToPoints(1);
            wordrange.PageSetup.RightMargin = wdApp.CentimetersToPoints(1);
            wordrange.PageSetup.TopMargin = wdApp.CentimetersToPoints(1);
            wordrange.PageSetup.BottomMargin = wdApp.CentimetersToPoints(1);

            wdApp.Visible = true;

            wdApp.ActiveWindow.Selection.ParagraphFormat.LineSpacingRule = Word.WdLineSpacing.wdLineSpaceSingle;
            wdApp.ActiveWindow.Selection.ParagraphFormat.SpaceAfter = 0.0F;
            wdDoc.PageSetup.PageHeight = 350 + (dataGridView1.Rows.Count * 45);
            wdDoc.PageSetup.PageWidth = 590;

            Word.Paragraph Input;
            Input = wdDoc.Content.Paragraphs.Add(ref wdMiss);
            Input.Range.Font.Name = "Times New Roman";
            Input.Range.Text = "Чек";
            Input.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            Input.Range.Font.Size = Convert.ToInt32(14);
            Input.Range.Font.Bold = 1;
            Input.SpaceAfter = 10;
            Input.Range.InsertParagraphAfter();
            Input.CloseUp();

            Word.Paragraph Countr;
            Countr = wdDoc.Content.Paragraphs.Add(ref wdMiss);
            Countr.Range.Font.Name = "Times New Roman";
            Countr.Range.Text = "Магазин находится по адресу: г. Заволжье ул. Пирогова д. 32";
            Countr.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            Countr.Range.Font.Size = Convert.ToInt32(14);
            Countr.Range.Font.Bold = 0;
            Countr.SpaceAfter = 10;
            Countr.Range.InsertParagraphAfter();
            Countr.CloseUp();

            Word.Paragraph Order;
            Order = wdDoc.Content.Paragraphs.Add(ref wdMiss);
            Order.Range.Text = "Заказ №" + numberOrder+ " от " + DateTime.Now.ToShortDateString() + " продавец " + userNameEmployee;
            Order.Range.Font.Name = "Times New Roman";
            Order.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            Order.Range.Font.Size = Convert.ToInt32(14);
            Order.Range.Font.Bold = 0;
            Order.SpaceAfter = 10;
            Order.Range.InsertParagraphAfter();
            Order.CloseUp();

            Word.Paragraph ForTab;
            ForTab = wdDoc.Content.Paragraphs.Add(ref wdMiss);
            ForTab.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            ForTab.Range.Font.Name = "Times New Roman";
            ForTab.Range.Font.Bold = 1;
            Object defaultTableBehavior = Word.WdDefaultTableBehavior.wdWord9TableBehavior;
            Object autoFitBehavior = Word.WdAutoFitBehavior.wdAutoFitWindow;

            //Добавляем таблицу и получаем объект wordtable 
            Word.Table wordtable = wdDoc.Tables.Add(ForTab.Range, dataGridView1.Rows.Count + 1, 3, ref defaultTableBehavior, ref autoFitBehavior);
            Word.Range wordcellrange = wdDoc.Tables[1].Cell(0, 0).Range;
            wordcellrange = wordtable.Cell(1, 0).Range;
            wordcellrange.Bold = 1;
            wordcellrange.Text = "Наименование";
            wordcellrange = wordtable.Cell(1, 2).Range;
            wordcellrange.Bold = 1;
            wordcellrange.Text = "Кол-во";
            wordcellrange = wordtable.Cell(1, 3).Range;
            wordcellrange.Bold = 1;
            wordcellrange.Text = "Стоимость";

            for (Int32 i = 0; i < dataGridView1.Rows.Count; i++)
            {
                wordcellrange = wordtable.Cell(RowCell, 0).Range;
                wordcellrange.Bold = 0;
                wordcellrange.Text = Convert.ToString(dataGridView1.Rows[i].Cells[1].Value);
                wordcellrange = wordtable.Cell(RowCell, 2).Range;
                wordcellrange.Bold = 0;
                wordcellrange.Text = Convert.ToString(dataGridView1.Rows[i].Cells[2].Value);
                wordcellrange = wordtable.Cell(RowCell, 3).Range;
                wordcellrange.Bold = 0;
               wordcellrange.Text = Convert.ToString(dataGridView1.Rows[i].Cells[3].Value);
                RowCell++;
            }

            Word.Paragraph Price;
            Price = wdDoc.Content.Paragraphs.Add(ref wdMiss);
            Price.Range.Text = "Стоимость: " + label9.Text;
            Price.Range.Font.Name = "Times New Roman";
            Price.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            Price.Range.Font.Size = Convert.ToInt32(14);
            Price.Range.Font.Bold = 0;
            Price.SpaceBefore = 10;
            Price.SpaceAfter = 10;
            Price.Range.InsertParagraphAfter();
            Price.CloseUp();

            Word.Paragraph Discount;
            Discount = wdDoc.Content.Paragraphs.Add(ref wdMiss);
            Discount.Range.Text = "Скидка: " + label8.Text + "%";
            Discount.Range.Font.Name = "Times New Roman";
            Discount.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            Discount.Range.Font.Size = Convert.ToInt32(14);
            Discount.Range.Font.Bold = 0;
            Discount.SpaceAfter = 10;
            Discount.Range.InsertParagraphAfter();
            Discount.CloseUp();

            Word.Paragraph fPrice;
            fPrice = wdDoc.Content.Paragraphs.Add(ref wdMiss);
            fPrice.Range.Text = "Итого: " + label10.Text;
            fPrice.Range.Font.Name = "Times New Roman";
            fPrice.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            fPrice.Range.Font.Size = Convert.ToInt32(14);
            fPrice.Range.Font.Bold = 0;
            fPrice.SpaceAfter = 10;
            fPrice.Range.InsertParagraphAfter();
            fPrice.CloseUp();
        }   //создание чека в Word
    }
}
