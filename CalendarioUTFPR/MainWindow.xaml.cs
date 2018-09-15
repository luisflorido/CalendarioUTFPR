using CalendarioUTFPR.Credentials;
using CalendarioUTFPR.Enums;
using CalendarioUTFPR.Request;
using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Forms;

namespace CalendarioUTFPR
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window 
    {
        private bool clicked = false;
        private Point lmAbs = new Point();
        public static NotifyIcon notify = new NotifyIcon();
        public bool silent = false;
        public Calendario cal;

        public MainWindow(bool silent)
        {
            this.silent = silent;
            InitializeComponent();
            StartCredential();
            ShowIcon();
        }

        private void ShowIcon()
        {
            notify.Visible = true;
            notify.Icon = Properties.Resources.icone;
            notify.ContextMenu = new ContextMenu();
            notify.ContextMenu.MenuItems.Add("Abrir");
            notify.ContextMenu.MenuItems.Add("Deslogar");
            notify.ContextMenu.MenuItems.Add("Sair");
            notify.ContextMenu.MenuItems[0].Click += new EventHandler(Open_Click);
            notify.ContextMenu.MenuItems[1].Click += new EventHandler(Logout_Click);
            notify.ContextMenu.MenuItems[2].Click += new EventHandler(Close_Click);
            notify.DoubleClick += new EventHandler(Open_Click);
        }
        private void Logout_Click(Object sender, EventArgs args)
        {
            CredentialManager.WriteCredential("CalendarioUTFPR", "", "");
            Environment.Exit(0);
        }

        private void Open_Click(Object sender, EventArgs args) {
            if (cal != null)
            {
                cal.WindowState = WindowState.Normal;
                cal.Show();
            }
            else
            {
                if (this.WindowState == WindowState.Minimized)
                {
                    this.WindowState = WindowState.Normal;
                }
                this.Show();
            }
        }
        private void Close_Click(Object sender, EventArgs args)
        {
            Environment.Exit(0);
        }

        private void StartCredential()
        {
            var cred = CredentialManager.ReadCredential("CalendarioUTFPR");

            if(cred != null && cred.Username != null && cred.Password != null)
            {
                this.Hide();
                var username = cred.Username.Split('|')[0];
                var campus = cred.Username.Split('|')[1];
                this.Logar(username, cred.Password, campus);
                //cal = new Calendario(cred.MoodleSession, cred.MoodleId);
                if (!silent)
                {
                    cal.Show();
                }
                else
                {
                    cal.Hide();
                }
            }
            else
            {
                this.Show();
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
            Logar();
        }

        private void Logar()
        {
            string username = user.Text.ToString().Trim();
            string password = pass.Password.ToString().Trim();
            string campus = cb.SelectedValue != null ? ((System.Windows.Controls.ComboBoxItem)cb.SelectedValue).Name : "";

            if (username != "" && password != "" && campus != "")
            {
                campus = GetPortalCodigo(campus);

                HTTPRequest request = new HTTPRequest(username, password);
                CookieCollection cookie = request.RequestToken();
                if (cookie.Count > 1 && cookie["MoodleSession"] != null && cookie["MOODLEID1_"] != null)
                {
                    string moodleSession = cookie["MoodleSession"].Value;
                    string moodleid = cookie["MOODLEID1_"].Value;
                    CredentialManager.WriteCredential("CalendarioUTFPR", username+"|"+campus, password);
                    this.Hide();
                    cal = new Calendario(username.Replace("a", ""), password, moodleSession, moodleid, campus);
                    cal.Show();
                }
                else
                {
                    new CustomMessage().Show("Erro!", "Usuário ou senha incorretos!");
                }
            }
            else
            {
                new CustomMessage().Show("Erro!", "Preencha os campos corretamente!");
            }
        }

        private void Logar(string username, string password, string campus)
        {
            HTTPRequest request = new HTTPRequest(username, password);
            CookieCollection cookie = request.RequestToken();
            if (cookie.Count > 1 && cookie["MoodleSession"] != null && cookie["MOODLEID1_"] != null)
            {
                string moodleSession = cookie["MoodleSession"].Value;
                string moodleid = cookie["MOODLEID1_"].Value;
                CredentialManager.WriteCredential("CalendarioUTFPR", username+"|"+campus, password);
                this.Hide();
                cal = new Calendario(username.Replace("a", ""), password, moodleSession, moodleid, campus);
                cal.Show();
            }
            else
            {
                new CustomMessage().Show("Erro!", "Usuário ou senha incorretos!");
            }
        }

        public void Expired()
        {
            this.Show();
            CredentialManager.WriteCredential("CalendarioUTFPR", "", "");
        }

        private void pass_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
            {
                Logar();
            }
        }
        public string GetPortalCodigo(string campus)
        {
            foreach (object obj in Enum.GetValues(typeof(Campus)))
            {
                if (obj.ToString().Equals(campus))
                {
                    Campus cp = (Campus)Enum.Parse(typeof(Campus), campus);

                    return GetAttribute<CampusAttr>(cp).PortalCodigo;
                }
            }
            return null;
        }

        private TAttribute GetAttribute<TAttribute>(Enum value)
        where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name)
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }
    }
}
