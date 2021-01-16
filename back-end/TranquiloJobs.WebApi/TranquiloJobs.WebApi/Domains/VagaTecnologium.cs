using System;
using System.Collections.Generic;

#nullable disable

namespace TranquiloJobs.WebApi.Domains
{
    public partial class VagaTecnologium
    {
        public int IdTecnologia { get; set; }
        public int IdVaga { get; set; }

        public virtual Tecnologium IdTecnologiaNavigation { get; set; }
        public virtual Vaga IdVagaNavigation { get; set; }
    }
}
