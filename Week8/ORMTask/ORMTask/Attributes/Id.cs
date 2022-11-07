namespace ORMTask.Attributes;

public class Id:Attribute
{
    public string columnName;

    public Id(string columnName)
    {
        this.columnName = columnName;
    }
}