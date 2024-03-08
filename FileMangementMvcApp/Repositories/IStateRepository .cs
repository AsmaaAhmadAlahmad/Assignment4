using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMangementMvcApp.Repositories
{
    public interface IStateRepository
    {
        void SetValue(string key ,string value);
        string GetValue(string key);

        void Remove(string key);
    }
}
