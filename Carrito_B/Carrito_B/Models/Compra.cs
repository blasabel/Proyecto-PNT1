using System;
using System.ComponentModel.DataAnnotations;


namespace Carrito_B.Models
{
    public class Compra
    {
        public int Id { get; set; }

        public Cliente Cliente { get; set; }

        public Carrito Carrito { get; set; }

        public int CarritoId { get; set; }
        
        [Display(Name = "Sucursal")]
        public int SucursalId { get; set; }

        public Sucursal Sucursal { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public decimal Total { get; set; }



    }
}
