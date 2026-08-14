// Permite utilizar listas.
using System.Collections.Generic;


namespace TiendaOnline.Dominio.DTO;

// Guarda los datos para crear una compra a proveedor.
public class CompraProveedorCrearDto
{
    // Proveedor al que se realiza la compra.
    public int IdProveedor { get; set; }

    // Productos incluidos en la compra.
    public List<DetalleCompraProveedorDto> Detalles { get; set; } =
        new List<DetalleCompraProveedorDto>();
}