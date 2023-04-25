using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Все_для_бани
{
    public partial class MainMenu : Form
    {
        public string userRole;
        public string userName;
        public MainMenu(string statusUser, string userNames)
        {
            InitializeComponent();
            userRole = statusUser;
            userName = userNames;
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            label2.Text = "Cотрудник " + userName;

            switch (userRole)
            {
                case "1": label3.Text = "Доступ: Менеджер"; button1.Visible = false; button6.Visible = false; break;
                case "2": label3.Text = "Доступ: Администратор"; break;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Guide guide = new Guide();
            this.Hide();
            guide.ShowDialog();
            this.Show();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Product product = new Product(userRole);
            this.Hide();
            product.ShowDialog();
            this.Show();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            Basket basket = new Basket(userName);
            this.Hide();
            basket.ShowDialog();
            this.Show();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            AccountOrders accountOrders = new AccountOrders(userRole);
            this.Hide();
            accountOrders.ShowDialog();
            this.Show();
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            SpecFeatures specFeatures = new SpecFeatures();
            this.Hide();
            specFeatures.ShowDialog();
            this.Show();
        }
    }
}
