using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;

namespace MinimalApi.Dominio.Interfaces
{
    public interface IAdministradorServicos
    {
        Administrador? Login(LoginDTO loginDTO);
        void Incluir(Administrador administrador);
        List<Administrador> Todos(int pagina);
        Administrador? BuscarPorId(int id);
    }
}