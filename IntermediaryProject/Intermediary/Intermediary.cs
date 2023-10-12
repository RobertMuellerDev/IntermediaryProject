namespace IntermediaryProject;

public class Intermediary {

    private string _name;
    private string _companyName;

    public string Name {
        get { return _name; }
    }

    public string CompanyName {
        get { return _companyName; }
    }

    public Intermediary(string name, string companyName) {
        _name = name;
        _companyName = companyName;
    }

}
