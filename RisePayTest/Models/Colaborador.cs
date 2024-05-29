using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RisePayTest.Models
{
    public class Colaborador
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        [EmailAddress(ErrorMessage = "O campo {0} não está no formato adequado")]
        public string Email { get; set; }
        public string Telefone { get; set; }
        public int IdCargo { get; set; }
        [ForeignKey(nameof(IdCargo))]
        public Cargo Cargo { get; set; }


    }
}
