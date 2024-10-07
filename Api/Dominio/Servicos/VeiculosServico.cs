using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Infraestrutura.Db;

namespace MinimalApi.Dominio.Servicos
{
    public class VeiculosServico : IVeiculosServicos
    {
        private readonly DbContexto _contexto;

        public VeiculosServico(DbContexto contexto)
        {
            _contexto = contexto;
        }

        public void Apagar(Veiculos veiculo)
        {
            _contexto.Veiculos.Remove(veiculo);
            _contexto.SaveChanges();
        }

        public void Atualizar(Veiculos veiculo)
        {
            _contexto.Veiculos.Update(veiculo);
            _contexto.SaveChanges();
        }

        public Veiculos? BuscaPorId(int id)
        {
            return _contexto.Veiculos.Where(v => v.Id == id).FirstOrDefault();
        }

        public void Incluir(Veiculos veiculo)
        {
            _contexto.Add(veiculo);
            _contexto.SaveChanges();

        }

        public List<Veiculos> Todos(int pagina = 1, string? nome = null, string? marca = null)
        {
            var veiculos = _contexto.Veiculos.AsQueryable();

            if (!string.IsNullOrEmpty(nome))
            {
                veiculos = veiculos.Where(v => EF.Functions.Like(v.Nome.ToLower(), $"%{nome.ToLower()}%"));
            }

            int itensPorPaginas = 10;

            veiculos = veiculos.Skip((pagina - 1) * itensPorPaginas).Take(itensPorPaginas);

            return veiculos.ToList();
        }
    }
}