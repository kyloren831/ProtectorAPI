using System.ComponentModel.DataAnnotations.Schema;

namespace ProtectorAPP.Models.Pantallas
{
    public class SistemaConPantallasViewModel
    {
        public int IdSistema { get; set; }

       
        public string Descripcion { get; set; }

       
        public string Url { get; set; }

     
        public string Estado { get; set; }

        public List<PantallaViewModel> Pantallas { get; set; }
    }
}
