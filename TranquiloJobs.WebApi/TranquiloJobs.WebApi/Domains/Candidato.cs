using System;
using System.Collections.Generic;

#nullable disable

namespace TranquiloJobs.WebApi.Domains
{
    public partial class Candidato
    {
        public Candidato()
        {
            Estagios = new HashSet<Estagio>();
            Inscricaos = new HashSet<Inscricao>();
        }

        public int IdCandidato { get; set; }
        public string NomeCompleto { get; set; }
        public string Rg { get; set; }
        public string Cpf { get; set; }
        public string Telefone { get; set; }
        public string LinkLinkedinCandidato { get; set; }
        public int IdCurso { get; set; }
        public int IdUsuario { get; set; }

        public virtual Curso IdCursoNavigation { get; set; }
        public virtual Usuario IdUsuarioNavigation { get; set; }
        public virtual ICollection<Estagio> Estagios { get; set; }
        public virtual ICollection<Inscricao> Inscricaos { get; set; }
    }
}
