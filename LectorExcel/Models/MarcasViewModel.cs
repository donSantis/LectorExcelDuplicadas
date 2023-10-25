namespace LectorExcel.Models
{
    using System.Collections.Generic;
    using LectorExcel.Models; // Asegúrate de importar el espacio de nombres correcto

    public class MarcasViewModel
    {
        public List<Marca>? Marcas { get; set; }
        public List<Marca>? Duplicadas { get; set; }
        public List<Marca>? Ocultas { get; set; }
    }
}
