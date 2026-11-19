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
        List<Product> allProducts;
        List<Product> selectedProducts = new List<Product>();
        List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();
        public ProductPage(User user)
        {
            InitializeComponent();
            if (user == null)
            {
                FIOTB.Text = "Вы авторизованы как гость";
                Role.Text = "Роль: Гость";
            }
            else
            {
                FIOTB.Text = "Вы авторизованы как " + user.UserSurname + " " + user.UserName + " " + user.UserPatronymic;
                switch (user.UserRole)
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
            ComboType.SelectedIndex = 0;
            UpdateProducts();
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ProductListView.SelectedItem as Product;

            if (selectedProduct == null)
            {
                MessageBox.Show("Выберите товар!");
                return;
            }

            MessageBox.Show("Товар " + selectedProduct.ProductName + " добавлен в заказ!");
        }

        private Order currentOrder = new Order();

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (ProductListView.SelectedIndex >= 0)
            {
                var prod = ProductListView.SelectedItem as Product;

                // Получаем новый ID заказа (+1 к максимальному в БД)
                int newOrderID = 1;
                if (Bayanov41SizeEntities.GetContext().Order.Any())
                {
                    newOrderID = Bayanov41SizeEntities.GetContext().Order.Max(o => o.OrderID) + 1;
                }

                // Добавляем товар в список для отображения в окне заказа (если его там еще нет)
                if (!selectedProducts.Any(p => p.ProductArticleNumber == prod.ProductArticleNumber))
                {
                    selectedProducts.Add(prod);
                }

                // Проверяем, есть ли уже такой товар в формируемом заказе
                var existingOrderProd = selectedOrderProducts
                    .FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);

                if (existingOrderProd == null)
                {
                    // Если товара еще нет — создаем новую запись в количестве 1 шт.
                    var newOrderProd = new OrderProduct
                    {
                        ProductID = newOrderID,
                        ProductArticleNumber = prod.ProductArticleNumber,
                        ProductCount = 1
                    };
                    selectedOrderProducts.Add(newOrderProd);
                }
                else
                {
                    // Если товар уже добавляли — увеличиваем количество
                    existingOrderProd.ProductCount++;
                }

                // Показываем кнопку просмотра заказа и снимаем выделение со строки
                OrderBtn.Visibility = Visibility.Visible;
                ProductListView.SelectedIndex = -1;
            }
        }

        private void OrderBtn_Click(object sender, RoutedEventArgs e)
        {
            selectedProducts = selectedProducts.Distinct().ToList();

            // Открываем модальное окно заказа и    передаем списки товаров и ФИО клиента
            OrderWindow orderWindow = new OrderWindow(selectedOrderProducts, selectedProducts, FIOTB.Text);
            orderWindow.ShowDialog();

            // После закрытия окна проверяем: если товары удалили или заказ сохранили — скрываем кнопку
            if (selectedProducts.Count == 0)
            {
                OrderBtn.Visibility = Visibility.Collapsed;
            }
        }
    }
}
