using System;
using System.Collections.Generic;

#nullable disable

namespace TranquiloJobs.WebApi.Domains
{
    public partial class Area
    {
        public Area()
        {
            Cursos = new HashSet<Curso>();
            Vagas = new HashSet<Vaga>();
        }

        public int IdArea { get; set; }
        public string NomeArea { get; set; }

        public virtual ICollection<Curso> Cursos { get; set; }
        public virtual ICollection<Vaga> Vagas { get; set; }
    }
}
