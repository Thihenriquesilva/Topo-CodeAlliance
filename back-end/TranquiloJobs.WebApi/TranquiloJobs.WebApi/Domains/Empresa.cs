using System;
using System.Collections.Generic;

#nullable disable

namespace TranquiloJobs.WebApi.Domains
{
    public partial class Empresa
    {
        public Empresa()
        {
            Estagios = new HashSet<Estagio>();
            Vagas = new HashSet<Vaga>();
        }

        public int IdEmpresa { get; set; }
        public string NomeReponsavel { get; set; }
        public string Cnpj { get; set; }
        public string EmailContato { get; set; }
        public string NomeFantasia { get; set; }
        public string RazaoSocial { get; set; }
        public string Telefone { get; set; }
        public int NumFuncionario { get; set; }
        public string NumCnae { get; set; }
        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public string Complemento { get; set; }
        public string Localidade { get; set; }
        public string Uf { get; set; }
        public int IdUsuario { get; set; }

        public virtual Usuario IdUsuarioNavigation { get; set; }
        public virtual ICollection<Estagio> Estagios { get; set; }
        public virtual ICollection<Vaga> Vagas { get; set; }
    }
}
