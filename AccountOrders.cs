using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MySql.Data.MySqlClient;
using Excel = Microsoft.Office.Interop.Excel;
namespace Все_для_бани
{
    public partial class AccountOrders : Form
    {
        //Объявление переменных
        public string connectionString = "host=localhost;uid=root;pwd=;database=trade";
        public string countInDb;
        public string userRole;
        public string Articl, Description, Category, Photo, defaulPhoto, NameProd, PriceProd;
        public string statusOrder, employeeOrder;
        public int howManyButtonUse = 0;
        public int sumPriceProd;
        private Excel.Application _excel;
        private Excel.Worksheet _sheet;

        private int _colCell; //Количество стобцов
        public List<string> card = new List<string>();
        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            update();
        }
        //Вызов формы просмотра заказа
        private void Button1_Click(object sender, EventArgs e)
        {
            string indexRow = "";
            try
            {
                if (dataGridView1.SelectedCells.Count > 0)
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if(row.Index == dataGridView1.CurrentCell.RowIndex)
                        {
                            indexRow = row.Cells[0].Value.ToString();
                        }
                    }

                    OrderInf order = new OrderInf(indexRow);
                    this.Hide();
                    order.ShowDialog();
                    this.Show();
                    update();
                }
                else
                {
                    MessageBox.Show("Выберите заказ для просмотра");
                }
            }
            catch(Exception)
            {
                MessageBox.Show("Ошибка");
            }
        }
        //Вызов функции после изменения элемента управления на форме
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            update();
        }
        //Проверка нажат ли элемент управления и дополнение запроса в базу
        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
            {
                checkBox2.Checked = false;
                statusOrder = " AND OrderStatus = 'Выполнен'";
            }
            else
            {
                statusOrder = "";
            }
            update();
        }
        //Проверка нажат ли элемент управления и дополнение запроса в базу
        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                checkBox1.Checked = false;
                statusOrder = " AND OrderStatus = 'Отменен'";
            }
            else
            {
                statusOrder = "";
            }
            update();
        }
        //Очищение филтров
        private void Button2_Click(object sender, EventArgs e)
        {
            comboBox2.SelectedIndex = -1;
            employeeOrder = "";
            statusOrder = "";
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            dateTimePicker1.Value = dateTimePicker1.MinDate;
            dateTimePicker2.Value = dateTimePicker2.MaxDate;
            update();
        }

        private void DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            update();
        }

        private void DateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            update();
        }
        //Экспорт в Excel файл
        private void Button3_Click(object sender, EventArgs e)
        {
            try
            {
                _excel = new Excel.Application(); //создаем COM-объект Excel
                _excel.SheetsInNewWorkbook = 1;//количество листов в книге
                _excel.Workbooks.Add(Type.Missing); //добавляем книгу
                Excel.Workbook workbook = _excel.Workbooks[1]; //получам ссылку на первую открытую книгу
                _sheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets.get_Item(1);//получаем ссылку на первый лист
                Excel.Range _excelCells1 = (Excel.Range)_sheet.get_Range("A1", "E1").Cells;
                // Производим объединение
                _excelCells1.Merge(Type.Missing);
                _sheet.Cells[1,1] = "Отчет дохода магазина <<Все для бани>>";
                _sheet.Cells[1, 1].HorizontalAlignment = -4108;
                Excel.Range _excelCells2 = (Excel.Range)_sheet.get_Range("A2", "E2").Cells;
                // Производим объединение
                _excelCells2.Merge(Type.Missing);
                _sheet.Cells[2, 1] = $"{dateTimePicker1.Value.ToShortDateString()} - {dateTimePicker2.Value.ToShortDateString()}";
                _sheet.Cells[2, 1].HorizontalAlignment = -4108;
                var dgw1 = new DataGridView();
                dgw1 = dataGridView1;
                var cntColl = dgw1.ColumnCount;
                var cntrow = dgw1.RowCount;
                try
                {
                    if (dgw1.RowCount != 0)
                    {

                        //Заполнение заголовков столбцов
                        for (int coll = 1; coll <= cntColl; coll++)
                        {
                            _sheet.Cells[3, coll] = dgw1.Columns[coll - 1].HeaderCell.Value;
                            ((Excel.Range)_sheet.get_Range($"A3:E3")).Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        }
                        //Заполнение ячеек данными
                        for (int row = 0; row <= cntrow - 1; row++)
                        {
                            for (int coll = 1; coll <= cntColl; coll++)
                            {
                                _sheet.Cells[row + 4, coll] = dgw1.Rows[row].Cells[coll - 1].Value;
                                ((Excel.Range)_sheet.get_Range($"A{row+4}:E{row+4}")).Cells.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            }
                        }
                        _sheet.Cells[4+cntrow, 5] = $"Общий доход: {sumPriceProd}";
                        _sheet.Columns.EntireColumn.AutoFit();
                    }
                    _excel.Visible = true;
                    Marshal.ReleaseComObject(_excel);
                }
                catch (Exception ee)
                {
                    MessageBox.Show("Ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка, ничего не выбрано");
            }
        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            employeeOrder = $" AND nameEmployee = '{comboBox2.Text}'";
        }
        //Настройка формы перед загрузкой
        private void AccountOrders_Load(object sender, EventArgs e)
        {
            switch(userRole)
            {
                case "1": label5.Visible = false; break;
            }
            comboBox1.SelectedIndex = 3;
            dateTimePicker1.MinDate = dateTimePicker1.Value.AddDays(-7);
            dateTimePicker1.MaxDate = dateTimePicker1.Value.AddDays(7);
            dateTimePicker2.MinDate = dateTimePicker2.Value.AddDays(-7);
            dateTimePicker2.MaxDate = dateTimePicker2.Value.AddDays(14);
            try
            {
                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand($@"SELECT distinct nameEmployee FROM trade.Orders;", con);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        comboBox2.Items.Add(rdr[0].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
            update();
        }

        public string userName;
        public int countBefore = 0;
        public int countAfet = 0;
        public AccountOrders(string Role)
        {
            InitializeComponent();
            userRole = Role;
        }
        //Запрос к базе по заполнение формы
        void update()
        {
            sumPriceProd = 0;
            try
            {
                
                string query;
                if(comboBox1.SelectedIndex == 3)
                {
                    query = $@"SELECT OrderID as 'Номер заказа', OrderDate as 'Дата заказа',nameEmployee as 'Менеджер', OrderStatus as 'Статус заказа', OrderPrice as 'Сумма заказа' FROM Orders WHERE OrderID LIKE '%{textBox1.Text.Trim()}%' AND OrderDate >= '{dateTimePicker1.Value.ToString("yyyy-MM-dd")} 00:00:00' AND OrderDate < '{dateTimePicker2.Value.ToString("yyyy-MM-dd")} 23:00:00'";
                    query += statusOrder;
                    query += employeeOrder;
                }
                else if(comboBox1.SelectedIndex == 0)
                {
                    query = $@"SELECT OrderID as 'Номер заказа', OrderDate as 'Дата заказа',nameEmployee as 'Менеджер', OrderStatus as 'Статус заказа', OrderPrice as 'Сумма заказа' FROM Orders WHERE OrderID LIKE '%{textBox1.Text.Trim()}%' AND OrderDate >= '{dateTimePicker1.Value.ToString("yyyy-MM-dd")} 00:00:00' AND OrderDate < '{dateTimePicker2.Value.ToString("yyyy-MM-dd")} 23:00:00'";
                    query += statusOrder;
                    query += employeeOrder;
                    query += " ORDER BY OrderPrice ASC";

                }
                else if(comboBox1.SelectedIndex == 1)
                {
                    query = $@"SELECT OrderID as 'Номер заказа', OrderDate as 'Дата заказа',nameEmployee as 'Менеджер', OrderStatus as 'Статус заказа', OrderPrice as 'Сумма заказа' FROM Orders WHERE OrderID LIKE '%{textBox1.Text.Trim()}%' AND OrderDate >= '{dateTimePicker1.Value.ToString("yyyy-MM-dd")} 00:00:00' AND OrderDate < '{dateTimePicker2.Value.ToString("yyyy-MM-dd")} 23:00:00'";
                    query += statusOrder;
                    query += employeeOrder;
                    query += " ORDER BY OrderDate ASC";
                }
                else
                {
                    query = $@"SELECT OrderID as 'Номер заказа', OrderDate as 'Дата заказа',nameEmployee as 'Менеджер', OrderStatus as 'Статус заказа', OrderPrice as 'Сумма заказа' FROM Orders WHERE OrderID LIKE '%{textBox1.Text.Trim()}%' AND OrderDate >= '{dateTimePicker1.Value.ToString("yyyy-MM-dd")} 00:00:00' AND OrderDate < '{dateTimePicker2.Value.ToString("yyyy-MM-dd")} 23:00:00'";
                    query += statusOrder;
                    query += employeeOrder;
                    query += " ORDER BY OrderPrice ASC, OrderDate ASC";
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
                }

                using (MySqlConnection con = new MySqlConnection())
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand($@"SELECT count(*) FROM Orders;", con);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        countInDb = rdr[0].ToString();
                    }
                }
                label1.Text = $"Количество записей: {dataGridView1.Rows.Count}/{countInDb}";
                switch(userRole)
                {
                    case "2":
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            sumPriceProd += Convert.ToInt32(row.Cells[4].Value);
                        }
                        label5.Text = $"Сумма оплаченнных заказов:  {sumPriceProd}";
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }
    }
}
