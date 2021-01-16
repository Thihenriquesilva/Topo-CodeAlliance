using System;
using System.Collections.Generic;

#nullable disable

namespace TranquiloJobs.WebApi.Domains
{
    public partial class Tecnologium
    {
        public Tecnologium()
        {
            VagaTecnologia = new HashSet<VagaTecnologium>();
        }

        public int IdTecnologia { get; set; }
        public string NomeTecnologia { get; set; }

        public virtual ICollection<VagaTecnologium> VagaTecnologia { get; set; }
    }
}
