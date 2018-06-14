using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzembly.Patcher
{
    public class PatchAttribute : Attribute
    {
        public Type TargetType { get; }

        public PatchAttribute(Type targetType)
        {
            TargetType = targetType;
        }
    }
}
