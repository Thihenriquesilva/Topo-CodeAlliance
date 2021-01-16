using System;
using System.Collections.Generic;

#nullable disable

namespace TranquiloJobs.WebApi.Domains
{
    public partial class TipoRegimePresencial
    {
        public TipoRegimePresencial()
        {
            Vagas = new HashSet<Vaga>();
        }

        public int IdTipoRegimePresencial { get; set; }
        public string NomeTipoRegimePresencial { get; set; }

        public virtual ICollection<Vaga> Vagas { get; set; }
    }
}
