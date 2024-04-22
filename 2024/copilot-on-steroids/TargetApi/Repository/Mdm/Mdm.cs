using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using FunctionCalling.ExternalServices.Mdm.Dto;
using FunctionCalling.Ioc;
using Microsoft.Extensions.Caching.Memory;

namespace FunctionCalling.ExternalServices.Mdm
{
    public sealed class Mdm : IMdm,ISingletonScope
    {
        #region Fields

        private readonly List<PortDetails> _ports =
            JsonSerializer.Deserialize<List<PortDetails>>(File.ReadAllText("Repository\\Mdm\\ports.json"));


        #endregion











        public IReadOnlyCollection<PortDetails> GetPortByUnCodeOrName(string searchValue)
        {
            var allPorts = GetAll();
            if (string.IsNullOrEmpty(searchValue))
            {
                return allPorts;
            }
            var ret = allPorts.Where(p =>
                (p.PortVersion?.Name ?? "").Contains(searchValue, StringComparison.OrdinalIgnoreCase)
                ||
                (p.PortVersion?.UNCode ?? "").Contains(searchValue, StringComparison.OrdinalIgnoreCase)
            ).ToList();
            return ret;
        }


        public IReadOnlyCollection<PortDetails> GetAll()
        {
            return _ports;
        }

      

       








     
    }

  
}