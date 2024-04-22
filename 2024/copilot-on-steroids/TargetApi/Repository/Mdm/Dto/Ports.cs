namespace FunctionCalling.ExternalServices.Mdm.Dto;



public class Country
{
    #region Properties

    public int CountryId { get; init; }

    public string LongDisplayName { get; init; } = "";

    public string Name { get; set; }

    public string IsoAlpha2Code { get; init; } = "";

    #endregion
}
public class PortName
{
    #region Properties

    public string UNCode { get; init; } = "";

    public string Name { get; init; } = "";

    #endregion
}
public class Region
{
    #region Properties

 
    public string Name { get; init; } = "";

    public string LongDisplayName { get; init; } = "";

  

    #endregion
}

public class PortDetails
{
    #region Properties

    public int PortId { get; init; }

    public int LocationId { get; init; }





    public Country Country { get; init; } = new();





    public Region MscRegion { get; init; }



    public PortName PortVersion { get; init; }

    public string LongDisplayName { get; init; }

    public int? AgencyId { get; init; }

    #endregion
}
public class CountrySubdivision
{
    #region Properties
    public string CountrySubdivisionId { get; init;  }
    public int CountryId { get; init;  }
    public string Name { get; init;  }
    public string IsoCode { get; init;  }
    #endregion
}