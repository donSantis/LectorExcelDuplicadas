using ExcelDataReader;
using LectorExcel.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

namespace LectorExcel.Controllers
{
    public class MarcaController : Controller
    {

        [HttpGet]
        public IActionResult Index(List<Marca> marcas = null)
        {
            marcas = marcas == null ? new List<Marca>() : marcas;
            return View(marcas);
        }
        [HttpPost]
        public IActionResult Index(IFormFile file, [FromServices] Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            string fileName = $"{hostingEnvironment.WebRootPath}\\files\\{file.FileName}";
            using (FileStream fileStream = System.IO.File.Create(fileName))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }
            var marcas = this.GetMarcasList(file.FileName);
            //var marcas2 = this.GetDuplicateMarcasList(file.FileName);
            return Index(marcas);
        }
        public List<Marca> GetMarcasList(string fname)
        {
            List<Marca> marcas = new List<Marca>();
            var fileName = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\files"}" + "\\" + fname;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    int id = 0;
                    // Obtener un substring desde el índice 0 hasta un número de caracteres específico, incluyendo espacios
                    int inicio = 10; // Índice de inicio
                    int longitud = 2; // Número de caracteres que deseas en el substring
                    string substring = string.Empty;
                    while (reader.Read())
                    {
                        var fechaMarca = reader.GetValue(9);

                        var marca = new Marca()
                        {
                            ID = id,
                            ID_MARCA = reader.GetValue(0).ToString(),
                            ID_USUARIO = reader.GetValue(1).ToString(),
                            FECHA_MARCA = fechaMarca.ToString(), // Elimina espacios al principio y al final
                            TIPO_MARCA = reader.GetValue(6).ToString(),
                            ORIGEN_MARCA = reader.GetValue(11).ToString(),
                            //HORA = ObtenerHora(fechaMarca.ToString()),
                            //HORA = fechaMarca.ToString(),
                            //MINUTO = ObtenerMin(fechaMarca.ToString()),
                            FECHA = ObtenerFecha(fechaMarca.ToString()),
                            KEY = ObtenerKeyMaster(reader.GetValue(1).ToString(), fechaMarca.ToString(), reader.GetValue(6).ToString()),
                            //MINUTO = fechaMarca.ToString(),
                            QUERY = "N/A",
                            DUPLICADO = false
                        };

                        // Verifica si la marca es duplicada
                        //if (marcas.Any(m => m.ID_USUARIO == marca.ID_USUARIO &&
                        //                   m.FECHA_MARCA == marca.FECHA_MARCA &&
                        //                   m.TIPO_MARCA == marca.TIPO_MARCA))
                        //{
                        //    marca.DUPLICADO = true;
                        //}
                        MarcaDuplicada(marcas, marca);

                        marcas.Add(marca);
                    }
                }
            }
            return marcas;
        }

        //public string ObtenerHora(string hora)
        //{
        //    var uwu = hora;
        //    uwu = hora.Length >= 13 ? hora.Substring(11, 2) : string.Empty;
        //    return uwu;
        //}
        //public string ObtenerMin(string min)
        //{
        //    var uwu = min;
        //    uwu = min.Length >= 13 ? min.Substring(14, 2) : string.Empty;
        //    return uwu;
        //}
        public string ObtenerFecha(string fecha)
        {
            var uwu = fecha;
            uwu = fecha.Length >= 13 ? fecha.Substring(0, 16) : string.Empty;
            uwu = Regex.Replace(uwu, @"[^\d]", "");
            return uwu;
        }
        public string ObtenerKeyMaster(string idusuario, string fecha, string tipo)
        {

            var uwu = idusuario + fecha + tipo;
            if (uwu == "ID_USUARIOTIPO_MARCA")
            {
                uwu = "KEY";
            }
            return uwu;
        }

        public bool MarcaDuplicada(List<Marca> marcas, Marca marca)
        {
            if (marcas.Any(m => m.ID_USUARIO == marca.ID_USUARIO &&
                                   m.FECHA_MARCA == marca.FECHA_MARCA &&
                                   m.QUERY == marca.QUERY &&
                                   m.TIPO_MARCA == marca.TIPO_MARCA))
            {
                return marca.DUPLICADO = true;
            }
            else
            {
                return marca.DUPLICADO = false;
            }
        }


        //public List<Marca> GetDuplicateMarcasList(string fname)
        //{
        //    List<Marca> marcas = GetMarcasList(fname);

        //    // Utiliza LINQ para encontrar las marcas duplicadas
        //    var duplicateMarcas = marcas
        //        .GroupBy(m => new { m.ID_USUARIO, m.FECHA_MARCA, m.TIPO_MARCA })
        //        .Where(g => g.Count() > 1)
        //        .SelectMany(g => g);

        //    return duplicateMarcas.ToList();
        //}
    }
}
