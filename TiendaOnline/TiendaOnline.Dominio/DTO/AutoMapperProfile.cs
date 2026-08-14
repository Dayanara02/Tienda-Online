// Permite utilizar AutoMapper.
using AutoMapper;

// Importa las entidades principales.
using TiendaOnline.Dominio.Entidades;

// Importa las entidades tipadas.
using TiendaOnline.Dominio.EntidadesTipadas;

// Define el espacio de nombres.
namespace TiendaOnline.Dominio.DTO;

// Configura los mapeos del proyecto.
public class AutoMapperProfile : Profile
{
    // Constructor del perfil.
    public AutoMapperProfile()
    {



        // Convierte Categorium a TCategoria.
        CreateMap<Categorium, TCategoria>()

            // Mapea el identificador.
            .ForMember(
                destino => destino.IdCategoria,
                opcion => opcion.MapFrom(
                    origen => origen.IdCategoria
                )
            )

            // Mapea la familia.
            .ForMember(
                destino => destino.IdFamilia,
                opcion => opcion.MapFrom(
                    origen => origen.IdFamilia
                )
            )

            // Mapea el nombre.
            .ForMember(
                destino => destino.Nombre,
                opcion => opcion.MapFrom(
                    origen => origen.Nombre
                )
            )

            // Mapea la descripción.
            .ForMember(
                destino => destino.Descripcion,
                opcion => opcion.MapFrom(
                    origen => origen.Descripcion
                )
            )

            // Mapea el estado.
            .ForMember(
                destino => destino.Estado,
                opcion => opcion.MapFrom(
                    origen => origen.Estado
                )
            );


        // Convierte TCategoria a Categorium.
        CreateMap<TCategoria, Categorium>()

            // Mapea el identificador.
            .ForMember(
                destino => destino.IdCategoria,
                opcion => opcion.MapFrom(
                    origen => origen.IdCategoria
                )
            )

            // Mapea la familia.
            .ForMember(
                destino => destino.IdFamilia,
                opcion => opcion.MapFrom(
                    origen => origen.IdFamilia
                )
            )

            // Mapea el nombre.
            .ForMember(
                destino => destino.Nombre,
                opcion => opcion.MapFrom(
                    origen => origen.Nombre
                )
            )

            // Mapea la descripción.
            .ForMember(
                destino => destino.Descripcion,
                opcion => opcion.MapFrom(
                    origen => origen.Descripcion
                )
            )

            // Mapea el estado.
            .ForMember(
                destino => destino.Estado,
                opcion => opcion.MapFrom(
                    origen => origen.Estado
                )
            )

            // Ignora la relación con familia.
            .ForMember(
                destino => destino.IdFamiliaNavigation,
                opcion => opcion.Ignore()
            )

            // Ignora los productos relacionados.
            .ForMember(
                destino => destino.Productos,
                opcion => opcion.Ignore()
            )

            // Ignora los descuentos relacionados.
            .ForMember(
                destino => destino.IdDescuentos,
                opcion => opcion.Ignore()
            );



        // Convierte Producto a TProducto.
        CreateMap<Producto, TProducto>()

            // Mapea el identificador.
            .ForMember(
                destino => destino.IdProducto,
                opcion => opcion.MapFrom(
                    origen => origen.IdProducto
                )
            )

            // Mapea la categoría.
            .ForMember(
                destino => destino.IdCategoria,
                opcion => opcion.MapFrom(
                    origen => origen.IdCategoria
                )
            )

            // Mapea el impuesto.
            .ForMember(
                destino => destino.IdImpuesto,
                opcion => opcion.MapFrom(
                    origen => origen.IdImpuesto
                )
            )

            // Mapea el nombre.
            .ForMember(
                destino => destino.Nombre,
                opcion => opcion.MapFrom(
                    origen => origen.Nombre
                )
            )

            // Mapea la descripción.
            .ForMember(
                destino => destino.Descripcion,
                opcion => opcion.MapFrom(
                    origen => origen.Descripcion
                )
            )

            // Mapea el código.
            .ForMember(
                destino => destino.Codigo,
                opcion => opcion.MapFrom(
                    origen => origen.Codigo
                )
            )

            // Mapea el precio.
            .ForMember(
                destino => destino.Precio,
                opcion => opcion.MapFrom(
                    origen => origen.Precio
                )
            )

            // Mapea el costo.
            .ForMember(
                destino => destino.Costo,
                opcion => opcion.MapFrom(
                    origen => origen.Costo
                )
            )

            // Mapea la imagen.
            .ForMember(
                destino => destino.Imagen,
                opcion => opcion.MapFrom(
                    origen => origen.Imagen
                )
            )

            // Mapea el stock mínimo.
            .ForMember(
                destino => destino.StockMinimo,
                opcion => opcion.MapFrom(
                    origen => origen.StockMinimo
                )
            )

            // Mapea el estado.
            .ForMember(
                destino => destino.Estado,
                opcion => opcion.MapFrom(
                    origen => origen.Estado
                )
            )

            // Mapea la fecha de registro.
            .ForMember(
                destino => destino.FechaRegistro,
                opcion => opcion.MapFrom(
                    origen => origen.FechaRegistro
                )
            );


        // Convierte TProducto a Producto.
        CreateMap<TProducto, Producto>()

            // Mapea el identificador.
            .ForMember(
                destino => destino.IdProducto,
                opcion => opcion.MapFrom(
                    origen => origen.IdProducto
                )
            )

            // Mapea la categoría.
            .ForMember(
                destino => destino.IdCategoria,
                opcion => opcion.MapFrom(
                    origen => origen.IdCategoria
                )
            )

            // Mapea el impuesto.
            .ForMember(
                destino => destino.IdImpuesto,
                opcion => opcion.MapFrom(
                    origen => origen.IdImpuesto
                )
            )

            // Mapea el nombre.
            .ForMember(
                destino => destino.Nombre,
                opcion => opcion.MapFrom(
                    origen => origen.Nombre
                )
            )

            // Mapea la descripción.
            .ForMember(
                destino => destino.Descripcion,
                opcion => opcion.MapFrom(
                    origen => origen.Descripcion
                )
            )

            // Mapea el código.
            .ForMember(
                destino => destino.Codigo,
                opcion => opcion.MapFrom(
                    origen => origen.Codigo
                )
            )

            // Mapea el precio.
            .ForMember(
                destino => destino.Precio,
                opcion => opcion.MapFrom(
                    origen => origen.Precio
                )
            )

            // Mapea el costo.
            .ForMember(
                destino => destino.Costo,
                opcion => opcion.MapFrom(
                    origen => origen.Costo
                )
            )

            // Mapea la imagen.
            .ForMember(
                destino => destino.Imagen,
                opcion => opcion.MapFrom(
                    origen => origen.Imagen
                )
            )

            // Mapea el stock mínimo.
            .ForMember(
                destino => destino.StockMinimo,
                opcion => opcion.MapFrom(
                    origen => origen.StockMinimo
                )
            )

            // Mapea el estado.
            .ForMember(
                destino => destino.Estado,
                opcion => opcion.MapFrom(
                    origen => origen.Estado
                )
            )

            // Mapea la fecha.
            .ForMember(
                destino => destino.FechaRegistro,
                opcion => opcion.MapFrom(
                    origen => origen.FechaRegistro
                )
            )

            // Ignora detalles del carrito.
            .ForMember(
                destino => destino.DetalleCarritos,
                opcion => opcion.Ignore()
            )

            // Ignora compras a proveedores.
            .ForMember(
                destino => destino.DetalleCompraProveedors,
                opcion => opcion.Ignore()
            )

            // Ignora listas de deseos.
            .ForMember(
                destino => destino.DetalleListaDeseos,
                opcion => opcion.Ignore()
            )

            // Ignora detalles de pedidos.
            .ForMember(
                destino => destino.DetallePedidos,
                opcion => opcion.Ignore()
            )

            // Ignora detalles de proformas.
            .ForMember(
                destino => destino.DetalleProformas,
                opcion => opcion.Ignore()
            )

            // Ignora evaluaciones.
            .ForMember(
                destino => destino.EvaluacionProductos,
                opcion => opcion.Ignore()
            )

            // Ignora la navegación de categoría.
            .ForMember(
                destino => destino.IdCategoriaNavigation,
                opcion => opcion.Ignore()
            )

            // Ignora la navegación de impuesto.
            .ForMember(
                destino => destino.IdImpuestoNavigation,
                opcion => opcion.Ignore()
            )

            // Ignora inventario.
            .ForMember(
                destino => destino.Inventario,
                opcion => opcion.Ignore()
            )

            // Ignora proveedores.
            .ForMember(
                destino => destino.ProductoProveedors,
                opcion => opcion.Ignore()
            )

            // Ignora descuentos.
            .ForMember(
                destino => destino.IdDescuentos,
                opcion => opcion.Ignore()
            );


    
        // Convierte Pedido a TPedido.

        CreateMap<Pedido, TPedido>()

            // Mapea el identificador.

            .ForMember(

                destino => destino.IdPedido,

                opcion => opcion.MapFrom(

                    origen => origen.IdPedido

                )

            )

            // Mapea el usuario.

            .ForMember(

                destino => destino.IdUsuario,

                opcion => opcion.MapFrom(

                    origen => origen.IdUsuario

                )

            )

            // Mapea la fecha.

            .ForMember(

                destino => destino.FechaPedido,

                opcion => opcion.MapFrom(

                    origen => origen.FechaPedido

                )

            )

            // Mapea el estado.

            .ForMember(

                destino => destino.Estado,

                opcion => opcion.MapFrom(

                    origen => origen.Estado

                )

            )

            // Mapea el subtotal.

            .ForMember(

                destino => destino.Subtotal,

                opcion => opcion.MapFrom(

                    origen => origen.Subtotal

                )

            )

            // Mapea el impuesto.

            .ForMember(

                destino => destino.Impuesto,

                opcion => opcion.MapFrom(

                    origen => origen.Impuesto

                )

            )

            // Mapea el descuento.

            .ForMember(

                destino => destino.Descuento,

                opcion => opcion.MapFrom(

                    origen => origen.Descuento

                )

            )

            // Mapea el total.

            .ForMember(

                destino => destino.Total,

                opcion => opcion.MapFrom(

                    origen => origen.Total

                )

            )

            // Mapea la dirección.

            .ForMember(

                destino => destino.DireccionEntrega,

                opcion => opcion.MapFrom(

                    origen => origen.DireccionEntrega

                )

            )

            // Mapea el identificador del estado.

            .ForMember(

                destino => destino.IdEstadoPedido,

                opcion => opcion.MapFrom(

                    origen => origen.IdEstadoPedido

                )

            );

        // Convierte TPedido a Pedido.

        CreateMap<TPedido, Pedido>()

            // Mapea el identificador.

            .ForMember(

                destino => destino.IdPedido,

                opcion => opcion.MapFrom(

                    origen => origen.IdPedido

                )

            )

            // Mapea el usuario.

            .ForMember(

                destino => destino.IdUsuario,

                opcion => opcion.MapFrom(

                    origen => origen.IdUsuario

                )

            )

            // Mapea la fecha.

            .ForMember(

                destino => destino.FechaPedido,

                opcion => opcion.MapFrom(

                    origen => origen.FechaPedido

                )

            )

            // Mapea el estado.

            .ForMember(

                destino => destino.Estado,

                opcion => opcion.MapFrom(

                    origen => origen.Estado

                )

            )

            // Mapea el subtotal.

            .ForMember(

                destino => destino.Subtotal,

                opcion => opcion.MapFrom(

                    origen => origen.Subtotal

                )

            )

            // Mapea el impuesto.

            .ForMember(

                destino => destino.Impuesto,

                opcion => opcion.MapFrom(

                    origen => origen.Impuesto

                )

            )

            // Mapea el descuento.

            .ForMember(

                destino => destino.Descuento,

                opcion => opcion.MapFrom(

                    origen => origen.Descuento

                )

            )

            // Mapea el total.

            .ForMember(

                destino => destino.Total,

                opcion => opcion.MapFrom(

                    origen => origen.Total

                )

            )

            // Mapea la dirección.

            .ForMember(

                destino => destino.DireccionEntrega,

                opcion => opcion.MapFrom(

                    origen => origen.DireccionEntrega

                )

            )

            // Mapea el identificador del estado.

            .ForMember(

                destino => destino.IdEstadoPedido,

                opcion => opcion.MapFrom(

                    origen => origen.IdEstadoPedido

                )

            )

            // Ignora los detalles relacionados.

            .ForMember(

                destino => destino.DetallePedidos,

                opcion => opcion.Ignore()

            )

            // Ignora el envío relacionado.

            .ForMember(

                destino => destino.Envio,

                opcion => opcion.Ignore()

            )

            // Ignora la factura.

            .ForMember(

                destino => destino.Factura,

                opcion => opcion.Ignore()

            )

            // Ignora la navegación del estado.

            .ForMember(

                destino => destino.IdEstadoPedidoNavigation,

                opcion => opcion.Ignore()

            )

            // Ignora la navegación del usuario.

            .ForMember(

                destino => destino.IdUsuarioNavigation,

                opcion => opcion.Ignore()

            )

            // Ignora los pagos relacionados.

            .ForMember(

                destino => destino.Pagos,

                opcion => opcion.Ignore()

            );



        // =====================================================
        // DETALLE PEDIDO
        // =====================================================

        // Mapea detalle de pedido.
        CreateMap<TDetallePedido, DetallePedido>()
            .ReverseMap();


        // =====================================================
        // USUARIO
        // =====================================================

        // Mapea usuario.
        CreateMap<TUsuario, Usuario>()
            .ReverseMap();
    }
}