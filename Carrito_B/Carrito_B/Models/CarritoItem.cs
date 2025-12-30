using Carrito_B.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Carrito_B.Models
{
    public class CarritoItem
    {
        public int Id { get; set; }

        public Carrito Carrito { get; set; }
        
        public int CarritoId { get; set; }
        public Producto Producto { get; set; }
      
        public int ProductoId { get; set; }

        [Required(ErrorMessage = Configs.CAMPO_REQUERIDO)]
        [Range(1,int.MaxValue, ErrorMessage = "El valor debe ser mayor a {1}")]
        public decimal ValorUnitario { get; set; }

        [Required(ErrorMessage = Configs.CAMPO_REQUERIDO)]
        [Range(1, int.MaxValue, ErrorMessage = "El valor debe ser mayor a {1}")]
        public int Cantidad { get; set; }

        public decimal Subtotal { get { return ValorUnitario * Cantidad; } }
    }
}
