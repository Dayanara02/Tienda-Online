// Permite dar formato a los montos.
using System.Globalization;

// Herramientas para crear el PDF.
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

// Importa las entidades.
using TiendaOnline.Dominio.Entidades;

// Importa la interfaz del PDF.
using TiendaOnline.Dominio.InterfacesLN;

namespace TiendaOnline.LogicaNegocio.Servicios;

// Genera el comprobante de pago.
public class PdfServicio : IPdfServicio
{
    // Genera el PDF con los datos del pedido.
    public byte[] GenerarComprobante(
        Pedido pedido,
        Pago pago)
    {
        // Crea el documento.
        return QuestPDF.Fluent.Document
            .Create(documento =>
            {
                // Configura la página.
                documento.Page(pagina =>
                {
                    // Usa tamaño A4.
                    pagina.Size(PageSizes.A4);

                    // Agrega margen.
                    pagina.Margin(35);

                    // Define el texto base.
                    pagina.DefaultTextStyle(
                        texto => texto.FontSize(10)
                    );

                    // Crea el encabezado.
                    pagina.Header()
                        .Column(columna =>
                        {
                            // Nombre de la tienda.
                            columna.Item()
                                .Text("ESENCIA")
                                .FontSize(22)
                                .Bold()
                                .FontColor("#1f4e79");

                            // Nombre del documento.
                            columna.Item()
                                .Text("Comprobante de pago")
                                .FontSize(14)
                                .FontColor("#163a5c");
                        });

                    // Crea el contenido.
                    pagina.Content()
                        .PaddingVertical(20)
                        .Column(columna =>
                        {
                            // Separa los elementos.
                            columna.Spacing(12);

                            // Número del pedido.
                            columna.Item()
                                .Text(
                                    $"Pedido: #{pedido.IdPedido}"
                                )
                                .Bold();

                            // Nombre del cliente.
                            columna.Item()
                                .Text(
                                    $"Cliente: {pedido.IdUsuarioNavigation.Nombre} {pedido.IdUsuarioNavigation.Apellido}"
                                );

                            // Correo del cliente.
                            columna.Item()
                                .Text(
                                    $"Correo: {pedido.IdUsuarioNavigation.Correo}"
                                );

                            // Fecha del pago.
                            columna.Item()
                                .Text(
                                    $"Fecha del pago: {pago.FechaPago:dd/MM/yyyy HH:mm}"
                                );

                            // Método utilizado.
                            columna.Item()
                                .Text(
                                    $"Método de pago: {pago.MetodoPago}"
                                );

                            // Referencia del pago.
                            columna.Item()
                                .Text(
                                    $"Referencia: {pago.Referencia}"
                                );

                            // Línea separadora.
                            columna.Item()
                                .PaddingTop(5)
                                .LineHorizontal(1)
                                .LineColor(
                                    Colors.Grey.Lighten2
                                );

                            // Título de productos.
                            columna.Item()
                                .PaddingTop(5)
                                .Text("Productos")
                                .FontSize(13)
                                .Bold()
                                .FontColor("#163a5c");

                            // Crea la tabla.
                            columna.Item()
                                .Table(tabla =>
                                {
                                    // Define las columnas.
                                    tabla.ColumnsDefinition(
                                        columnas =>
                                        {
                                            // Nombre del producto.
                                            columnas.RelativeColumn(4);

                                            // Cantidad.
                                            columnas.ConstantColumn(60);

                                            // Precio.
                                            columnas.ConstantColumn(90);

                                            // Subtotal.
                                            columnas.ConstantColumn(90);
                                        }
                                    );

                                    // Crea el encabezado.
                                    tabla.Header(encabezado =>
                                    {
                                        // Producto.
                                        encabezado.Cell()
                                            .Background("#eaf2f8")
                                            .Padding(6)
                                            .Text("Producto")
                                            .Bold();

                                        // Cantidad.
                                        encabezado.Cell()
                                            .Background("#eaf2f8")
                                            .Padding(6)
                                            .Text("Cantidad")
                                            .Bold();

                                        // Precio.
                                        encabezado.Cell()
                                            .Background("#eaf2f8")
                                            .Padding(6)
                                            .Text("Precio")
                                            .Bold();

                                        // Subtotal.
                                        encabezado.Cell()
                                            .Background("#eaf2f8")
                                            .Padding(6)
                                            .Text("Subtotal")
                                            .Bold();
                                    });

                                    // Recorre los productos.
                                    foreach (
                                        var detalle
                                        in pedido.DetallePedidos
                                    )
                                    {
                                        // Nombre del producto.
                                        tabla.Cell()
                                            .BorderBottom(1)
                                            .BorderColor(
                                                Colors.Grey.Lighten2
                                            )
                                            .Padding(6)
                                            .Text(
                                                detalle
                                                    .IdProductoNavigation
                                                    .Nombre
                                            );

                                        // Cantidad.
                                        tabla.Cell()
                                            .BorderBottom(1)
                                            .BorderColor(
                                                Colors.Grey.Lighten2
                                            )
                                            .Padding(6)
                                            .Text(
                                                detalle.Cantidad
                                                    .ToString()
                                            );

                                        // Precio unitario.
                                        tabla.Cell()
                                            .BorderBottom(1)
                                            .BorderColor(
                                                Colors.Grey.Lighten2
                                            )
                                            .Padding(6)
                                            .Text(
                                                FormatearMonto(
                                                    detalle.PrecioUnitario
                                                )
                                            );

                                        // Subtotal del producto.
                                        tabla.Cell()
                                            .BorderBottom(1)
                                            .BorderColor(
                                                Colors.Grey.Lighten2
                                            )
                                            .Padding(6)
                                            .Text(
                                                FormatearMonto(
                                                    detalle.Subtotal
                                                )
                                            );
                                    }
                                });

                            // Muestra los montos.
                            columna.Item()
                                .PaddingTop(8)
                                .AlignRight()
                                .Column(resumen =>
                                {
                                    // Separa los valores.
                                    resumen.Spacing(5);

                                    // Subtotal.
                                    resumen.Item()
                                        .Text(
                                            $"Subtotal: {FormatearMonto(pedido.Subtotal)}"
                                        );

                                    // Descuento.
                                    resumen.Item()
                                        .Text(
                                            $"Descuento: {FormatearMonto(pedido.Descuento)}"
                                        );

                                    // Impuesto.
                                    resumen.Item()
                                        .Text(
                                            $"Impuesto: {FormatearMonto(pedido.Impuesto)}"
                                        );

                                    // Total final.
                                    resumen.Item()
                                        .PaddingTop(5)
                                        .Text(
                                            $"TOTAL: {FormatearMonto(pedido.Total)}"
                                        )
                                        .FontSize(14)
                                        .Bold()
                                        .FontColor("#1f4e79");
                                });

                            // Título de dirección.
                            columna.Item()
                                .PaddingTop(10)
                                .Text("Dirección de entrega")
                                .Bold();

                            // Dirección del pedido.
                            columna.Item()
                                .Text(
                                    pedido.DireccionEntrega
                                    ?? "No indicada"
                                );

                            // Estado del pago.
                            columna.Item()
                                .PaddingTop(10)
                                .Text(
                                    $"Estado del pago: {pago.Estado}"
                                )
                                .Bold()
                                .FontColor("#287444");
                        });

                    // Crea el pie de página.
                    pagina.Footer()
                        .AlignCenter()
                        .Text(texto =>
                        {
                            // Nombre de la tienda.
                            texto.Span(
                                "Esencia Tienda Online - Página "
                            );

                            // Número de página.
                            texto.CurrentPageNumber();
                        });
                });
            })

            // Convierte el documento en PDF.
            .GeneratePdf();
    }

    // Da formato de colones.
    private static string FormatearMonto(
        decimal monto)
    {
        // Usa el formato de Costa Rica.
        return $"₡{monto.ToString(
            "N0",
            CultureInfo.GetCultureInfo("es-CR")
        )}";
    }
}