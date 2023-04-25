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
using System.Diagnostics;
using System.IO;
namespace Все_для_бани
{
    public partial class SpecFeatures : Form
    {
        public string connectionString = "host=localhost;uid=root;pwd=;database=";
        public SpecFeatures()
        {
            InitializeComponent();
        }
        //Импорт данных
        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                if(File.Exists("dumpTrade.sql"))
                {
                    using (MySqlConnection con = new MySqlConnection())
                    {
                        con.ConnectionString = connectionString;
                        con.Open();
                        MySqlCommand cmd = new MySqlCommand($@"CREATE DATABASE trade;", con);
                        if (cmd.ExecuteNonQuery() == 1)
                        {

                        }
                        else
                        {
                            MessageBox.Show("Ошибка, БД с таким именем уже существует");
                        }
                    }
                    Process process = Process.Start(new ProcessStartInfo
                    {
                        FileName = "cmd",
                        Arguments = "/c mysql -u root -p trade < dumpTrade.sql",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                    });
                    MessageBox.Show("Успешно");
                }
                else
                {
                    MessageBox.Show("Нет, дамп файла");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка");
            }
        }
        //Экспорт данных
        private void Button2_Click(object sender, EventArgs e)
        {
            if(!File.Exists("dumpTrade.sql"))
            {
                try
                {
                    Process process = Process.Start(new ProcessStartInfo
                    {
                        FileName = "cmd",
                        Arguments = "/c mysqldump -u root -p trade > dumpTrade.sql",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                    });
                    MessageBox.Show("Успешно");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка");
                }
            }
            else
            {
                MessageBox.Show("Удалите старый, дамп файл");
            }
        }

        private void SpecFeatures_Load(object sender, EventArgs e)
        {

        }
    }
}
