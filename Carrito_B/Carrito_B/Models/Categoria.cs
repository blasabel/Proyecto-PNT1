using Carrito_B.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Carrito_B.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = Configs.CAMPO_REQUERIDO)]
        [MaxLength(50, ErrorMessage = Configs.MAX_LENGTH)]
        [RegularExpression("^[a-zA-ZñÑáéíóúÁÉÍÓÚüÜ ]+$", ErrorMessage = Configs.SOLO_LETRAS)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = Configs.CAMPO_REQUERIDO)]
        [MaxLength(150, ErrorMessage = Configs.MAX_LENGTH)]
        [DataType(DataType.MultilineText)]
        public string Descripcion { get; set; }

        public List<Producto> Productos { get; set; }
    }
}
