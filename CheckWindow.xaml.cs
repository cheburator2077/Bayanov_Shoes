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
using System.Windows.Shapes;

namespace Bayanov_Shoes
{
    /// <summary>
    /// Логика взаимодействия для CheckWindow.xaml
    /// </summary>
    public partial class CheckWindow : Window
    {
        public CheckWindow(Order order, List<Product> products, string pickupAddress, string totalSum)
        {
            InitializeComponent();
            OrderNumTB.Text = $"Номер заказа: {order.OrderID}";
            OrderDateTB.Text = $"Дата заказа: {order.OrderDate:dd.MM.yyyy}";
            DeliveryDateTB.Text = $"Дата доставки: {order.OrderDeliveryDate:dd.MM.yyyy}";
            // Вычисляем срок доставки в днях (3 или 6 дней)
            int days = (order.OrderDeliveryDate.Date - order.OrderDate.Date).Days;
            DeliveryDaysTB.Text = $"Срок доставки: {days} дн.";

            PickupPointTB.Text = $"Пункт выдачи: {pickupAddress}";
            if (order.User != null)
            {
                ClientTB.Text = $"Клиент: {order.User.UserSurname} {order.User.UserName} {order.User.UserPatronymic}";
            }
            else
            {
                ClientTB.Text = "Клиент: Гость";
            }

            ProductsList.ItemsSource = products;
            TotalSumTB.Text = $"Итого к оплате: {totalSum}";
            OrderCodeTB.Text = order.OrderCode.ToString();
        }
    }
}
