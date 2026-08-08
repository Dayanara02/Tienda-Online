using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.Dominio.EntidadesTipadas;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.AccesoDatos.Context;

public partial class TiendaOnlineContext : DbContext
{
    public TiendaOnlineContext()
    {
    }

    public TiendaOnlineContext(DbContextOptions<TiendaOnlineContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BitacoraSistema> BitacoraSistemas { get; set; }

    public virtual DbSet<Carrito> Carritos { get; set; }

    public virtual DbSet<Categorium> Categoria { get; set; }

    public virtual DbSet<CompraProveedor> CompraProveedors { get; set; }

    public virtual DbSet<Descuento> Descuentos { get; set; }

    public virtual DbSet<DetalleCarrito> DetalleCarritos { get; set; }

    public virtual DbSet<DetalleCompraProveedor> DetalleCompraProveedors { get; set; }

    public virtual DbSet<DetalleListaDeseo> DetalleListaDeseos { get; set; }

    public virtual DbSet<DetallePedido> DetallePedidos { get; set; }

    public virtual DbSet<DetalleProforma> DetalleProformas { get; set; }

    public virtual DbSet<DireccionUsuario> DireccionUsuarios { get; set; }

    public virtual DbSet<Envio> Envios { get; set; }

    public virtual DbSet<EstadoPago> EstadoPagos { get; set; }

    public virtual DbSet<EstadoPedido> EstadoPedidos { get; set; }

    public virtual DbSet<EvaluacionProducto> EvaluacionProductos { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<FamiliaProducto> FamiliaProductos { get; set; }

    public virtual DbSet<HistorialAcceso> HistorialAccesos { get; set; }

    public virtual DbSet<Impuesto> Impuestos { get; set; }

    public virtual DbSet<Inventario> Inventarios { get; set; }

    public virtual DbSet<ListaDeseo> ListaDeseos { get; set; }

    public virtual DbSet<MetodoPago> MetodoPagos { get; set; }

    public virtual DbSet<MovimientoInventario> MovimientoInventarios { get; set; }

    public virtual DbSet<Notificacion> Notificacions { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<ProductoProveedor> ProductoProveedors { get; set; }

    public virtual DbSet<Proforma> Proformas { get; set; }

    public virtual DbSet<Proveedor> Proveedors { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<VwCatalogoProducto> VwCatalogoProductos { get; set; }

    public virtual DbSet<VwProductosMasVendido> VwProductosMasVendidos { get; set; }

    public virtual DbSet<VwProductosStockBajo> VwProductosStockBajos { get; set; }

    public virtual DbSet<VwVentasPorFecha> VwVentasPorFechas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DAYANARA\\SQLEXPRESS;Initial Catalog=TiendaOnline;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BitacoraSistema>(entity =>
        {
            entity.HasKey(e => e.IdBitacora).HasName("PK__Bitacora__ED3A1B13E5ABE20F");

            entity.ToTable("BitacoraSistema");

            entity.Property(e => e.Accion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TablaAfectada)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.BitacoraSistemas)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bitacora_Usuario");
        });

        modelBuilder.Entity<Carrito>(entity =>
        {
            entity.HasKey(e => e.IdCarrito).HasName("PK__Carrito__8B4A618C0A50748D");

            entity.ToTable("Carrito");

            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Activo");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Carritos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Carrito_Usuario");
        });

        modelBuilder.Entity<Categorium>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK__Categori__A3C02A102CFC9629");

            entity.HasIndex(e => new { e.IdFamilia, e.Nombre }, "UQ_Categoria_Familia_Nombre").IsUnique();

            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.IdFamiliaNavigation).WithMany(p => p.Categoria)
                .HasForeignKey(d => d.IdFamilia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Categoria_Familia");

            entity.HasMany(d => d.IdDescuentos).WithMany(p => p.IdCategoria)
                .UsingEntity<Dictionary<string, object>>(
                    "CategoriaDescuento",
                    r => r.HasOne<Descuento>().WithMany()
                        .HasForeignKey("IdDescuento")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CategoriaDescuento_Descuento"),
                    l => l.HasOne<Categorium>().WithMany()
                        .HasForeignKey("IdCategoria")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CategoriaDescuento_Categoria"),
                    j =>
                    {
                        j.HasKey("IdCategoria", "IdDescuento").HasName("PK__Categori__027DB6A15ABC1DA4");
                        j.ToTable("CategoriaDescuento");
                    });
        });

