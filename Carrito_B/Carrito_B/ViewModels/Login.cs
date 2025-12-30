using System.ComponentModel.DataAnnotations;

namespace Carrito_B.ViewModels
{
    public class Login
    {

        [Required(ErrorMessage = "El {0} es obligatorio")]
        [StringLength(30, ErrorMessage = "El {0} no puede superar los {1} caracteres")]
        [Display(Name = "Nombre de usuario")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "El {0} es obligatorio")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        public bool Recordame { get; set; }
    }
}
