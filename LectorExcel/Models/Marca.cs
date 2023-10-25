namespace LectorExcel.Models
{
	public class Marca
	{
        public int ID { get; set; } = 0;
        public string ID_MARCA { get; set; } = "";
        public string ID_USUARIO { get; set; } = "";
		public string FECHA_MARCA { get; set; } = "";
		public string TIPO_MARCA { get; set; } = "";
		public string ORIGEN_MARCA  { get; set; } = "";
		public string QUERY { get; set; } = "";
		public string KEY { get; set; } = "";
		//public string HORA{ get; set; } = "";
		//public string MINUTO{ get; set; } = "";
		public string FECHA{ get; set; } = "";
        public bool DUPLICADO { get; set; } // Campo para indicar si la marca es duplicada

    }
}