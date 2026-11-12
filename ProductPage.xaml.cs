using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bayanov_Shoes
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        public ProductPage(User user)
        {
            InitializeComponent();
            FIOTB.Text = "Вы авторизованы как " + user.UserSurname + " " + user.UserName + " " + user.UserPatronymic;
            switch(user.UserRole)
            {
                case 1:
                    Role.Text = "Роль: Клиент"; break;
                case 2:
                    Role.Text = "Роль: Менеджер"; break;
                case 3:
                    Role.Text = "Роль: Администратор"; break;
            }
            var currentservice = Bayanov41SizeEntities.GetContext().Product.ToList();
            ProductListView.ItemsSource = currentservice;
        }

        private void UpdateProducts()
        {
            var currentProducts = Bayanov41SizeEntities.GetContext().Product.ToList();
            if (ComboType.SelectedIndex == 0)
            {
                currentProducts = currentProducts.Where(p => (p.ProductDiscountAmount >= 0 && p.ProductDiscountAmount <= 100)).ToList();
            }
            if (ComboType.SelectedIndex == 1)
            {
                currentProducts = currentProducts.Where(p => (p.ProductDiscountAmount >= 0 && p.ProductDiscountAmount < 9.99)).ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                currentProducts = currentProducts.Where(p => (p.ProductDiscountAmount >= 10 && p.ProductDiscountAmount < 14.99)).ToList();
            }
            if (ComboType.SelectedIndex == 3)
            {
                currentProducts = currentProducts.Where(p => (p.ProductDiscountAmount >= 15 && p.ProductDiscountAmount <= 100)).ToList();
            }
            currentProducts = currentProducts.Where(p => p.ProductName.ToLower().Contains(TBox_Search.Text.ToLower())).ToList();
            if (RButton_Up.IsChecked.Value)
            {
                currentProducts = currentProducts.OrderBy(p => p.ProductCost).ToList();
            }
            if (RButton_Down.IsChecked.Value)
            {
                currentProducts = currentProducts.OrderByDescending(p => p.ProductCost).ToList();
            }
            ProductListView.ItemsSource = currentProducts;
            AmountCounter.Text = "Кол-во " + currentProducts.Count().ToString() + " из " + Bayanov41SizeEntities.GetContext().Product.ToList().Count().ToString();
        }

        private void TBox_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void RButton_Up_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProducts();
        }

        private void RButton_Down_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProducts();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void OrderBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
