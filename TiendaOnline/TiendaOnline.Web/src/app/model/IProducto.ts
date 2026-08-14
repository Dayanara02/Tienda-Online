// Representa un producto recibido desde la API.
export interface IProducto {

  // Identificador del producto.
  idProducto: number;

  // Identificador de la categoría.
  idCategoria: number;

  // Nombre de la categoría.
  categoria: string;

  // Nombre del producto.
  nombre: string;

  // Descripción del producto.
  descripcion: string | null;

  // Precio del producto.
  precio: number;

  // Imagen del producto.
  imagen: string | null;

  // Cantidad disponible.
  stock: number;
}
