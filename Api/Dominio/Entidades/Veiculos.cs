using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalApi.Dominio.Entidades
{
    public class Veiculos
    {
        [Key] //define como chave primaria
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //identificador incremental
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Nome { get; set; } = default!;

        [Required]
        [StringLength(100)]
        public string Marca { get; set; } = default!;

        [Required]
        public int Ano { get; set; } = default!;
    }
}