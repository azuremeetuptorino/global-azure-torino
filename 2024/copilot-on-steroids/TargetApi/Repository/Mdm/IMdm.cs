using FunctionCalling.ExternalServices.Mdm.Dto;
using System.Collections;
using System.Runtime.Serialization;

namespace FunctionCalling.ExternalServices.Mdm
{





    public interface IMdm
    {

        IReadOnlyCollection<PortDetails> GetPortByUnCodeOrName(string searchValue);
        IReadOnlyCollection<PortDetails> GetAll();
    }



}