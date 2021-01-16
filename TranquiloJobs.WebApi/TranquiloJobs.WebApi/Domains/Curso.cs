using System;
using System.Collections.Generic;

#nullable disable

namespace TranquiloJobs.WebApi.Domains
{
    public partial class Curso
    {
        public Curso()
        {
            Candidatos = new HashSet<Candidato>();
        }

        public int IdCurso { get; set; }
        public string NomeCurso { get; set; }
        public string TipoCurso { get; set; }
        public int IdArea { get; set; }

        public virtual Area IdAreaNavigation { get; set; }
        public virtual ICollection<Candidato> Candidatos { get; set; }
    }
}
