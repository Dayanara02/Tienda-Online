using System; // Permite utilizar tipos básicos del sistema, como DateTime.
using System.Collections.Generic; // Permite trabajar con colecciones genéricas.

namespace TiendaOnline.Dominio.Entidades; 

public partial class BitacoraSistema // Representa la entidad encargada de registrar acciones realizadas en el sistema.
{
    public int IdBitacora { get; set; } // Identificador único de cada registro de la bitácora.

    public int IdUsuario { get; set; } // Guarda el identificador del usuario que realizó la acción.

    public string Accion { get; set; } = null!; // Guarda el tipo de acción realizada por el usuario.

    public string? TablaAfectada { get; set; } // Indica la tabla de la base de datos que fue modificada.

    public int? IdRegistro { get; set; } // Guarda el ID del registro afectado por la acción, si existe.

    public string? Descripcion { get; set; } // Contiene una descripción adicional sobre lo ocurrido.

    public DateTime Fecha { get; set; } // Guarda la fecha y hora en que se realizó la acción.

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!; // Permite acceder a los datos del usuario relacionado con la bitácora.
}
