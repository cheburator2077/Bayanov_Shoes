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
using System.Windows.Threading;

namespace Bayanov_Shoes
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        private string captchaCode;
        private bool captchaRequired = false;
        public AuthPage()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTB.Text;
            string password = PasswordTB.Text;
            if (login == "" || password == "")
            {
                MessageBox.Show("Есть пустые поля");
                return;
            }

            if (captchaRequired)
            {
                if (CaptchaTB.Text != captchaCode)
                {
                    MessageBox.Show("Неверно введена капча. Вход заблокирован на 10 секунд");
                    BlockFor10Seconds();
                    return;
                }
            }

            User user = Bayanov41SizeEntities.GetContext().User.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);
            if (user != null)
            {
                Manager.MainFrame.Navigate(new ProductPage(user));
                LoginTB.Text = "";
                PasswordTB.Text = "";
            }
            else
            {
                MessageBox.Show("Введены неверные данные");
                captchaRequired = true;
                ShowCaptcha();
            }
        }

        private void ShowCaptcha()
        {
            string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
            Random rnd = new Random();
            captchaCode = new string(Enumerable.Range(0, 4)
                .Select(_ => chars[rnd.Next(chars.Length)]).ToArray());

            captchaOneWord.Text = captchaCode[0].ToString();
            captchaTwoWord.Text = captchaCode[1].ToString();
            captchaThreeWord.Text = captchaCode[2].ToString();
            captchaFourWord.Text = captchaCode[3].ToString();

            CaptchaText.Visibility = Visibility.Visible;
            CaptchaTB.Visibility = Visibility.Visible;
            CaptchaPanel.Visibility = Visibility.Visible;
            CaptchaTB.Clear();
        }

        private void BlockFor10Seconds()
        {
            LoginBtn.IsEnabled = false;
            LoginTB.IsEnabled = false;
            PasswordTB.IsEnabled = false;
            CaptchaTB.IsEnabled = false;

            int secondsLeft = 10;
            LoginBtn.Content = $"Ждите {secondsLeft} сек.";

            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (s, args) =>
            {
                secondsLeft--;
                if (secondsLeft > 0)
                {
                    LoginBtn.Content = $"Ждите {secondsLeft} сек.";
                }
                else
                {
                    timer.Stop();
                    LoginBtn.IsEnabled = true;
                    LoginTB.IsEnabled = true;
                    PasswordTB.IsEnabled = true;
                    CaptchaTB.IsEnabled = true;
                    LoginBtn.Content = "Войти";
                    ShowCaptcha();
                }
            };
            timer.Start();
        }

        private void LoginHostBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
