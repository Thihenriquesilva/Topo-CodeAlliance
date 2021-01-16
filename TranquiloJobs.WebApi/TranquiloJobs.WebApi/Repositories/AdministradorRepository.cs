using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SenaiTechVagas.WebApi.Interfaces;
using SenaiTechVagas.WebApi.Utils;
using SenaiTechVagas.WebApi.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TranquiloJobs.WebApi.Contexts;
using TranquiloJobs.WebApi.Domains;

namespace SenaiTechVagas.WebApi.Repositories
{
    public class AdministradorRepository : EmpresaRepository, IAdministradorRepository
    {
        public bool AtualizarCurso(int id, Curso curso)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    Curso cursoBuscado = ctx.Cursos.Find(id);
                    cursoBuscado.NomeCurso = curso.NomeCurso;
                    cursoBuscado.TipoCurso = curso.TipoCurso;
                    ctx.Cursos.Update(cursoBuscado);
                    ctx.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool CadastrarCurso(Curso curso)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    ctx.Add(curso);
                    ctx.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public string CadastrarEstagio(Estagio estagio)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    var Candidato = ctx.Candidatos.FirstOrDefault(u => u.IdUsuario == estagio.IdCandidato);//estagio.Idcandidato equivale ao id de Usuario do candidato que esta chegando da requisição
                    if (Candidato == null)
                        return "Candidato não encontardo";

                    var resposta = VerificarSeExiste(Candidato.IdCandidato);
                    if (resposta == true)
                        return "Estágio ja cadastrado";

