using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Threading;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Common;
using Uranus.Domain;
using Uranus.Domain.Entities;

namespace Uranus.Suite.Controllers
{
    public class DashboardController : Controller
    {
        Performance performance = new Performance();

        // GET: Dashboard
        public ActionResult Index()
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                performance.NomeServidor = Environment.MachineName;

                var total = UsuariosBo.Consultar();
                var conectados = UsuariosConectadosBo.Consultar();

                performance.UsuariosConectados = (conectados == 0 ? "1" : conectados.ToString());
                performance.UsuariosAtivos = String.Format("{0:.##}", (((decimal)(conectados == 0 ? 1 : conectados) / (decimal)total) * 100));

                PerformanceCPU();
                PerformanceMemory();
                PerformanceDrive();

                performance.AgendasDisponiveis = AgendasBo.ConsultarAgendasDisponiveis();
                performance.AgendasConfirmadas = AgendasBo.ConsultarAgendasConfirmadas();

                var logados = UsuariosConectadosBo.ListarUsuariosConectados();
                performance.UsuariosLogados = logados;

                var sistemas = UsuariosConectadosBo.ListarSistemasOperacionais();
                performance.SistemasOperacionais = sistemas;

                var navegadores = UsuariosConectadosBo.ListarNavegadores();
                performance.Navegadores = navegadores;



                return View(performance);
            }
        }

        public static void ConnectedUsers()
        {
            if (Sessao.Usuario != null)
            {
                UsuariosConectados conectado = UsuariosConectadosBo.Consultar(Sessao.Conectado.IP, Sessao.Usuario.ID, Sessao.Conectado.SistemaOperacional, Sessao.Conectado.Navegador);

                if (conectado == null)
                {
                    conectado = new UsuariosConectados();
                    conectado.IP = Sessao.Conectado.IP;
                    conectado.IdUsuario = Sessao.Usuario.ID;
                    conectado.SistemaOperacional = Sessao.Conectado.SistemaOperacional;
                    conectado.Navegador = Sessao.Conectado.Navegador;
                    conectado.DataHora = DateTime.Now;
                    UsuariosConectadosBo.Inserir(conectado);
                }
                else
                {
                    conectado.DataHora = DateTime.Now;
                    UsuariosConectadosBo.Salvar(conectado);
                }
            }
        }

        private void PerformanceCPU()
        {
            PerformanceCounter cpuCounter = new PerformanceCounter("Processor Information", "% Processor Performance", "_Total");
            double cpuValue = cpuCounter.NextValue();

            Thread loop = new Thread(() => InfiniteLoop());
            loop.Start();

            Thread.Sleep(250);
            cpuValue = cpuCounter.NextValue();
            loop.Abort();

            using (ManagementObject obj = new ManagementObject("Win32_Processor.DeviceID='CPU0'"))
            {
                double maxSpeed = Convert.ToDouble(obj["MaxClockSpeed"]) / 1000;
                double turboSpeed = maxSpeed * cpuValue / 100;
                performance.ProcessadorVelocidade = String.Format("{0:0.00}", turboSpeed);
            }


            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            int usage = (int)cpuCounter.NextValue();
            while (usage == 0 || usage > 80)
            {
                Thread.Sleep(250);
                usage = (int)cpuCounter.NextValue();
            }

            performance.ProcessadorUtilizacao = usage.ToString();
        }

        private void PerformanceMemory()
        {
            Int64 available = PerformanceInfo.GetPhysicalAvailableMemoryInMiB();
            Int64 total = PerformanceInfo.GetTotalMemoryInMiB();
            decimal percentFree = ((decimal)available / (decimal)total) * 100;
            decimal percentOccupied = 100 - percentFree;

            performance.MemoriaDisponivel = String.Format("{0:#,#}", available); // Memória Disponível (MB)
            performance.MemoriaLivre = String.Format("{0:.##}", percentFree); // Memória Livre (%)
            performance.MemoriaTotal = String.Format("{0:#,#}", total); // Memória Total (MB)
            performance.MemoriaOcupada = String.Format("{0:.##}", percentOccupied); // Memória Ocupada (%)
        }

        private void PerformanceDrive()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                if (d.Name == "C:\\")
                {
                    if (d.IsReady == true)
                    {
                        Int64 available = d.TotalFreeSpace; // (Bytes)
                        Int64 total = d.TotalSize; // (Bytes)
                        decimal percentFree = ((decimal)available / (decimal)total) * 100;
                        decimal percentOccupied = (((decimal)total - (decimal)available) / (decimal)total) * 100;

                        available = (available / 1024) / 1024; // (Mega Bytes)
                        total = (total / 1024) / 1024; // (Mega Bytes)

                        performance.EspacoDisponivel = String.Format("{0:#,#}", available); // Espaço Disponível (MB)
                        performance.EspacoLivre = String.Format("{0:.##}", percentFree); // Espaço Livre (%)
                        performance.EspacoTotal = String.Format("{0:#,#}", total); // Espaço Total (MB)
                        performance.EspacoOcupada = String.Format("{0:.##}", percentOccupied); // Espaço Ocupada (%)
                    }

                    break;
                }
            }
        }

        private void InfiniteLoop()
        {
            int i = 0;

            while (true)
                i = i + 1 - 1;
        }
    }
}