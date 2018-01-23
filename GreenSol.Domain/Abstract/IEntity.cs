using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenSol.Domain.Abstract
{
    public interface IEntity
    {
        //[NotMapped]
        //int Id { get; set; }

        string Name { get; set; }
    }
}
