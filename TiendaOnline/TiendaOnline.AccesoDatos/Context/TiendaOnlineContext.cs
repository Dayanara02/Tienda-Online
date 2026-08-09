// Permite utilizar tipos básicos de C#.
using System;

// Permite utilizar colecciones genéricas y diccionarios.
using System.Collections.Generic;

// Permite utilizar Entity Framework Core,
// incluyendo DbContext, DbSet, relaciones y SQL Server.
using Microsoft.EntityFrameworkCore;

// Importa las entidades tipadas que representan vistas SQL.
using TiendaOnline.Dominio.EntidadesTipadas;

// Importa las entidades normales que representan tablas.
using TiendaOnline.Dominio.Entidades;


// Define el espacio de nombres del contexto de acceso a datos.
namespace TiendaOnline.AccesoDatos.Context;


// Esta clase representa la conexión entre Entity Framework
// y la base de datos TiendaOnline.
public partial class TiendaOnlineContext : DbContext
{
    // Constructor vacío del contexto.
    public TiendaOnlineContext()
    {
    }


    // Constructor que recibe las opciones configuradas
    // desde Program.cs mediante inyección de dependencias.
    public TiendaOnlineContext(
        DbContextOptions<TiendaOnlineContext> options)
        : base(options)
    {
    }


    // Cada DbSet representa una tabla o vista
    // disponible dentro de la base de datos.

    // Tabla BitacoraSistema.
    public virtual DbSet<BitacoraSistema> BitacoraSistemas { get; set; }

    // Tabla Carrito.
    public virtual DbSet<Carrito> Carritos { get; set; }

    // Tabla Categoria.
    public virtual DbSet<Categorium> Categoria { get; set; }

    // Tabla CompraProveedor.
    public virtual DbSet<CompraProveedor> CompraProveedors { get; set; }

    // Tabla Descuento.
    public virtual DbSet<Descuento> Descuentos { get; set; }

    // Tabla DetalleCarrito.
    public virtual DbSet<DetalleCarrito> DetalleCarritos { get; set; }

    // Tabla DetalleCompraProveedor.
    public virtual DbSet<DetalleCompraProveedor> DetalleCompraProveedors { get; set; }

    // Tabla DetalleListaDeseo.
    public virtual DbSet<DetalleListaDeseo> DetalleListaDeseos { get; set; }

    // Tabla DetallePedido.
    public virtual DbSet<DetallePedido> DetallePedidos { get; set; }

    // Tabla DetalleProforma.
    public virtual DbSet<DetalleProforma> DetalleProformas { get; set; }

    // Tabla DireccionUsuario.
    public virtual DbSet<DireccionUsuario> DireccionUsuarios { get; set; }

    // Tabla Envio.
    public virtual DbSet<Envio> Envios { get; set; }

    // Tabla EstadoPago.
    public virtual DbSet<EstadoPago> EstadoPagos { get; set; }

    // Tabla EstadoPedido.
    public virtual DbSet<EstadoPedido> EstadoPedidos { get; set; }

    // Tabla EvaluacionProducto.
    public virtual DbSet<EvaluacionProducto> EvaluacionProductos { get; set; }

    // Tabla Factura.
    public virtual DbSet<Factura> Facturas { get; set; }

    // Tabla FamiliaProducto.
    public virtual DbSet<FamiliaProducto> FamiliaProductos { get; set; }

    // Tabla HistorialAcceso.
    public virtual DbSet<HistorialAcceso> HistorialAccesos { get; set; }

    // Tabla Impuesto.
    public virtual DbSet<Impuesto> Impuestos { get; set; }

    // Tabla Inventario.
    public virtual DbSet<Inventario> Inventarios { get; set; }

    // Tabla ListaDeseo.
    public virtual DbSet<ListaDeseo> ListaDeseos { get; set; }

    // Tabla MetodoPago.
    public virtual DbSet<MetodoPago> MetodoPagos { get; set; }

    // Tabla MovimientoInventario.
    public virtual DbSet<MovimientoInventario> MovimientoInventarios { get; set; }

    // Tabla Notificacion.
    public virtual DbSet<Notificacion> Notificacions { get; set; }

    // Tabla Pago.
    public virtual DbSet<Pago> Pagos { get; set; }

    // Tabla Pedido.
    public virtual DbSet<Pedido> Pedidos { get; set; }

    // Tabla Producto.
    public virtual DbSet<Producto> Productos { get; set; }

    // Tabla ProductoProveedor.
    public virtual DbSet<ProductoProveedor> ProductoProveedors { get; set; }

    // Tabla Proforma.
    public virtual DbSet<Proforma> Proformas { get; set; }

    // Tabla Proveedor.
    public virtual DbSet<Proveedor> Proveedors { get; set; }

    // Tabla Rol.
    public virtual DbSet<Rol> Rols { get; set; }

    // Tabla Usuario.
    public virtual DbSet<Usuario> Usuarios { get; set; }


    // Vistas de SQL Server.

    // Vista del catálogo de productos.
    public virtual DbSet<VwCatalogoProducto> VwCatalogoProductos { get; set; }

    // Vista de productos más vendidos.
    public virtual DbSet<VwProductosMasVendido> VwProductosMasVendidos { get; set; }

    // Vista de productos con stock bajo.
    public virtual DbSet<VwProductosStockBajo> VwProductosStockBajos { get; set; }

    // Vista de ventas agrupadas por fecha.
    public virtual DbSet<VwVentasPorFecha> VwVentasPorFechas { get; set; }


    // Configura la conexión con SQL Server
    // cuando no se recibe una configuración externa.
    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)

