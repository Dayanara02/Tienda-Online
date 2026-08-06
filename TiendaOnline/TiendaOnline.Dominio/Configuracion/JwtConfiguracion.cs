namespace TiendaOnline.Dominio.Configuracion;

public class JwtConfiguracion
{
    public string Clave { get; set; } = string.Empty;

    public string Emisor { get; set; } = string.Empty;

    public string Audiencia { get; set; } = string.Empty;

    public int DuracionMinutos { get; set; }
}