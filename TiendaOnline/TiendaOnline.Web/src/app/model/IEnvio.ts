// Representa la información de un envío.
export interface IEnvio {

  // Identificador del envío.
  idEnvio: number;

  // Pedido relacionado.
  idPedido: number;

  // Dirección de entrega.
  idDireccion?: number | null;

  // Empresa encargada del envío.
  empresaEnvio: string | null;

  // Número utilizado para seguimiento.
  numeroSeguimiento: string | null;

  // Estado actual del envío.
  estado: string;

  // Fecha en que fue enviado.
  fechaEnvio?: string | null;

  // Fecha en que fue entregado.
  fechaEntrega?: string | null;
}
