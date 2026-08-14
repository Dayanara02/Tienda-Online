// Permite crear controladores y respuestas HTTP.
using Microsoft.AspNetCore.Mvc;

// Permite realizar consultas con Entity Framework Core.
using Microsoft.EntityFrameworkCore;

// Permite proteger los endpoints.
using Microsoft.AspNetCore.Authorization;

// Permite trabajar con el entorno
// y encontrar la carpeta wwwroot.
using Microsoft.AspNetCore.Hosting;

// Contexto de la base de datos.
using TiendaOnline.AccesoDatos.Context;

// Entidades del proyecto.
using TiendaOnline.Dominio.Entidades;

// Herramientas para generar PDF.
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


// Define el espacio de nombres.
namespace TiendaOnline.API.Controllers;


// Solo usuarios autenticados.
[Authorize]

// Indica que funciona como API.
[ApiController]

// Ruta principal.
[Route("api/[controller]")]
public class FacturasController : ControllerBase
{
    // Contexto de SQL Server.
    private readonly TiendaOnlineContext _context;

    // Permite conocer la ruta física
    // donde está ejecutándose la API.
    private readonly IWebHostEnvironment _environment;


    // =====================================================
    // CONSTRUCTOR
    // =====================================================

    public FacturasController(
        TiendaOnlineContext context,
        IWebHostEnvironment environment)
    {
        // Guarda el contexto.
        _context = context;

        // Guarda el entorno.
        _environment = environment;
    }


    // =====================================================
    // GET: api/Facturas
    // =====================================================

    // Obtiene todas las facturas.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Factura>>>
        GetFacturas()
    {
        return await _context.Facturas
            .AsNoTracking()
            .ToListAsync();
    }


    // =====================================================
    // GET: api/Facturas/5
    // =====================================================

    // Obtiene una factura
    // mediante su ID.
    [HttpGet("{id}")]
    public async Task<ActionResult<Factura>>
        GetFactura(int id)
    {
        // Busca la factura.
        var factura =
            await _context.Facturas
                .FindAsync(id);


        // Si no existe...
        if (factura == null)
        {
            return NotFound();
        }


        // Devuelve factura.
        return factura;
    }


    // =====================================================
    // GET: api/Facturas/5/pdf
    // =====================================================

    // Permite abrir o descargar
    // el PDF físico de una factura.
    [HttpGet("{id}/pdf")]
    public async Task<IActionResult>
        GetPdfFactura(int id)
    {
        // Busca la factura.
        var factura =
            await _context.Facturas
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    f =>
                        f.IdFactura == id
                );


        // Comprueba existencia.
        if (factura == null)
        {
            return NotFound(
                "La factura no existe."
            );
        }


        // Comprueba que tenga URL.
        if (
            string.IsNullOrWhiteSpace(
                factura.UrlPdf
            )
        )
        {
            return NotFound(
                "La factura no tiene un PDF registrado."
            );
        }


        // Obtiene ruta física.
        var rutaFisica =
            ObtenerRutaFisicaPdf(
                factura.UrlPdf
            );


        // Comprueba que exista
        // realmente el archivo.
        if (
            !System.IO.File.Exists(
                rutaFisica
            )
        )
        {
            return NotFound(
                "El archivo PDF no existe."
            );
        }


        // Lee los bytes.
        var archivo =
            await System.IO.File
                .ReadAllBytesAsync(
                    rutaFisica
                );


