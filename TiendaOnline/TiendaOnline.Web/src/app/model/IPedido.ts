// Importa el modelo de detalle.
import { IDetallePedido } from './IDetallePedido';

// Representa un pedido del cliente.
export interface IPedido {

  // Identificador del pedido.
  idPedido: number;

  // Usuario propietario.
  idUsuario?: number;

  // Fecha del pedido.
  fechaPedido: string;

  // Estado general.
  estado: string;

  // Estado del pago.
  estadoPago: string;

  // Subtotal del pedido.
  subtotal: number;

  // Impuesto aplicado.
  impuesto: number;

  // Descuento aplicado.
  descuento: number;

  // Total final.
  total: number;

  // Dirección de entrega.
  direccionEntrega: string | null;

  // Método de pago utilizado.
  metodoPago?: string | null;

  // Fecha del pago.
  fechaPago?: string | null;

  // Indica si puede pagarse.
  puedePagar?: boolean;

  // Indica si puede cancelarse.
  puedeCancelar?: boolean;

  // Productos incluidos.
  detalles?: IDetallePedido[];
}
