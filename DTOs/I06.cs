using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiffGenerator2.DTOs
{
    [System.SerializableAttribute()]
    /* 
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    */
    public class I06
    {
        /*
        public string I06_OP_TIP { get; set; }
        public string I06_VAL_POZ { get; set; }
        public string I06_PVM_TIP { get; set; }
        public string I06_OP_STORNO { get; set; }
        public string I06_DOK_NR { get; set; }
        public string I06_OP_DATA { get; set; }
        public string I06_DOK_DATA { get; set; }
        public string I06_KODAS_MS { get; set; }
        public string I06_KODAS_KS { get; set; }
        public string I06_KODAS_SS { get; set; }
        public string I06_PAV { get; set; }
        public string I06_ADR { get; set; }
        public string I06_ATSTOVAS { get; set; }
        public string I06_KODAS_VS { get; set; }
        public string I06_PAV2 { get; set; }
        public string I06_ADR2 { get; set; }
        public string I06_ADR3 { get; set; }
        public string I06_KODAS_VL { get; set; }
        public string I06_KODAS_XS { get; set; }
        public string I06_KODAS_SS_P { get; set; }
        public string I06_PASTABOS { get; set; }
        public string I06_MOK_DOK { get; set; }
        public double I06_MOK_SUMA { get; set; }
        public string I06_KODAS_SS_M { get; set; }
        public double I06_SUMA_VAL { get; set; }
        public double I06_SUMA { get; set; }
        public double I06_SUMA_PVM { get; set; }
        public double I06_KURSAS { get; set; }
        public string I06_PERKELTA { get; set; }
        public string I06_ADDUSR { get; set; }
        public string I06_R_DATE { get; set; }
        public string I06_USERIS { get; set; }
        public string I06_KODAS_AU { get; set; }
        public string I06_KODAS_SM { get; set; }
        public string I06_INTRASTAT { get; set; }
        public string I06_DOK_REG { get; set; }
        public string I06_KODAS_AK { get; set; }
        public string I06_SUMA_WK { get; set; }
        public string I06_KODAS_LS_1 { get; set; }
        public string I06_KODAS_LS_2 { get; set; }
        public string I06_KODAS_LS_3 { get; set; }
        public string I06_KODAS_LS_4 { get; set; }
        public string I06_VAL_POZ_PVM { get; set; }
        public double I06_PVM_VAL { get; set; }
        public string I06_WEB_POZ { get; set; }
        public string I06_WEB_ATAS { get; set; }
        public string I06_WEB_PERKELTA { get; set; }
        public string I06_WEB_PERKELTA_I { get; set; }
        public string I06_KODAS_ZN { get; set; }
        public string I06_BUSENA { get; set; }
        public string I06_APRASYMAS1 { get; set; }
        public string I06_APRASYMAS2 { get; set; }
        public string I06_APRASYMAS3 { get; set; }
        public string I06_ISAF { get; set; }
        public string WEB_GLN { get; set; }
        public string WEB_GLN_KS { get; set; }
        public string PAP_1 { get; set; }
        public string PAP_2 { get; set; }
        */
        [System.Xml.Serialization.XmlElementAttribute("I07")]
        public List<I07> I07 { get; set; }
    }
}
