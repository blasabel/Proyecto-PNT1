using Carrito_B.Helpers;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Carrito_B.Models
{
    public class Persona : IdentityUser<int>
    {
        [Required (ErrorMessage = Configs.CAMPO_REQUERIDO)]
        [StringLength(30, ErrorMessage = Configs.STRING_LENGTH)]
        [Display(Name = "Nombre de usuario")]
        public string UserName {
            get { return base.UserName; }
            set { base.UserName = value; } 
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
        [Phone(ErrorMessage = "El {0} no tiene un formato válido")]
        [StringLength(20, ErrorMessage = Configs.STRING_LENGTH)]
        [RegularExpression("^[0-9()+-]+$", ErrorMessage = "El {0} solo puede contener números y los caracteres +, -, (, )")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = Configs.CAMPO_REQUERIDO)]
        [StringLength(100, ErrorMessage = Configs.STRING_LENGTH)]
        [RegularExpression("^[a-zA-Z0-9ñÑáéíóúÁÉÍÓÚüÜ #.,-]+$", ErrorMessage = "La {0} contiene caracteres no válidos")]
        public string Direccion { get; set; }

       
        public DateTime FechaAlta { get; set; }

        [Required(ErrorMessage = Configs.CAMPO_REQUERIDO)]
        [EmailAddress(ErrorMessage = Configs.EMAIL_VALIDO)]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]{1,60}@[a-zA-Z0-9.-]{1,255}$",ErrorMessage = "El email no cumple con el formato o supera los límites permitidos")]
        public override string Email {
            get { return base.Email; }
            set {  base.Email = value; }
        }

        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}
