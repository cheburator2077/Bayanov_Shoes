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
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();
        List<Product> selectedProducts = new List<Product>();
        private Order currentOrder = new Order();
        private OrderProduct currentOrderProduct = new OrderProduct();
        public OrderWindow(List<OrderProduct> selectedOrderProducts, List<Product> selectedProducts, string FIO)
        {
            InitializeComponent();

            // Загружаем пункты выдачи
            var currentPickups = Bayanov41SizeEntities.GetContext().PickUpPoint.ToList();
            PickUpCB.ItemsSource = currentPickups;

            // Заполняем ФИО клиента и номер заказа
            FIOTB.Text = FIO;
            if (selectedOrderProducts.Any())
                OrderNumberTB.Text = selectedOrderProducts.First().ProductID.ToString();

            // Передаем товары в список и проставляем актуальное количество
            OrderListView.ItemsSource = selectedProducts;
            foreach (Product p in selectedProducts)
            {
                p.ProductQuantity = 1;
                foreach (OrderProduct q in selectedOrderProducts)
                {
                    if (p.ProductArticleNumber == q.ProductArticleNumber)
                        p.ProductQuantity = q.ProductCount;
                }
            }

            this.selectedOrderProducts = selectedOrderProducts;
            this.selectedProducts = selectedProducts;
            OrderDP.SelectedDate = DateTime.Now;

            UpdateOrderSummary();
            SetDeliveryDate();
        }

        private void SetDeliveryDate()
        {
            bool allInStockMoreThanThree = selectedProducts.All(p => (p.ProductQuantityInStock - p.ProductQuantity) > 3);

            int daysToAdd = allInStockMoreThanThree ? 3 : 6;
            DeliveryDP.SelectedDate = DateTime.Now.AddDays(daysToAdd);

        }

        private void UpdateOrderSummary()
        {
            decimal totalSum = 0;

            foreach (var prod in selectedProducts)
            {
                decimal itemPrice = prod.ProductCost * prod.ProductQuantity;
                decimal discount = itemPrice * (Convert.ToDecimal(prod.ProductDiscountAmount) / 100m);

                totalSum += (itemPrice - discount);
            }

            SumTB.Text = $"{totalSum:F2} руб.";
        }

        private void BtnPlus_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;

            if (prod.ProductQuantity >= prod.ProductQuantityInStock)
            {
                MessageBox.Show($"На складе доступно только {prod.ProductQuantityInStock} шт. этого товара.",
                                "Ограничение количества", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Прерываем выполнение, не увеличивая количество
            }

            prod.ProductQuantity++;

            var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
            if (selectedOP != null)
                selectedOP.ProductCount++;

            SetDeliveryDate();
            UpdateOrderSummary();
            OrderListView.Items.Refresh();
        }

        private void BtnMinus_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);

            if (prod.ProductQuantity > 1)
            {
                prod.ProductQuantity    --;
                if (selectedOP != null)
                    selectedOP.ProductCount--;
            }
            else
            {
                selectedProducts.Remove(prod);
                if (selectedOP != null)
                    selectedOrderProducts.Remove(selectedOP);
                if (selectedProducts.Count == 0)
                    this.Close();
            }

            SetDeliveryDate();
            UpdateOrderSummary();
            OrderListView.Items.Refresh(); // Без этой строки цифра не обновляется
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PickUpCB.SelectedItem == null)
            {
                MessageBox.Show("Выберите пункт выдачи!");
                return;
            }

            // 1. Заполняем данные самого заказа
            currentOrder.OrderDate = DateTime.Now;
            currentOrder.OrderDeliveryDate = DeliveryDP.SelectedDate.Value;
            currentOrder.OrderPickupPoint = (PickUpCB.SelectedItem as PickUpPoint).ID; // Проверьте имя ID пункта выдачи!
            currentOrder.OrderStatus = "Новый";
            currentOrder.OrderCode = new Random().Next(100, 999);

            // Если клиент авторизован — ищем его ID в БД, если Гость — ставим null
            if (FIOTB.Text == "Гость" || string.IsNullOrWhiteSpace(FIOTB.Text))
            {
                currentOrder.OrderClientID = null; // Если в БД поле называется ClientID
            }
            else
            {
                // Ищем пользователя по ФИО
                var user = Bayanov41SizeEntities.GetContext().User.ToList()
                    .FirstOrDefault(u => $"{u.UserSurname} {u.UserName} {u.UserPatronymic}" == FIOTB.Text);
                if (user != null)
                    currentOrder.OrderClientID = user.UserID;
            }

            // 2. Сначала добавляем сам заказ в контекст
            Bayanov41SizeEntities.GetContext().Order.Add(currentOrder);

            // 3. Добавляем товары заказа, привязывая их к объекту currentOrder (БД сама подставит правильный OrderID!)
            foreach (var op in selectedOrderProducts)
            {
                op.ProductID = currentOrder.OrderID;
                Bayanov41SizeEntities.GetContext().OrderProduct.Add(op);
            }

            try
            {
                Bayanov41SizeEntities.GetContext().SaveChanges();
                MessageBox.Show("Заказ успешно сохранен!");
                this.Close();

                var selectedPickup = PickUpCB.SelectedItem as PickUpPoint;
                string pickupAddress = $"{selectedPickup.PickUpPointIndex} г. {selectedPickup.PickUpPointCity} ул. {selectedPickup.PickUpPointStreet} {selectedPickup.PickUpPointHouse}";

                // Открываем окно талона заказа поверх окна оформления
                CheckWindow ticketWindow = new CheckWindow(currentOrder, selectedProducts, pickupAddress, SumTB.Text);
                ticketWindow.ShowDialog();

                // Очищаем корзину и закрываем окно заказа после закрытия талона
                selectedProducts.Clear();
                selectedOrderProducts.Clear();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
