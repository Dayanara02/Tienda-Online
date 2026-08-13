
namespace TiendaOnline.Dominio.Configuracion
{
    // Contiene los datos necesarios para configurar la autenticación mediante JWT.
    public class JwtConfiguracion
    {
        // Clave secreta utilizada para generar y validar los tokens JWT.
        public string Clave { get; set; } = string.Empty;

        // Identifica quién emite el token.
        public string Emisor { get; set; } = string.Empty;

        // Define quién puede recibir o utilizar el token generado.
        public string Audiencia { get; set; } = string.Empty;

      // Indica cuánto tiempo permanece válido el token, expresado en minutos.
        public int DuracionMinutos { get; set; }
    }
}