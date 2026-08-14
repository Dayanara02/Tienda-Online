// Representa una categoría del sistema.
export interface ICategoria {

  // Identificador de la categoría.
  idCategoria: number;

  // Familia relacionada.
  idFamilia: number;

  // Nombre de la categoría.
  nombre: string;

  // Descripción opcional.
  descripcion: string | null;

  // Indica si está activa.
  estado: boolean;
}
