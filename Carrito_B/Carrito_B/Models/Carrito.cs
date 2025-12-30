using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Carrito_B.Helpers;

namespace Carrito_B.Models
{
    public class Carrito
    {
        public int Id { get; set; }

        public bool Activo { get; set; } = true;

        public Cliente Cliente { get; set; }

        [Display(Name = Configs.DISPLAY_CLIENTE)]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = Configs.CAMPO_REQUERIDO)]
        [MinLength(1,ErrorMessage = "La lista debe tener por lo menos {1} item")]
        public List<CarritoItem> CarritoItems{ get; set; }
        
        public Compra Compra { get; set; }

        public decimal Subtotal { get { return CarritoItems?.Sum(item => item.Subtotal) ?? 0; } }
    }
}