        modelBuilder.Entity<CompraProveedor>(entity =>
        {
            entity.HasKey(e => e.IdCompraProveedor).HasName("PK__CompraPr__83490C249C03EE4A");

            entity.ToTable("CompraProveedor");

            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.FechaCompra)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Impuesto).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.CompraProveedors)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CompraProveedor_Proveedor");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.CompraProveedors)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CompraProveedor_Usuario");
        });

        modelBuilder.Entity<Descuento>(entity =>
        {
            entity.HasKey(e => e.IdDescuento).HasName("PK__Descuent__1BD9CB1B6C5C23B3");

            entity.ToTable("Descuento");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Porcentaje).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<DetalleCarrito>(entity =>
        {
            entity.HasKey(e => e.IdDetalleCarrito).HasName("PK__DetalleC__27A5F83BC2D6C32E");

            entity.ToTable("DetalleCarrito");

            entity.HasIndex(e => new { e.IdCarrito, e.IdProducto }, "UQ_DetalleCarrito").IsUnique();

            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.IdCarritoNavigation).WithMany(p => p.DetalleCarritos)
                .HasForeignKey(d => d.IdCarrito)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleCarrito_Carrito");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleCarritos)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleCarrito_Producto");
        });

        modelBuilder.Entity<DetalleCompraProveedor>(entity =>
        {
            entity.HasKey(e => e.IdDetalleCompra).HasName("PK__DetalleC__E046CCBBC1702FAA");

            entity.ToTable("DetalleCompraProveedor");

            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.IdCompraProveedorNavigation).WithMany(p => p.DetalleCompraProveedors)
                .HasForeignKey(d => d.IdCompraProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleCompra_Compra");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleCompraProveedors)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleCompra_Producto");
        });

        modelBuilder.Entity<DetalleListaDeseo>(entity =>
        {
            entity.HasKey(e => new { e.IdListaDeseos, e.IdProducto }).HasName("PK__DetalleL__0ABCEFCF57AA0859");

            entity.Property(e => e.FechaAgregado)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdListaDeseosNavigation).WithMany(p => p.DetalleListaDeseos)
                .HasForeignKey(d => d.IdListaDeseos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleListaDeseos_Lista");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleListaDeseos)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleListaDeseos_Producto");
        });

        modelBuilder.Entity<DetallePedido>(entity =>
        {
            entity.HasKey(e => e.IdDetallePedido).HasName("PK__DetalleP__48AFFD95B1411977");

            entity.ToTable("DetallePedido");

            entity.Property(e => e.Descuento).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Impuesto).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.IdPedidoNavigation).WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.IdPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetallePedido_Pedido");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetallePedido_Producto");
        });

        modelBuilder.Entity<DetalleProforma>(entity =>
        {
            entity.HasKey(e => e.IdDetalleProforma).HasName("PK__DetalleP__AF9413BA5D645AA1");

            entity.ToTable("DetalleProforma");

            entity.Property(e => e.Descuento).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Impuesto).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleProformas)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleProforma_Producto");

            entity.HasOne(d => d.IdProformaNavigation).WithMany(p => p.DetalleProformas)
                .HasForeignKey(d => d.IdProforma)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleProforma_Proforma");
        });

        modelBuilder.Entity<DireccionUsuario>(entity =>
        {
            entity.HasKey(e => e.IdDireccion).HasName("PK__Direccio__1F8E0C769F6F2CE4");

            entity.ToTable("DireccionUsuario");

            entity.Property(e => e.Canton)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CodigoPostal)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.DireccionExacta)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Distrito)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Provincia)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.DireccionUsuarios)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DireccionUsuario_Usuario");
        });

        modelBuilder.Entity<Envio>(entity =>
        {
            entity.HasKey(e => e.IdEnvio).HasName("PK__Envio__B814A62E849B43A4");

            entity.ToTable("Envio");

            entity.HasIndex(e => e.IdPedido, "UQ__Envio__9D335DC2EBFB49C5").IsUnique();

            entity.Property(e => e.EmpresaEnvio)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.FechaEntrega).HasColumnType("datetime");
            entity.Property(e => e.FechaEnvio).HasColumnType("datetime");
            entity.Property(e => e.NumeroSeguimiento)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.IdDireccionNavigation).WithMany(p => p.Envios)
                .HasForeignKey(d => d.IdDireccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Envio_Direccion");

            entity.HasOne(d => d.IdPedidoNavigation).WithOne(p => p.Envio)
                .HasForeignKey<Envio>(d => d.IdPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Envio_Pedido");
        });

        modelBuilder.Entity<EstadoPago>(entity =>
        {
            entity.HasKey(e => e.IdEstadoPago).HasName("PK__EstadoPa__540F03E93C64D911");

            entity.ToTable("EstadoPago");

            entity.HasIndex(e => e.Nombre, "UQ__EstadoPa__75E3EFCFB7C3A4C8").IsUnique();

            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EstadoPedido>(entity =>
        {
            entity.HasKey(e => e.IdEstadoPedido).HasName("PK__EstadoPe__86B98371DA12C497");

            entity.ToTable("EstadoPedido");

            entity.HasIndex(e => e.Nombre, "UQ__EstadoPe__75E3EFCF1BB0A861").IsUnique();

            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EvaluacionProducto>(entity =>
        {
            entity.HasKey(e => e.IdEvaluacion).HasName("PK__Evaluaci__A7EA657C5C48E026");

            entity.ToTable("EvaluacionProducto");

            entity.HasIndex(e => new { e.IdUsuario, e.IdProducto }, "UQ_Evaluacion_Usuario_Producto").IsUnique();

            entity.Property(e => e.Comentario)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.FechaEvaluacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.EvaluacionProductos)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Evaluacion_Producto");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.EvaluacionProductos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Evaluacion_Usuario");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.IdFactura).HasName("PK__Factura__50E7BAF1F40535ED");

            entity.ToTable("Factura");

            entity.HasIndex(e => e.IdPedido, "UQ__Factura__9D335DC2439E741C").IsUnique();

            entity.HasIndex(e => e.NumeroFactura, "UQ__Factura__CF12F9A6645D9C76").IsUnique();

            entity.Property(e => e.Descuento).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.FechaEmision)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Impuesto).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.NumeroFactura)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Subtotal).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.UrlPdf)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("UrlPDF");

            entity.HasOne(d => d.IdPedidoNavigation).WithOne(p => p.Factura)
                .HasForeignKey<Factura>(d => d.IdPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Factura_Pedido");
        });

        modelBuilder.Entity<FamiliaProducto>(entity =>
        {
            entity.HasKey(e => e.IdFamilia).HasName("PK__FamiliaP__751F80CF41522F19");

            entity.ToTable("FamiliaProducto");

            entity.HasIndex(e => e.Nombre, "UQ__FamiliaP__75E3EFCF0D2B12AF").IsUnique();

            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasMany(d => d.IdDescuentos).WithMany(p => p.IdFamilia)
                .UsingEntity<Dictionary<string, object>>(
                    "FamiliaDescuento",
                    r => r.HasOne<Descuento>().WithMany()
                        .HasForeignKey("IdDescuento")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_FamiliaDescuento_Descuento"),
                    l => l.HasOne<FamiliaProducto>().WithMany()
                        .HasForeignKey("IdFamilia")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_FamiliaDescuento_Familia"),
                    j =>
                    {
                        j.HasKey("IdFamilia", "IdDescuento").HasName("PK__FamiliaD__D4A21C7E3E210393");
                        j.ToTable("FamiliaDescuento");
                    });
        });

        modelBuilder.Entity<HistorialAcceso>(entity =>
        {
            entity.HasKey(e => e.IdHistorialAcceso).HasName("PK__Historia__5EC8FB766D6C6AA4");

            entity.ToTable("HistorialAcceso");

            entity.Property(e => e.DireccionIp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DireccionIP");
            entity.Property(e => e.FechaAcceso)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.HistorialAccesos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistorialAcceso_Usuario");
        });

        modelBuilder.Entity<Impuesto>(entity =>
        {
            entity.HasKey(e => e.IdImpuesto).HasName("PK__Impuesto__A9B88928350A869C");

            entity.ToTable("Impuesto");

            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Porcentaje).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<Inventario>(entity =>
        {
            entity.HasKey(e => e.IdInventario).HasName("PK__Inventar__1927B20CFD7B7B9E");

            entity.ToTable("Inventario");

            entity.HasIndex(e => e.IdProducto, "UQ__Inventar__09889211666847F6").IsUnique();

            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdProductoNavigation).WithOne(p => p.Inventario)
                .HasForeignKey<Inventario>(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventario_Producto");
        });

        modelBuilder.Entity<ListaDeseo>(entity =>
        {
            entity.HasKey(e => e.IdListaDeseos).HasName("PK__ListaDes__1A2466EE44568A04");

            entity.HasIndex(e => e.IdUsuario, "UQ_ListaDeseos_Usuario").IsUnique();

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdUsuarioNavigation).WithOne(p => p.ListaDeseo)
                .HasForeignKey<ListaDeseo>(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ListaDeseos_Usuario");
        });

        modelBuilder.Entity<MetodoPago>(entity =>
        {
            entity.HasKey(e => e.IdMetodoPago).HasName("PK__MetodoPa__6F49A9BEE582A2C1");

            entity.ToTable("MetodoPago");

            entity.HasIndex(e => e.Nombre, "UQ__MetodoPa__75E3EFCF63A345EE").IsUnique();

            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MovimientoInventario>(entity =>
        {
            entity.HasKey(e => e.IdMovimiento).HasName("PK__Movimien__881A6AE09E3470EA");

            entity.ToTable("MovimientoInventario");

            entity.Property(e => e.FechaMovimiento)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Motivo)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.TipoMovimiento)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.IdInventarioNavigation).WithMany(p => p.MovimientoInventarios)
                .HasForeignKey(d => d.IdInventario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MovimientoInventario_Inventario");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.MovimientoInventarios)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MovimientoInventario_Usuario");
        });

        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.HasKey(e => e.IdNotificacion).HasName("PK__Notifica__F6CA0A8594CBDA73");

            entity.ToTable("Notificacion");

            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Mensaje)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Titulo)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Notificacions)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notificacion_Usuario");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago).HasName("PK__Pago__FC851A3AD9130F16");

            entity.ToTable("Pago");

            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.FechaPago).HasColumnType("datetime");
            entity.Property(e => e.MetodoPago)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Monto).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Referencia)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.IdEstadoPagoNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdEstadoPago)
                .HasConstraintName("FK_Pago_EstadoPago");

            entity.HasOne(d => d.IdMetodoPagoNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdMetodoPago)
                .HasConstraintName("FK_Pago_MetodoPago");

            entity.HasOne(d => d.IdPedidoNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdPedido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pago_Pedido");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.IdPedido).HasName("PK__Pedido__9D335DC3C05578FB");

            entity.ToTable("Pedido");

            entity.Property(e => e.Descuento).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.DireccionEntrega)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.FechaPedido)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Impuesto).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.IdEstadoPedidoNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.IdEstadoPedido)
                .HasConstraintName("FK_Pedido_EstadoPedido");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pedido_Usuario");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__Producto__09889210EA6E46CF");

            entity.ToTable("Producto");

            entity.HasIndex(e => e.Codigo, "UQ__Producto__06370DACB4734ED1").IsUnique();

            entity.Property(e => e.Codigo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Costo).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Imagen)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Precio).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.StockMinimo).HasDefaultValue(5);

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Producto_Categoria");

            entity.HasOne(d => d.IdImpuestoNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdImpuesto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Producto_Impuesto");

            entity.HasMany(d => d.IdDescuentos).WithMany(p => p.IdProductos)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductoDescuento",
                    r => r.HasOne<Descuento>().WithMany()
                        .HasForeignKey("IdDescuento")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ProductoDescuento_Descuento"),
                    l => l.HasOne<Producto>().WithMany()
                        .HasForeignKey("IdProducto")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ProductoDescuento_Producto"),
                    j =>
                    {
                        j.HasKey("IdProducto", "IdDescuento").HasName("PK__Producto__A8350EA1778CFC75");
                        j.ToTable("ProductoDescuento");
                    });
        });

        modelBuilder.Entity<ProductoProveedor>(entity =>
        {
            entity.HasKey(e => new { e.IdProducto, e.IdProveedor }).HasName("PK__Producto__E703F10AEE696E77");

            entity.ToTable("ProductoProveedor");

            entity.Property(e => e.CodigoProveedor)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.PrecioCompra).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.ProductoProveedors)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductoProveedor_Producto");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.ProductoProveedors)
                .HasForeignKey(d => d.IdProveedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductoProveedor_Proveedor");
        });

        modelBuilder.Entity<Proforma>(entity =>
        {
            entity.HasKey(e => e.IdProforma).HasName("PK__Proforma__6731B48A086CC9A4");

            entity.ToTable("Proforma");

            entity.Property(e => e.Descuento).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Impuesto).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.UrlPdf)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("UrlPDF");

            entity.HasOne(d => d.IdDireccionNavigation).WithMany(p => p.Proformas)
                .HasForeignKey(d => d.IdDireccion)
                .HasConstraintName("FK_Proforma_Direccion");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Proformas)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Proforma_Usuario");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("PK__Proveedo__E8B631AF77F95480");

            entity.ToTable("Proveedor");

            entity.HasIndex(e => e.Identificacion, "UQ__Proveedo__D6F931E5CCF5F21B").IsUnique();

            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Direccion)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Identificacion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Rol__2A49584CE88CC92E");

            entity.ToTable("Rol");

            entity.HasIndex(e => e.Nombre, "UQ__Rol__75E3EFCFAA09AD13").IsUnique();

            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__5B65BF9701E21C52");

            entity.ToTable("Usuario");

            entity.HasIndex(e => e.Correo, "UQ__Usuario__60695A193BA4E085").IsUnique();

            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Contrasena)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Rol");
        });

        modelBuilder.Entity<VwCatalogoProducto>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_CatalogoProductos");

            entity.Property(e => e.Categoria)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Codigo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Costo).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Familia)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Imagen)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Impuesto).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Precio).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Producto)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwProductosMasVendido>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_ProductosMasVendidos");

            entity.Property(e => e.Producto)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.TotalVentas).HasColumnType("decimal(38, 2)");
        });

        modelBuilder.Entity<VwProductosStockBajo>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_ProductosStockBajo");

            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwVentasPorFecha>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_VentasPorFecha");

            entity.Property(e => e.Ingresos).HasColumnType("decimal(38, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
