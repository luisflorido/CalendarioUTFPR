using CalendarioUTFPR.Enums;
using CalendarioUTFPR.Structs;
using CalendarioUTFPR.Utils;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
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
        private string moodleSession, moodleId, campus;
        private bool clicked = false;
        private Point lmAbs = new Point();

        public static Dictionary<int, string> licoes;
        public List<Materia> materias;
        private DispatcherTimer timer;
        private string username, password;
        public const string specifiedDate = "19:00:00";

        public Calendario(string username, string password, string moodleSession, string moodleId, string campus)
        {
            this.username = username;
            this.password = password;
            this.campus = campus;

            InitializeComponent();
            materias = new List<Materia>();
            this.moodleSession = moodleSession;
            this.moodleId = moodleId;
            IniciarCalendario();
            IniciarPortalAluno();
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
                if ((((licao.Key - 1) == atual.Day) || (licao.Key == atual.Day)) && (licao.Value != "Nenhum evento."))
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
            client.DownloadStringAsync(new Uri(URLS.MOODLE));
        }
        private void IniciarPortalAluno()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            WebClient client = new WebClient();
            String encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(this.username + ":" + this.password));
     
            client.Headers.Add("Authorization", "Basic " + encoded);
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(StringDownloadedPA);
            client.DownloadStringAsync(new Uri(URLS.PORTAL_INICIO(this.campus)));
        }

        private void LoadMaterias(object cursoCodigo, object alcuordemnr)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            WebClient client = new WebClient();
            String encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(this.username + ":" + this.password));

            client.Headers.Add("Authorization", "Basic " + encoded);
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(StringDownloadedMA);
            client.DownloadStringAsync(new Uri(URLS.PORTAL_MATERIAS(this.campus, username.ToString().Substring(0, username.ToString().Length - 1), (string)cursoCodigo, (string)alcuordemnr)));
        }

        private void LoadPlanejamentos(List<Materia> materias)
        {
            string url = "";
            foreach(Materia m in materias)
            {
                url = URLS.PORTAL_PLANEJAMENTO(m.Codigo);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                WebClient client = new WebClient();
                String encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(this.username + ":" + this.password));

                client.Headers.Add("Authorization", "Basic " + encoded);
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(StringDownloadedPlanejamento);
                client.DownloadStringAsync(new Uri(url));
            }
        }

        private void StringDownloadedPlanejamento(object sender, DownloadStringCompletedEventArgs args)
        {
            try
            {
                string result = args.Result;

                var doc = new HtmlDocument();
                doc.LoadHtml(result);

                var itens = doc.DocumentNode.SelectNodes("//tbody//tr//td");
                var count = 0;

                foreach(var item in itens)
                {
                    Regex rgx = new Regex(@"(Avaliação [0-9]|Prova [0-9])");
                    if (rgx.Match(item.InnerHtml).Success)
                    {
                        string data = itens[count - 4].InnerHtml;
                        int diaAtual = DateTime.Now.Day;
                        int diaProva = int.Parse(data.Split('/')[0]);
                        int mesProva = int.Parse(data.Split('/')[1]);
                        if(DateTime.Now.Month == mesProva)
                        {
                            string materiaNome = "\nPROVA";
                            List<Day> licoes = new List<Day>();
                            foreach(var licao in lv.ItemsSource as List<Day>)
                            {
                                if(licao.Dia == diaProva)
                                    licoes.Add(new Day() { Dia = diaProva, Licao = materiaNome });
                                else
                                    licoes.Add(licao);
                            }
                            lv.ItemsSource = licoes;
                            if (diaAtual == diaProva || (diaAtual + 1 == diaProva))
                                MainWindow.notify.ShowBalloonTip(4000, "Prova", "Prova "+(diaAtual == diaProva ? "hoje" : "amanhã")+" dia "+diaProva+"/"+ mesProva, System.Windows.Forms.ToolTipIcon.Info);
                        }
                    }
                    count++;
                }
            }
            catch(Exception e)
            {
                new CustomMessage().Show("Erro!", "Erro ao obter planejamento de aulas.");
                File.WriteAllText("error.txt", e.Message);
            }
        }

        private void StringDownloadedMA(object sender, DownloadStringCompletedEventArgs args)
        {
            try
            {
                string result = args.Result;

                var doc = new HtmlDocument();
                doc.LoadHtml(result);

                var itens = doc.DocumentNode.SelectNodes("//tr[@class='imprime']//td");
                var count = 0;
                foreach(var item in itens)
                {
                    if (item.InnerHtml.Contains("fcImprimirPA(")){
                        Regex rgxCodigo = new Regex(@"\'[^()]*\'");
                        string codigo = rgxCodigo.Match(item.InnerHtml).Value;
                        materias.Add(new Materia { Nome = itens[count - 3].InnerHtml, Codigo = codigo.Replace("'", "") });
                    }
                    count++;
                }
                LoadPlanejamentos(materias);
            }
            catch (Exception)
            {
                new CustomMessage().Show("Erro!", "Erro ao obter matérias.");
            }
        }

        private void StringDownloadedPA(object sender, DownloadStringCompletedEventArgs args)
         {
            try {
                string result = args.Result;

                Regex rgx = new Regex(@"curscodnr: [\d]+");
                Regex rgxAlcu = new Regex(@"alcuordemnr: [\d]+");
                var cursoCodigo = new Regex(@"[\d]+").Match(rgx.Match(result).Value).Value;
                var alcuordemnr = new Regex(@"[\d]+").Match(rgxAlcu.Match(result).Value).Value;

                LoadMaterias(cursoCodigo, alcuordemnr);
            }
            catch (Exception)
            {
                new CustomMessage().Show("Erro!", "Erro ao entrar no Portal do Aluno.");
                App.mw.Expired();
            }
        }

        private void StringDownloaded(object sender, DownloadStringCompletedEventArgs args)
         {
            try
            {
                string result = args.Result;

                var doc = new HtmlDocument();
                doc.LoadHtml(result);

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
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";
            const string stripFormatting = @"<[^>]*(>|$)";
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            text = WebUtility.HtmlDecode(text);
            text = tagWhiteSpaceRegex.Replace(text, "><");
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            text = stripFormattingRegex.Replace(text, string.Empty);

            return text;
        }
    }
}
