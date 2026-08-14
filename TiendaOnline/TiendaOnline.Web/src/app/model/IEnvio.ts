// Representa la información de un envío.
export interface IEnvio {

  // Identificador del envío.
  idEnvio: number;

  // Pedido relacionado.
  idPedido: number;

  // Dirección relacionada.
  idDireccion: number;

  // Empresa encargada.
  empresaEnvio: string | null;

  // Número de seguimiento.
  numeroSeguimiento: string | null;

  // Fecha de envío.
  fechaEnvio: string | null;

  // Fecha de entrega.
  fechaEntrega: string | null;

  // Estado actual.
  estado: string;

  // Dirección del pedido.
  direccion?: string | null;

  // Provincia de entrega.
  provincia?: string | null;

  // Cantón de entrega.
  canton?: string | null;

  // Distrito de entrega.
  distrito?: string | null;
}
