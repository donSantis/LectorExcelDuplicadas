using ExcelDataReader;
using LectorExcel.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace LectorExcel.Controllers
{
    public class DuplicadaController : Controller
    {
        [HttpGet]
        public IActionResult Index(MarcasViewModel marcas = null)
        {
            marcas = marcas == null ? new MarcasViewModel() : marcas;
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

            var viewModel = this.GetMarcasList(file.FileName);
            DownloadDuplicadasQuery(viewModel);
            // Crea una instancia de MarcasViewModel y asigna las listas
            var marcasViewModel = new MarcasViewModel
            {
                Marcas = viewModel.Marcas,
                Duplicadas = viewModel.Duplicadas,
                Ocultas = viewModel.Ocultas
            };
            

            // Devuelve la vista con el modelo MarcasViewModel
            return View(marcasViewModel);
        }


        public MarcasViewModel GetMarcasList(string fname)
        {
            List<Marca> marcas = new List<Marca>();
            List<Marca> duplicadas = new List<Marca>();
            List<Marca> ocultas = new List<Marca>();
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
                            KEY = ObtenerKeyMaster(reader.GetValue(1).ToString(), ObtenerFecha(fechaMarca.ToString()), reader.GetValue(6).ToString()),
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
                        if (IgnorarMarca(marca))
                        {
                            ocultas.Add(marca);
                        }
                        else if (MarcaDuplicada(marcas, marca))
                        {
                            QueryCreator(marca);
                            duplicadas.Add(marca);
                        }
                        else
                        {
                            marcas.Add(marca);
                        }
                        id = id + 1;
                    }
                }
            }
            var viewModel = new MarcasViewModel
            {
                Marcas = marcas,
                Ocultas = ocultas,
                Duplicadas = duplicadas
            };

            return viewModel;
        }
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
                                   m.KEY == marca.KEY &&
                                   m.TIPO_MARCA == marca.TIPO_MARCA))
            {
                return marca.DUPLICADO = true;
            }
            else
            {
                return marca.DUPLICADO = false;
            }
        }

        public string QueryCreator(Marca marca = null)
        {
            var select = "UPDATE MARCA SET ORIGEN_MARCA = '";
            var where = "' WHERE ID_USUARIO = ";
            var and = " AND ID_MARCA = ";
            var uwu = OrigenNuevo(marca);

            marca.QUERY = select + uwu + where + marca.ID_USUARIO + and + marca.ID_MARCA;
            return marca.QUERY;
        }
        public bool IgnorarMarca(Marca marca)
        {
            if (marca.ORIGEN_MARCA == "web" || marca.ORIGEN_MARCA == "RCGPRS" || marca.ORIGEN_MARCA == "RCfile" || marca.ORIGEN_MARCA == "IVR" || marca.ORIGEN_MARCA == "app" || marca.ORIGEN_MARCA == "app-manual" || marca.ORIGEN_MARCA == "Huellero")
            {
                return false;
            }
            else if(marca.ORIGEN_MARCA == "web-hla" || marca.ORIGEN_MARCA == "web-hide" || marca.ORIGEN_MARCA == "GPRS-hide" || marca.ORIGEN_MARCA == "ivr-hide" || marca.ORIGEN_MARCA == "IVR" || marca.ORIGEN_MARCA == "huel-hide" || marca.ORIGEN_MARCA == "app-hla")
            {
                return true;

            }
            return false;

        }
        //public string OcultarIgnorar(Marca marca = null)
        //{
        //    if (marca.ORIGEN_MARCA == "web" || marca.ORIGEN_MARCA == "RCGPRS" || marca.ORIGEN_MARCA == "RCfile" || marca.ORIGEN_MARCA == "IVR"|| marca.ORIGEN_MARCA == "app" || marca.ORIGEN_MARCA == "app-manual" || marca.ORIGEN_MARCA == "Huellero")
        //    {
        //        marca.QUERY = "UPDATE MARCA SET ORIGEN_MARCA";
        //    }
        //    if (marca.ORIGEN_MARCA == "web-hide" || marca.ORIGEN_MARCA == "GPRS-hide" || marca.ORIGEN_MARCA == "ivr-hide" || marca.ORIGEN_MARCA == "huel-hide")
        //    {
        //        marca.QUERY = "Marca ya oculta";
        //    }

        //    marca.QUERY = "ID_USUARIOTIPO_MARCA";
        //    return marca.QUERY;
        //}
        public string OrigenNuevo(Marca marca = null)
        {
            if (marca.ORIGEN_MARCA == "web")
            {
                return "web-hla";
            }
            if ( marca.ORIGEN_MARCA == "RCGPRS" )
            {
                return "GPRS-hide";
            }
            if (marca.ORIGEN_MARCA == "IVR" )
            {
                return "ivr-hide";
            }
            if (marca.ORIGEN_MARCA == "Huellero")
            {
                return "huel-hide";
            }
            if (marca.ORIGEN_MARCA == "app")
            {
                return "app-hide";
            }
            return "no identificado";

        }

        public IActionResult DownloadDuplicadasQuery(MarcasViewModel marcas = null)
        {
            var duplicadas = marcas.Duplicadas; // Asegúrate de tener esta lista disponible
            string fileName = "duplicadas_query.txt"; // Nombre del archivo

            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", fileName);
                // Combina la carpeta "Downloads" en la carpeta del perfil de usuario con el nombre del archivo

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (Marca marca in duplicadas)
                    {
                        // Escribe el contenido de la propiedad Query en el archivo
                        writer.WriteLine(marca.QUERY);
                    }
                }

                // Preparar el archivo para su descarga
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

                return File(fileBytes, "text/plain", fileName);
            }
            catch (Exception ex)
            {
                // Manejo de excepciones (puedes personalizar esto)
                Console.WriteLine("Error al exportar marcas duplicadas: " + ex.Message);
                return Content("Error al exportar marcas duplicadas.");
            }
        }
    }
}
