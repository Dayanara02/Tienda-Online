// Permite utilizar listas.
using System.Collections.Generic;


namespace TiendaOnline.Dominio.DTO;

// Guarda los datos necesarios para crear una proforma.
public class ProformaCrearDto
{
    // Dirección relacionada con la proforma.
    public int? IdDireccion { get; set; }

    // Fecha hasta la que será válida.
    public DateOnly? FechaVencimiento { get; set; }

    // Productos incluidos en la proforma.
    public List<DetallePedidoCrearDto> Detalles { get; set; } =
        new List<DetallePedidoCrearDto>();
}