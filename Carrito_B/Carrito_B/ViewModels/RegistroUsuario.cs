using Carrito_B.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Carrito_B.ViewModels
{
    public class RegistroUsuario
    {
        [Required(ErrorMessage = "El {0} es obligatorio")]
        [EmailAddress(ErrorMessage = "El {0} no es valido")]
        public string Email { get; set; }

        [Required(ErrorMessage = Configs.CAMPO_REQUERIDO)]
        [StringLength(30, ErrorMessage = Configs.STRING_LENGTH)]
        [Display(Name = "Nombre de usuario")]
        public string UserName
        {
            get;set;
        }

        [Required(ErrorMessage = Configs.CAMPO_REQUERIDO)]
        [StringLength(50, ErrorMessage = Configs.STRING_LENGTH)]
        [RegularExpression("^[a-zA-ZñÑáéíóúÁÉÍÓÚüÜ ]+$", ErrorMessage = Configs.SOLO_LETRAS)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = Configs.CAMPO_REQUERIDO)]
        [StringLength(50, ErrorMessage = Configs.STRING_LENGTH)]
        [RegularExpression("^[a-zA-ZñÑáéíóúÁÉÍÓÚüÜ ]+$", ErrorMessage = Configs.SOLO_LETRAS)]
        public string Apellido { get; set; }

        [Required(ErrorMessage = Configs.CAMPO_REQUERIDO)]
        [StringLength(8, MinimumLength = 7, ErrorMessage = Configs.STRING_LENGTH_RANGE)]
        [RegularExpression(@"^[1-9][0-9]{6,7}$", ErrorMessage = Configs.DNI_INCORRECTO)]
        public string DNI { get; set; }

        [Required(ErrorMessage = Configs.CAMPO_REQUERIDO)]
        [MaxLength(11, ErrorMessage = Configs.MAX_LENGTH)]
        [Display(Name = Configs.DISPLAY_CUIL)]
        [RegularExpression(@"^[1-9][0-9]{10}$", ErrorMessage = $"{Configs.SOLO_NUMEROS} y debe tener 11 digitos.")]
        public string IdentificacionUnica { get; set; }

        [Required(ErrorMessage = Configs.CAMPO_REQUERIDO)]
        [Phone(ErrorMessage = "El {0} no tiene un formato válido")]
        [StringLength(20, ErrorMessage = Configs.STRING_LENGTH)]
        [RegularExpression("^[0-9()+-]+$", ErrorMessage = "El {0} solo puede contener números y los caracteres +, -, (, )")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = Configs.CAMPO_REQUERIDO)]
        [StringLength(100, ErrorMessage = Configs.STRING_LENGTH)]
        [RegularExpression("^[a-zA-Z0-9ñÑáéíóúÁÉÍÓÚüÜ #.,-]+$", ErrorMessage = "La {0} contiene caracteres no válidos")]
        public string Direccion { get; set; }


        [Required(ErrorMessage = "El {0} es obligatorio")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "El {0} es obligatorio")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmacion contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña no coincide")]
        public string ConfirmacionPassword { get; set; }
    }
}
