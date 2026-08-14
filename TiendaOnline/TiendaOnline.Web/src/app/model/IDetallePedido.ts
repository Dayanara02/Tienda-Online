// Representa un producto dentro de un pedido.
export interface IDetallePedido {

  // Identificador del detalle.
  idDetallePedido: number;

  // Identificador del producto.
  idProducto: number;

  // Nombre del producto.
  nombreProducto: string;

  // Cantidad comprada.
  cantidad: number;

  // Precio por unidad.
  precioUnitario: number;

  // Descuento aplicado.
  descuento: number;

  // Impuesto aplicado.
  impuesto: number;

  // Subtotal del producto.
  subtotal: number;
}
