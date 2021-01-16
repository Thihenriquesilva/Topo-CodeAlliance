using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SenaiTechVagas.WebApi.Interfaces;
using SenaiTechVagas.WebApi.Utils;
using SenaiTechVagas.WebApi.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using TranquiloJobs.WebApi.Contexts;
using TranquiloJobs.WebApi.Domains;

namespace SenaiTechVagas.WebApi.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        public Usuario Login(string email, string senha)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    senha = Crypter.Criptografador(senha);
                    return ctx.Usuarios.FirstOrDefault(u => u.Email == email && u.Senha == senha);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public List<Curso> ListarCurso()
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    return ctx.Cursos.ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public List<Tecnologium> ListarTecnologia()
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    return ctx.Tecnologia.ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        public bool CadastrarEmpresa(CadastrarEmpresaViewModel empresa)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    Usuario usuario = new Usuario()
                    {
                        IdTipoUsuario = 3,
                        Email = empresa.Email.Trim(),
                        Senha = empresa.Senha,
                        RespostaSeguranca = empresa.RespostaSeguranca,
                        PerguntaSeguranca = empresa.PerguntaSeguranca,
                        CaminhoImagem = empresa.CaminhoImagem
                    };

                    Empresa NovaEmpresa = new Empresa()
                    {
                        NomeReponsavel = empresa.NomeReponsavel,
                        Cnpj = empresa.Cnpj.Trim(),
                        EmailContato = empresa.EmailContato.Trim(),
                        NomeFantasia = empresa.NomeFantasia,
                        RazaoSocial = empresa.RazaoSocial,
                        Telefone = empresa.Telefone.Trim(),
                        NumFuncionario = empresa.NumFuncionario,
                        NumCnae = empresa.NumCnae.Trim(),
                        Cep = empresa.Cep.Trim(),
                        Logradouro = empresa.Logradouro,
                        Complemento = empresa.Complemento,
                        Localidade = empresa.Localidade,
                        Uf = empresa.Estado,
                        IdUsuarioNavigation = usuario
                    };
                    ctx.Add(NovaEmpresa);
                    ctx.SaveChanges();
                    string variavel = "ImageBackUp/";
                    if (usuario.CaminhoImagem == "Teste.webp" || usuario.CaminhoImagem == "user.png")
                        variavel = "imgPerfil/";

                    string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), variavel + usuario.CaminhoImagem);
                    string pathMove = Path.Combine(Directory.GetCurrentDirectory(), "imgPerfil/" + usuario.CaminhoImagem);
                    File.Move(pathToSave, pathMove, true);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        public bool CadastrarCandidato(CadastrarCandidatoViewModel NovoCandidato)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    Usuario user = new Usuario()
                    {
                        Email = NovoCandidato.Email.Trim(),
                        Senha = NovoCandidato.Senha,
                        IdTipoUsuario = 2,
                        RespostaSeguranca = NovoCandidato.RespostaSeguranca,
                        PerguntaSeguranca = NovoCandidato.PerguntaSeguranca,
                        CaminhoImagem = NovoCandidato.CaminhoImagem
                    };
                    Candidato applicant = new Candidato()
                    {
                        IdUsuarioNavigation = user,
                        NomeCompleto = NovoCandidato.NomeCompleto,
                        Rg = NovoCandidato.Rg.Trim(),
                        Cpf = NovoCandidato.Cpf.Trim(),
                        Telefone = NovoCandidato.Telefone.Trim(),
                        LinkLinkedinCandidato = NovoCandidato.LinkLinkedinCandidato.Trim(),
                        IdCurso = NovoCandidato.IdCurso
                    };

                    ctx.Add(applicant);
                    ctx.SaveChanges();
                    string variavel = "ImageBackUp/";
                    if (user.CaminhoImagem == "Teste.webp" || user.CaminhoImagem == "user.png")
                        variavel = "imgPerfil/";
                    string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), variavel + user.CaminhoImagem);
                    string pathMove = Path.Combine(Directory.GetCurrentDirectory(), "imgPerfil/" + user.CaminhoImagem);
                    File.Move(pathToSave, pathMove, true);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public List<Vaga> ListarVagasEmGeral()
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    return ctx.Vagas.Select(u =>
                    new Vaga
                    {
                        IdVaga = u.IdVaga,
                        TituloVaga = u.TituloVaga,
                        IdAreaNavigation = u.IdAreaNavigation,
                        IdEmpresaNavigation =
                    new Empresa
                    {
                        IdUsuarioNavigation =
                    new Usuario { CaminhoImagem = u.IdEmpresaNavigation.IdUsuarioNavigation.CaminhoImagem }
                    }
                    }).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }

        }

        public VagaCompletaViewModel BuscarVagaPeloId(int idVaga)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    var Vaga = ctx.Vagas.Select(u =>
                    new Vaga
                    {
                        IdVaga = u.IdVaga,
                        IdArea = u.IdArea,
                        TituloVaga = u.TituloVaga,
                        Experiencia = u.Experiencia,
                        TipoContrato = u.TipoContrato,
                        Salario = u.Salario,
                        Localidade = u.Localidade,
                        DescricaoVaga = u.DescricaoVaga,
                        DescricaoBeneficio = u.DescricaoBeneficio,
                        DescricaoEmpresa = u.DescricaoEmpresa,
                        Cep = u.Cep,
                        Estado = u.Estado,
                        Complemento = u.Complemento,
                        Logradouro = u.Logradouro,
                        IdAreaNavigation =
                    new Area { NomeArea = u.IdAreaNavigation.NomeArea },
                        IdTipoRegimePresencialNavigation =
                    new TipoRegimePresencial { NomeTipoRegimePresencial = u.IdTipoRegimePresencialNavigation.NomeTipoRegimePresencial },
                        IdEmpresaNavigation =
                    new Empresa
                    {
                        RazaoSocial = u.IdEmpresaNavigation.RazaoSocial,
                        IdUsuarioNavigation =
                    new Usuario { CaminhoImagem = u.IdEmpresaNavigation.IdUsuarioNavigation.CaminhoImagem }
                    }
                    }).FirstOrDefault(u => u.IdVaga == idVaga);

                    var tecs = ctx.VagaTecnologia.Select(u =>
                    new VagaTecnologium
                    {
                        IdVaga = u.IdVaga,
                        IdTecnologiaNavigation =
                    new Tecnologium { NomeTecnologia = u.IdTecnologiaNavigation.NomeTecnologia }
                    }).Where(u => u.IdVaga == idVaga).ToList();

                    List<string> tecnologias = new List<string>();
                    for (int i = 0; i < tecs.Count; i++)
                    {
                        tecnologias.Add(tecs[i].IdTecnologiaNavigation.NomeTecnologia);
                    }
                    return new VagaCompletaViewModel()
                    {
                        IdVaga = Vaga.IdVaga,
                        IdArea = Vaga.IdArea,
                        TipoContrato = Vaga.TipoContrato,
                        TituloVaga = Vaga.TituloVaga,
                        TipoPresenca = Vaga.IdTipoRegimePresencialNavigation.NomeTipoRegimePresencial,
                        Salario = Vaga.Salario,
                        DescricaoBeneficio = Vaga.DescricaoBeneficio,
                        DescricaoEmpresa = Vaga.DescricaoEmpresa,
                        DescricaoVaga = Vaga.DescricaoVaga,
                        Experiencia = Vaga.Experiencia,
                        NomeArea = Vaga.IdAreaNavigation.NomeArea,
                        RazaoSocial = Vaga.IdEmpresaNavigation.RazaoSocial,
                        Localidade = Vaga.Localidade,
                        Complemento = Vaga.Complemento,
                        Cep = Vaga.Cep,
                        Estado = Vaga.Estado,
                        Logradouro = Vaga.Logradouro,
                        Tecnologias = tecnologias,
                        CaminhoImagem = Vaga.IdEmpresaNavigation.IdUsuarioNavigation.CaminhoImagem
                    };
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public string VerificarSeCredencialJaFoiCadastrada(VerificacaoViewModel vm)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    var usuarioBuscado = ctx.Usuarios.FirstOrDefault(x => x.Email == vm.Email);
                    if (usuarioBuscado != null)
                        return "O email ja foi cadastrado";

                    if (vm.Rg != null || vm.Cpf != null)
                    {
                        var candidatoBuscado = ctx.Candidatos.FirstOrDefault(x => x.Cpf == vm.Cpf);
                        if (candidatoBuscado != null)
                            return "O cpf ja foi cadastrado";
                        var candidatoBuscadoLink = ctx.Candidatos.FirstOrDefault(x => x.LinkLinkedinCandidato == vm.LinkLinkedinCandidato);
                        if (candidatoBuscadoLink != null)
                            return "O linkedin ja foi cadastrado";
                        var candidatoBuscadoTelefone = ctx.Candidatos.FirstOrDefault(x => x.Telefone == vm.Telefone);
                        if (candidatoBuscadoTelefone != null)
                            return "O telefone ja foi cadastrado";
                        var candidatoBuscadoRg = ctx.Candidatos.FirstOrDefault(x => x.Rg == vm.Rg);
                        if (candidatoBuscado != null || candidatoBuscadoRg != null)
                            return "O rg ja foi cadastrado";
                        else
                            return null;
                    }
                    var empresaBuscada = ctx.Empresas.FirstOrDefault(x => x.Cnpj == vm.Cnpj);
                    if (empresaBuscada != null)
                        return "O cnpj ja foi cadastrado";
                    var empresaBuscadaRazao = ctx.Empresas.FirstOrDefault(x => x.RazaoSocial == vm.RazaoSocial);
                    if (empresaBuscadaRazao != null)
                        return "A razao social ja foi cadastrada";
                    var empresaBuscadaNomeFantasia = ctx.Empresas.FirstOrDefault(x => x.NomeFantasia == vm.NomeFantasia);
                    if (empresaBuscadaNomeFantasia != null)
                        return "O nome fantasia ja foi cadastrado";
                    else
                        return null;
                }
                catch (Exception)
                {
                    return "Erro no sistema";
                }
            }
        }

        public bool RecuperarSenha(RecuperarSenhaViewModel vm)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    var usuario = ctx.Usuarios.FirstOrDefault(u => u.Email == vm.Email && u.PerguntaSeguranca == vm.Pergunta && u.RespostaSeguranca == vm.Resposta);
                    if (usuario == null)
                        return false;
                    usuario.Senha = Crypter.Criptografador(vm.NovaSenha);
                    ctx.Update(usuario);
                    ctx.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public List<Area> ListarAreas()
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    return ctx.Areas.ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public bool AlterarSenhaUsuarioLogado(AlterarSenhaUsuarioLogadoViewModel vm, int idUsuario)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    Usuario usuarioBuscado = ctx.Usuarios.Find(idUsuario);
                    if (usuarioBuscado == null)
                        return false;

                    vm.NovaSenha = Crypter.Criptografador(vm.NovaSenha);
                    vm.SenhaAtual = Crypter.Criptografador(vm.SenhaAtual);
                    if (usuarioBuscado.Senha == vm.SenhaAtual)
                    {
                        usuarioBuscado.Senha = vm.NovaSenha;
                        ctx.Update(usuarioBuscado);
                        ctx.SaveChanges();
                        return true;
                    }

                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public string AlterarImagemPerfil(int idUsuario, IFormFile img)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    Usuario user = ctx.Usuarios.Find(idUsuario);
                    string OldImage = user.CaminhoImagem;
                    var imagem = Upload(img, "imgPerfil");
                    if (imagem == null)
                        return null;
                    user.CaminhoImagem = imagem;
                    ctx.Update(user);
                    ctx.SaveChanges();
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "imgPerfil/");
                    if (OldImage != "user.png" && OldImage != "Teste.webp")
                    {
                        string CaminhoDoArquivo = pathToSave + OldImage;
                        File.Delete(CaminhoDoArquivo);
                    }
                    return imagem;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public string Upload(IFormFile arquivo, string savingFolder)
        {
            try
            {
                if (savingFolder == null)
                {
                    savingFolder = Path.Combine("ImageBackUp");
                }

                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), savingFolder);

                //Se a pasta estiver com mais de um numero de imagens determinado ele faz a limpa para não ter problema de desempenho
                if (savingFolder == "ImageBackUp")
                {
                    string[] fileEntries = Directory.GetFiles(pathToSave);
                    if (fileEntries.Length >= 10)
                    {
                        for (int i = 0; i < fileEntries.Length; i++)
                        {
                            File.Delete(fileEntries[i]);
                        }
                    }
                }

                if (arquivo.FileName.Length > 3)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(arquivo.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        arquivo.CopyTo(stream);
                    }
                    var NomeArquivo = arquivo.FileName;
                    string Extensao = NomeArquivo.Split('.')[1].Trim();
                    string Nome = Guid.NewGuid().ToString() + "." + Extensao;
                    string sourceFile = Path.Combine(Directory.GetCurrentDirectory(), savingFolder + "/" + arquivo.FileName);
                    string source = Path.Combine(Directory.GetCurrentDirectory(), savingFolder + "/");
                    FileInfo fi = new FileInfo(sourceFile);
                    fi.MoveTo(source + Nome);
                    return Nome;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
