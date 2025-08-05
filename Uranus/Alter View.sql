USE [Uranus]
GO
/****** Object:  View [dbo].[View_Processos_Listagem]    Script Date: 04/04/2022 18:27:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER VIEW [dbo].[View_Processos_Listagem]
AS
WITH ProcessosParte AS (SELECT        IdProcesso, MIN(ID) AS MenorID
                                                         FROM            dbo.ProcessosPartes
                                                         GROUP BY IdProcesso), ProcessosAutor AS
    (SELECT        IdProcesso, MIN(ID) AS MenorID
      FROM            dbo.ProcessosAutores
      GROUP BY IdProcesso)
    SELECT DISTINCT 
                              P1.ID, PA2.ID AS IdAcao, PA2.NumeroProcesso, ISNULL(P4.Nome, '') AS Autor, ISNULL(P5.Nome, '') AS Reu, ISNULL(S.Nome, '') AS Sede, ISNULL(PA3.AreaAtuacao, '') AS Area, ISNULL(P3.Nome, '') AS Profissional, P1.Status, 
                              P1.DataInclusao, ISNULL(P4.NomeBusca, '') + ' ' + ISNULL(P5.NomeBusca, '') AS NomeBusca, ISNULL(PV.Sigla, '') AS VaraSigla, ISNULL(PV.Vara, '') AS VaraNome
     FROM            dbo.Processos AS P1 LEFT OUTER JOIN
                              dbo.Profissionais AS P2 ON P1.IdProfissionalResponsavel = P2.ID LEFT OUTER JOIN
                              dbo.Pessoas AS P3 ON P2.IDPessoa = P3.ID AND P3.Nome <> 'Usuario de migração de dados' LEFT OUTER JOIN
                              dbo.ProcessosAutores AS PA ON P1.ID = PA.IdProcesso LEFT OUTER JOIN
                              dbo.Clientes AS C1 ON PA.IdCliente = C1.ID LEFT OUTER JOIN
                              dbo.Sedes AS S ON C1.IdSede = S.ID LEFT OUTER JOIN
                              dbo.Pessoas AS P4 ON C1.IDPessoa = P4.ID LEFT OUTER JOIN
                              dbo.ProcessosAcoes AS PA2 ON P1.ID = PA2.IdProcesso LEFT OUTER JOIN
                              dbo.ProcessosAreas AS PA3 ON PA2.IdProcessosArea = PA3.ID LEFT OUTER JOIN
                              dbo.ProcessosVara AS PV ON PA2.IdVara = PV.ID LEFT OUTER JOIN
                              dbo.ProcessosPartes AS PP ON PP.IdProcesso = P1.ID LEFT OUTER JOIN
                              dbo.Clientes AS C2 ON PP.IdCliente = C2.ID LEFT OUTER JOIN
                              dbo.Pessoas AS P5 ON C2.IDPessoa = P5.ID
GO

