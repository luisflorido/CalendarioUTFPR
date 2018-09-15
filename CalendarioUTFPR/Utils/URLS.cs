namespace CalendarioUTFPR.Utils
{
    public class URLS
    {
        public static string PORTAL_INICIO(string campus) { return "https://utfws.utfpr.edu.br/" + campus + "/sistema/mpmenu.inicio"; }
        public static string PORTAL_MATERIAS(string campus, string codigo, string cursoCodigo, string alcuordemnr) { return "https://utfws.utfpr.edu.br/"+campus+"/sistema/mpconfirmacaomatricula.pcTelaAluno?p_pesscodnr=" + codigo + "&p_curscodnr=" + cursoCodigo + "&p_alcuordemnr=" + alcuordemnr; }
        public static string PORTAL_PLANEJAMENTO(string codigoMateria) { return "https://utfws.utfpr.edu.br/aluno02/sistema/mpPlanejamentoAula.pcPlanejFinalizado?p_turmidvc=" + codigoMateria + "&p_print=0"; }

        public static string MOODLE = "http://moodle.utfpr.edu.br/"; 
    }
}