#warning To protect potentially sensitive information in your connection string, you should move it out of source code.

        // Conecta Entity Framework con SQL Server.
        => optionsBuilder.UseSqlServer(
            "Server=DAYANARA\\SQLEXPRESS;" +
            "Initial Catalog=TiendaOnline;" +
            "Trusted_Connection=True;" +
            "TrustServerCertificate=True;"
        );


    // Configura las entidades, claves, relaciones,
    // tamaños de campos y restricciones de la base de datos.
    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        // =========================================================
        // BITACORA SISTEMA
        // =========================================================

        modelBuilder.Entity<BitacoraSistema>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdBitacora)
                .HasName("PK__Bitacora__ED3A1B13E5ABE20F");

            // Define el nombre de la tabla.
            entity.ToTable("BitacoraSistema");

            // Configura el campo Accion.
            entity.Property(e => e.Accion)
                .HasMaxLength(100)
                .IsUnicode(false);

            // Configura el campo Descripcion.
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false);

            // Configura la fecha automática.
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            // Configura TablaAfectada.
            entity.Property(e => e.TablaAfectada)
                .HasMaxLength(100)
                .IsUnicode(false);

            // Relación entre BitacoraSistema y Usuario.
            entity.HasOne(d => d.IdUsuarioNavigation)
                .WithMany(p => p.BitacoraSistemas)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bitacora_Usuario");
        });


        // =========================================================
        // CARRITO
        // =========================================================

        modelBuilder.Entity<Carrito>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdCarrito)
                .HasName("PK__Carrito__8B4A618C0A50748D");

            // Define el nombre de la tabla.
            entity.ToTable("Carrito");

            // Configura Estado.
            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Activo");

            // Configura FechaCreacion.
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            // Relación Carrito - Usuario.
            entity.HasOne(d => d.IdUsuarioNavigation)
                .WithMany(p => p.Carritos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Carrito_Usuario");
        });


        // =========================================================
        // CATEGORIA
        // =========================================================

        modelBuilder.Entity<Categorium>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdCategoria)
                .HasName("PK__Categori__A3C02A102CFC9629");

            // Evita categorías duplicadas
            // dentro de la misma familia.
            entity.HasIndex(
                e => new
                {
                    e.IdFamilia,
                    e.Nombre
                },
                "UQ_Categoria_Familia_Nombre"
            )
            .IsUnique();

            // Configura Descripcion.
            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .IsUnicode(false);

            // Configura Estado.
            entity.Property(e => e.Estado)
                .HasDefaultValue(true);

            // Configura Nombre.
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);

            // Relación Categoria - FamiliaProducto.
            entity.HasOne(d => d.IdFamiliaNavigation)
                .WithMany(p => p.Categoria)
                .HasForeignKey(d => d.IdFamilia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Categoria_Familia");

            // Relación muchos a muchos
            // entre Categoria y Descuento.
            entity.HasMany(d => d.IdDescuentos)
                .WithMany(p => p.IdCategoria)
                .UsingEntity<Dictionary<string, object>>(
                    "CategoriaDescuento",

                    r => r.HasOne<Descuento>()
                        .WithMany()
                        .HasForeignKey("IdDescuento")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName(
                            "FK_CategoriaDescuento_Descuento"
                        ),

                    l => l.HasOne<Categorium>()
                        .WithMany()
                        .HasForeignKey("IdCategoria")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName(
                            "FK_CategoriaDescuento_Categoria"
                        ),

                    j =>
                    {
                        // Define la clave compuesta.
                        j.HasKey(
                            "IdCategoria",
                            "IdDescuento"
                        )
                        .HasName(
                            "PK__Categori__027DB6A15ABC1DA4"
                        );

                        // Define la tabla puente.
                        j.ToTable("CategoriaDescuento");
                    }
                );
        });


        // =========================================================
        // COMPRA PROVEEDOR
        // =========================================================

        modelBuilder.Entity<CompraProveedor>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdCompraProveedor)
                .HasName("PK__CompraPr__83490C249C03EE4A");

            // Define la tabla.
            entity.ToTable("CompraProveedor");

            // Configura Estado.
            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Pendiente");

            // Configura FechaCompra.
            entity.Property(e => e.FechaCompra)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            // Configura campos monetarios.
            entity.Property(e => e.Impuesto)
                .HasColumnType("decimal(12, 2)");

            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(12, 2)");

            entity.Property(e => e.Total)
                .HasColumnType("decimal(12, 2)");

            // Relación CompraProveedor - Proveedor.
            entity.HasOne(d => d.IdProveedorNavigation)
                .WithMany(p => p.CompraProveedors)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_CompraProveedor_Proveedor"
                );

            // Relación CompraProveedor - Usuario.
            entity.HasOne(d => d.IdUsuarioNavigation)
                .WithMany(p => p.CompraProveedors)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_CompraProveedor_Usuario"
                );
        });


        // =========================================================
        // DESCUENTO
        // =========================================================

        modelBuilder.Entity<Descuento>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdDescuento)
                .HasName("PK__Descuent__1BD9CB1B6C5C23B3");

            // Define la tabla.
            entity.ToTable("Descuento");

            // Configura Descripcion.
            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .IsUnicode(false);

            // Configura Estado.
            entity.Property(e => e.Estado)
                .HasDefaultValue(true);

            // Configura Nombre.
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);

            // Configura Porcentaje.
            entity.Property(e => e.Porcentaje)
                .HasColumnType("decimal(5, 2)");
        });


        // =========================================================
        // DETALLE CARRITO
        // =========================================================

        modelBuilder.Entity<DetalleCarrito>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdDetalleCarrito)
                .HasName("PK__DetalleC__27A5F83BC2D6C32E");

            // Define la tabla.
            entity.ToTable("DetalleCarrito");

            // Evita repetir el mismo producto
            // en el mismo carrito.
            entity.HasIndex(
                e => new
                {
                    e.IdCarrito,
                    e.IdProducto
                },
                "UQ_DetalleCarrito"
            )
            .IsUnique();

            // Configura PrecioUnitario.
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(12, 2)");

            // Relación DetalleCarrito - Carrito.
            entity.HasOne(d => d.IdCarritoNavigation)
                .WithMany(p => p.DetalleCarritos)
                .HasForeignKey(d => d.IdCarrito)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_DetalleCarrito_Carrito"
                );

            // Relación DetalleCarrito - Producto.
            entity.HasOne(d => d.IdProductoNavigation)
                .WithMany(p => p.DetalleCarritos)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_DetalleCarrito_Producto"
                );
        });


        // =========================================================
        // DETALLE COMPRA PROVEEDOR
        // =========================================================

        modelBuilder.Entity<DetalleCompraProveedor>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdDetalleCompra)
                .HasName("PK__DetalleC__E046CCBBC1702FAA");

            // Define la tabla.
            entity.ToTable("DetalleCompraProveedor");

            // Configura PrecioUnitario.
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(12, 2)");

            // Configura Subtotal.
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(12, 2)");

            // Relación con CompraProveedor.
            entity.HasOne(d => d.IdCompraProveedorNavigation)
                .WithMany(p => p.DetalleCompraProveedors)
                .HasForeignKey(d => d.IdCompraProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_DetalleCompra_Compra"
                );

            // Relación con Producto.
            entity.HasOne(d => d.IdProductoNavigation)
                .WithMany(p => p.DetalleCompraProveedors)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_DetalleCompra_Producto"
                );
        });


        // =========================================================
        // DETALLE LISTA DESEO
        // =========================================================

        modelBuilder.Entity<DetalleListaDeseo>(entity =>
        {
            // Define una clave primaria compuesta.
            entity.HasKey(
                e => new
                {
                    e.IdListaDeseos,
                    e.IdProducto
                }
            )
            .HasName(
                "PK__DetalleL__0ABCEFCF57AA0859"
            );

            // Configura FechaAgregado.
            entity.Property(e => e.FechaAgregado)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            // Relación con ListaDeseo.
            entity.HasOne(d => d.IdListaDeseosNavigation)
                .WithMany(p => p.DetalleListaDeseos)
                .HasForeignKey(d => d.IdListaDeseos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_DetalleListaDeseos_Lista"
                );

            // Relación con Producto.
            entity.HasOne(d => d.IdProductoNavigation)
                .WithMany(p => p.DetalleListaDeseos)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_DetalleListaDeseos_Producto"
                );
        });


        // =========================================================
        // DETALLE PEDIDO
        // =========================================================

        modelBuilder.Entity<DetallePedido>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdDetallePedido)
                .HasName("PK__DetalleP__48AFFD95B1411977");

            // Define la tabla.
            entity.ToTable("DetallePedido");

            // Configura campos monetarios.
            entity.Property(e => e.Descuento)
                .HasColumnType("decimal(12, 2)");

            entity.Property(e => e.Impuesto)
                .HasColumnType("decimal(12, 2)");

            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(12, 2)");

            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(12, 2)");

            // Relación DetallePedido - Pedido.
            entity.HasOne(d => d.IdPedidoNavigation)
                .WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.IdPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_DetallePedido_Pedido"
                );

            // Relación DetallePedido - Producto.
            entity.HasOne(d => d.IdProductoNavigation)
                .WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_DetallePedido_Producto"
                );
        });


        // =========================================================
        // DETALLE PROFORMA
        // =========================================================

        modelBuilder.Entity<DetalleProforma>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdDetalleProforma)
                .HasName("PK__DetalleP__AF9413BA5D645AA1");

            // Define la tabla.
            entity.ToTable("DetalleProforma");

            // Configura los campos monetarios.
            entity.Property(e => e.Descuento)
                .HasColumnType("decimal(12, 2)");

            entity.Property(e => e.Impuesto)
                .HasColumnType("decimal(12, 2)");

            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(12, 2)");

            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(12, 2)");

            // Relación con Producto.
            entity.HasOne(d => d.IdProductoNavigation)
                .WithMany(p => p.DetalleProformas)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_DetalleProforma_Producto"
                );

            // Relación con Proforma.
            entity.HasOne(d => d.IdProformaNavigation)
                .WithMany(p => p.DetalleProformas)
                .HasForeignKey(d => d.IdProforma)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_DetalleProforma_Proforma"
                );
        });


        // =========================================================
        // DIRECCION USUARIO
        // =========================================================

        modelBuilder.Entity<DireccionUsuario>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdDireccion)
                .HasName("PK__Direccio__1F8E0C769F6F2CE4");

            // Define la tabla.
            entity.ToTable("DireccionUsuario");

            // Configura Canton.
            entity.Property(e => e.Canton)
                .HasMaxLength(100)
                .IsUnicode(false);

            // Configura CodigoPostal.
            entity.Property(e => e.CodigoPostal)
                .HasMaxLength(20)
                .IsUnicode(false);

            // Configura DireccionExacta.
            entity.Property(e => e.DireccionExacta)
                .HasMaxLength(300)
                .IsUnicode(false);

            // Configura Distrito.
            entity.Property(e => e.Distrito)
                .HasMaxLength(100)
                .IsUnicode(false);

            // Configura Estado.
            entity.Property(e => e.Estado)
                .HasDefaultValue(true);

            // Configura Provincia.
            entity.Property(e => e.Provincia)
                .HasMaxLength(100)
                .IsUnicode(false);

            // Relación con Usuario.
            entity.HasOne(d => d.IdUsuarioNavigation)
                .WithMany(p => p.DireccionUsuarios)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_DireccionUsuario_Usuario"
                );
        });


        // =========================================================
        // ENVIO
        // =========================================================

        modelBuilder.Entity<Envio>(entity =>
        {
            // Define IdEnvio como clave primaria.
            entity.HasKey(e => e.IdEnvio)
                .HasName("PK__Envio__B814A62E849B43A4");

            // Define el nombre de la tabla.
            entity.ToTable("Envio");

            // Hace que IdPedido sea único.
            // Esto permite que un Pedido tenga como máximo un Envio.
            entity.HasIndex(
                e => e.IdPedido,
                "UQ__Envio__9D335DC2EBFB49C5"
            )
            .IsUnique();

            // Configura EmpresaEnvio.
            entity.Property(e => e.EmpresaEnvio)
                .HasMaxLength(150)
                .IsUnicode(false);

            // Configura Estado.
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Pendiente");

            // Configura FechaEntrega.
            entity.Property(e => e.FechaEntrega)
                .HasColumnType("datetime");

            // Configura FechaEnvio.
            entity.Property(e => e.FechaEnvio)
                .HasColumnType("datetime");

            // Configura NumeroSeguimiento.
            entity.Property(e => e.NumeroSeguimiento)
                .HasMaxLength(100)
                .IsUnicode(false);


            // -----------------------------------------------------
            // RELACIÓN ENVIO - DIRECCION
            // -----------------------------------------------------

            // Un envío tiene una dirección.
            // Una dirección puede utilizarse en varios envíos.
            entity.HasOne(
                    d => d.IdDireccionNavigation
                )
                .WithMany(
                    p => p.Envios
                )
                .HasForeignKey(
                    d => d.IdDireccion
                )
                .OnDelete(
                    DeleteBehavior.ClientSetNull
                )
                .HasConstraintName(
                    "FK_Envio_Direccion"
                );


            // -----------------------------------------------------
            // RELACIÓN UNO A UNO ENVIO - PEDIDO
            // -----------------------------------------------------

            // Envio es la entidad dependiente.
            // Pedido es la entidad principal.
            entity.HasOne(
                    e => e.IdPedidoNavigation
                )

                // Indica la navegación inversa desde Pedido.
                .WithOne(
                    p => p.Envio
                )

                // Especifica claramente que Envio.IdPedido
                // es la llave foránea de la relación.
                .HasForeignKey<Envio>(
                    e => e.IdPedido
                )

                // Indica explícitamente que Pedido.IdPedido
                // es la clave principal que recibe la relación.
                .HasPrincipalKey<Pedido>(
                    p => p.IdPedido
                )

                // Como IdPedido no acepta null,
                // la relación es obligatoria para Envio.
                .IsRequired()

                // Mantiene el comportamiento existente
                // cuando se intenta eliminar el principal.
                .OnDelete(
                    DeleteBehavior.ClientSetNull
                )

                // Mantiene el nombre de la FK existente en SQL Server.
                .HasConstraintName(
                    "FK_Envio_Pedido"
                );
        });


        // =========================================================
        // ESTADO PAGO
        // =========================================================

        modelBuilder.Entity<EstadoPago>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdEstadoPago)
                .HasName("PK__EstadoPa__540F03E93C64D911");

            // Define la tabla.
            entity.ToTable("EstadoPago");

            // Nombre debe ser único.
            entity.HasIndex(
                e => e.Nombre,
                "UQ__EstadoPa__75E3EFCFB7C3A4C8"
            )
            .IsUnique();

            // Configura Descripcion.
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);

            // Configura Estado.
            entity.Property(e => e.Estado)
                .HasDefaultValue(true);

            // Configura Nombre.
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });


        // =========================================================
        // ESTADO PEDIDO
        // =========================================================

        modelBuilder.Entity<EstadoPedido>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdEstadoPedido)
                .HasName("PK__EstadoPe__86B98371DA12C497");

            // Define la tabla.
            entity.ToTable("EstadoPedido");

            // Evita nombres duplicados.
            entity.HasIndex(
                e => e.Nombre,
                "UQ__EstadoPe__75E3EFCF1BB0A861"
            )
            .IsUnique();

            // Configura Descripcion.
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);

            // Configura Estado.
            entity.Property(e => e.Estado)
                .HasDefaultValue(true);

            // Configura Nombre.
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });


        // =========================================================
        // EVALUACION PRODUCTO
        // =========================================================

        modelBuilder.Entity<EvaluacionProducto>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdEvaluacion)
                .HasName("PK__Evaluaci__A7EA657C5C48E026");

            // Define la tabla.
            entity.ToTable("EvaluacionProducto");

            // Un usuario solo puede evaluar una vez
            // el mismo producto.
            entity.HasIndex(
                e => new
                {
                    e.IdUsuario,
                    e.IdProducto
                },
                "UQ_Evaluacion_Usuario_Producto"
            )
            .IsUnique();

            // Configura Comentario.
            entity.Property(e => e.Comentario)
                .HasMaxLength(500)
                .IsUnicode(false);

            // Configura Estado.
            entity.Property(e => e.Estado)
                .HasDefaultValue(true);

            // Configura FechaEvaluacion.
            entity.Property(e => e.FechaEvaluacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            // Relación con Producto.
            entity.HasOne(d => d.IdProductoNavigation)
                .WithMany(p => p.EvaluacionProductos)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_Evaluacion_Producto"
                );

            // Relación con Usuario.
            entity.HasOne(d => d.IdUsuarioNavigation)
                .WithMany(p => p.EvaluacionProductos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_Evaluacion_Usuario"
                );
        });


        // =========================================================
        // FACTURA
        // =========================================================

        modelBuilder.Entity<Factura>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdFactura)
                .HasName("PK__Factura__50E7BAF1F40535ED");

            // Define la tabla.
            entity.ToTable("Factura");

            // Un pedido solamente puede tener una factura.
            entity.HasIndex(
                e => e.IdPedido,
                "UQ__Factura__9D335DC2439E741C"
            )
            .IsUnique();

            // NumeroFactura debe ser único.
            entity.HasIndex(
                e => e.NumeroFactura,
                "UQ__Factura__CF12F9A6645D9C76"
            )
            .IsUnique();

            // Configura campos monetarios.
            entity.Property(e => e.Descuento)
                .HasColumnType("decimal(12, 2)");

            // Configura FechaEmision.
            entity.Property(e => e.FechaEmision)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.Impuesto)
                .HasColumnType("decimal(12, 2)");

            // Configura NumeroFactura.
            entity.Property(e => e.NumeroFactura)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(12, 2)");

            entity.Property(e => e.Total)
                .HasColumnType("decimal(12, 2)");

            // Configura la dirección del archivo PDF.
            entity.Property(e => e.UrlPdf)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("UrlPDF");

            // Relación uno a uno Factura - Pedido.
            entity.HasOne(d => d.IdPedidoNavigation)
                .WithOne(p => p.Factura)
                .HasForeignKey<Factura>(d => d.IdPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_Factura_Pedido"
                );
        });


        // =========================================================
        // FAMILIA PRODUCTO
        // =========================================================

        modelBuilder.Entity<FamiliaProducto>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdFamilia)
                .HasName("PK__FamiliaP__751F80CF41522F19");

            // Define la tabla.
            entity.ToTable("FamiliaProducto");

            // Nombre debe ser único.
            entity.HasIndex(
                e => e.Nombre,
                "UQ__FamiliaP__75E3EFCF0D2B12AF"
            )
            .IsUnique();

            // Configura Descripcion.
            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .IsUnicode(false);

            // Configura Estado.
            entity.Property(e => e.Estado)
                .HasDefaultValue(true);

            // Configura Nombre.
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);

            // Relación muchos a muchos
            // entre FamiliaProducto y Descuento.
            entity.HasMany(d => d.IdDescuentos)
                .WithMany(p => p.IdFamilia)
                .UsingEntity<Dictionary<string, object>>(
                    "FamiliaDescuento",

                    r => r.HasOne<Descuento>()
                        .WithMany()
                        .HasForeignKey("IdDescuento")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName(
                            "FK_FamiliaDescuento_Descuento"
                        ),

                    l => l.HasOne<FamiliaProducto>()
                        .WithMany()
                        .HasForeignKey("IdFamilia")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName(
                            "FK_FamiliaDescuento_Familia"
                        ),

                    j =>
                    {
                        // Define la clave compuesta.
                        j.HasKey(
                            "IdFamilia",
                            "IdDescuento"
                        )
                        .HasName(
                            "PK__FamiliaD__D4A21C7E3E210393"
                        );

                        // Define la tabla puente.
                        j.ToTable("FamiliaDescuento");
                    }
                );
        });


        // =========================================================
        // HISTORIAL ACCESO
        // =========================================================

        modelBuilder.Entity<HistorialAcceso>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdHistorialAcceso)
                .HasName("PK__Historia__5EC8FB766D6C6AA4");

            // Define la tabla.
            entity.ToTable("HistorialAcceso");

            // Configura DireccionIP.
            entity.Property(e => e.DireccionIp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DireccionIP");

            // Configura FechaAcceso.
            entity.Property(e => e.FechaAcceso)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            // Relación con Usuario.
            entity.HasOne(d => d.IdUsuarioNavigation)
                .WithMany(p => p.HistorialAccesos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_HistorialAcceso_Usuario"
                );
        });


        // =========================================================
        // IMPUESTO
        // =========================================================

        modelBuilder.Entity<Impuesto>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdImpuesto)
                .HasName("PK__Impuesto__A9B88928350A869C");

            // Define la tabla.
            entity.ToTable("Impuesto");

            // Configura Estado.
            entity.Property(e => e.Estado)
                .HasDefaultValue(true);

            // Configura Nombre.
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);

            // Configura Porcentaje.
            entity.Property(e => e.Porcentaje)
                .HasColumnType("decimal(5, 2)");
        });


        // =========================================================
        // INVENTARIO
        // =========================================================

        modelBuilder.Entity<Inventario>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdInventario)
                .HasName("PK__Inventar__1927B20CFD7B7B9E");

            // Define la tabla.
            entity.ToTable("Inventario");

            // Cada producto tiene un único inventario.
            entity.HasIndex(
                e => e.IdProducto,
                "UQ__Inventar__09889211666847F6"
            )
            .IsUnique();

            // Configura FechaActualizacion.
            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            // Relación uno a uno Inventario - Producto.
            entity.HasOne(d => d.IdProductoNavigation)
                .WithOne(p => p.Inventario)
                .HasForeignKey<Inventario>(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_Inventario_Producto"
                );
        });


        // =========================================================
        // LISTA DESEO
        // =========================================================

        modelBuilder.Entity<ListaDeseo>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdListaDeseos)
                .HasName("PK__ListaDes__1A2466EE44568A04");

            // Cada usuario tiene una sola lista de deseos.
            entity.HasIndex(
                e => e.IdUsuario,
                "UQ_ListaDeseos_Usuario"
            )
            .IsUnique();

            // Configura FechaCreacion.
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            // Relación uno a uno ListaDeseo - Usuario.
            entity.HasOne(d => d.IdUsuarioNavigation)
                .WithOne(p => p.ListaDeseo)
                .HasForeignKey<ListaDeseo>(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_ListaDeseos_Usuario"
                );
        });


        // =========================================================
        // METODO PAGO
        // =========================================================

        modelBuilder.Entity<MetodoPago>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdMetodoPago)
                .HasName("PK__MetodoPa__6F49A9BEE582A2C1");

            // Define la tabla.
            entity.ToTable("MetodoPago");

            // Evita nombres duplicados.
            entity.HasIndex(
                e => e.Nombre,
                "UQ__MetodoPa__75E3EFCF63A345EE"
            )
            .IsUnique();

            // Configura Descripcion.
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);

            // Configura Estado.
            entity.Property(e => e.Estado)
                .HasDefaultValue(true);

            // Configura Nombre.
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });


        // =========================================================
        // MOVIMIENTO INVENTARIO
        // =========================================================

        modelBuilder.Entity<MovimientoInventario>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdMovimiento)
                .HasName("PK__Movimien__881A6AE09E3470EA");

            // Define la tabla.
            entity.ToTable("MovimientoInventario");

            // Configura FechaMovimiento.
            entity.Property(e => e.FechaMovimiento)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            // Configura Motivo.
            entity.Property(e => e.Motivo)
                .HasMaxLength(300)
                .IsUnicode(false);

            // Configura TipoMovimiento.
            entity.Property(e => e.TipoMovimiento)
                .HasMaxLength(30)
                .IsUnicode(false);

            // Relación con Inventario.
            entity.HasOne(d => d.IdInventarioNavigation)
                .WithMany(p => p.MovimientoInventarios)
                .HasForeignKey(d => d.IdInventario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_MovimientoInventario_Inventario"
                );

            // Relación con Usuario.
            entity.HasOne(d => d.IdUsuarioNavigation)
                .WithMany(p => p.MovimientoInventarios)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_MovimientoInventario_Usuario"
                );
        });


        // =========================================================
        // NOTIFICACION
        // =========================================================

        modelBuilder.Entity<Notificacion>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdNotificacion)
                .HasName("PK__Notifica__F6CA0A8594CBDA73");

            // Define la tabla.
            entity.ToTable("Notificacion");

            // Configura Estado.
            entity.Property(e => e.Estado)
                .HasDefaultValue(true);

            // Configura FechaCreacion.
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            // Configura Mensaje.
            entity.Property(e => e.Mensaje)
                .HasMaxLength(500)
                .IsUnicode(false);

            // Configura Tipo.
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false);

            // Configura Titulo.
            entity.Property(e => e.Titulo)
                .HasMaxLength(150)
                .IsUnicode(false);

            // Relación con Usuario.
            entity.HasOne(d => d.IdUsuarioNavigation)
                .WithMany(p => p.Notificacions)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_Notificacion_Usuario"
                );
        });


        // =========================================================
        // PAGO
        // =========================================================

        modelBuilder.Entity<Pago>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdPago)
                .HasName("PK__Pago__FC851A3AD9130F16");

            // Define la tabla.
            entity.ToTable("Pago");

            // Configura Estado.
            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Pendiente");

            // Configura FechaPago.
            entity.Property(e => e.FechaPago)
                .HasColumnType("datetime");

            // Configura MetodoPago.
            entity.Property(e => e.MetodoPago)
                .HasMaxLength(50)
                .IsUnicode(false);

            // Configura Monto.
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(12, 2)");

            // Configura Referencia.
            entity.Property(e => e.Referencia)
                .HasMaxLength(100)
                .IsUnicode(false);

            // Relación Pago - EstadoPago.
            entity.HasOne(d => d.IdEstadoPagoNavigation)
                .WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdEstadoPago)
                .HasConstraintName(
                    "FK_Pago_EstadoPago"
                );

            // Relación Pago - MetodoPago.
            entity.HasOne(d => d.IdMetodoPagoNavigation)
                .WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdMetodoPago)
                .HasConstraintName(
                    "FK_Pago_MetodoPago"
                );

            // Relación Pago - Pedido.
            entity.HasOne(d => d.IdPedidoNavigation)
                .WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_Pago_Pedido"
                );
        });


        // =========================================================
        // PEDIDO
        // =========================================================

        modelBuilder.Entity<Pedido>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdPedido)
                .HasName("PK__Pedido__9D335DC3C05578FB");

            // Define la tabla.
            entity.ToTable("Pedido");

            // Configura campos monetarios.
            entity.Property(e => e.Descuento)
                .HasColumnType("decimal(12, 2)");

            // Configura DireccionEntrega.
            entity.Property(e => e.DireccionEntrega)
                .HasMaxLength(300)
                .IsUnicode(false);

            // Configura Estado.
            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Pendiente");

            // Configura FechaPedido.
            entity.Property(e => e.FechaPedido)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            // Configura Impuesto.
            entity.Property(e => e.Impuesto)
                .HasColumnType("decimal(12, 2)");

            // Configura Subtotal.
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(12, 2)");

            // Configura Total.
            entity.Property(e => e.Total)
                .HasColumnType("decimal(12, 2)");

            // Relación Pedido - EstadoPedido.
            entity.HasOne(d => d.IdEstadoPedidoNavigation)
                .WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.IdEstadoPedido)
                .HasConstraintName(
                    "FK_Pedido_EstadoPedido"
                );

            // Relación Pedido - Usuario.
            entity.HasOne(d => d.IdUsuarioNavigation)
                .WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_Pedido_Usuario"
                );
        });


        // =========================================================
        // PRODUCTO
        // =========================================================

        modelBuilder.Entity<Producto>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdProducto)
                .HasName("PK__Producto__09889210EA6E46CF");

            // Define la tabla.
            entity.ToTable("Producto");

            // Codigo debe ser único.
            entity.HasIndex(
                e => e.Codigo,
                "UQ__Producto__06370DACB4734ED1"
            )
            .IsUnique();

            // Configura Codigo.
            entity.Property(e => e.Codigo)
                .HasMaxLength(50)
                .IsUnicode(false);

            // Configura Costo.
            entity.Property(e => e.Costo)
                .HasColumnType("decimal(12, 2)");

            // Configura Descripcion.
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false);

            // Configura Estado.
            entity.Property(e => e.Estado)
                .HasDefaultValue(true);

            // Configura FechaRegistro.
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            // Configura Imagen.
            entity.Property(e => e.Imagen)
                .HasMaxLength(500)
                .IsUnicode(false);

            // Configura Nombre.
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);

            // Configura Precio.
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(12, 2)");

            // Configura StockMinimo.
            entity.Property(e => e.StockMinimo)
                .HasDefaultValue(5);

            // Relación Producto - Categoria.
            entity.HasOne(d => d.IdCategoriaNavigation)
                .WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_Producto_Categoria"
                );

            // Relación Producto - Impuesto.
            entity.HasOne(d => d.IdImpuestoNavigation)
                .WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdImpuesto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_Producto_Impuesto"
                );

            // Relación muchos a muchos
            // entre Producto y Descuento.
            entity.HasMany(d => d.IdDescuentos)
                .WithMany(p => p.IdProductos)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductoDescuento",

                    r => r.HasOne<Descuento>()
                        .WithMany()
                        .HasForeignKey("IdDescuento")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName(
                            "FK_ProductoDescuento_Descuento"
                        ),

                    l => l.HasOne<Producto>()
                        .WithMany()
                        .HasForeignKey("IdProducto")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName(
                            "FK_ProductoDescuento_Producto"
                        ),

                    j =>
                    {
                        // Define la clave primaria compuesta.
                        j.HasKey(
                            "IdProducto",
                            "IdDescuento"
                        )
                        .HasName(
                            "PK__Producto__A8350EA1778CFC75"
                        );

                        // Define la tabla puente.
                        j.ToTable("ProductoDescuento");
                    }
                );
        });


        // =========================================================
        // PRODUCTO PROVEEDOR
        // =========================================================

        modelBuilder.Entity<ProductoProveedor>(entity =>
        {
            // Define la clave primaria compuesta.
            entity.HasKey(
                e => new
                {
                    e.IdProducto,
                    e.IdProveedor
                }
            )
            .HasName(
                "PK__Producto__E703F10AEE696E77"
            );

            // Define la tabla.
            entity.ToTable("ProductoProveedor");

            // Configura CodigoProveedor.
            entity.Property(e => e.CodigoProveedor)
                .HasMaxLength(100)
                .IsUnicode(false);

            // Configura Estado.
            entity.Property(e => e.Estado)
                .HasDefaultValue(true);

            // Configura PrecioCompra.
            entity.Property(e => e.PrecioCompra)
                .HasColumnType("decimal(12, 2)");

            // Relación con Producto.
            entity.HasOne(d => d.IdProductoNavigation)
                .WithMany(p => p.ProductoProveedors)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_ProductoProveedor_Producto"
                );

            // Relación con Proveedor.
            entity.HasOne(d => d.IdProveedorNavigation)
                .WithMany(p => p.ProductoProveedors)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_ProductoProveedor_Proveedor"
                );
        });


        // =========================================================
        // PROFORMA
        // =========================================================

        modelBuilder.Entity<Proforma>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdProforma)
                .HasName("PK__Proforma__6731B48A086CC9A4");

            // Define la tabla.
            entity.ToTable("Proforma");

            // Configura Descuento.
            entity.Property(e => e.Descuento)
                .HasColumnType("decimal(12, 2)");

            // Configura Estado.
            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Pendiente");

            // Configura FechaCreacion.
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            // Configura campos monetarios.
            entity.Property(e => e.Impuesto)
                .HasColumnType("decimal(12, 2)");

            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(12, 2)");

            entity.Property(e => e.Total)
                .HasColumnType("decimal(12, 2)");

            // Configura UrlPDF.
            entity.Property(e => e.UrlPdf)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("UrlPDF");

            // Relación Proforma - Direccion.
            entity.HasOne(d => d.IdDireccionNavigation)
                .WithMany(p => p.Proformas)
                .HasForeignKey(d => d.IdDireccion)
                .HasConstraintName(
                    "FK_Proforma_Direccion"
                );

            // Relación Proforma - Usuario.
            entity.HasOne(d => d.IdUsuarioNavigation)
                .WithMany(p => p.Proformas)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_Proforma_Usuario"
                );
        });


        // =========================================================
        // PROVEEDOR
        // =========================================================

        modelBuilder.Entity<Proveedor>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdProveedor)
                .HasName("PK__Proveedo__E8B631AF77F95480");

            // Define la tabla.
            entity.ToTable("Proveedor");

            // Identificacion debe ser única.
            entity.HasIndex(
                e => e.Identificacion,
                "UQ__Proveedo__D6F931E5CCF5F21B"
            )
            .IsUnique();

            // Configura Correo.
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .IsUnicode(false);

            // Configura Direccion.
            entity.Property(e => e.Direccion)
                .HasMaxLength(300)
                .IsUnicode(false);

            // Configura Estado.
            entity.Property(e => e.Estado)
                .HasDefaultValue(true);

            // Configura Identificacion.
            entity.Property(e => e.Identificacion)
                .HasMaxLength(50)
                .IsUnicode(false);

            // Configura Nombre.
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);

            // Configura Telefono.
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false);
        });


        // =========================================================
        // ROL
        // =========================================================

        modelBuilder.Entity<Rol>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdRol)
                .HasName("PK__Rol__2A49584CE88CC92E");

            // Define la tabla.
            entity.ToTable("Rol");

            // Nombre debe ser único.
            entity.HasIndex(
                e => e.Nombre,
                "UQ__Rol__75E3EFCFAA09AD13"
            )
            .IsUnique();

            // Configura Descripcion.
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);

            // Configura Estado.
            entity.Property(e => e.Estado)
                .HasDefaultValue(true);

            // Configura Nombre.
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });


        // =========================================================
        // USUARIO
        // =========================================================

        modelBuilder.Entity<Usuario>(entity =>
        {
            // Define la clave primaria.
            entity.HasKey(e => e.IdUsuario)
                .HasName("PK__Usuario__5B65BF9701E21C52");

            // Define la tabla.
            entity.ToTable("Usuario");

            // Correo debe ser único.
            entity.HasIndex(
                e => e.Correo,
                "UQ__Usuario__60695A193BA4E085"
            )
            .IsUnique();

            // Configura Apellido.
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .IsUnicode(false);

            // Configura Contrasena.
            entity.Property(e => e.Contrasena)
                .HasMaxLength(255)
                .IsUnicode(false);

            // Configura Correo.
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .IsUnicode(false);

            // Configura Estado.
            entity.Property(e => e.Estado)
                .HasDefaultValue(true);

            // Configura FechaRegistro.
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            // Configura Nombre.
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);

            // Configura Telefono.
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false);

            // Relación Usuario - Rol.
            entity.HasOne(d => d.IdRolNavigation)
                .WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName(
                    "FK_Usuario_Rol"
                );
        });


        // =========================================================
        // VISTA CATALOGO PRODUCTOS
        // =========================================================

        modelBuilder.Entity<VwCatalogoProducto>(entity =>
        {
            // Indica que esta entidad representa una vista
            // y no tiene clave primaria.
            entity.HasNoKey()
                .ToView("vw_CatalogoProductos");

            // Configura Categoria.
            entity.Property(e => e.Categoria)
                .HasMaxLength(100)
                .IsUnicode(false);

            // Configura Codigo.
            entity.Property(e => e.Codigo)
                .HasMaxLength(50)
                .IsUnicode(false);

            // Configura Costo.
            entity.Property(e => e.Costo)
                .HasColumnType("decimal(12, 2)");

            // Configura Familia.
            entity.Property(e => e.Familia)
                .HasMaxLength(100)
                .IsUnicode(false);

            // Configura Imagen.
            entity.Property(e => e.Imagen)
                .HasMaxLength(500)
                .IsUnicode(false);

            // Configura Impuesto.
            entity.Property(e => e.Impuesto)
                .HasColumnType("decimal(5, 2)");

            // Configura Precio.
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(12, 2)");

            // Configura Producto.
            entity.Property(e => e.Producto)
                .HasMaxLength(150)
                .IsUnicode(false);
        });


        // =========================================================
        // VISTA PRODUCTOS MAS VENDIDOS
        // =========================================================

        modelBuilder.Entity<VwProductosMasVendido>(entity =>
        {
            // Representa una vista sin clave primaria.
            entity.HasNoKey()
                .ToView("vw_ProductosMasVendidos");

            // Configura Producto.
            entity.Property(e => e.Producto)
                .HasMaxLength(150)
                .IsUnicode(false);

            // Configura TotalVentas.
            entity.Property(e => e.TotalVentas)
                .HasColumnType("decimal(38, 2)");
        });


        // =========================================================
        // VISTA PRODUCTOS STOCK BAJO
        // =========================================================

        modelBuilder.Entity<VwProductosStockBajo>(entity =>
        {
            // Representa una vista sin clave primaria.
            entity.HasNoKey()
                .ToView("vw_ProductosStockBajo");

            // Configura Nombre.
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);
        });


        // =========================================================
        // VISTA VENTAS POR FECHA
        // =========================================================

        modelBuilder.Entity<VwVentasPorFecha>(entity =>
        {
            // Representa una vista sin clave primaria.
            entity.HasNoKey()
                .ToView("vw_VentasPorFecha");

            // Configura Ingresos.
            entity.Property(e => e.Ingresos)
                .HasColumnType("decimal(38, 2)");
        });


        // Permite agregar configuraciones adicionales
        // desde otra parte de esta clase partial.
        OnModelCreatingPartial(modelBuilder);
    }


    // Método parcial que permite extender
    // la configuración del contexto.
    partial void OnModelCreatingPartial(
        ModelBuilder modelBuilder
    );
}