using LogicaNegocio.Excepciones;
using LogicaNegocio.InterfacesDominio;
using LogicaNegocio.ValueObjects.VOAeropuerto;

namespace LogicaNegocio.Entidades
{
    // Catálogo estable: los aeropuertos no se eliminan (LETRA 3.2)
    public class Aeropuerto : IValidable
    {
        public int Id { get; set; }
        public CodigoIATAVO CodigoIATA { get; set; }
        public string Nombre { get; set; }
        public string Ciudad { get; set; }
        public string Pais { get; set; }

        public Aeropuerto() { }

        public Aeropuerto(string codigoIATA, string nombre, string ciudad, string pais)
        {
            CodigoIATA = codigoIATA;
            Nombre = nombre;
            Ciudad = ciudad;
            Pais = pais;
        }

        public void Validar()
        {
            if (CodigoIATA is null) throw new AeropuertoException("El código IATA es obligatorio");
            CodigoIATA.Validar();

            if (string.IsNullOrWhiteSpace(Nombre)) throw new AeropuertoException("El nombre es obligatorio");
            if (string.IsNullOrWhiteSpace(Ciudad)) throw new AeropuertoException("La ciudad es obligatoria");
            if (string.IsNullOrWhiteSpace(Pais)) throw new AeropuertoException("El país es obligatorio");
        }
    }
}