                    Estagio estage = new Estagio()
                    {
                        IdCandidato = Candidato.IdCandidato,
                        IdEmpresa = estagio.IdEmpresa,
                        PeriodoEstagio = estagio.PeriodoEstagio,
                        DataCadastro = DateTime.Now
                    };
                    ctx.Add(estage);
                    ctx.SaveChanges();
                    return "Estágio casdastrado com sucesso";
                }
                catch (Exception)
                {
                    return "Erro no sistema";
                }
            }
        }

        public bool AtualizarEstagio(int idEstagio, int estagioAtualizado)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    Estagio estagioBuscado = ctx.Estagios.Find(idEstagio);
                    if (estagioBuscado == null)
                        return false;

                    estagioBuscado.PeriodoEstagio = estagioAtualizado;
                    ctx.Update(estagioBuscado);
                    ctx.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        public bool DeletarEstagioPorId(int idEstagio)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    Estagio estagioBuscado = ctx.Estagios.Find(idEstagio);
                    if (estagioBuscado == null)
                        return false;

                    ctx.Remove(estagioBuscado);
                    ctx.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public List<ListarEstagiosViewModel> ListarEstagios()
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    List<ListarEstagiosViewModel> list = new List<ListarEstagiosViewModel>();
                    var Estagios = ctx.Estagios.Select(u => new Estagio
                    {
                        IdEstagio = u.IdEstagio,
                        DataCadastro = u.DataCadastro,
                        PeriodoEstagio = u.PeriodoEstagio,
                        IdCandidatoNavigation =
                        new Candidato
                        {
                            IdUsuario = u.IdCandidatoNavigation.IdUsuario,
                            NomeCompleto = u.IdCandidatoNavigation.NomeCompleto,
                            Telefone = u.IdCandidatoNavigation.Telefone,
                            IdUsuarioNavigation =
                        new Usuario { IdUsuario = u.IdCandidatoNavigation.IdUsuario, Email = u.IdCandidatoNavigation.IdUsuarioNavigation.Email, CaminhoImagem = u.IdCandidatoNavigation.IdUsuarioNavigation.CaminhoImagem },
                            IdCursoNavigation =
                        new Curso { NomeCurso = u.IdCandidatoNavigation.IdCursoNavigation.NomeCurso }
                        },
                        IdEmpresaNavigation =
                        new Empresa { RazaoSocial = u.IdEmpresaNavigation.RazaoSocial }
                    }).ToList();
                    for (int i = 0; i < Estagios.Count; i++)
                    {
                        ListarEstagiosViewModel lvm = new ListarEstagiosViewModel();
                        lvm.idEstagio = Estagios[i].IdEstagio;
                        lvm.IdUsuario = Estagios[i].IdCandidatoNavigation.IdUsuario;
                        lvm.NomeCompleto = Estagios[i].IdCandidatoNavigation.NomeCompleto;
                        lvm.Telefone = Estagios[i].IdCandidatoNavigation.Telefone;
                        lvm.EmailCandidato = Estagios[i].IdCandidatoNavigation.IdUsuarioNavigation.Email;
                        lvm.CaminhoImagem = Estagios[i].IdCandidatoNavigation.IdUsuarioNavigation.CaminhoImagem;
                        lvm.NomeCurso = Estagios[i].IdCandidatoNavigation.IdCursoNavigation.NomeCurso;
                        lvm.RazaoSocial = Estagios[i].IdEmpresaNavigation.RazaoSocial;
                        lvm.PeriodoEstagio = Estagios[i].PeriodoEstagio;
                        var resultado = tempoDeEstagio(Estagios[i].DataCadastro, DateTime.Now);
                        lvm.TempoEstagiado = resultado;
                        if (lvm.TempoEstagiado >= lvm.PeriodoEstagio)
                            lvm.StatusEstagio = "Estagio encerrado";
                        else
                            lvm.StatusEstagio = "Estagiando";

                        list.Add(lvm);
                    }
                    return list;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        static int tempoDeEstagio(DateTime dataInicioEstagio, DateTime dataAtual)
        {
            long elapsedTicks = dataAtual.Ticks - dataInicioEstagio.Ticks;
            TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
            double tempoEmDiasDouble = elapsedSpan.TotalDays;
            int tempoEmDiasInt = Convert.ToInt32(tempoEmDiasDouble);
            return tempoEmDiasInt / 30;
        }

        public bool VerificarSeExiste(int idCandidato)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    Estagio estagioBuscado = ctx.Estagios.FirstOrDefault(e => e.IdCandidato == idCandidato);
                    if (estagioBuscado != null)
                        return true;

                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public int[] ContadorCadastros()
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    var Empresa = ctx.Empresas.ToList().Count;
                    var Candidato = ctx.Candidatos.ToList().Count;
                    var Estagios = ctx.Estagios.ToList().Count;
                    List<int> Cont = new List<int>();
                    Cont.Add(Empresa);
                    Cont.Add(Estagios);
                    Cont.Add(Candidato);
                    return Cont.ToArray();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public bool AtualizarTipoUsuario(int id, TipoUsuario tipoUsuario)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    TipoUsuario tipoUsuarioBuscado = ctx.TipoUsuarios.Find(id);
                    tipoUsuarioBuscado.NomeTipoUsuario = tipoUsuario.NomeTipoUsuario;
                    ctx.TipoUsuarios.Update(tipoUsuarioBuscado);
                    ctx.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool CadastrarTipoUsuario(TipoUsuario tipoUsuario)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    ctx.Add(tipoUsuario);
                    ctx.SaveChanges();
                    return true;

                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public List<TipoUsuario> ListarTipoUsuario()
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    return ctx.TipoUsuarios.ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        public bool AtualizarStatusInscricao(int id, StatusInscricao status)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    var statusBuscado = ctx.StatusInscricaos.Find(id);
                    if (statusBuscado == null)
                        return false;

                    if (status.NomeStatusInscricao != null)
                        statusBuscado.NomeStatusInscricao = status.NomeStatusInscricao;

                    ctx.Update(statusBuscado);
                    ctx.SaveChanges();

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool CadastrarStatusInscricao(StatusInscricao statusInscricao)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    ctx.Add(statusInscricao);
                    ctx.SaveChanges();

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        public List<StatusInscricao> ListarStatusInscricao()
        {
            using (JobsContext ctx = new JobsContext())
            {
                return ctx.StatusInscricaos.ToList();
            }
        }
        public bool AtualizarTecnologia(int id, Tecnologium tecnologia)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    Tecnologium tecnologiaBuscada = ctx.Tecnologia.Find(id);
                    tecnologiaBuscada.NomeTecnologia = tecnologia.NomeTecnologia;
                    ctx.Tecnologia.Update(tecnologiaBuscada);
                    ctx.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool CadastrarTecnologia(Tecnologium tecnologia)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    ctx.Add(tecnologia);
                    ctx.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        public List<Candidato> ListarCandidatos()
        {
            using (JobsContext ctx = new JobsContext())
            {

                try
                {
                    return ctx.Candidatos.Select(u =>
                    new Candidato
                    {
                        NomeCompleto = u.NomeCompleto,
                        IdUsuario = u.IdUsuario,
                        IdUsuarioNavigation =
                    new Usuario { Email = u.IdUsuarioNavigation.Email, CaminhoImagem = u.IdUsuarioNavigation.CaminhoImagem, IdTipoUsuario = u.IdUsuarioNavigation.IdTipoUsuario }
                    })
                    .Where(u => u.IdUsuarioNavigation.IdTipoUsuario != 4).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public bool DeletarInscricao(int idInscricao)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    Inscricao inscricaoBuscada = ctx.Inscricaos.Find(idInscricao);
                    if (inscricaoBuscada == null)
                        return false;

                    ctx.Remove(inscricaoBuscada);
                    ctx.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        public bool DeletarEmpresaPorId(int idUsuario)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    Empresa empresaBuscado = ctx.Empresas.FirstOrDefault(u => u.IdUsuario == idUsuario);
                    if (empresaBuscado == null)
                        return false;

                    List<Estagio> ListaDeEstagios = ctx.Estagios.Where(i => i.IdEmpresa == empresaBuscado.IdEmpresa).ToList();
                    for (int i = 0; i < ListaDeEstagios.Count; i++)
                    {

                        if (DeletarEstagioPorId(ListaDeEstagios[i].IdEmpresa))
                            continue;

                        break;
                    }
                    List<VagaTecnologium> ListaDeVagaTecnologia = ctx.VagaTecnologia.Where(i => i.IdVagaNavigation.IdEmpresa == empresaBuscado.IdEmpresa).ToList();
                    for (int i = 0; i < ListaDeVagaTecnologia.Count; i++)
                    {
                        VagaTecnologium vaga = new VagaTecnologium()
                        {
                            IdTecnologia = ListaDeVagaTecnologia[i].IdTecnologia,
                            IdVaga = ListaDeVagaTecnologia[i].IdVaga
                        };
                        if (RemoverTecnologiaDaVaga(vaga))
                            continue;

                        break;
                    }

                    List<Vaga> ListaDeVaga = ctx.Vagas.Where(i => i.IdEmpresa == empresaBuscado.IdEmpresa).ToList();
                    for (int i = 0; i < ListaDeVaga.Count; i++)
                    {
                        if (DeletarVaga(ListaDeVaga[i].IdVaga))
                            continue;
                    }
                    DeletarUsuarioEmpresaCandidato(empresaBuscado.IdUsuario);
                    ctx.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }


        public bool DeletarCandidato(int IdUsuario)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    Candidato CandidatoBuscado = ctx.Candidatos.FirstOrDefault(u => u.IdUsuario == IdUsuario);
                    if (CandidatoBuscado == null)
                        return false;

                    List<Inscricao> listaDeInscricao = ctx.Inscricaos.
                        Where(l => l.IdCandidato == CandidatoBuscado.IdCandidato).ToList();
                    for (int i = 0; i < listaDeInscricao.Count; i++)
                    {
                        DeletarInscricao(listaDeInscricao[i].IdInscricao);
                    }
                    Estagio estagioBuscado = ctx.Estagios.FirstOrDefault(e => e.IdCandidato == CandidatoBuscado.IdCandidato);
                    if (estagioBuscado != null)
                    {
                        ctx.Remove(estagioBuscado);
                        ctx.SaveChanges();
                    }
                    DeletarUsuarioEmpresaCandidato(CandidatoBuscado.IdUsuario);
                    ctx.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }


        /// <summary>
        /// Foi necessario criar esse metodo para poder deletar empresa/Candidato e usuario,no mesmo metodo estava dando erro
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public bool DeletarUsuarioEmpresaCandidato(int idUsuario)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    Usuario usuarioBuscado = ctx.Usuarios.Find(idUsuario);
                    Empresa empresaBuscada = ctx.Empresas.FirstOrDefault(e => e.IdUsuario == idUsuario);
                    if (empresaBuscada != null)
                    {
                        ctx.Remove(empresaBuscada);
                        ctx.Remove(usuarioBuscado);
                        ctx.SaveChanges();
                        if (usuarioBuscado.CaminhoImagem != "Teste.webp")
                        {
                            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "imgPerfil/");
                            string CaminhoDoArquivo = pathToSave + usuarioBuscado.CaminhoImagem;
                            File.Delete(CaminhoDoArquivo);
                        }
                        return true;
                    }

                    Candidato candidatoBuscado = ctx.Candidatos.FirstOrDefault(e => e.IdUsuario == idUsuario);
                    if (candidatoBuscado != null)
                    {
                        ctx.Remove(candidatoBuscado);
                        ctx.Remove(usuarioBuscado);
                        ctx.SaveChanges();
                        if (usuarioBuscado.CaminhoImagem != "user.png")
                        {
                            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "imgPerfil/");
                            string CaminhoDoArquivo = pathToSave + usuarioBuscado.CaminhoImagem;
                            File.Delete(CaminhoDoArquivo);
                        }
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
        public List<Empresa> ListarEmpresa()
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    return ctx.Empresas.Select(u =>
                    new Empresa
                    {
                        RazaoSocial = u.RazaoSocial,
                        IdUsuario = u.IdUsuario,
                        EmailContato = u.EmailContato,
                        IdUsuarioNavigation =
                    new Usuario { CaminhoImagem = u.IdUsuarioNavigation.CaminhoImagem, IdTipoUsuario = u.IdUsuarioNavigation.IdTipoUsuario }
                    })
                    .Where(u => u.IdUsuarioNavigation.IdTipoUsuario != 4).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        public bool DeletarVagaEmpresa(int idVaga)
        {
            try
            {
                return DeletarVaga(idVaga);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public List<Usuario> ListaDebanidos()
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    return ctx.Usuarios.Where(u => u.IdTipoUsuario == 4).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        public bool DesbanirUsuario(int idUsuario)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    Empresa empresaBuscada = ctx.Empresas.Include(u => u.IdUsuarioNavigation).FirstOrDefault(e => e.IdUsuario == idUsuario);
                    if (empresaBuscada != null)
                    {
                        if (empresaBuscada.IdUsuarioNavigation.IdTipoUsuario != 3)
                        {
                            empresaBuscada.IdUsuarioNavigation.IdTipoUsuario = 3;
                            ctx.Update(empresaBuscada);
                            ctx.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                    Candidato candidatoBuscado = ctx.Candidatos.Include(u => u.IdUsuarioNavigation).FirstOrDefault(e => e.IdUsuario == idUsuario);
                    if (candidatoBuscado != null)
                    {
                        if (candidatoBuscado.IdUsuarioNavigation.IdTipoUsuario != 2)
                        {
                            candidatoBuscado.IdUsuarioNavigation.IdTipoUsuario = 2;
                            ctx.Update(candidatoBuscado);
                            ctx.SaveChanges();
                            return true;
                        }
                    }
                    Usuario ColaboradorBuscado = ctx.Usuarios.Find(idUsuario);
                    if (empresaBuscada == null && candidatoBuscado == null && ColaboradorBuscado != null && ColaboradorBuscado.IdTipoUsuario == 4)
                    {
                        ColaboradorBuscado.IdTipoUsuario = 1;
                        ctx.Update(ColaboradorBuscado);
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
        public bool BanirUsuario(int idUsuario)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    Usuario usuarioBuscadoCandidato = ctx.Usuarios.Include(u => u.Candidato).FirstOrDefault(u => u.IdUsuario == idUsuario);
                    Usuario usuarioBuscadoEmpresa = ctx.Usuarios.Include(u => u.Empresa).FirstOrDefault(u => u.IdUsuario == idUsuario);
                    Usuario usuarioBuscadoColaborador = ctx.Usuarios.Find(idUsuario);
                    if (usuarioBuscadoCandidato == null && usuarioBuscadoEmpresa == null && usuarioBuscadoColaborador == null)
                        return false;

                    if (usuarioBuscadoCandidato.IdTipoUsuario != 4 && usuarioBuscadoCandidato.Candidato != null)
                    {
                        Candidato candidato = ctx.Candidatos.Include(u => u.Inscricaos).FirstOrDefault(u => u.IdUsuarioNavigation.IdUsuario == usuarioBuscadoCandidato.IdUsuario);
                        for (int i = 0; i < candidato.Inscricaos.Count; i++)
                        {
                            Inscricao inscricao = ctx.Inscricaos.FirstOrDefault(u => u.IdCandidato == candidato.IdCandidato);
                            if (inscricao == null)
                                break;
                            DeletarInscricao(inscricao.IdInscricao);
                        }
                        usuarioBuscadoCandidato.IdTipoUsuario = 4;
                        ctx.Update(usuarioBuscadoCandidato);
                        ctx.SaveChanges();
                        return true;
                    }
                    else if (usuarioBuscadoEmpresa.IdTipoUsuario != 4 && usuarioBuscadoEmpresa.Empresa != null)
                    {
                        Empresa empresaBuscada = ctx.Empresas.Include(u => u.Vagas).FirstOrDefault(u => u.IdUsuarioNavigation.IdUsuario == usuarioBuscadoEmpresa.IdUsuario);
                        for (int i = 0; i < empresaBuscada.Vagas.Count; i++)
                        {
                            Vaga vagaBuscada = ctx.Vagas.FirstOrDefault(u => u.IdEmpresa == empresaBuscada.IdEmpresa);
                            if (vagaBuscada == null)
                                break;

                            DeletarVaga(vagaBuscada.IdVaga);
                        }
                        usuarioBuscadoEmpresa.IdTipoUsuario = 4;
                        ctx.Update(usuarioBuscadoEmpresa);
                        ctx.SaveChanges();
                        return true;
                    }
                    else if (usuarioBuscadoColaborador.IdTipoUsuario != 4 && usuarioBuscadoColaborador.IdTipoUsuario == 1)
                    {
                        usuarioBuscadoColaborador.IdTipoUsuario = 4;
                        ctx.Update(usuarioBuscadoColaborador);
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

        public bool CadastrarAdministardor(Usuario usuario)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    usuario.CaminhoImagem = "user.png";
                    usuario.IdTipoUsuario = 1;
                    ctx.Add(usuario);
                    ctx.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool DeletarAdministrador(int id)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    Usuario usuario = ctx.Usuarios.Find(id);
                    if (usuario.IdTipoUsuario != 1 || usuario == null || usuario.IdUsuario == 1)
                        return false;
                    else
                    {
                        ctx.Remove(usuario);
                        ctx.SaveChanges();
                        return true;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public List<Usuario> ListarAdministradores()
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    return ctx.Usuarios.Where(v => v.IdTipoUsuario == 1).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public bool CadastrarArea(Area area)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    ctx.Add(area);
                    ctx.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool AtualizarArea(int idArea, Area area)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    Area areaBuscada = ctx.Areas.Find(idArea);
                    if (area.NomeArea != null)
                        areaBuscada.NomeArea = area.NomeArea;

                    ctx.Update(areaBuscada);
                    ctx.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool AlterarSenhaDoUsuario(string email, string NovaSenha)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    var usuario = ctx.Usuarios.FirstOrDefault(u => u.Email == email);
                    if (usuario == null)
                        return false;
                    else
                    {
                        usuario.Senha = Crypter.Criptografador(NovaSenha);
                        ctx.Update(usuario);
                        ctx.SaveChanges();
                        return true;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public List<Inscricao> ListarCandidatosInscritosEmpresa(int idVaga)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    return ctx.Inscricaos.Select(u =>
                        new Inscricao
                        {
                            IdVaga = u.IdVaga,
                            IdInscricao = u.IdInscricao,
                            IdCandidato = u.IdCandidato,
                            IdCandidatoNavigation =
                        new Candidato
                        {
                            NomeCompleto = u.IdCandidatoNavigation.NomeCompleto,
                            Telefone = u.IdCandidatoNavigation.Telefone,
                            IdCursoNavigation =
                        new Curso { NomeCurso = u.IdCandidatoNavigation.IdCursoNavigation.NomeCurso },
                            IdUsuarioNavigation =
                        new Usuario { CaminhoImagem = u.IdCandidatoNavigation.IdUsuarioNavigation.CaminhoImagem, Email = u.IdCandidatoNavigation.IdUsuarioNavigation.Email }
                        }
                        })
                        .Where(u => u.IdVaga == idVaga).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public List<Usuario> ListarEmailsCandidato()
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    var a = ctx.Usuarios.Where(u => u.IdTipoUsuario == 2).Select(c => new Usuario { IdUsuario = c.IdUsuario, Email = c.Email }).ToList();
                    return a;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public List<Empresa> ListarNomeEmpresas()
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    return ctx.Empresas.Select(u => new Empresa { IdEmpresa = u.IdEmpresa, RazaoSocial = u.RazaoSocial }).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public bool AdicionarTipoPresenca(TipoRegimePresencial trp)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    if (trp.NomeTipoRegimePresencial != null)
                    {
                        ctx.Add(trp);
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

        public bool AtualizarTipoPresenca(int id, TipoRegimePresencial trp)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    TipoRegimePresencial tipo = ctx.TipoRegimePresencials.Find(id);
                    if (trp.NomeTipoRegimePresencial != null && tipo != null)
                    {
                        ctx.Update(trp);
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

        public string BuscarImagemPerfilAdm(int idAms)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    var usuario = ctx.Usuarios.Find(idAms);
                    return usuario.CaminhoImagem;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public Empresa BuscarEmpresaPorIdUsuarioAdm(int idUsuario)
        {
            try
            {
                return BuscarEmpresaPorIdUsuario(idUsuario);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<ListarVagasViewModel> ListarVagasDaEmpresaAdm(int idEmpresa)
        {
            try
            {
                return ListarVagasDaEmpresa(idEmpresa);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public Candidato BuscarCandidatoPorIdUsuarioAdm(int idUsuario)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    return ctx.Candidatos.Select(u =>
                    new Candidato
                    {
                        IdUsuario = u.IdUsuario,
                        IdCandidato = u.IdCandidato,
                        IdCurso = u.IdCurso,
                        NomeCompleto = u.NomeCompleto,
                        Rg = u.Rg,
                        Cpf = u.Cpf,
                        LinkLinkedinCandidato = u.LinkLinkedinCandidato,
                        Telefone = u.Telefone,
                        IdCursoNavigation =
                        new Curso
                        {
                            NomeCurso = u.IdCursoNavigation.NomeCurso,
                            IdAreaNavigation =
                        new Area { NomeArea = u.IdCursoNavigation.IdAreaNavigation.NomeArea }
                        },
                        IdUsuarioNavigation =
                    new Usuario { CaminhoImagem = u.IdUsuarioNavigation.CaminhoImagem }
                    })
                    .FirstOrDefault(u => u.IdUsuario == idUsuario);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public List<ListarVagasViewModel> ListarInscricoes(int idUsuario)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    Candidato candidato = ctx.Candidatos.FirstOrDefault(c => c.IdUsuario == idUsuario);
                    if (candidato == null)
                        return null;

                    List<Inscricao> ListaDeInscricoes = ctx.Inscricaos.Where(v => v.IdCandidato == candidato.IdCandidato).ToList();
                    if (ListaDeInscricoes == null)
                        return null;

                    List<ListarVagasViewModel> ListVaga = new List<ListarVagasViewModel>();
                    for (int i = 0; i < ListaDeInscricoes.Count; i++)
                    {
                        Vaga v = ctx.Vagas.Select(u =>
                        new Vaga
                        {
                            TituloVaga = u.TituloVaga,
                            IdVaga = u.IdVaga,
                            IdAreaNavigation =
                        new Area { NomeArea = u.IdAreaNavigation.NomeArea },
                            IdEmpresaNavigation =
                        new Empresa
                        {
                            IdUsuarioNavigation =
                        new Usuario { CaminhoImagem = u.IdEmpresaNavigation.IdUsuarioNavigation.CaminhoImagem }
                        }
                        })
                        .FirstOrDefault(u => u.IdVaga == ListaDeInscricoes[i].IdVaga);
                        ListVaga.Add(new ListarVagasViewModel()
                        {
                            TituloVaga = v.TituloVaga,
                            IdVaga = v.IdVaga,
                            IdInscricao = ListaDeInscricoes[i].IdInscricao,
                            CaminhoImagem = v.IdEmpresaNavigation.IdUsuarioNavigation.CaminhoImagem,
                            NomeArea = v.IdAreaNavigation.NomeArea
                        });
                    }
                    return ListVaga;

                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public bool DeletarUsuarioBanido(int idUsuario)
        {
            using (JobsContext ctx = new JobsContext())
            {
                try
                {
                    Candidato c = ctx.Candidatos.FirstOrDefault(u => u.IdUsuario == idUsuario);
                    if (c == null)
                    {
                        Empresa e = ctx.Empresas.FirstOrDefault(u => u.IdUsuario == idUsuario);
                        if (e == null)
                            return false;

                        DeletarEmpresaPorId(e.IdUsuario);
                        return true;
                    }
                    else
                    {
                        DeletarCandidato(c.IdUsuario);
                        return true;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
