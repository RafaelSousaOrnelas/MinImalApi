using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;

namespace MinimalApi.Dominio.Interfaces
{
    public interface IVeiculosServicos
    {
        List<Veiculos> Todos(int pagina = 1, string? nome = null, string? marca = null);

        Veiculos? BuscaPorId(int id);

        void Incluir(Veiculos veiculo);

        void Atualizar(Veiculos veiculo);

        void Apagar(Veiculos veiculo);
    }
}