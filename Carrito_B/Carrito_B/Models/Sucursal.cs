using Carrito_B.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Carrito_B.Models
{
    public class Sucursal
    {
        public int Id { get; set; }

        [Required(ErrorMessage = Configs.CAMPO_REQUERIDO)]
        public string Nombre {  get; set; }

        [Required(ErrorMessage = Configs.CAMPO_REQUERIDO)]
        public string Direccion {  get; set; }

        [Required(ErrorMessage = Configs.CAMPO_REQUERIDO)]
        [EmailAddress(ErrorMessage = Configs.EMAIL_VALIDO)]
        public string Email { get; set; }

        public bool Activa {  get; set; }

        public string Telefono { get; set; }

        public List<StockItem> StockItems { get; set; }

        public List<Compra> Compras {  get; set; }
    }
}
