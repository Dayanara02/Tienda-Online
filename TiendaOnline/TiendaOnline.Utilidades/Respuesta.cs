using System;

// Define el espacio de nombres donde se encuentra
// la clase utilizada para manejar respuestas.
namespace TiendaOnline.Utilidades
{
    // Clase genérica que permite devolver una respuesta
    // indicando si la operación fue exitosa y sus datos.
    public partial class Respuesta<T>
    {
        // Indica si la operación se realizó correctamente.
        public bool Success { get; set; }

        // Contiene los datos obtenidos cuando la operación
        // fue exitosa.
        public T? Data { get; set; }

        // Contiene el mensaje de error cuando la operación falla.
        public string Error { get; set; }

        // Constructor por defecto.
        public Respuesta()
        {
            // Por defecto, la respuesta se considera exitosa.
            Success = true;

            // Inicializa el mensaje de error vacío.
            Error = "";
        }

        // Constructor utilizado para crear una respuesta
        // indicando su estado, datos y posible error.
        protected Respuesta(bool success, T data, string error)
        {
            // Guarda si la operación fue exitosa.
            Success = success;

            // Guarda los datos de la respuesta.
            Data = data;

            // Guarda el mensaje de error.
            Error = error;
        }

        // Crea una respuesta exitosa con los datos recibidos.
        public static Respuesta<T> Ok(T data) =>
            new(true, data, null);

        // Crea una respuesta fallida con el mensaje de error.
        public static Respuesta<T> Fail(string error) =>
            new(false, default, error);
    }
}