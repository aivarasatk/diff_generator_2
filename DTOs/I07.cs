using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiffGenerator2.DTOs
{
    [System.SerializableAttribute()]
    public class I07
    {
        private string[] _code;
        [System.Xml.Serialization.XmlElementAttribute("I07_KODAS")]
        public string[] Code {
            get { return _code; }
            set { _code = value.Select(v => v.Trim()).ToArray(); }
        }

        private string _name;
        [System.Xml.Serialization.XmlElementAttribute("I07_PAV")]
        public string Name {
            get { return _name; }
            set { _name = value.Trim(); }
        }

        private string _maker;
        [System.Xml.Serialization.XmlElementAttribute("I07_KODAS_IS")]
        public string Maker {
            get { return _maker; }
            set { _maker = value.Trim(); }
        }

        [System.Xml.Serialization.XmlElementAttribute("I07_KIEKIS")]
        public int Amount { get; set; }
        [XmlIgnore]
        public DateTime DateDateTime { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("I07_GALIOJA_IKI")]
        public string DateString
        {
            get { return this.DateDateTime.ToString("yyyy-MM-dd"); }
            set { this.DateDateTime = DateTime.ParseExact(value, "yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture); }
        }

        [System.Xml.Serialization.XmlElementAttribute("I07_APRASYMAS1")]
        public string Details1 { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("I07_APRASYMAS2")]
        public string Details2 { get; set; }
    }
}
