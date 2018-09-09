using CalendarioUTFPR.Structs;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows;
using System.Windows.Threading;

namespace CalendarioUTFPR
{
    /// <summary>
    /// Lógica interna para Calendario.xaml
    /// </summary>
    public partial class Calendario : Window
    {
        string moodleSession, moodleId;
        private bool clicked = false;
        private Point lmAbs = new Point();

        public static Dictionary<int, string> licoes;
        private DispatcherTimer timer;
        public const string specifiedDate = "19:00:00";

        public Calendario(string moodleSession, string moodleId)
        {
            InitializeComponent();
            this.moodleSession = moodleSession;
            this.moodleId = moodleId;
            IniciarCalendario();
            licoes = new Dictionary<int, string>();
            BackgroundNotify();
        }


        private void BackgroundNotify()
        {
            string[] sDate = specifiedDate.Split(':');
            DateTime dateNow = DateTime.Now;
            DateTime date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, int.Parse(sDate[0]), int.Parse(sDate[1]), int.Parse(sDate[2]));
            if (dateNow > date)
                date = date.AddDays(1);

            TimeSpan rest = date - dateNow;
            timer = new DispatcherTimer();
            timer.Interval = rest;
            timer.Tick += VerifyNotifications;
            timer.Start();
        }

        private void VerifyNotifications(Object o, EventArgs args)
        {
            timer.Stop();
            Notify();
        }

        private void Notify()
        {
            foreach (KeyValuePair<int, string> licao in licoes)
            {
                DateTime atual = DateTime.Now;
                if (((licao.Key - 1) == atual.Day) || (licao.Key == atual.Day))
                {
                    MainWindow.notify.ShowBalloonTip(4000, "Lição para "+(licao.Key == atual.Day ? "hoje" : "amanhã")+" dia " + licao.Key, licao.Value, System.Windows.Forms.ToolTipIcon.Info);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Minimized;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Hide();
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
                Point MousePositionAbs = new Point(Convert.ToInt16(this.Left) + MousePosition.X, Convert.ToInt16(this.Top) + MousePosition.Y);
                this.Left = this.Left + (MousePositionAbs.X - this.lmAbs.X);
                this.Top = this.Top + (MousePositionAbs.Y - this.lmAbs.Y);
                this.lmAbs = MousePositionAbs;
            }
        }

        private void IniciarCalendario()
        {
            WebClient client = new WebClient();
            client.Encoding = ASCIIEncoding.UTF8;
            client.Headers.Add(HttpRequestHeader.Cookie, "MoodleSession="+this.moodleSession+";"+"MOODLEID1_="+this.moodleId);
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(StringDownloaded);
            client.DownloadStringAsync(new Uri("http://moodle.utfpr.edu.br/"));
        }

        private void StringDownloaded(object sender, DownloadStringCompletedEventArgs args)
        {
            try
            {
                string result = args.Result;

                var doc = new HtmlDocument();
                doc.LoadHtml(result);

                var body = doc.GetElementbyId("inst3").OuterHtml;

                var days = doc.DocumentNode.SelectNodes("//*[contains(@class, 'day')]");
                string s = "";

                foreach (HtmlNode e in days)
                {
                    s += e.OuterHtml;
                }

                var rdoc = new HtmlDocument();
                rdoc.LoadHtml(s);

                var td = rdoc.DocumentNode.SelectNodes("//td//a");

                foreach (HtmlNode e in td)
                {
                    var decoded = HttpUtility.HtmlDecode(e.Attributes["data-content"].Value);
                    var div = new HtmlDocument();
                    div.LoadHtml(decoded);

                    var divs = div.DocumentNode.SelectNodes("//div");

                    if(divs != null && divs.Count > 1)
                    {
                        string sb = "";
                        foreach(HtmlNode node in divs)
                        {
                            sb += node.InnerText+"\n";
                        }
                        licoes[int.Parse(e.InnerText)] = "\n" + sb;
                    }
                    else
                    {
                        licoes[int.Parse(e.InnerText)] = "\n" + HtmlToPlainText(e.Attributes["data-content"].Value);
                    }
                }

                List<Day> dias = new List<Day>();

                foreach(KeyValuePair<int, string> licao in licoes)
                {
                    dias.Add(new Day() { Dia = licao.Key, Licao = licao.Value });
                }
                lv.ItemsSource = dias;
                Notify();
            }
            catch (Exception)
            {
                this.Close();
                new MainWindow(false).Expired();
                new CustomMessage().Show("Erro!", "Sessão expirada! Logue novamente.");
            }
        }

        private void lv_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            dia.Opacity = 1;
            dian.Opacity = 1;
            licao.Opacity = 1;
            lv2.Opacity = 1;
            lv2.IsEnabled = true;

            Day d = (Day)e.AddedItems[0];
            List<Licao> lic = new List<Licao>();

            lic.Add(new Licao() { Tema = d.Licao });
            dian.Content = d.Dia;
            lv2.ItemsSource = lic;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private static string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);

            return text;
        }
    }
}
