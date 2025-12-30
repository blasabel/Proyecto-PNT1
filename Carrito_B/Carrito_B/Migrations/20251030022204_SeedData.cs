using Microsoft.EntityFrameworkCore.Migrations;
using static System.Net.WebRequestMethods;

#nullable disable

namespace Carrito_B.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
               name: "Descripcion",
               table: "Categorias",
               type: "nvarchar(150)", 
               maxLength: 150,
               nullable: false,
               oldClrType: typeof(string),
               oldType: "nvarchar(50)", 
               oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Marcas",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.InsertData(
               table: "Categorias",
               columns: new[] { "Id", "Nombre", "Descripcion" },
               values: new object[,]
               {
                    { 1, "Remeras", "Remeras unisex" },
                    { 2, "Joggins", "Pantalon de corte casual" },
                    { 3, "Jeans", "Pantalon Vaquero" },
                    { 4, "De vestir",  "Prendas formales" },
                    { 5, "Deportiva",  "Prendas deportivas y fitness" },
                    { 6, "Blusas", "Camisas casuales" },
                    { 7, "Zapatos","Zapatos formales" },
                    { 8, "Zapatillas", "Calzado versátil/Deportivo" },
                    { 9, "Buzos", "Prenda de abrigo de manga larga" },
                    { 10, "Campera", "Prenda de abrigo abierta por delante" },
                    { 11, "Accesorios", "Accesorios de todo tipo" }
               });

            migrationBuilder.InsertData(
                table: "Marcas",
                columns: new[] { "Id", "Nombre", "Descripcion" },
                values: new object[,]
                {
                    { 1, "Zara", "Zara" },
                    { 2, "Bolivia", "Bolivia" },
                    { 3, "Levis", "Levis" },
                    { 4, "Nike",  "Nike" },
                    { 5, "Adidas",  "Adidas" },
                    { 6, "Puma",  "Puma" },
                    { 7, "Macowens",  "Macowens" },
                    { 8, "Portsaid", "Portsaid" },
                    { 9, "Bowen", "Bowen" },
                    { 10, "Pull&Bear",  "Pull&Bear" },
                    { 11, "Bershka",  "Bershka" },
                    { 12, "Isadora","Isadora" }
                });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Nombre", "Descripcion", "PrecioVigente", "CategoriaId", "MarcaId", "Imagen", "Activo" },
                values: new object[,]
                {
                    {1,  "Zapatillas Palermo Cuero",  "Color PUMA White-Vapor Gray-Gum",  139999.99, 8,  6, "https://images.puma.com/image/upload/f_auto,q_auto,w_600,b_rgb:FAFAFA/global/396464/01/sv01/fnd/ARG/fmt/png", true },
                    {2,  "Zapatillas Deportivas",  "Zapatillas Adidas Runfalcon",  64999.99,  5,  5 , "https://assets.adidas.com/images/w_600,f_auto,q_auto/2d2249871d004e4e91c0af34014a7c8f_9366/Zapatillas_Runfalcon_2.0_Blanco_HQ3789_01_standard.jpg", true },
                    {3,  "Camiseta Titular Arg",  "Camiseta Titular Argentina 2026 version Jugador", 200000.00,  5,  5, "https://assets.adidas.com/images/h_2000,f_auto,q_auto,fl_lossy,c_fill,g_auto/44ad9429d16a46ffb404df292923d173_9366/Camiseta_Titular_Argentina_26_Version_Jugador_Blanco_JM5897_HM53.jpg", true },
                    {4,  "Camisa Lino",  "Camisa de vestir color blanco",  65000.50,  4,  1, "https://static.zara.net/assets/public/816f/52a0/62fc4e0cbd7b/ad0ca00582ec/03090444250-000-p/03090444250-000-p.jpg?ts=1763726132245&w=1024", true },
                    {5,  "Pantalones PUMA", "Colaboración PUMA x KIDSUPER ",  179999.99,  2,  6, "https://images.puma.com/image/upload/f_auto,q_auto,w_600,b_rgb:FAFAFA/global/632375/63/mod01/fnd/ARG/fmt/png", true },
                    {6,  "Remera angelitos", "Remera con estampa en frente y espalda",65000.00 ,1,2,"https://boliviauniverso.com/files/productos/2740/BS2600020_00_0200.jpg?v=2.1" , true},
                    {7,  "Borcego acolchado","Borcego detalle acolchado color mostaza",  69000.00,  7,  11, "https://static.bershka.net/assets/public/2a99/adac/a3f34325aa1b/b7daa23169e7/12100664107-a3o/12100664107-a3o.jpg?ts=1752752171403&w=850", true },
                    {8,  "Buzo Knit", "Jersey de punto con texto", 59999.00, 9, 10, "https://static.pullandbear.net/assets/public/2872/004b/17c2428f91a8/0cb0d68dbf6c/07550547700-A6M/07550547700-A6M.jpg?ts=1757870892373&w=1024&f=auto", true },
                    {9,  "Remera Levis", "Remera algodón unisex", 5999.99, 1, 3, "https://acdn-us.mitiendanube.com/stores/002/186/544/products/r11-630d2b8f7320a4ea9116540263656702-1024-1024.jpg", true },
                    {10, "Vestido Lino Chemise", "Vestido de calce chemise tijuca", 170000.50, 4, 8, "https://portsaid.vtexassets.com/arquivos/ids/453028-1200-auto?width=1200&height=auto&aspect=true", true },
                    {11, "Jean Loose Denim", "Jean Loose Denim oxidado", 179000.00, 3, 9, "https://bowen.com.ar/media/catalog/product/cache/347cf619bf64f51fa1562e72e60dffd8/p/5/p590701216tr032_1.webp", true },
                    {12, "Campera Suede Leather", "Campera Suede Leather color gris", 200000.00, 10, 9, "https://bowen.com.ar/media/catalog/product/cache/347cf619bf64f51fa1562e72e60dffd8/p/6/p600602091gy0sm_1.webp", true },
                    {13, "Jean Baggy azul", "Jean Baggy levis", 129999.00, 3, 3, "https://levisarg.vtexassets.com/arquivos/ids/898615-1600-auto?v=638923420184900000&width=1600&height=auto&aspect=true", true },
                    {14, "Reloj analógico", "Reloj analógico con malla de mesh ", 57000.00, 11, 12, "https://ar-isadora.bluestargroup-cdn.com/media/catalog/product/4/5/45221801_0_1_20241205120813.jpg?quality=75&bg-color=255,255,255&fit=bounds&height=985&width=770&canvas=770:985", true },
                    {15, "Saco azul claro", "Saco de punto melange azul claro", 150000.00, 4, 7, "https://www.macowens.com.ar/media/catalog/product/m/a/macowens-saco_2002-060_005.jpg?optimize=high&bg-color=255,255,255&fit=bounds&height=1350&width=900&canvas=900:1350", true },
                    {16, "Gorra lavanda NYC", "Gorra con visera de NYC",70000.00, 11, 1, "https://static.zara.net/assets/public/6270/8814/e04b4d9cac7a/bd7266f5e65b/09065418400-000-e1/09065418400-000-e1.jpg?ts=1756820880974&w=750", true },
                    {17, "Camisa sport melange rosa", "Camisa sport manga larga color rosa", 37000.99, 4, 7, "https://www.macowens.com.ar/media/catalog/product/m/a/macowens-camisa-sport_42237-083_101.jpg?optimize=high&bg-color=255,255,255&fit=bounds&height=1350&width=900&canvas=900:1350", true },
                    {18, "Air Jordan 1", "Air Jordan 1 Low SE", 249999.99, 8, 4, "https://nikearprod.vtexassets.com/arquivos/ids/1460993-1200-1200?width=1200&height=1200&aspect=true", true },
                    {19, "Blusa Lino Print Leaves", "Blusa de calce recto manga corta ", 75000.00, 6, 8, "https://portsaid.vtexassets.com/arquivos/ids/460557-1200-auto?width=1200&height=auto&aspect=true", true },
                    {20, "Buzo cremallera blanco", "Buzo con cuello y cremallera print", 59000.00, 9, 11, "https://static.bershka.net/assets/public/1bd1/5f79/32ac4980b3f1/703c66e98bca/07410732251-p/07410732251-p.jpg?ts=1763638135376&w=850", true },
                    {21, "Joggers Tiro Pro ", "Estructura de tejido espaciador", 65000.00, 2, 5, "https://assets.adidas.com/images/h_2000,f_auto,q_auto,fl_lossy,c_fill,g_auto/b4d5d4fd2b8b4ac380389e4c73ac22bf_9366/Joggers_Tiro_Pro_Seleccion_Argentina_26_Azul_JZ5963_01_laydown.jpg", true },
                    {22, "Cartera city denim", "Cartera city de denim", 4200.00, 11, 12, "https://ar-isadora.bluestargroup-cdn.com/media/catalog/product/5/1/51457801_0_1_20250829152718.jpg?quality=75&bg-color=255,255,255&fit=bounds&height=985&width=770&canvas=770:985", true },
                    {23, "Nike Total 90", "Short de Fútbol para Hombre", 80000.00, 5, 4, "https://nikearprod.vtexassets.com/arquivos/ids/1548494-1200-1200?width=1200&height=1200&aspect=true", true },
                    {24, "Buzo Tamarisco", "Buzo tejido confeccionado en 100% algodón", 69999.99, 9, 2, "https://boliviauniverso.com/files/productos/2825/BS2605006_22_02.jpg?v=2.1", true }
                });

            migrationBuilder.InsertData(
                table: "Sucursales",
                columns: new[] { "Id", "Nombre", "Direccion", "Email", "Activa", "Telefono" },
                values: new object[,]
                {
                    { 1, "Caballito", "Av. Diaz Velez 5054", "supercaballito@carrito.com", true, "12345678" },
                    { 2, "Palermo", "Paraguay 5180", "superpalermo@carrito.com", true, "87654321"  },
                    { 3, "Retiro", "Juncal 701", "superretiro@carrito.com", true, "12398755" },
                    {4, "Belgrano",  "Av. Cabildo 1450",  "superbelgrano@carrito.com",  true,  "11223344" },
                    {5, "San Isidro",  "Av. Márquez 2200",  "supersanisidro@carrito.com",  true,  "11557788" },
                    {6, "La Plata",  "Calle 7 N°1200", "superlaplata@carrito.com",  true,  "221998877" },
                    {7, "Recoleta", "Av. Santa Fe 1890",  "superrecoleta@carrito.com",  true, "11445566" },
                    {8, "Quilmes",  "Av. Calchaquí 3200",  "superquilmes@carrito.com",  true,  "11447788" },
                    { 9, "Morón",  "Av. Rivadavia 18200",  "supermoron@carrito.com",  true,  "11449900" },
                    {10, "San Justo",  "Av. Illia 2450",  "supersanjusto@carrito.com",  true, "11443322" },
                    {11, "Tigre",  "Av. Cazón 950",  "supertigre@carrito.com",  true, "11446655" }
                });

            // Seed StockItems (cantidad por sucursal y producto)
            migrationBuilder.InsertData(
                table: "StockItems",
                columns: new[] { "Id", "SucursalId", "ProductoId", "Cantidad" },
                values: new object[,]
                {
                    { 1, 1, 1, 50 },
                    { 2, 1, 2, 10 },
                    { 3, 1, 3, 20 },
                    { 4, 2, 1, 30 },
                    { 5, 2, 4, 5 },
                    { 6, 2, 5, 2 },
                    { 7, 3, 2, 15 },
                    { 8, 3, 6, 40 },
                    { 9, 3, 7, 25 },
                    { 10, 4, 8, 8 },
                    { 11, 4, 9, 20 },
                    { 12, 5, 10, 100 },
                    { 13, 5, 11, 12 },
                    { 14, 6, 12, 50 },
                    { 15, 6, 1, 10 },
                    { 16, 7, 2, 5 },
                    { 17, 8, 3, 15 },
                    { 18, 9, 4, 3 },
                    { 19, 10, 5, 6 },
                    { 20, 11, 7, 7 },
                    { 21, 1, 4, 15 },
                    { 22, 1, 5, 8 },
                    { 23, 2, 2, 12 },
                    { 24, 2, 6, 6 },
                    { 25, 3, 1, 7 },
                    { 26, 3, 3, 10 },
                    { 27, 4, 1, 9 },
                    { 28, 4, 2, 4 },
                    { 29, 4, 3, 6 },
                    { 30, 5, 1, 20 },
                    { 31, 5, 2, 5 },
                    { 32, 5, 6, 10 },
                    { 33, 6, 2, 15 },
                    { 34, 6, 3, 8 },
                    { 35, 6, 4, 2 },
                    { 36, 7, 1, 6 },
                    { 37, 7, 3, 4 },
                    { 38, 7, 5, 3 },
                    { 39, 7, 6, 7 },
                    { 40, 8, 1, 5 },
                    { 41, 8, 2, 6 },
                    { 42, 8, 4, 2 },
                    { 43, 8, 7, 9 },
                    { 44, 9, 1, 12 },
                    { 45, 9, 2, 7 },
                    { 46, 9, 3, 9 },
                    { 47, 9, 5, 4 },
                    { 48, 10, 1, 14 },
                    { 49, 10, 2, 9 },
                    { 50, 10, 3, 11 },
                    { 51, 10, 4, 5 },
                    { 52, 11, 1, 10 },
                    { 53, 11, 2, 8 },
                    { 54, 11, 3, 6 },
                    { 55, 11, 4, 3 },
                    { 56, 1, 13, 8 },
                    { 57, 3, 13, 5 },
                    { 58, 5, 13, 10 },
                    { 59, 2, 14, 7 },
                    { 60, 6, 14, 4 },
                    { 61, 9, 14, 6 },
                    { 62, 1, 15, 20 },
                    { 63, 4, 15, 12 },
                    { 64, 8, 15, 9 },
                    { 65, 2, 16, 5 },
                    { 66, 5, 16, 3 },
                    { 67, 11, 16, 2 },
                    { 68, 3, 17, 10 },
                    { 69, 7, 17, 6 },
                    { 70, 10, 17, 4 },
                    { 71, 4, 18, 8 },
                    { 72, 6, 18, 7 },
                    { 73, 9, 18, 5 },
                    { 74, 5, 19, 15 },
                    { 75, 8, 19, 6 },
                    { 76, 11, 19, 4 },
                    { 77, 1, 20, 25 },
                    { 78, 2, 20, 10 },
                    { 79, 6, 20, 12 },
                    { 80, 3, 21, 9 },
                    { 81, 5, 21, 7 },
                    { 82, 10, 21, 5 },
                    { 83, 4, 22, 6 },
                    { 84, 7, 22, 8 },
                    { 85, 11, 22, 4 },
                    { 86, 2, 23, 30 },
                    { 87, 9, 23, 18 },
                    { 88, 6, 23, 12 },
                    { 89, 8, 24, 10 },
                    { 90, 10, 24, 6 },
                    { 91, 11, 24, 3 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar productos existentes previamente (parcial)
            migrationBuilder.DeleteData(table: "Productos", keyColumn: "Id", keyValue: 1);
            migrationBuilder.DeleteData(table: "Productos", keyColumn: "Id", keyValue: 2);
            migrationBuilder.DeleteData(table: "Productos", keyColumn: "Id", keyValue: 3);

            // Eliminar marcas (parcial)
            migrationBuilder.DeleteData(table: "Marcas", keyColumn: "Id", keyValue: 1);
            migrationBuilder.DeleteData(table: "Marcas", keyColumn: "Id", keyValue: 2);
            migrationBuilder.DeleteData(table: "Marcas", keyColumn: "Id", keyValue: 3);

            // Eliminar categorías (parcial)
            migrationBuilder.DeleteData(table: "Categorias", keyColumn: "Id", keyValue: 1);
            migrationBuilder.DeleteData(table: "Categorias", keyColumn: "Id", keyValue: 2);
            migrationBuilder.DeleteData(table: "Categorias", keyColumn: "Id", keyValue: 3);


        }
    }
}

