using CalendarioUTFPR.Credentials;
using CalendarioUTFPR.Request;
using System;
using System.Net;
using System.Windows;

namespace CalendarioUTFPR
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window 
    {
        private bool clicked = false;
        private Point lmAbs = new Point();

        public MainWindow()
        {
            InitializeComponent();
            startCredential();
        }

        private void startCredential()
        {
            var cred = CredentialManager.ReadCredential("CalendarioUTFPR");

            if(cred != null && cred.MoodleSession != null && cred.MoodleId != null)
            {
                this.Hide();
                new Calendario(cred.MoodleSession, cred.MoodleId).Show();
            }
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Minimized;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string username = user.Text.ToString().Trim();
            string password = pass.Password.ToString().Trim();
            if(username != "" && password != "")
            {
                HTTPRequest request = new HTTPRequest(username, password);
                CookieCollection cookie = request.requestToken();
                if (cookie.Count > 1 && cookie["MoodleSession"] != null && cookie["MOODLEID1_"] != null)
                {
                    string moodleSession = cookie["MoodleSession"].Value;
                    string moodleid = cookie["MOODLEID1_"].Value;
                    CredentialManager.WriteCredential("CalendarioUTFPR", moodleSession, moodleid);
                    this.Hide();
                    new Calendario(moodleSession, moodleid).Show();
                }
                else
                {
                    new CustomMessage().show("Erro!", "Usuário ou senha incorretos!");
                }
            }
            else
            {
                new CustomMessage().show("Erro!", "Preencha os campos corretamente!");
            }
        }

        public void expired()
        {
            this.Show();
            CredentialManager.WriteCredential("CalendarioUTFPR", "", "");
        }
    }
}
