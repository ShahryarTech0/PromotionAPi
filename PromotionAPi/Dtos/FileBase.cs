using Merchants.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchants.Application.Dto
{
    public abstract class FileBase :BaseEntity
    {
        public required string Name { get; set; }
        public required string ImageUrl { get; set; }
    }
}
