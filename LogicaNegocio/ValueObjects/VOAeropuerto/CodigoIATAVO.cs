using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using LogicaNegocio.Excepciones;
using LogicaNegocio.InterfacesDominio;

namespace LogicaNegocio.ValueObjects.VOAeropuerto
{
    [ComplexType]
    public class CodigoIATAVO : IValidable
    {
        public string Valor { get; set; }

        public CodigoIATAVO() { }

        public CodigoIATAVO(string valor)
        {
            Valor = valor;
        }

        // Permite asignar directamente: CodigoIATAVO codigo = "MVD";
        public static implicit operator CodigoIATAVO(string valor)
        {
            return new CodigoIATAVO(valor);
        }

        public void Validar()
        {
            if (string.IsNullOrWhiteSpace(Valor)) 
                throw new AeropuertoException("El código IATA es obligatorio");
            if (!Regex.IsMatch(Valor, @"^[A-Z]{3}$")) 
                throw new AeropuertoException("El código IATA debe ser exactamente 3 letras mayúsculas (ej. MVD)");
        }

        public override string ToString()
        {
            return Valor;
        }
    }
}
