using Carrito_B.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Carrito_B.Models
{
    public class Cliente : Persona
    {
        public List<Carrito> Carritos { get; set; }

        public List<Compra> Compras{ get; set; }
        
        [Required(ErrorMessage = Configs.CAMPO_REQUERIDO)]
        [MaxLength(11,ErrorMessage = Configs.MAX_LENGTH)]
        [Display(Name = Configs.DISPLAY_CUIL)]
        [RegularExpression("^[0-9]+$", ErrorMessage = Configs.SOLO_NUMEROS)]
        public string IdentificacionUnica { get; set; }
    }
}
