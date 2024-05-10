using ABI.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiroProcessReporter.Helper
{
    public class DataContextWrapper<T>
    {
        public T Value { get; set; }

        public DataContextWrapper(T value)
        {
            this.Value = value;
        }
    }
}
