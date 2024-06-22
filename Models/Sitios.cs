using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;


namespace PM0220242P.Models
{
    [Table("Personas")]
    public class Sitios
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string Descripcion { get; set; }
        public string Foto { get; set; } // Esta propiedad puede ser un byte[] en lugar de string si prefieres

        // Eliminamos las propiedades que ya no se usarán
    }
}