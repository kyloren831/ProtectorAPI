using System;
using System.ComponentModel.DataAnnotations;

namespace ProtectorAPI.Models
{
    public class BitacoraUsuarios
    {
        [Key]
        public int IdBitacora { get; set; }
        public int IdUsuario { get; set; }
        public string Accion { get; set; }
        public string UsuarioBD { get; set; }
        public DateTime FechaAccion { get; set; }
        public string Detalle { get; set; }

        public Usuario Usuario { get; set; }
    }
}
