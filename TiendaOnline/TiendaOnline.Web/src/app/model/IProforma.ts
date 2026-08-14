// Representa una proforma.
export interface IProforma {

  // Identificador de la proforma.
  idProforma: number;

  // Usuario relacionado.
  idUsuario: number;

  // Dirección relacionada.
  idDireccion: number | null;

  // Fecha de creación.
  fechaCreacion: string;

  // Fecha de vencimiento.
  fechaVencimiento: string | null;

  // Subtotal calculado.
  subtotal: number;

  // Impuesto aplicado.
  impuesto: number;

  // Descuento aplicado.
  descuento: number;

  // Total final.
  total: number;

  // Estado de la proforma.
  estado: string;

  // Ruta del PDF.
  urlPdf: string | null;
}
