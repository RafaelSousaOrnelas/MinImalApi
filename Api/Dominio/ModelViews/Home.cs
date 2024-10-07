using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalApi.Dominio.ModelViews
{
    public struct Home
    {

        public string Mensgem { get => "Bem vindo a Api"; }
        public string Doumentacao { get => "/swagger"; }
    }
}