namespace AeroGateWebApp.Auxiliar
{
    /// <summary>
    /// Extrae un mensaje de error legible de una respuesta HTTP no exitosa de la API.
    /// La API devuelve el detalle del error en el cuerpo (texto plano); si viene vacío,
    /// se arma un mensaje con el código de estado.
    /// </summary>
    public class ManejadorErroresApi
    {
        public static string ObtenerMensajeError(HttpResponseMessage respuesta)
        {
            string body = ClienteHttpAuxiliar.ObtenerBody(respuesta);

            if (string.IsNullOrWhiteSpace(body))
                return $"Error {(int)respuesta.StatusCode}: {respuesta.ReasonPhrase}";

            return body.Trim('"');
        }
    }
}
