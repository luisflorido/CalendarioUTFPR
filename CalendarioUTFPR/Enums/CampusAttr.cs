using System;

namespace CalendarioUTFPR.Enums
{
    public class CampusAttr : Attribute
    {
        public string PortalCodigo { get; set; }

        internal CampusAttr(string portalCodigo)
        {
            this.PortalCodigo = portalCodigo;
        }
    }
}
