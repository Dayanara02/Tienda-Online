namespace TiendaOnline.Dominio.EntidadesTipadas;

public class TDescuento
{
    public int IdDescuento { get; set; }

    public int CantidadMinima { get; set; }

    public decimal Porcentaje { get; set; }

    public bool Estado { get; set; }
}