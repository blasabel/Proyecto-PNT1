using Carrito_B.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Carrito_B.Models
{
    public class Producto
    {
        public int Id { get; set; }

        public bool Activo { get; set; } = true;

        [Required(ErrorMessage = Configs.CAMPO_REQUERIDO)]
        [MaxLength(25,ErrorMessage = Configs.MAX_LENGTH)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = Configs.CAMPO_REQUERIDO)]
        [MaxLength(100, ErrorMessage = Configs.MAX_LENGTH)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = Configs.CAMPO_REQUERIDO)]
        [Range(0.01, int.MaxValue,ErrorMessage = Configs.PRECIO_MINIMO)]
        [RegularExpression(@"^\d+([,.]\d{1,2})?$", ErrorMessage = Configs.PRECIO_FORMATO)]
        [Display(Name = Configs.DISPLAY_PRECIO_VIGENTE)]
        public decimal PrecioVigente { get; set; }

        public Categoria Categoria { get; set; }

        [Display(Name = "Categoria")]
        public int CategoriaId { get; set; }

        public string Imagen { get; set; } = Configs.IMAGEN_DEFAULT;

        public Marca Marca { get; set; }

        public int MarcaId { get; set; }

        public List<CarritoItem> CarritoItems{ get; set; }

        public List<StockItem> StockItems { get; set; }
    }
}
