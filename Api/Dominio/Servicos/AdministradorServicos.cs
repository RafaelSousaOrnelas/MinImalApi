using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Infraestrutura.Db;

namespace MinimalApi.Dominio.Servicos
{
    public class AdministradorServicos : IAdministradorServicos
    {
        private readonly DbContexto _contexto;
        public AdministradorServicos(DbContexto contexto)
        {
            _contexto = contexto;

        }

        public Administrador? BuscarPorId(int id)
        {
            return _contexto.Administradores.Where(v => v.Id == id).FirstOrDefault();
        }

        public void Incluir(Administrador administrador)
        {
            _contexto.Administradores.Add(administrador);
            _contexto.SaveChanges();
        }

        public Administrador? Login(LoginDTO loginDTO)
        {
            var adm = _contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault(); //retorna algo ou nulo
            return adm;
        }

        public List<Administrador> Todos(int pagina)
        {
            var administradores = _contexto.Administradores.AsQueryable();

            int itensPorPaginas = 10;

            administradores = administradores.Skip((pagina - 1) * itensPorPaginas).Take(itensPorPaginas);

            return administradores.ToList();
        }
    }
}