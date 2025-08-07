namespace TaxFiling.Domain.Common;

public class DropdownItem
{
    public Guid Id { get; set; }
    public string Value { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsChecked { get; set; }
}
