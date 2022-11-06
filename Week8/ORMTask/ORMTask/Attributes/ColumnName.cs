namespace ORMTask.Attributes;

public class ColumnName:Attribute
{
    public string columnName;

    public ColumnName(string columnName)
    {
        this.columnName = columnName;
    }
}