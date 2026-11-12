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
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Page
    {
        List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();
        List<Product> selectedProducts = new List<Product>();
        private Order currentOrder = new Order();
        private OrderProduct currentOrderProduct = new OrderProduct();
        public OrderWindow(List<OrderProduct> selectedOrderProducts, List<Product> selecteProducts, string FIO)
        {
            InitializeComponent();
            var currentPickUps = Bayanov41SizeEntities.GetContext().PickUpPoint.ToList();
            //ComboOrderPickUp.ItemsSource = selectedOrderProducts;

            ClientTB.Text = FIO;
            TBOrderID.Text = selectedOrderProducts.First().ProductID.ToString();

            MenuListView.ItemsSource = selectedOrderProducts;
            foreach (Product p in selecteProducts)
            {
                p.ProductQuantityInStock = 1;
                foreach (OrderProduct q in selectedOrderProducts)
                {
                    if (p.ProductArticleNumber == q.ProductArticleNumber)
                        p.ProductQuantityInStock = q.ProductCount;
                }
            }

            this.selectedOrderProducts = selectedOrderProducts;
            this.selectedProducts = selecteProducts;
            //там ордердп дописать(дата заказа)
            //здесь должна быть функция SetDeliveryDate()
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
