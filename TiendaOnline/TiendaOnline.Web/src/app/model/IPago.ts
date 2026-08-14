// Representa un pago realizado.
export interface IPago {

  // Identificador del pago.
  idPago: number;

  // Pedido relacionado.
  idPedido: number;

  // Método utilizado.
  metodoPago: string;

  // Referencia del pago.
  referencia: string;

  // Monto pagado.
  monto: number;

  // Fecha del pago.
  fechaPago: string;

  // Estado del pago.
  estado: string;

  // Identificador del método.
  idMetodoPago: number;

  // Identificador del estado.
  idEstadoPago: number;
}
