using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace JitterClickTest
{
    public partial class MainWindow : Window
    {
        private int clickCount;
        private DateTime lastClickTime;
        private DispatcherTimer timer;
        private TimeSpan remainingTime;
        private Random random;
        private double maxWidth;
        private double maxHeight;

        public MainWindow()
        {
            InitializeComponent();
            clickCount = 0;
            lastClickTime = DateTime.Now;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;

            remainingTime = TimeSpan.FromSeconds(10);
            UpdateTimeRemainingText();

            random = new Random();

            // Ana pencere boyutlarına göre maksimum genişlik ve yüksekliği ayarlayalım
            maxWidth = 292;
            maxHeight = 239;
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            string playerName = nameTextBox.Text;

            if (!string.IsNullOrWhiteSpace(playerName))
            {
                AnimateBackgroundChange(); // Animate the background color change
                startScreen.Visibility = Visibility.Collapsed;
                gameScreen.Visibility = Visibility.Visible;
                clickButton.IsEnabled = true;
                clickButton.Focus();
            }
        }


        private void clickButton_Click(object sender, RoutedEventArgs e)
        {
            if (!timer.IsEnabled)
            {
                timer.Start();
            }

            MoveButtonToRandomLocation();
            AnimateButtonColorChange();
            clickCount++;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            remainingTime = remainingTime.Subtract(TimeSpan.FromMilliseconds(100));
            UpdateTimeRemainingText();

            if (remainingTime.TotalMilliseconds <= 0)
            {
                timer.Stop();
                clickButton.IsEnabled = false;
                clickCountTextBlock.Text = "Score: " + clickCount.ToString();

                // Süre dolduğunda animasyonu durdur ve son renkte bırak
                StopButtonColorAnimation();
            }
        }

        private void UpdateTimeRemainingText()
        {
            timeRemainingTextBlock.Text = "Remaining Time: " + remainingTime.ToString("s\\.fff");
        }

        private void MoveButtonToRandomLocation()
        {
            double newLeft = random.NextDouble() * maxWidth;
            double newTop = random.NextDouble() * maxHeight;

            Canvas.SetLeft(clickButton, newLeft);
            Canvas.SetTop(clickButton, newTop);
        }

        private void AnimateButtonColorChange()
        {
            Color randomColor = Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));

            // Renk geçiş animasyonunu oluştur
            ColorAnimation colorAnimation = new ColorAnimation(randomColor, TimeSpan.FromMilliseconds(500));
            colorAnimation.AutoReverse = true; // Renk değişimini geriye doğru tekrarla
            colorAnimation.EasingFunction = new QuadraticEase(); // Animasyonu yumuşat

            // Arka plan rengini değiştiren Storyboard'u başlat
            Storyboard.SetTarget(colorAnimation, clickButton);
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("Background.Color"));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(colorAnimation);
            storyboard.Begin();
        }

        private void StopButtonColorAnimation()
        {
            // Animasyonu durdur ve son renkte bırak
            clickButton.Background.BeginAnimation(SolidColorBrush.ColorProperty, null);
        }
        private void AnimateBackgroundChange()
        {
            // Her animasyon döngüsünde farklı bir random renk oluştur
            Color randomColor = GetRandomColor();

            // Renk geçiş animasyonunu oluştur
            ColorAnimation colorAnimation = new ColorAnimation(randomColor, TimeSpan.FromSeconds(1));
            colorAnimation.AutoReverse = true; // Renk değişimini geriye doğru tekrarla
            colorAnimation.Completed += ColorAnimation_Completed; // Animasyon tamamlandığında yeni bir animasyon başlat

            // Grid'in arka plan rengini değiştirmek için bir SolidColorBrush oluştur
            SolidColorBrush brush = new SolidColorBrush();
            gameScreen.Background = brush;

            // Renk animasyonunu SolidColorBrush'in Color özelliğiyle ilişkilendir
            brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
        }

        private void ColorAnimation_Completed(object sender, EventArgs e)
        {
            // Animasyon tamamlandığında yeni bir animasyon başlat
            AnimateBackgroundChange();
        }

        private Color GetRandomColor()
        {
            return Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
        }

    }
}
