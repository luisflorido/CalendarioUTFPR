using System;
using System.Windows;

namespace CalendarioUTFPR
{
    /// <summary>
    /// Lógica interna para CustomMessage.xaml
    /// </summary>
    public partial class CustomMessage : Window
    {
        private bool clicked = false;
        private Point lmAbs = new Point();

        public CustomMessage()
        {
            InitializeComponent();
        }

        private void PnMouseDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            clicked = true;
            this.lmAbs = e.GetPosition(this);
            this.lmAbs.Y = Convert.ToInt16(this.Top) + this.lmAbs.Y;
            this.lmAbs.X = Convert.ToInt16(this.Left) + this.lmAbs.X;
        }

        private void PnMouseUp(object sender, System.Windows.Input.MouseEventArgs e)
        {
            clicked = false;
        }

        private void PnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (clicked)
            {
                Point MousePosition = e.GetPosition(this);
                Point MousePositionAbs = new Point();
                MousePositionAbs.X = Convert.ToInt16(this.Left) + MousePosition.X;
                MousePositionAbs.Y = Convert.ToInt16(this.Top) + MousePosition.Y;
                this.Left = this.Left + (MousePositionAbs.X - this.lmAbs.X);
                this.Top = this.Top + (MousePositionAbs.Y - this.lmAbs.Y);
                this.lmAbs = MousePositionAbs;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void show(string m1, string m2)
        {
            this.Title = m1;
            this.msg1.Content = m1;
            this.msg2.Content = m2;
            this.Show();
        }
    }
}
