using Carrito_B.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Carrito_B.Models
{
    public class StockItem
    {
        public int Id { get; set; }
        public int SucursalId { get; set; }
        public int ProductoId { get; set; }
        public Sucursal Sucursal { get; set; }
        public Producto Producto { get; set; }

        [Required(ErrorMessage = Configs.CAMPO_REQUERIDO)]
        [Range(0, int.MaxValue, ErrorMessage = Configs.MAYOR_QUE)]
        public int Cantidad { get; set; }
    }
}
