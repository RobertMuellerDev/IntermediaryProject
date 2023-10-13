namespace IntermediaryProject;

public class Intermediary {

    private readonly string _name;
    private readonly string _companyName;
    private int _capital;

    public int Capital {
        get { return _capital; }
    }

    public string Name {
        get { return _name; }
    }

    public string CompanyName {
        get { return _companyName; }
    }

    public Intermediary(string name, string companyName, int startingCapital) {
        _name = name;
        _companyName = companyName;
        _capital = startingCapital;
    }

}
