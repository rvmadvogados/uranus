using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.App.Controllers
{
    public class ListarAgendamentosController : ApiController
    {
        public class ListarAgendamentosRequest
        {
            public string CpfCnpj { get; set; }
        }

        public class ListarAgendamentosResponse
        {
            public string Status { get; set; }
            public List<Agenda> Agendas { get; set; }
        }

        public class Agenda
        {
            public int? IdProfissional { get; set; }

            public int? IdArea { get; set; }

            public string Turno { get; set; }

            public string Data { get; set; }

            public string Hora { get; set; }

            public bool PreAgendamento { get; set; }

   //         public DateTime? DataCriacaoPreAgendamento { get; set; }
        }

        // POST api/listaragendamentos
        public ListarAgendamentosResponse Get(string token, string cpfcnpj)
        {
            ListarAgendamentosResponse response = new ListarAgendamentosResponse();

            try
            {
                if (TokenController.Validar(token))
                {
                    var agendas = Listar(cpfcnpj);

                    if (agendas != null && agendas.Count > 0)
                    {
                        response.Status = "Sucesso";
                        response.Agendas = new List<Agenda>();

                        foreach (var item in agendas)
                        {
                            Agenda agenda = new Agenda();

                            if (item.IdAgenda.HasValue)
                            {
                                agenda.IdProfissional = item.Agendas.IdProfissional;
                                agenda.Data = item.Agendas.Data.ToString("dd-MM-yyyy");
                                agenda.Hora = item.Agendas.Hora;
                                agenda.PreAgendamento = false;
                            }
                            else
                            {
                                if (item.IdProfissional.HasValue)
                                {
                                    agenda.IdProfissional = item.IdProfissional.Value;
                                }

                                if (item.IdArea.HasValue)
                                {
                                    agenda.IdArea = item.IdArea.Value;
                                }

                                //agenda.Data = item.Data;
                                agenda.Turno = item.Turno;
                                agenda.PreAgendamento = true;
                            }

                            response.Agendas.Add(agenda);
                        }
                    }
                    else
                    {
                        response.Status = "Insucesso";
                    }

                    return response;
                }
                else
                {
                    response.Status = "Token Inválido";
                    return response;
                }
            }
            catch (System.Exception ex)
            {
                response.Status = "Falha";
                return response;
            }

        }

        public static List<PreAgendamento> Listar(string CpfCnpj)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.PreAgendamento.Include("Agendas")
                            where d.CpfCnpj == CpfCnpj
                            select d;

                return query.ToList();
            }
        }
    }
}