        // Devuelve el PDF.
        return File(
            archivo,
            "application/pdf",
            $"{factura.NumeroFactura}.pdf"
        );
    }


    // =====================================================
    // POST: api/Facturas
    // =====================================================

    // Crea la factura y genera
    // automáticamente su PDF.
    [HttpPost]
    public async Task<ActionResult<Factura>>
        PostFactura(
            Factura factura)
    {
        // La base genera el ID.
        factura.IdFactura = 0;


        // Guarda fecha actual.
        factura.FechaEmision =
            DateTime.Now;


        // Primero registra factura
        // para obtener IdFactura.
        _context.Facturas.Add(
            factura
        );


        await _context
            .SaveChangesAsync();


        // =================================================
        // GENERAR NÚMERO DE FACTURA
        // =================================================

        // Si no vino número,
        // genera FAC-000001,
        // FAC-000002, etc.
        if (
            string.IsNullOrWhiteSpace(
                factura.NumeroFactura
            )
        )
        {
            factura.NumeroFactura =
                $"FAC-{factura.IdFactura:D6}";
        }


        // =================================================
        // CREAR CARPETA
        // =================================================

        // Obtiene la carpeta wwwroot.
        var webRoot =
            ObtenerWebRoot();


        // Crea:
        // wwwroot/facturas
        var carpetaFacturas =
            Path.Combine(
                webRoot,
                "facturas"
            );


        // Si no existe,
        // la crea automáticamente.
        Directory.CreateDirectory(
            carpetaFacturas
        );


        // =================================================
        // NOMBRE DEL PDF
        // =================================================

        var nombreArchivo =
            $"{factura.NumeroFactura}.pdf";


        // Ruta física.
        var rutaPdf =
            Path.Combine(
                carpetaFacturas,
                nombreArchivo
            );


        // =================================================
        // GENERAR PDF
        // =================================================

        GenerarPdfFactura(
            factura,
            rutaPdf
        );


        // =================================================
        // GUARDAR URL EN SQL
        // =================================================

        factura.UrlPdf =
            $"facturas/{nombreArchivo}";


        // Guarda número y ruta.
        await _context
            .SaveChangesAsync();


        // Devuelve factura creada.
        return CreatedAtAction(
            nameof(GetFactura),

            new
            {
                id =
                    factura.IdFactura
            },

            factura
        );
    }


    // =====================================================
    // PUT: api/Facturas/5
    // =====================================================

    // Actualiza la factura
    // y vuelve a generar su PDF.
    [HttpPut("{id}")]
    public async Task<IActionResult>
        PutFactura(
            int id,
            Factura factura)
    {
        // Busca factura existente.
        var facturaActual =
            await _context.Facturas
                .FindAsync(id);


        // Si no existe...
        if (
            facturaActual == null
        )
        {
            return NotFound();
        }


        // Actualiza pedido.
        facturaActual.IdPedido =
            factura.IdPedido;


        // Si recibe número,
        // lo actualiza.
        if (
            !string.IsNullOrWhiteSpace(
                factura.NumeroFactura
            )
        )
        {
            facturaActual.NumeroFactura =
                factura.NumeroFactura;
        }


        // Actualiza subtotal.
        facturaActual.Subtotal =
            factura.Subtotal;


        // Actualiza impuesto.
        facturaActual.Impuesto =
            factura.Impuesto;


        // Actualiza descuento.
        facturaActual.Descuento =
            factura.Descuento;


        // Actualiza total.
        facturaActual.Total =
            factura.Total;


        // Si todavía no tiene número...
        if (
            string.IsNullOrWhiteSpace(
                facturaActual.NumeroFactura
            )
        )
        {
            facturaActual.NumeroFactura =
                $"FAC-{facturaActual.IdFactura:D6}";
        }


        // Obtiene wwwroot.
        var webRoot =
            ObtenerWebRoot();


        // Carpeta facturas.
        var carpetaFacturas =
            Path.Combine(
                webRoot,
                "facturas"
            );


        // Crea carpeta.
        Directory.CreateDirectory(
            carpetaFacturas
        );


        // Nombre del archivo.
        var nombreArchivo =
            $"{facturaActual.NumeroFactura}.pdf";


        // Ruta completa.
        var rutaPdf =
            Path.Combine(
                carpetaFacturas,
                nombreArchivo
            );


        // Vuelve a generar el PDF.
        GenerarPdfFactura(
            facturaActual,
            rutaPdf
        );


        // Actualiza URL.
        facturaActual.UrlPdf =
            $"facturas/{nombreArchivo}";


        // Guarda cambios.
        await _context
            .SaveChangesAsync();


        return NoContent();
    }


    // =====================================================
    // DELETE: api/Facturas/5
    // =====================================================

    // Elimina factura y PDF.
    [HttpDelete("{id}")]
    public async Task<IActionResult>
        DeleteFactura(int id)
    {
        // Busca factura.
        var factura =
            await _context.Facturas
                .FindAsync(id);


        // Si no existe.
        if (
            factura == null
        )
        {
            return NotFound();
        }


        // =================================================
        // ELIMINAR PDF FÍSICO
        // =================================================

        if (
            !string.IsNullOrWhiteSpace(
                factura.UrlPdf
            )
        )
        {
            // Obtiene ruta.
            var rutaPdf =
                ObtenerRutaFisicaPdf(
                    factura.UrlPdf
                );


            // Comprueba archivo.
            if (
                System.IO.File.Exists(
                    rutaPdf
                )
            )
            {
                // Elimina archivo.
                System.IO.File.Delete(
                    rutaPdf
                );
            }
        }


        // Elimina factura.
        _context.Facturas.Remove(
            factura
        );


        // Guarda eliminación.
        await _context
            .SaveChangesAsync();


        return NoContent();
    }


    // =====================================================
    // GENERAR PDF
    // =====================================================

    // Crea físicamente
    // el documento PDF.
    private void GenerarPdfFactura(
        Factura factura,
        string rutaPdf)
    {
        // Crea el documento.
        Document.Create(
            container =>
            {
                // Configura página.
                container.Page(
                    page =>
                    {
                        // Tamaño A4.
                        page.Size(
                            PageSizes.A4
                        );


                        // Margen.
                        page.Margin(
                            40
                        );


                        // Fondo blanco.
                        page.Background(
                            Colors.White
                        );


                        // Fuente general.
                        page.DefaultTextStyle(
                            estilo =>
                                estilo.FontSize(
                                    11
                                )
                        );


                        // ===============================
                        // ENCABEZADO
                        // ===============================

                        page.Header()
                            .Column(
                                columna =>
                                {
                                    columna.Item()
                                        .Text(
                                            "ESENCIA"
                                        )
                                        .FontSize(
                                            24
                                        )
                                        .Bold();


                                    columna.Item()
                                        .Text(
                                            "Factura electrónica"
                                        )
                                        .FontSize(
                                            14
                                        );


                                    columna.Item()
                                        .PaddingTop(
                                            5
                                        )
                                        .Text(
                                            factura.NumeroFactura
                                        )
                                        .Bold();
                                }
                            );


                        // ===============================
                        // CONTENIDO
                        // ===============================

                        page.Content()
                            .PaddingVertical(
                                25
                            )
                            .Column(
                                columna =>
                                {
                                    // Fecha.
                                    columna.Item()
                                        .Text(
                                            $"Fecha de emisión: " +
                                            $"{factura.FechaEmision:dd/MM/yyyy hh:mm tt}"
                                        );


                                    // Pedido.
                                    columna.Item()
                                        .PaddingTop(
                                            5
                                        )
                                        .Text(
                                            $"Pedido: #{factura.IdPedido}"
                                        );


                                    // Separador.
                                    columna.Item()
                                        .PaddingVertical(
                                            20
                                        )
                                        .LineHorizontal(
                                            1
                                        );


                                    // Subtotal.
                                    columna.Item()
                                        .Row(
                                            fila =>
                                            {
                                                fila.RelativeItem()
                                                    .Text(
                                                        "Subtotal"
                                                    );

                                                fila.ConstantItem(
                                                    150
                                                )
                                                    .AlignRight()
                                                    .Text(
                                                        $"₡{factura.Subtotal:N2}"
                                                    );
                                            }
                                        );


                                    // Impuesto.
                                    columna.Item()
                                        .PaddingTop(
                                            8
                                        )
                                        .Row(
                                            fila =>
                                            {
                                                fila.RelativeItem()
                                                    .Text(
                                                        "Impuesto"
                                                    );

                                                fila.ConstantItem(
                                                    150
                                                )
                                                    .AlignRight()
                                                    .Text(
                                                        $"₡{factura.Impuesto:N2}"
                                                    );
                                            }
                                        );


                                    // Descuento.
                                    columna.Item()
                                        .PaddingTop(
                                            8
                                        )
                                        .Row(
                                            fila =>
                                            {
                                                fila.RelativeItem()
                                                    .Text(
                                                        "Descuento"
                                                    );

                                                fila.ConstantItem(
                                                    150
                                                )
                                                    .AlignRight()
                                                    .Text(
                                                        $"₡{factura.Descuento:N2}"
                                                    );
                                            }
                                        );


                                    // Separador.
                                    columna.Item()
                                        .PaddingVertical(
                                            15
                                        )
                                        .LineHorizontal(
                                            1
                                        );


                                    // Total.
                                    columna.Item()
                                        .Row(
                                            fila =>
                                            {
                                                fila.RelativeItem()
                                                    .Text(
                                                        "TOTAL"
                                                    )
                                                    .FontSize(
                                                        16
                                                    )
                                                    .Bold();


                                                fila.ConstantItem(
                                                    150
                                                )
                                                    .AlignRight()
                                                    .Text(
                                                        $"₡{factura.Total:N2}"
                                                    )
                                                    .FontSize(
                                                        16
                                                    )
                                                    .Bold();
                                            }
                                        );


                                    // Mensaje.
                                    columna.Item()
                                        .PaddingTop(
                                            35
                                        )
                                        .Text(
                                            "Gracias por su compra."
                                        )
                                        .FontSize(
                                            12
                                        );
                                }
                            );


                        // ===============================
                        // PIE DE PÁGINA
                        // ===============================

                        page.Footer()
                            .AlignCenter()
                            .Text(
                                texto =>
                                {
                                    texto.Span(
                                        "Página "
                                    );

                                    texto.CurrentPageNumber();

                                    texto.Span(
                                        " de "
                                    );

                                    texto.TotalPages();
                                }
                            );
                    }
                );
            }
        )
        // Guarda físicamente
        // el documento.
        .GeneratePdf(
            rutaPdf
        );
    }


    // =====================================================
    // OBTENER WWWROOT
    // =====================================================

    private string ObtenerWebRoot()
    {
        // Si ASP.NET ya conoce wwwroot,
        // utiliza esa ruta.
        if (
            !string.IsNullOrWhiteSpace(
                _environment.WebRootPath
            )
        )
        {
            return _environment
                .WebRootPath;
        }


        // Si todavía no existe,
        // crea una ruta wwwroot
        // dentro de TiendaOnline.API.
        var webRoot =
            Path.Combine(
                _environment.ContentRootPath,
                "wwwroot"
            );


        // Crea la carpeta.
        Directory.CreateDirectory(
            webRoot
        );


        return webRoot;
    }


    // =====================================================
    // CONVERTIR URL A RUTA FÍSICA
    // =====================================================

    private string ObtenerRutaFisicaPdf(
        string urlPdf)
    {
        // Cambia las barras
        // para trabajar correctamente
        // con Windows.
        var rutaRelativa =
            urlPdf.Replace(
                '/',
                Path.DirectorySeparatorChar
            );


        // Combina wwwroot
        // con la ruta guardada en SQL.
        return Path.Combine(
            ObtenerWebRoot(),
            rutaRelativa
        );
    }
}