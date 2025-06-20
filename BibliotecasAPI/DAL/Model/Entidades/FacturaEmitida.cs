using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.DAL.Model.Entidades
{
    [PrimaryKey("Mes", "Año")]
    public class FacturaEmitida
    {
        public int Mes { get; set; }
        public int Año { get; set; }
    }
}
