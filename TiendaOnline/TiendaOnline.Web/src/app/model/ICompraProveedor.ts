// Importa los detalles de la compra.
import { IDetalleCompraProveedor } from './IDetalleCompraProveedor';

// Representa una compra a proveedor.
export interface ICompraProveedor {

  // Identificador de la compra.
  idCompraProveedor: number;

  // Proveedor relacionado.
  idProveedor: number;

  // Usuario que registró la compra.
  idUsuario: number;

  // Fecha de la compra.
  fechaCompra: string;

  // Subtotal de la compra.
  subtotal: number;

  // Impuesto aplicado.
  impuesto: number;

  // Total final.
  total: number;

  // Estado de la compra.
  estado: string;

  // Productos de la compra.
  detalles?: IDetalleCompraProveedor[];
}
