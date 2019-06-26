using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiffGenerator2.Services
{
    [System.SerializableAttribute()]
   /* [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]*/
    public class I07
    {
        [System.Xml.Serialization.XmlElementAttribute("I07_KODAS")]
        public string[] I07_KODAS { get; set; }
        public string I07_PAV { get; set; }
        public string I07_KODAS_IS { get; set; }
        public int I07_KIEKIS { get; set; }
        [XmlIgnore]
        public DateTime I07_GALIOJA_IKI_DateTime { get; set; }

        //[XmlElement("I07_GALIOJA_IKI_DateTime")]
        public string I07_GALIOJA_IKI
        {
            //get { return this.I07_GALIOJA_IKI_DateTime.ToString("yyyy.MM.dd HH:mm");}
            get { return this.I07_GALIOJA_IKI_DateTime.ToString("yyyy-MM-dd"); }
            set { this.I07_GALIOJA_IKI_DateTime = DateTime.ParseExact(value, "yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture); }
        }
        public string I07_APRASYMAS1 { get; set; }
        public string I07_APRASYMAS2 { get; set; }

        /*
        public string DI07_BAR_KODAS { get; set; }
        public int I07_EIL_NR { get; set; }
        public string I07_TIPAS { get; set; }
        public string I07_KODAS_TR { get; set; }
        public string I07_KODAS_OS { get; set; }
        public string I07_KODAS_OS_C { get; set; }
        public string I07_SERIJA { get; set; }
        public string I07_KODAS_US { get; set; }
        public int I07_FRAKCIJA { get; set; }
        public string I07_KODAS_US_P { get; set; }
        public string I07_KODAS_US_A { get; set; }
        public int I07_ALT_KIEKIS { get; set; }
        public int I07_ALT_FRAK { get; set; }
        public double I07_VAL_KAINA { get; set; }
        public double I07_SUMA_VAL { get; set; }
        public double I07_KAINA_BE { get; set; }
        public double I07_KAINA_SU { get; set; }
        public double I07_NUOLAIDA { get; set; }
        public string I07_ISLAIDU_M { get; set; }
        public double I07_ISLAIDOS { get; set; }
        public double I07_ISLAIDOS_PVM { get; set; }
        public string I07_MUITAS_M { get; set; }
        public double I07_MUITAS { get; set; }
        public double I07_MUITAS_PVM { get; set; }
        public string I07_AKCIZAS_M { get; set; }
        public double I07_AKCIZAS { get; set; }
        public double I07_AKCIZAS_PVM { get; set; }
        public string I07_MOKESTIS { get; set; }
        public double I07_MOKESTIS_P { get; set; }
        public double I07_PVM { get; set; }
        public double I07_SUMA { get; set; }
        public double I07_PAR_KAINA { get; set; }
        public double I07_PAR_KAINA_N { get; set; }
        public double I07_MOK_SUMA { get; set; }       
        public double I07_SAVIKAINA { get; set; }      
        public string I07_PERKELTA { get; set; }
        public string I07_ADDUSR { get; set; }
        public string I07_USERIS { get; set; }
        public string I07_R_DATE { get; set; }
        public string I07_SERTIFIKATAS { get; set; }
        public string I07_KODAS_KT { get; set; }
        public string I07_KODAS_K0 { get; set; }
        public string I07_KODAS_KV { get; set; }
        public string I07_KODAS_VZ { get; set; }
        public string I07_ADD_DATE { get; set; }
        public string I07_APSKRITIS { get; set; }
        public string I07_SANDORIS { get; set; }
        public string I07_SALYGOS { get; set; }
        public string I07_RUSIS { get; set; }
        public string I07_SALIS { get; set; }
        public string I07_MATAS { get; set; }
        public string I07_SALIS_K { get; set; }
        public string I07_MASE { get; set; }
        public string I07_INT_KIEKIS { get; set; }
        public double I07_PVM_VAL { get; set; }
        public string I07_KODAS_KS { get; set; }     
        public string I07_APRASYMAS3 { get; set; }
        public string I07_KODAS_KL { get; set; }
        public double T_KIEKIS { get; set; }
        public string PAP_1 { get; set; }
        public string PAP_2 { get; set; }
        */
    }
}
