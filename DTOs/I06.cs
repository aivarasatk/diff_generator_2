using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiffGenerator2.DTOs
{
    [System.SerializableAttribute()]
    public class I06
    {
        [System.Xml.Serialization.XmlElementAttribute("I07")]
        public List<I07> I07 { get; set; }
    }
}
