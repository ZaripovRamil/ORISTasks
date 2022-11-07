namespace ORMTask.Attributes;

public class Column:Attribute
{
    public readonly string ColumnName;

    public Column(string columnName)
    {
        ColumnName = columnName;
    }
}