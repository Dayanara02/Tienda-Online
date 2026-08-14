// Importa herramientas de formularios reactivos.
import {
  FormGroup,
  ValidationErrors
} from '@angular/forms';

// Contiene utilidades para formularios.
export class FormUtils {

  // Convierte errores en mensajes.
  static getTextError(
    errors: ValidationErrors
  ): string | null {

    // Recorre los errores encontrados.
    for (
      const key of Object.keys(errors)
    ) {

      // Evalúa el tipo de error.
      switch (key) {

        // Campo obligatorio.
        case 'required':
          return 'Este campo es requerido.';

        // Longitud mínima.
        case 'minlength':
          return `Mínimo de ${errors['minlength']
              .requiredLength
            } caracteres.`;

        // Longitud máxima.
        case 'maxlength':
          return `Máximo de ${errors['maxlength']
              .requiredLength
            } caracteres.`;

        // Correo inválido.
        case 'email':
          return 'El correo no es válido.';

        // Valor mínimo.
        case 'min':
          return `Valor mínimo de ${errors['min'].min
            }.`;
      }
    }

    // No encontró un error conocido.
    return null;
  }

  // Indica si un campo tiene errores.
  static isValidField(
    form: FormGroup,
    fieldName: string
  ): boolean | null {

    // Obtiene el campo.
    const campo =
      form.controls[fieldName];

    // Valida que exista.
    if (!campo) {
      return null;
    }

    // Comprueba errores y uso.
    return !!(
      campo.errors &&
      campo.touched
    );
  }

  // Obtiene el mensaje de error.
  static getFieldError(
    form: FormGroup,
    fieldName: string
  ): string | null {

    // Obtiene el campo.
    const campo =
      form.controls[fieldName];

    // Valida que exista.
    if (!campo) {
      return null;
    }

    // Obtiene los errores.
    const errors =
      campo.errors ?? {};

    // Devuelve el mensaje.
    return FormUtils
      .getTextError(
        errors
      );
  }
}
